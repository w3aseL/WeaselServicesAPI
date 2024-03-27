using DataAccessLayer;
using DataAccessLayer.Models;

namespace WeaselServicesAPI.Services
{
    public class DeviceService
    {
        private readonly ServicesAPIContext _ctx;

        public DeviceService(ServicesAPIContext ctx)
        {
            _ctx = ctx;
        }

        public bool VerifyDeviceToken(string uuidStr)
        {
            Guid? uuid = null;

            if (Guid.TryParse(uuidStr, out Guid uuidParse)) uuid = uuidParse;

            if (uuid == null) return false;

            return _ctx.Devices.Any(d => d.Uuid == uuid);
        }

        public Guid CreateDevice(string deviceName, string deviceId, string manufacturer, string ipAddr)
        {
            var existingDevice = _ctx.Devices.FirstOrDefault(d => 
                d.DeviceName == deviceName &&
                d.DeviceId == deviceId &&
                d.Manufacturer == manufacturer &&
                d.DeviceIpaddress == ipAddr);

            if (existingDevice != null) return existingDevice.Uuid;

            var device = new Device
            {
                DeviceName = deviceName,
                DeviceId = deviceId,
                Manufacturer = manufacturer,
                DeviceIpaddress = ipAddr,
                Uuid = Guid.NewGuid()
            };

            _ctx.Devices.Add(device);

            _ctx.SaveChanges();

            return device.Uuid;
        }
    }
}
