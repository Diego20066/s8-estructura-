// Archivo: SistemaBiblioteca.Aplicacion/ServicioInventario.cs
using System;
using System.Collections.Generic;
using System.Linq; 
using SistemaBiblioteca.Dominio;

namespace SistemaBiblioteca.Aplicacion
{
    public class ServicioInventario : IGestionInventario
    {
        private readonly List<Libro> _catalogo;
        private int _proximoIdLibro;

        public ServicioInventario()
        {
            _catalogo = new List<Libro>
            {
                new Libro(1, "Cien Años de Soledad", "García Márquez", "978-0307474728", 5),
                new Libro(2, "1984", "George Orwell", "978-0451524935", 3)
            };

            // Usamos LINQ para determinar el próximo ID correctamente
            _proximoIdLibro = _catalogo.Any() ? _catalogo.Max(l => l.IdLibro) + 1 : 1;
        }

        // ----------------------------------------------------
        // IMPLEMENTACIÓN DE LA INTERFAZ: Agregar y Eliminar
        // ----------------------------------------------------

        /// <summary>
        /// Agrega un libro al catálogo y asigna un Id único.
        /// </summary>
        public void AgregarLibro(Libro libro)
        {
            if (libro == null) throw new ArgumentNullException(nameof(libro));

            // Asignamos un Id único al libro antes de añadirlo
            libro.IdLibro = _proximoIdLibro++;
            _catalogo.Add(libro);
        }

        /// <summary>
        /// Elimina un libro por su Id; lanza InvalidOperationException si no existe.
        /// </summary>
        public void EliminarLibro(int idLibro)
        {
            var libro = _catalogo.FirstOrDefault(l => l.IdLibro == idLibro);
            if (libro == null)
            {
                throw new InvalidOperationException($"No se encontró ningún libro con Id {idLibro}.");
            }

            _catalogo.Remove(libro);
        }

        // ----------------------------------------------------
        // MÉTODO BuscarLibro (CORRECCIÓN DE MANEJO DE NULLS)
        // ----------------------------------------------------

        /// <summary>
        /// Busca un libro por título, autor o ISBN usando LINQ de manera robusta.
        /// </summary>
        public Libro BuscarLibro(string criterioBusqueda)
        {
            if (string.IsNullOrWhiteSpace(criterioBusqueda))
            {
                // Devolvemos el primer elemento si el criterio está vacío (o null)
                return _catalogo.FirstOrDefault() ?? throw new InvalidOperationException("El catálogo está vacío.");
            }

            string criterio = criterioBusqueda.ToLowerInvariant();

            // CONSULTA LINQ: Se utiliza el operador nulo condicional (?) o la negación (!) 
            // si se asume que las propiedades no son null (como en nuestra implementación de Libro.cs)
            return _catalogo.FirstOrDefault(l =>
                // Aseguramos que Title/Author no sean null antes de llamar a ToLowerInvariant,
                // aunque en nuestro modelo no deberían serlo, esto previene errores
                (l.Titulo != null && l.Titulo.ToLowerInvariant().Contains(criterio)) ||
                (l.Autor != null && l.Autor.ToLowerInvariant().Contains(criterio)) ||
                (l.ISBN != null && l.ISBN.Contains(criterio)))
                ?? throw new InvalidOperationException($"No se encontró ningún libro que coincida con '{criterioBusqueda}'.");
        }

        // ----------------------------------------------------
        // MÉTODOS ADICIONALES DE SOPORTE
        // ----------------------------------------------------

        /// <summary>
        /// Obtiene un libro por su ID.
        /// </summary>
        public Libro? ObtenerPorId(int idLibro)
        {
            return _catalogo.FirstOrDefault(l => l.IdLibro == idLibro);
        }

        /// <summary>
        /// Devuelve todos los libros del catálogo.
        /// </summary>
        public IEnumerable<Libro> ObtenerTodo()
        {
            return _catalogo;
        }
    }
}
