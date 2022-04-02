using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace tcc_device_simulation
{
    public class SimulationTask
    {
        private readonly Simulation _simulation;
        private readonly SimulationOptions _options;
        private readonly Random _random;
        private readonly int _minutesToRun;
        private IoTHubConnectionService _connectionService;
        private Dictionary<string, DeviceClient> _deviceClients;

        public SimulationTask(SimulationOptions options, IoTHubConnectionService iotHubConnectionService, int minutesToRun)
        {
            _options = options;
            _random = new Random();
            _minutesToRun = minutesToRun;
            _connectionService = iotHubConnectionService;
            _deviceClients = new Dictionary<string, DeviceClient>();
            
            var devices = new List<SimulatedDevice>();

            for (var i = 0; i < options.TreatmentEquipmentDevices; i++)
            {
                devices.Add(new SimulatedDevice()
                {
                    DeviceId = $"{Constants.TreatmentEquipmentModel.BaseName}{(Constants.TreatmentEquipmentModel.InitialIdReference + i)}",
                    Model = Constants.TreatmentEquipmentModel
                });
            }

            for (var i = 0; i < options.TreatmentRoomDevices; i++)
            {
                devices.Add(new SimulatedDevice()
                {
                    DeviceId = $"{Constants.TreatmentRoomModel.BaseName}{(Constants.TreatmentRoomModel.InitialIdReference + i)}",
                    Model = Constants.TreatmentRoomModel
                });
            }

            _simulation = new Simulation() { Devices = devices, Status = Constants.SimulationStatusCreated };
        }

        public Simulation GetSimulation()
        {
            return _simulation;
        }

        public void Stop()
        {
            _simulation.FinishedAt = DateTime.Now;
        }

        public async Task Start()
        {
            Console.WriteLine("Inicializando dispositivos...");

            foreach (var device in _simulation.Devices)
            {
                device.NextGeneration = DateTime.Now.AddSeconds(_random.Next(device.MinSecondsBeforeGeneration, device.MaxSecondsBeforeGeneration));
                var deviceClient = await _connectionService.GetOrCreateDeviceAsync(device.DeviceId);
                _deviceClients.Add(device.DeviceId, deviceClient);
                Console.WriteLine($"Dispositivo {device.DeviceId} inicializado");
            }

            Console.WriteLine("Dispositivos inicializados");
            _simulation.StartedAt = DateTime.Now;
            _simulation.Status = Constants.SimulationStatusStarted;
            _simulation.FinishedAt = _simulation.StartedAt.AddSeconds(_minutesToRun * 60);

            while (DateTime.Now.CompareTo(_simulation.FinishedAt) == -1)
            {
                foreach (var device in _simulation.Devices)
                {
                    if (DateTime.Now.CompareTo(device.NextGeneration) > -1)
                    {
                        _ = _connectionService.SendMessage(_deviceClients[device.DeviceId]);
                        device.MessagesCounter++;
                        device.NextGeneration = DateTime.Now.AddSeconds(_random.Next(device.MinSecondsBeforeGeneration, device.MaxSecondsBeforeGeneration));
                        Console.WriteLine($"Enviando atualização de propriedade do dispositivo {device.DeviceId}");
                    }
                }
            }

            _simulation.Status = Constants.SimulationStatusFinished;
            Console.WriteLine("A simulação acabou. Pressione a tecla `Escape` para continuar.");
        }
    }
}
