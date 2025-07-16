namespace gestionale_scalzo.Model.DTO
{
    public class ClientDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? PIva { get; set; }
        public string TaxCode { get; set; }

        public override string? ToString()
        {
            return "ID: " + Convert.ToString(this.Id) + " Name: " + this.Name + " Surname: " + this.Surname + " Email: " + this.Email
                + " PhoneNumber: " + this.PhoneNumber + " PIva: " + this.PIva + " TaxCode: " + this.TaxCode;
        }
    }
}
