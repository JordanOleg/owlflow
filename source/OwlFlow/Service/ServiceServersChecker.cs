using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OwlFlow.Models;

namespace OwlFlow.Service
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
        private int _countInterlockedRequest;
        public int CountRequest { get { return _countInterlockedRequest; } }
        public ServiceServersChecker(ILogger<ServiceServersChecker> logger, ServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
            this._logger = logger;
        }
        private bool ParseHeaders(HttpResponseHeaders httpHeaders)
        {
            Server server = new Server();
            try
            {
                string ping = httpHeaders.GetValues("Ping").First();
                server.Ping = int.Parse(ping);
                string countOnline = httpHeaders.GetValues("CountOnline").First();
                server.CountClient = int.Parse(countOnline);
                string useCPU = httpHeaders.GetValues("UseCPU").First();
                server.UseCPU = int.Parse(useCPU);
                string useMemory = httpHeaders.GetValues("UseMemory").First();
                server.UseMemory = int.Parse(useMemory);
                string countRequest = httpHeaders.GetValues("CountAllRequestUser").First();
                server.CountAllRequestUser = int.Parse(countRequest);
                string maxCapacityPeople = httpHeaders.GetValues("MaxCapacityPeople").First();
                server.MaxCapacityPeoples = int.Parse(maxCapacityPeople);
                string overloading = httpHeaders.GetValues("OverloadingPermission").First();
                server.OverloadingPermission = bool.Parse(overloading);
                _parsesServer.Add(server);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
        private async Task<bool> RequestChecked(HttpClient httpClient, Uri uri, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken);
            return ParseHeaders(response.Headers);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_serviceRepository.Servers != null || _serviceRepository.Servers.Count != 0)
                {
                    _parsesServer = new ConcurrentBag<Server>();
                    ParallelLoopResult loopResult =
                    Parallel.ForEach(_serviceRepository.Servers, async (server) =>
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            Uri.TryCreate($"{server.IPAddress}/heathChecked", UriKind.RelativeOrAbsolute, out Uri? uri);
                            if (uri == null)
                            {
                                _logger.LogWarning($"{uri} uri can`t create uri for ${server.Name}:{server.Id}");
                            }
                            bool result = await this.RequestChecked(client, uri!, stoppingToken);
                            if (!result)
                            {
                                _logger.LogWarning($"{server.Name}:{server.IPAddress} does not respond");
                            }
                            Interlocked.And(ref _countInterlockedRequest, 1);
                        }
                    });
                    if (!loopResult.IsCompleted)
                    {
                        _logger.LogInformation("Not all completed request checked");
                    }
                    else
                    {
                        _serviceRepository.Servers = _parsesServer.ToList();
                    }
                }
                // TODO: try used PeriodicTimer 
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}