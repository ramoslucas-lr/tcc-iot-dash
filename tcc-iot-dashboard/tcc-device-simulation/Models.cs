namespace tcc_device_simulation
{
    public class Simulation
    {
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public List<SimulatedDevice> Devices { get; set; }
    }

    public class SimulatedDevice
    {
        public string DeviceId { get; set; }
        public DeviceModel Model { get; set; }
        public int MinSecondsBeforeGeneration { get; set; } = 3;
        public int MaxSecondsBeforeGeneration { get; set; } = 6;
        public DateTime NextGeneration { get; set; }
        public int MessagesCounter { get; set; } =0;
    }

    public class DeviceModel
    {
        public string BaseName { get; set; }
        public int InitialIdReference { get; set; }
    }

}
