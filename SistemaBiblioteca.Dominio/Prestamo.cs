// Archivo: SistemaBiblioteca.Dominio/Prestamo.cs
namespace SistemaBiblioteca.Dominio
{
    public class Prestamo : TransaccionBase
    {
        public DateTime FechaDevolucionEsperada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public bool EstaVencido => FechaDevolucionReal == null && DateTime.Now.Date > FechaDevolucionEsperada.Date; // Comparar solo fechas
        public decimal TasaMultaDiaria { get; private set; } = 0.50m;

        // Modificamos el constructor para recibir la fecha calculada
        public Prestamo(int idTransaccion, int idUsuario, int idLibro, DateTime fechaDevolucionCalculada)
            : base(idTransaccion, idUsuario, idLibro)
        {
            // Asignamos directamente la fecha calculada por el servicio
            FechaDevolucionEsperada = fechaDevolucionCalculada.Date; // Guardar solo la fecha
        }

        public override decimal CalcularCosto(DateTime fechaEvaluacion)
        {
            DateTime fechaFin = (FechaDevolucionReal ?? fechaEvaluacion).Date; // Usar solo fechas

            // Asegurarse que la fecha esperada también sea solo fecha
            DateTime fechaEsperada = FechaDevolucionEsperada.Date;

            if (fechaFin > fechaEsperada)
            {
                TimeSpan diferencia = fechaFin - fechaEsperada;
                // TotalDays ya da la diferencia correcta, incluso si es < 1. Floor no es estrictamente necesario si queremos cobrar desde el primer instante de retraso.
                // Si queremos cobrar solo por días COMPLETOS de retraso, usamos Math.Floor. Usemos TotalDays directamente por simplicidad.
                int diasTardios = (int)diferencia.TotalDays;

                if (diasTardios > 0)
                {
                    return diasTardios * TasaMultaDiaria;
                }
            }
            return 0m;
        }

        public decimal CalcularCosto()
        {
            return CalcularCosto(DateTime.Now);
        }
    }
}