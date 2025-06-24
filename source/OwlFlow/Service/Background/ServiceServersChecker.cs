using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using OwlFlow.Models;

namespace OwlFlow.Service.Background
{
    public class ServiceServersChecker : BackgroundService
    {
        private ILogger<ServiceServersChecker> _log;
        private ServiceRepository _serviceRepository;
        //private ConcurrentBag<Server> _parsesServer;
        //private ConcurrentQueue<Server> _failedRequestServer;
        private int _countInterlockedRequest;
        public int CountRequest { get { return _countInterlockedRequest; } }
        public ServiceServersChecker(ILogger<ServiceServersChecker> logger, ServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
            this._log = logger;
        }
        private void Initialize(Server server, RemoteDataServer remoteDataServer)
        {
            server.UseCPU = remoteDataServer.UseCPU;
            server.UseMemory = remoteDataServer.UseMemory;
            server.MaxCapacityClient = remoteDataServer.MaxCapacityClient;
            server.CountClient = remoteDataServer.CountClient;
            server.CountAllRequestUser = remoteDataServer.CountAllRequestUser;
        }
        private async Task<bool> DeserializeJson(HttpResponseMessage httpResponse, Server server)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Ігнорувати регістр властивостей
                    IncludeFields = false,              // Не включати поля (якщо не потрібно)
                };
                using Stream stream = httpResponse.Content.ReadAsStream();
                _log.LogInformation($"Response {server.Name} = {await httpResponse.Content.ReadAsStringAsync()}");
                RemoteDataServer tryDeserialyze = await JsonSerializer.DeserializeAsync<RemoteDataServer>(stream, options);
                if (tryDeserialyze != null)
                {
                    Initialize(server, tryDeserialyze);
                    _log.LogInformation($"Server: {server.Name}\n\ncpu:{server.UseCPU}%, m: {server.UseMemory}%, ping:{server.Ping}ms, {server.CountAllRequestUser}");
                    server.IsConnected = true;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                return false;
            }
        }
        private async Task<bool> RequestChecked(HttpClient httpClient, string ipAddress,  Server server, Uri uri)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Ping pingClass = new Ping();
                    PingReply pingReply = pingClass.Send(IPEndPoint.Parse(ipAddress).Address.ToString(), 200);
                    server.Ping = pingReply.RoundtripTime;
                    return await DeserializeJson(response, server);
                }
                else return false;
            }
            catch
            {
                _log.LogError("RequestChecked Error");
                return false;
            }
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Task result = AllRequestServer();
            await result;
            _log.LogInformation($"start: {result.IsFaulted} {result.IsCanceled}");
            await base.StartAsync(cancellationToken);
        }

        private async Task AllRequestServer()
        {
            try
            {
                foreach (Server server in _serviceRepository.Servers)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        Uri.TryCreate($"http://{server.IPAddress}/health", UriKind.Absolute, out Uri? uri);
                        if (uri == null)
                        {
                            _log.LogWarning($"{uri} uri can`t create uri for ${server.Name}:{server.Id}");
                        }
                        bool result = await this.RequestChecked(client, server.IPAddress, server, uri!);
                        server.IsConnected = result;
                        _log.LogInformation($"{server.Name} - {result}");
                        Interlocked.Increment(ref _countInterlockedRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _log.LogInformation("Start ExecuteAsync()");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_serviceRepository.Servers != null && _serviceRepository.Servers.Count > 0)
                    {
                        await AllRequestServer();
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex.ToString());
                }
                _log.LogWarning($"iteration Checker");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        /* protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_serviceRepository.Servers != null && _serviceRepository.Servers.Count > 0)
                    {
                        _failedRequestServer = new ConcurrentQueue<Server>();
                        var tasks = new List<Task>();
                        _parsesServer = new ConcurrentBag<Server>();
                        foreach (Server server in _serviceRepository.Servers)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    Uri.TryCreate($"{server.IPAddress}/health", UriKind.RelativeOrAbsolute, out Uri? uri);
                                    if (uri == null)
                                    {
                                        _logger.LogWarning($"{uri} uri can`t create uri for ${server.Name}:{server.Id}");
                                    }
                                    await this.RequestChecked(client, server, uri!, stoppingToken)
                                                .ContinueWith((task) =>
                                                {
                                                    _failedRequestServer.Enqueue(server);
                                                }, stoppingToken, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);
                                    Interlocked.Increment(ref _countInterlockedRequest);
                                }
                            }));
                        }
                        await Task.WhenAll(tasks);
                        if (_failedRequestServer != null && _failedRequestServer.Count > 0)
                        {
                            foreach (Server server in _failedRequestServer)
                            {
                                server.ResetProperty();
                                _parsesServer.Add(server);
                            }
                        }
                        _serviceRepository.Servers = _parsesServer.ToList();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
                // TODO: try used PeriodicTimer 
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        } */
    }
}