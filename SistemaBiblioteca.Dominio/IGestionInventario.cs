// Archivo: SistemaBiblioteca.Dominio/IGestionInventario.cs

namespace SistemaBiblioteca.Dominio
{
    /// <summary>
    /// Contrato que define las operaciones básicas necesarias para mantener el inventario de la biblioteca.
    /// </summary>
    public interface IGestionInventario
    {
        // Métodos abstractos del contrato:
        
        /// <summary>
        /// Agrega un nuevo libro al catálogo.
        /// </summary>
        void AgregarLibro(Libro libro);
        
        /// <summary>
        /// Elimina un libro del catálogo por su identificador.
        /// </summary>
        void EliminarLibro(int idLibro);
        
        /// <summary>
        /// Busca un libro por título, autor o ISBN.
        /// </summary>
        Libro BuscarLibro(string criterioBusqueda);
    }
}