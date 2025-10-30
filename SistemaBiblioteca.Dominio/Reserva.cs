// Archivo: SistemaBiblioteca.Dominio/Reserva.cs

namespace SistemaBiblioteca.Dominio
{
    /// <summary>
    /// Representa la solicitud de un usuario por un libro que no está disponible.
    /// </summary>
    public class Reserva
    {
        public int IdReserva { get; set; }
        public int IdLibro { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaReserva { get; set; }
        public string EstadoReserva { get; set; } // Ej: "Pendiente", "Lista", "Cancelada"

        /// <summary>
        /// Constructor para una nueva reserva.
        /// </summary>
        public Reserva(int idReserva, int idLibro, int idUsuario)
        {
            IdReserva = idReserva;
            IdLibro = idLibro;
            IdUsuario = idUsuario;
            FechaReserva = DateTime.Now;
            EstadoReserva = "Pendiente";
        }

        /// <summary>
        /// Cambia el estado de la reserva a "Lista" cuando el libro es devuelto.
        /// </summary>
        public void ConfirmarReserva()
        {
            if (EstadoReserva == "Pendiente")
            {
                EstadoReserva = "Lista";
                // Lógica de notificación al usuario
                Console.WriteLine($"[Sistema]: La reserva {IdReserva} está lista para ser recogida.");
            }
        }
    }
}