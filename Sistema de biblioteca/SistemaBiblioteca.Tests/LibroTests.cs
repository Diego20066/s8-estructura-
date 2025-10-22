// Archivo: SistemaBiblioteca.Tests/LibroTests.cs

using Xunit;
using System;
using SistemaBiblioteca.Dominio;

namespace SistemaBiblioteca.Tests
{
    public class LibroTests
    {
        // ----------------------------------------------------
        // PRUEBA 1: Caso Feliz (Préstamo)
        // ----------------------------------------------------
        [Fact]
        public void ActualizarDisponibilidad_PrestarLibro_DebeReducirCopias()
        {
            // Arrange
            // Libro con 5 copias disponibles inicialmente
            var libro = new Libro(1, "Título Test", "Autor Test", "ISBN123", 5);

            // Act
            libro.ActualizarDisponibilidad(esPrestamo: true);

            // Assert
            // Verifica que la copia disponible se haya reducido a 4
            Assert.Equal(4, libro.CopiasDisponibles);
        }

        // ----------------------------------------------------
        // PRUEBA 2: Caso Feliz (Devolución)
        // ----------------------------------------------------
        [Fact]
        public void ActualizarDisponibilidad_DevolverLibro_DebeAumentarCopias()
        {
            // Arrange
            var libro = new Libro(1, "Título Test", "Autor Test", "ISBN123", 5);
            // Simular un préstamo para tener 4 disponibles
            libro.ActualizarDisponibilidad(esPrestamo: true); 

            // Act
            libro.ActualizarDisponibilidad(esPrestamo: false);

            // Assert
            // Verifica que la copia disponible haya vuelto a 5
            Assert.Equal(5, libro.CopiasDisponibles);
        }

        // ----------------------------------------------------
        // PRUEBA 3: Caso de Error (Lanzamiento de Excepción)
        // ----------------------------------------------------
        [Fact]
        public void ActualizarDisponibilidad_PedirSinCopias_DebeLanzarExcepcion()
        {
            // Arrange
            // Libro que solo tiene 1 copia
            var libro = new Libro(1, "Título Test", "Autor Test", "ISBN123", 1);
            // Prestar la única copia (queda 0 disponible)
            libro.ActualizarDisponibilidad(esPrestamo: true); 

            // Act & Assert
            // Verifica que al intentar prestar otra vez, se lanza la excepción esperada
            Assert.Throws<InvalidOperationException>(() => 
            {
                libro.ActualizarDisponibilidad(esPrestamo: true);
            });
        }
    }
}