// Archivo: SistemaBiblioteca.Dominio/Prestamo.cs

namespace SistemaBiblioteca.Dominio
{
    public class Prestamo : TransaccionBase
    {
        public DateTime FechaDevolucionEsperada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public bool EstaVencido => FechaDevolucionReal == null && DateTime.Now > FechaDevolucionEsperada;
        public decimal TasaMultaDiaria { get; private set; } = 0.50m; // Sobrecarga de propiedad

        // Constructor que llama a la base
        public Prestamo(int idTransaccion, int idUsuario, int idLibro, int diasPrestamo)
            : base(idTransaccion, idUsuario, idLibro)
        {
            FechaDevolucionEsperada = FechaTransaccion.AddDays(diasPrestamo);
        }

        /// <summary>
        /// Implementación del método abstracto para calcular la multa.
        /// </summary>
        public override decimal CalcularCosto(DateTime fechaEvaluacion)
        {
            // Si el libro ya fue devuelto, usamos la fecha real para el cálculo
            DateTime fechaFin = FechaDevolucionReal ?? fechaEvaluacion; 

            if (fechaFin > FechaDevolucionEsperada)
            {
                // Sobrecarga de operación: usa TimeSpan para calcular días
                TimeSpan diferencia = fechaFin - FechaDevolucionEsperada;
                // Usar Floor para no contar fracciones de día como día completo.
                int diasTardios = (int)Math.Floor(diferencia.TotalDays);

                if (diasTardios > 0)
                {
                    return diasTardios * TasaMultaDiaria;
                }
            }
            return 0m;
        }

        // Sobrecarga de método (sin parámetros)
        public decimal CalcularCosto()
        {
            return CalcularCosto(DateTime.Now);
        }
    }
}