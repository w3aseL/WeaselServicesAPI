using System.ComponentModel.DataAnnotations;

namespace WeaselServicesAPI.Models
{
    public class TestSampleEmailModel
    {
        [Required]
        public string Email { get; set; }
    }
}
