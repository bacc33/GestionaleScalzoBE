namespace gestionale_scalzo.Model.DTO
{
    public class FilteredOrderDataDTO
    {
        public string? NumeroOrdine { get; set; }
        public string? CfCliente { get; set; }
        public DateOnly? DataInserimentoInizio { get; set; }
        public DateOnly? DataInserimentoFine { get; set; }
        public DateOnly? DataPartenzaInizio { get; set; }
        public DateOnly? DataPartenzaFine { get; set; }
    }
}
