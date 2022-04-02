using System;
using System.Text;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Common.Exceptions;
using Newtonsoft.Json;

namespace tcc_device_simulation
{
    public class IoTHubConnectionService
    {
        private readonly Random _random;
        private readonly string _connectionString;
        private readonly string _hostName;
        private readonly RegistryManager _registryManager;

        public IoTHubConnectionService(string connectionString)
        {
            _random = new Random();
            _connectionString = connectionString;
            _hostName = connectionString.Split(";")[0].Split("=")[1];
            _registryManager = RegistryManager.CreateFromConnectionString(_connectionString);
        }

        public async Task<DeviceClient> GetOrCreateDeviceAsync(string deviceId)
        {
            var device = new Device(deviceId);

            try
            {
                device = await _registryManager.AddDeviceAsync(device);
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await _registryManager.GetDeviceAsync(deviceId);
            }

            var deviceConnectionString = $"HostName={_hostName};DeviceId={deviceId};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";
            return DeviceClient.CreateFromConnectionString(deviceConnectionString);

        }

        public async Task SendMessage(DeviceClient deviceClient)
        {
            try
            {
                var telemetryDataPoint = new Dictionary<string, object>();
                telemetryDataPoint.Add("OnUse", true);
                telemetryDataPoint.Add("NumericIndicator", _random.Next(0,100));

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(messageString))
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8"
                };

                await deviceClient.SendEventAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem ao dispositivo {ex}");
            }
        }
    }
}