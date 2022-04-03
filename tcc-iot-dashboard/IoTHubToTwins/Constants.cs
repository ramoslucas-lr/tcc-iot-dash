using System;
using System.Collections.Generic;
using System.Text;

namespace tcc_azure_functions
{
    public static class Constants
    {
        public const string TreatmentEquipmentInitials = "EQPTO";
        public const string TreatmentRoomInitials = "SALA";

        public static readonly List<string> TreatmentEquipmentProperties = new List<string> { "OnUse", "NumericIndicator" };
        public static readonly List<string> TreatmentRoomProperties = new List<string> { "OnUse", "NumericIndicator" };

        public static readonly List<string> Telemetries = new List<string> { "" };
    }

    public enum DeviceType
    {
        TreatmentEquipment,
        TreatmentRoom,
        Unknown
    }
}
