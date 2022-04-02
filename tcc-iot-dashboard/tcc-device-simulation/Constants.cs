using System;

namespace tcc_device_simulation
{
    public static class Constants
    {
        public const string SimulationStatusCreated = "Created";
        public const string SimulationStatusStarted = "Started";
        public const string SimulationStatusFinished = "Finished";

        public static readonly DeviceModel TreatmentEquipmentModel = new DeviceModel()
        {
            BaseName = "EQPTO_FISIO-",
            InitialIdReference = 1
        };
        public static readonly DeviceModel TreatmentRoomModel = new DeviceModel()
        {
            BaseName = "SALA_FISIO-",
            InitialIdReference = 10
        };
    }
}
