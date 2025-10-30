// Archivo: SistemaBiblioteca.Dominio/Recomendacion.cs
namespace SistemaBiblioteca.Dominio
{
    public enum EstadoRecomendacion
    {
        Pendiente,
        Aprobada, // Podría significar que se comprará el libro
        Rechazada
    }

    public class Recomendacion
    {
        public int IdRecomendacion { get; set; }
        public int IdUsuarioRecomienda { get; set; } // Quién la hizo
        public string TituloLibro { get; set; }
        public string AutorLibro { get; set; }
        public DateTime FechaRecomendacion { get; set; }
        public EstadoRecomendacion Estado { get; set; }
        public string? MotivoRechazo { get; set; } // Opcional, si el admin la rechaza

        public Recomendacion(int id, int idUsuario, string titulo, string autor)
        {
            IdRecomendacion = id;
            IdUsuarioRecomienda = idUsuario;
            TituloLibro = titulo;
            AutorLibro = autor;
            FechaRecomendacion = DateTime.Now;
            Estado = EstadoRecomendacion.Pendiente;
        }
    }
}