using gestionale_scalzo.Migrations;

namespace gestionale_scalzo.Model.DTO
{
    public class TypologyDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, " + 
                   $"Type: {Type}, " +
                   $"Name: {Name}, ";
        }
    }
}
