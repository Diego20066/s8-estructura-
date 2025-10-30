// Archivo: SistemaBiblioteca.Dominio/Multa.cs
namespace SistemaBiblioteca.Dominio
{
    public class Multa
    {
        public int IdMulta { get; set; }
        public int IdPrestamo { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaPago { get; set; }
        // Cambiamos EstadoPago string por el enum EstadoMulta
        public EstadoMulta Estado { get; set; }

        public Multa(int idMulta, int idPrestamo, decimal monto)
        {
            IdMulta = idMulta;
            IdPrestamo = idPrestamo;
            Monto = monto;
            FechaGeneracion = DateTime.Now;
            Estado = EstadoMulta.Pendiente; // Estado inicial
        }

        public void PagarMulta()
        {
            if (Estado != EstadoMulta.Pendiente) // Verificamos si no está Pendiente
            {
                throw new InvalidOperationException($"La multa ya se encuentra {Estado}.");
            }
            FechaPago = DateTime.Now;
            Estado = EstadoMulta.Pagada; // Cambiamos al estado Pagada
        }

        // NUEVO: Método para anular la multa (lo llamará el servicio con permiso de Admin)
        public void Anular()
        {
             if (Estado == EstadoMulta.Anulada)
             {
                  throw new InvalidOperationException("La multa ya está anulada.");
             }
             if (Estado == EstadoMulta.Pagada)
             {
                 // Podríamos decidir si se puede anular una multa pagada (quizás requiere reembolso?)
                 // Por ahora, lanzamos excepción.
                  throw new InvalidOperationException("No se puede anular una multa que ya ha sido pagada.");
             }
             Estado = EstadoMulta.Anulada;
             // Podríamos añadir una propiedad RazónAnulacion si fuera necesario
        }
    }
}