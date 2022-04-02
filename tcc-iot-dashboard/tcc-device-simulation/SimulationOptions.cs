namespace tcc_device_simulation
{
    public class SimulationOptions
    {
        public int SimulationTimeInSeconds { get; set; }
        public int TreatmentRoomDevices { get; set; }
        public int TreatmentEquipmentDevices { get; set; }

        public static SimulationOptions GetDefault()
        {
            return new SimulationOptions
            {
                TreatmentRoomDevices = 2,
                TreatmentEquipmentDevices = 6
            };
        }
    }
}
