using System;

namespace tcc_azure_functions
{
    public static class Utils
    {
        public static DeviceType GetDeviceType(string id)
        {
            if (id.Contains(Constants.TreatmentEquipmentInitials, StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.TreatmentEquipment;
            }
            else if (id.Contains(Constants.TreatmentRoomInitials, StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.TreatmentRoom;
            }
            else
            {
                return DeviceType.Unknown;
            }
        }
    }
}
