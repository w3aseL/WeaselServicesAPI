using System.ComponentModel.DataAnnotations;

namespace WeaselServicesAPI.Models
{
    public class NewDeviceModel
    {
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string DeviceName { get; set; }
        [Required]
        public string Manufacturer { get; set; }
    }
}
