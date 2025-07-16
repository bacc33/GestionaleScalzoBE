using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gestionale_scalzo.Model
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? NumeroOrdine { get; set; }
        public int NumeroColli { get; set; }
        public string? Varieta { get; set; }

        // ===== Pedane =====        
        public int TipologiaPedaneId { get; set; }
        public int NumeroPedane { get; set; }

        // ===== Cassette =====        
        public int TipologiaCassetteId { get; set; }
        public int NumeroCassette { get; set; }
        public float PesoCassetta { get; set; }

        // ===== Bancale =====        
        public int TipologiaBancaleId { get; set; }

        public float KiliNetti { get; set; }
        public float KiliTotali { get; set; }

        // ===== Cliente =====
        [Display(Name = "Client")]
        public string? Cliente { get; set; }

        [ForeignKey(nameof(Cliente))]
        public virtual Client? Client { get; set; }

        // ===== Date =====
        [Column(TypeName = "date")]
        public DateOnly DataInserimento { get; set; }
        [Column(TypeName = "date")]
        public DateOnly DataPartenza { get; set; }

        // ===== Prezzo e trasporto =====
        public float PrezzoOrdine { get; set; }
        public string? CompagniaTrasporto { get; set; }
        public string? Scarico { get; set; }
        public float Peso { get; set; }
    }
}
