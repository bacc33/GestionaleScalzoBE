using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestionale_scalzo.Model
{
    public class Client
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PIva { get; set; }
        public string? ResidenceAddress { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string TaxCode { get; set; }
    }
}
