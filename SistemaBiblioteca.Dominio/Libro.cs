// Archivo: SistemaBiblioteca.Dominio/Libro.cs

namespace SistemaBiblioteca.Dominio
{
    public class Libro
    {
        // Encapsulamiento con Auto-Properties
        public int IdLibro { get; set; }
        public string Titulo { get; set; }
        public string Autor { get; set; }
        public string ISBN { get; set; }
        public int TotalCopias { get; set; }
        // private set asegura que solo el propio objeto puede modificarlo
        public int CopiasDisponibles { get; private set; } 

        // Constructor por defecto
        // public Libro() { }

        // Constructor sobrecargado
        public Libro(int idLibro, string titulo, string autor, string isbn, int totalCopias)
        {
            // Validación básica en el constructor
            if (totalCopias <= 0)
                throw new ArgumentException("El total de copias debe ser mayor que cero.", nameof(totalCopias));

            IdLibro = idLibro;
            Titulo = titulo;
            Autor = autor;
            ISBN = isbn;
            TotalCopias = totalCopias;
            CopiasDisponibles = totalCopias;
        }

        /// <summary>
        /// Actualiza la disponibilidad del libro.
        /// </summary>
        /// <param name="esPrestamo">True si es préstamo (reduce), False si es devolución (aumenta).</param>
        public void ActualizarDisponibilidad(bool esPrestamo)
        {
            if (esPrestamo)
            {
                if (CopiasDisponibles <= 0)
                {
                    // Lanza una excepción en lugar de solo retornar false
                    throw new InvalidOperationException($"No hay copias disponibles de '{Titulo}' para prestar.");
                }
                CopiasDisponibles--;
            }
            else // Es devolución
            {
                if (CopiasDisponibles >= TotalCopias)
                {
                    // Lanza una excepción si se intenta devolver más de lo que la biblioteca tiene
                    throw new InvalidOperationException($"Error: Se ha intentado devolver una copia extra de '{Titulo}'.");
                }
                CopiasDisponibles++;
            }
        }
    }
}