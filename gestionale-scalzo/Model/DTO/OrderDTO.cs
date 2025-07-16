namespace gestionale_scalzo.Model.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string NumeroOrdine { get; set; }
        public string CodiceFiscaleCliente { get; set; }
        public int NumeroColli { get; set; }
        public int NumeroPedane { get; set; }
        public int NumeroCassette { get; set; }
        public float PesoCassetta { get; set; }
        public float Peso { get; set; }
        public int TipologiaPedane { get; set; }
        public int TipologiaCassette { get; set; }
        public int TipologiaBancale { get; set; }
        public float KiliNetti { get; set; }
        public float KiliTotali { get; set; }
        public string? Scarico { get; set; }
        public string Varieta { get; set; }
        public DateOnly DataInserimento { get; set; }
        public DateOnly DataPartenza { get; set; }
        public float PrezzoOrdine { get; set; }
        public string CompagniaTrasporto { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, " +
                   $"NumeroOrdine: {NumeroOrdine}, " +
                   $"Cliente: {CodiceFiscaleCliente}, " +
                   $"NumeroColli: {NumeroColli}, " +
                   $"NumeroPedane: {NumeroPedane}, " +
                   $"NumeroCassette: {NumeroCassette}, " +
                   $"PesoCassetta: {PesoCassetta}, " +
                   $"Peso: {Peso}, " +
                   $"TipologiaPedane: {TipologiaPedane}, " +
                   $"TipologiaCassetta: {TipologiaCassette}, " +
                   $"TipologiaBancale: {TipologiaBancale}, " +
                   $"KiliNetti: {KiliNetti}, " +
                   $"KiliTotali: {KiliTotali}, " +
                   $"Scarico: {Scarico}, " +
                   $"Varieta: {Varieta}, " +
                   $"DataInserimento: {DataInserimento:dd/MM/yyyy}, " +
                   $"DataInserimento: {DataPartenza:dd/MM/yyyy}, " +
                   $"PrezzoOrdine: {PrezzoOrdine}, " +
                   $"CompagniaTrasporto: {CompagniaTrasporto}";
        }


    }
}
