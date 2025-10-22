// Archivo: SistemaBiblioteca.Dominio/TransaccionBase.cs

namespace SistemaBiblioteca.Dominio
{
    // Clase Abstracta
    public abstract class TransaccionBase
    {
        public int IdTransaccion { get; set; }
        public int IdUsuario { get; set; }
        public int IdLibro { get; set; }
        public DateTime FechaTransaccion { get; set; }

        public TransaccionBase(int idTransaccion, int idUsuario, int idLibro)
        {
            IdTransaccion = idTransaccion;
            IdUsuario = idUsuario;
            IdLibro = idLibro;
            FechaTransaccion = DateTime.Now;
        }

        // Método abstracto que debe ser implementado por las subclases
        public abstract decimal CalcularCosto(DateTime fechaEvaluacion);
    }
}