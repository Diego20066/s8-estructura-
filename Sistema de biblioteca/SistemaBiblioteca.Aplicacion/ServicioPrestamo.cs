// Archivo: SistemaBiblioteca.Aplicacion/ServicioPrestamo.cs

using System;
using System.Collections.Generic;
using System.Linq;
using SistemaBiblioteca.Dominio;

namespace SistemaBiblioteca.Aplicacion
{
    /// <summary>
    /// Servicio que gestiona las transacciones de préstamos, devoluciones y multas.
    /// (Cumple con el Principio de Responsabilidad Única - S de SOLID)
    /// </summary>
    public class ServicioPrestamo
    {
        // Colecciones Genéricas para el almacenamiento de datos transaccionales
        private readonly List<Prestamo> _prestamos;
        private readonly List<Usuario> _usuarios;
        private readonly List<Multa> _multas;
        
        // Servicios necesarios para interactuar con otras partes del sistema (Composición)
        private readonly ServicioInventario _servicioInventario;

        private int _proximoIdPrestamo = 1;
        private int _proximoIdMulta = 1;

        // ----------------------------------------------------
        // PROPIEDADES PÚBLICAS DE SOLO LECTURA
        // ----------------------------------------------------
        public List<Usuario> Usuarios => _usuarios;
        public List<Prestamo> Prestamos => _prestamos;


        public ServicioPrestamo(ServicioInventario servicioInventario)
        {
            _prestamos = new List<Prestamo>();
            _usuarios = new List<Usuario>();
            _multas = new List<Multa>();
            _servicioInventario = servicioInventario;
            
            // Inicializar algunos usuarios para las pruebas
            _usuarios.Add(new Estudiante(1, "Ana", "Gómez", "ana@mail.com", "Ingeniería"));
            _usuarios.Add(new Docente(2, "Dr. Ricardo", "Soto", "ricardo@mail.com", "Humanidades"));
        }
        
        // ----------------------------------------------------
        // LÓGICA DE NEGOCIO Y MANEJO DE POLIMORFISMO
        // ----------------------------------------------------

        /// <summary>
        /// Registra un nuevo préstamo, aplicando las reglas de capacidad del usuario.
        /// </summary>
        public Prestamo EmitirLibro(int idUsuario, int idLibro)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
            var libro = _servicioInventario.ObtenerPorId(idLibro);

            if (usuario == null || libro == null)
            {
                throw new KeyNotFoundException("Usuario o Libro no encontrado.");
            }

            // POLIMORFISMO en acción: llama al método PedirLibro()
            // La regla (3 o 5 libros) se obtiene dinámicamente según si es Docente o Estudiante.
            int limitePrestamo = usuario.PedirLibro(); 
            
            // CONSULTA LINQ: Contar cuántos libros tiene prestados actualmente
            int prestamosActuales = _prestamos.Count(p => 
                p.IdUsuario == idUsuario && p.FechaDevolucionReal == null);

            if (prestamosActuales >= limitePrestamo)
            {
                throw new InvalidOperationException($"El usuario {usuario.Nombre} ha alcanzado su límite de {limitePrestamo} préstamos.");
            }

            // Intentar actualizar disponibilidad (lanza excepción si no hay copias)
            libro.ActualizarDisponibilidad(esPrestamo: true); 

            // Crear y registrar el nuevo préstamo
            var nuevoPrestamo = new Prestamo(_proximoIdPrestamo++, idUsuario, idLibro, diasPrestamo: 7);
            _prestamos.Add(nuevoPrestamo);
            
            return nuevoPrestamo;
        }

        /// <summary>
        /// Procesa la devolución de un libro y aplica multas si es necesario.
        /// </summary>
        public Multa? DevolverLibro(int idPrestamo)
        {
            var prestamo = _prestamos.FirstOrDefault(p => p.IdTransaccion == idPrestamo);

            if (prestamo == null || prestamo.FechaDevolucionReal != null)
            {
                throw new InvalidOperationException("El préstamo no existe o ya fue devuelto.");
            }
            
            var libro = _servicioInventario.ObtenerPorId(prestamo.IdLibro);
            
            // 1. Actualizar el estado del préstamo
            prestamo.FechaDevolucionReal = DateTime.Now;
            
            // 2. Actualizar la disponibilidad del libro
            libro?.ActualizarDisponibilidad(esPrestamo: false);

            // 3. Calcular multa (Sobrecarga de método)
            decimal montoMulta = prestamo.CalcularCosto(); 
            
            if (montoMulta > 0)
            {
                // Crear y registrar la Multa si aplica
                var multa = new Multa(_proximoIdMulta++, prestamo.IdTransaccion, montoMulta);
                _multas.Add(multa);
                return multa;
            }
            return null; // No hay multa
        }
        
        // ----------------------------------------------------
        // CONSULTAS LINQ AVANZADAS
        // ----------------------------------------------------

        /// <summary>
        /// Utiliza LINQ para obtener todos los préstamos que están vencidos actualmente.
        /// </summary>
        public IEnumerable<Prestamo> ObtenerPrestamosVencidos()
        {
            // CONSULTA LINQ: Devuelve préstamos que no han sido devueltos y cuya fecha esperada es anterior a hoy.
            var hoy = DateTime.Today;
            return _prestamos.Where(p => 
                p.FechaDevolucionReal == null && 
                p.FechaDevolucionEsperada < hoy);
        }
    }
}