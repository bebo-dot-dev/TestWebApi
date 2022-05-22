using System.ComponentModel.DataAnnotations;

namespace TestWebApi.Models
{
    public class RequestModel
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, 10)]
        public int Rating { get; set; }
    }
}
