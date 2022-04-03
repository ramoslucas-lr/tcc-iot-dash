using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Microsoft.Azure.Devices.Serialization;
using Azure.Identity;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure;

namespace tcc_azure_functions
{
    public class IoTHubToTwins
    {
        private static readonly string adtInstanceUrl = Environment.GetEnvironmentVariable("ADT_SERVICE_URL");
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("IoTHubToTwins")]

        public async Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log)
        {
            if (adtInstanceUrl == null) log.LogError("\"ADT_SERVICE_URL\" não configurada");

            try
            {
                if (eventGridEvent != null && eventGridEvent.Data != null)
                {
                    ManagedIdentityCredential cred = new ManagedIdentityCredential("https://digitaltwins.azure.net");
                    DigitalTwinsClient client = new DigitalTwinsClient(new Uri(adtInstanceUrl), cred, new DigitalTwinsClientOptions { Transport = new HttpClientTransport(httpClient) });
                    log.LogInformation(eventGridEvent.Data.ToString());

                    JObject deviceMessage = (JObject)JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
                    string deviceId = (string)deviceMessage["systemProperties"]["iothub-connection-device-id"];
                    var deviceType = Utils.GetDeviceType(deviceId);
                    var body = deviceMessage["body"];

                    switch (deviceType)
                    {
                        case DeviceType.TreatmentRoom:
                            await UpdateDigitalTwinProperty(client, deviceId, body, Constants.TreatmentRoomProperties);
                            break;
                        case DeviceType.TreatmentEquipment:
                            await UpdateDigitalTwinProperty(client, deviceId, body, Constants.TreatmentEquipmentProperties);
                            break;
                        default:
                            log.LogInformation("Unknown device type.");
                            break;
                    }
                    log.LogInformation($"DigitalTwinUpdated");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error in ingest function: {ex.Message}");
            }
        }

        private async Task UpdateDigitalTwinProperty(DigitalTwinsClient client, string deviceId, JToken body, List<string> properties)
        {
            foreach (var property in properties)
            {
                await UpdateDigitalTwinProperty(client, deviceId, body, property);
            }
        }

        private async Task UpdateDigitalTwinProperty(DigitalTwinsClient client, string deviceId, JToken body, string propertyName)
        {
            var propertyToken = body[propertyName];
            if (Constants.Telemetries.Contains(propertyName.ToUpper()))
            {
                var data = new Dictionary<string, double>();
                data.Add(propertyName, propertyToken.Value<double>());
                await client.PublishTelemetryAsync(deviceId, null, JsonConvert.SerializeObject(data));
            }
            else
            {
                // Update twin using device property
                var updateTwinData = new JsonPatchDocument();
                var uou = new UpdateOperationsUtility();
                updateTwinData.AppendReplace($"/{propertyName}", propertyToken.Value<double>());
                await client.UpdateDigitalTwinAsync(deviceId, updateTwinData);
            }
        }
    }
}