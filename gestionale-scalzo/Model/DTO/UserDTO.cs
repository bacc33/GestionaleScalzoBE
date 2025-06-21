using Microsoft.AspNetCore.Identity;

namespace gestionale_scalzo.Model.DTO
{
    public class UserDTO
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }

        public IEnumerable<IdentityError> Errori {  get; set; }
    }
}
