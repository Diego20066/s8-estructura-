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
            int diasDePrestamo = 7;
            // *** CORRECCIÓN AQUÍ ***
            // Calculamos la fecha de devolución esperada
            DateTime fechaDevolucionEsperada = DateTime.Today.AddDays(diasDePrestamo);
            // Creamos el préstamo pasando la fecha calculada
            var prestamo = new Prestamo(idTransaccion: 1, idUsuario: 101, idLibro: 1, fechaDevolucionCalculada: fechaDevolucionEsperada);

            // Act
            // Calcular multa usando la fecha de mañana (todavía dentro del plazo)
            DateTime fechaEvaluacion = DateTime.Today.AddDays(1); // FechaTransaccion ahora es DateTime.Now por defecto
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
            int diasDePrestamo = 7;
            int diasAtras = 10; // El préstamo se inició hace 10 días
            DateTime fechaInicioPrestamo = DateTime.Now.AddDays(-diasAtras); // Fecha real de inicio

            // *** CORRECCIÓN AQUÍ ***
            // Calculamos la fecha de devolución esperada basada en la fecha de inicio simulada
            DateTime fechaDevolucionEsperadaCalculada = fechaInicioPrestamo.Date.AddDays(diasDePrestamo); // .Date para asegurar comparación solo por fecha

            // Creamos el préstamo pasando la fecha de devolución calculada
            var prestamo = new Prestamo(idTransaccion: 2, idUsuario: 102, idLibro: 2, fechaDevolucionCalculada: fechaDevolucionEsperadaCalculada)
            {
                 // Sobreescribimos la FechaTransaccion para simular que fue en el pasado
                 FechaTransaccion = fechaInicioPrestamo
                 // Ya no necesitamos sobreescribir FechaDevolucionEsperada aquí, se calcula y asigna en el constructor
            };


            // Act
            // La multa se calcula usando la fecha actual (DateTime.Now)
            // Días de retraso esperados: diasAtras - diasDePrestamo = 10 - 7 = 3 días
            decimal multa = prestamo.CalcularCosto(); // Usa la sobrecarga sin parámetro (DateTime.Now)

            // Assert
            // 3 días de retraso * 0.50 m = 1.50
            Assert.Equal(1.50m, multa);
        }
    }
}