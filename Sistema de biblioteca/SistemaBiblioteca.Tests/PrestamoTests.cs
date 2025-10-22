// Archivo: SistemaBiblioteca.Tests/PrestamoTests.cs

using Xunit;
using System;
using SistemaBiblioteca.Dominio;

namespace SistemaBiblioteca.Tests
{
    public class PrestamoTests
    {
        // Tasa de multa diaria definida en Prestamo.cs es 0.50m

        // ----------------------------------------------------
        // PRUEBA 1: Devolución a tiempo (Sin multa)
        // ----------------------------------------------------
        [Fact]
        public void CalcularCosto_DevolucionATiempo_DebeSerCero()
        {
            // Arrange
            // Préstamo creado hoy con 7 días para devolver
            var prestamo = new Prestamo(1, 101, 1, diasPrestamo: 7); 
            
            // Act
            // Calcular multa usando la fecha de mañana (todavía dentro de los 7 días)
            DateTime fechaEvaluacion = prestamo.FechaTransaccion.AddDays(1);
            decimal multa = prestamo.CalcularCosto(fechaEvaluacion);

            // Assert
            Assert.Equal(0m, multa);
        }

        // ----------------------------------------------------
        // PRUEBA 2: Devolución tardía (Con multa)
        // ----------------------------------------------------
        [Fact]
        public void CalcularCosto_DevolucionTardia_DebeCalcularMulta()
        {
            // Arrange
            // Préstamo creado hace 10 días, con 7 días para devolver.
            // Venció hace 3 días. (FechaTransaccion fue 10 días antes)
            DateTime fechaPasada = DateTime.Now.AddDays(-10); 
            var prestamo = new Prestamo(2, 102, 2, diasPrestamo: 7) 
            {
                FechaTransaccion = fechaPasada // Simula la fecha de inicio
            };
            prestamo.FechaDevolucionEsperada = fechaPasada.AddDays(7); // Venció hace 3 días

            // Act
            // La multa se calcula usando la fecha actual (DateTime.Now)
            // Días de retraso: 10 - 7 = 3 días
            decimal multa = prestamo.CalcularCosto(); // Usa la sobrecarga sin parámetro

            // Assert
            // 3 días de retraso * 0.50 m = 1.50
            Assert.Equal(1.50m, multa);
        }
    }
}