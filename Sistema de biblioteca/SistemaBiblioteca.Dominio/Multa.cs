// Archivo: SistemaBiblioteca.Dominio/Multa.cs

namespace SistemaBiblioteca.Dominio
{
    /// <summary>
    /// Representa una penalización generada por la tardanza en un préstamo.
    /// </summary>
    public class Multa
    {
        public int IdMulta { get; set; }
        public int IdPrestamo { get; set; } // Clave foránea al Préstamo que causó la multa
        public decimal Monto { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaPago { get; set; } // Nullable: indica si ya se pagó
        public string EstadoPago { get; set; } // Ej: "Pendiente", "Pagada"

        /// <summary>
        /// Constructor para crear una multa basada en un préstamo vencido.
        /// </summary>
        public Multa(int idMulta, int idPrestamo, decimal monto)
        {
            IdMulta = idMulta;
            IdPrestamo = idPrestamo;
            Monto = monto;
            FechaGeneracion = DateTime.Now;
            EstadoPago = "Pendiente";
        }
        
        /// <summary>
        /// Registra el pago de la multa.
        /// </summary>
        public void PagarMulta()
        {
            if (EstadoPago == "Pagada")
            {
                throw new InvalidOperationException("La multa ya se encuentra pagada.");
            }
            FechaPago = DateTime.Now;
            EstadoPago = "Pagada";
        }
    }
}