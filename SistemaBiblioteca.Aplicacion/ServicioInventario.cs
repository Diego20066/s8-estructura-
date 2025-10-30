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
                // Podrías añadir más libros iniciales aquí
            };
            _proximoIdLibro = _catalogo.Any() ? _catalogo.Max(l => l.IdLibro) + 1 : 1;
        }

        // Helper privado para verificar si el usuario es Administrador
        private void VerificarAdmin(Usuario usuario)
        {
            if (usuario.Rol != RolUsuario.Administrador)
            {
                throw new UnauthorizedAccessException("Acción no autorizada. Se requiere rol de Administrador.");
            }
        }

        // --- Modificación: Agregar Control de Acceso ---
        public void AgregarLibro(Libro libro, Usuario usuarioSolicitante)
        {
            VerificarAdmin(usuarioSolicitante); // Verifica si es Admin

            if (libro == null) throw new ArgumentNullException(nameof(libro));

            // Validar que no exista ya un libro con el mismo ISBN (opcional pero recomendado)
            if (_catalogo.Any(l => l.ISBN == libro.ISBN))
            {
                 throw new InvalidOperationException($"Ya existe un libro con el ISBN {libro.ISBN}.");
            }

            libro.IdLibro = _proximoIdLibro++;
            _catalogo.Add(libro);
            Console.WriteLine($"[Inventario] Libro '{libro.Titulo}' agregado por {usuarioSolicitante.Nombre}.");
        }

        // --- Interfaz IGestionInventario implementada sin usuario (para compatibilidad si es necesario) ---
        // Si IGestionInventario no puede cambiarse, mantenemos estos métodos
        // pero idealmente la interfaz también debería incluir el usuario.
        public void AgregarLibro(Libro libro)
        {
             // Esta versión no puede verificar el rol. Podría ser llamada internamente o requerir refactorización.
             // O lanzar una excepción indicando que se debe usar la versión con usuario.
             throw new NotSupportedException("Use AgregarLibro(Libro libro, Usuario usuarioSolicitante) para control de acceso.");
            /* // Implementación sin seguridad (si es necesaria):
             if (libro == null) throw new ArgumentNullException(nameof(libro));
             libro.IdLibro = _proximoIdLibro++;
             _catalogo.Add(libro); */
        }

        // --- Modificación: Agregar Control de Acceso ---
        public void EliminarLibro(int idLibro, Usuario usuarioSolicitante)
        {
            VerificarAdmin(usuarioSolicitante); // Verifica si es Admin

            var libro = _catalogo.FirstOrDefault(l => l.IdLibro == idLibro);
            if (libro == null)
            {
                throw new InvalidOperationException($"No se encontró ningún libro con Id {idLibro}.");
            }

            // Validar si el libro está actualmente prestado antes de eliminar (opcional pero recomendado)
            // Esto requeriría acceso al ServicioPrestamo, lo cual complica un poco las dependencias.
            // Por ahora, lo eliminamos directamente.

            _catalogo.Remove(libro);
            Console.WriteLine($"[Inventario] Libro '{libro.Titulo}' (ID: {idLibro}) eliminado por {usuarioSolicitante.Nombre}.");
        }

        // --- Interfaz IGestionInventario implementada sin usuario ---
         public void EliminarLibro(int idLibro)
        {
             throw new NotSupportedException("Use EliminarLibro(int idLibro, Usuario usuarioSolicitante) para control de acceso.");
            /* // Implementación sin seguridad (si es necesaria):
             var libro = _catalogo.FirstOrDefault(l => l.IdLibro == idLibro);
             if (libro == null) throw new InvalidOperationException($"No se encontró libro con Id {idLibro}.");
             _catalogo.Remove(libro); */
        }


        // BuscarLibro no requiere rol de Admin, se mantiene igual
        public Libro BuscarLibro(string criterioBusqueda)
        {
            if (string.IsNullOrWhiteSpace(criterioBusqueda))
            {
                 return _catalogo.FirstOrDefault() ?? throw new InvalidOperationException("El catálogo está vacío.");
            }

            string criterio = criterioBusqueda.ToLowerInvariant();
            var libroEncontrado = _catalogo.FirstOrDefault(l =>
                (l.Titulo != null && l.Titulo.ToLowerInvariant().Contains(criterio)) ||
                (l.Autor != null && l.Autor.ToLowerInvariant().Contains(criterio)) ||
                (l.ISBN != null && l.ISBN.Contains(criterio)));

             if (libroEncontrado == null)
             {
                  throw new InvalidOperationException($"No se encontró ningún libro que coincida con '{criterioBusqueda}'.");
             }
             return libroEncontrado;
        }

        public Libro? ObtenerPorId(int idLibro)
        {
            return _catalogo.FirstOrDefault(l => l.IdLibro == idLibro);
        }

        public IEnumerable<Libro> ObtenerTodo()
        {
            return _catalogo;
        }
    }
}