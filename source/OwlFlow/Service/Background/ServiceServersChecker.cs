using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using OwlFlow.Models;

namespace OwlFlow.Service.Background
{
    public class ServiceServersChecker : BackgroundService
    {
        private List<string> _headers = new List<string>()
        {
            "Ping", "CountOnline", "UseCPU", "UseMemory", "CountAllRequestUser",
            "MaxCapacityPeople", "OverloadingPermission"
        };
        
        private ILogger<ServiceServersChecker> _logger;
        private ServiceRepository _serviceRepository;
        private ConcurrentBag<Server> _parsesServer;
        private ConcurrentQueue<Server> _failedRequestServer;
        private int _countInterlockedRequest;
        public int CountRequest { get { return _countInterlockedRequest; } }
        public ServiceServersChecker(ILogger<ServiceServersChecker> logger, ServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
            this._logger = logger;
        }
        private bool DeserializeJson(HttpResponseMessage httpResponse, Server server)
        {
            try
            {
                using Stream stream = httpResponse.Content.ReadAsStream();
                Server tryDeserialyze = (Server)JsonSerializer.Deserialize(stream,
                        JsonTypeInfo.CreateJsonTypeInfo(typeof(Server), JsonSerializerOptions.Web))!;
                if (tryDeserialyze != null)
                {
                    tryDeserialyze.Id = server.Id;
                    tryDeserialyze.Name = server.Name;
                    tryDeserialyze.IsConnected = true;
                    _parsesServer.Add(tryDeserialyze);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        private async Task<bool> RequestChecked(HttpClient httpClient, Server server, Uri uri, CancellationToken cancellationToken)
        {
            int countReq = 0;
            Request:
            HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken);
            if (response.StatusCode == HttpStatusCode.OK)
                return DeserializeJson(response, server);
            else if (countReq > 0 && 2 < countReq)
            {
                await Task.Delay(15);
                goto Request;
            }
            else return false;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                                    Uri.TryCreate($"{server.IPAddress}/heathChecked", UriKind.RelativeOrAbsolute, out Uri? uri);
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
        }
    }
}