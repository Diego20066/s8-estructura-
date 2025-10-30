// Archivo: SistemaBiblioteca.Aplicacion/ServicioPrestamo.cs
using System;
using System.Collections.Generic;
using System.Linq;
using SistemaBiblioteca.Dominio;

namespace SistemaBiblioteca.Aplicacion
{
    public class ServicioPrestamo
    {
        private readonly List<Prestamo> _prestamos;
        private readonly List<Usuario> _usuarios;
        private readonly List<Multa> _multas;
        private readonly List<Recomendacion> _recomendaciones; // NUEVO: Lista para recomendaciones
        private readonly ServicioInventario _servicioInventario;

        private int _proximoIdPrestamo = 1;
        private int _proximoIdMulta = 1;
        private int _proximoIdUsuario = 1; // Para asignar IDs a nuevos usuarios
        private int _proximoIdRecomendacion = 1; // Para IDs de recomendaciones


        // Propiedades públicas para acceder a las listas (solo lectura externa)
        public IReadOnlyList<Usuario> Usuarios => _usuarios.AsReadOnly();
        public IReadOnlyList<Prestamo> Prestamos => _prestamos.AsReadOnly();
        public IReadOnlyList<Multa> Multas => _multas.AsReadOnly();
        public IReadOnlyList<Recomendacion> Recomendaciones => _recomendaciones.AsReadOnly();


        public ServicioPrestamo(ServicioInventario servicioInventario)
        {
            _prestamos = new List<Prestamo>();
            _usuarios = new List<Usuario>();
            _multas = new List<Multa>();
            _recomendaciones = new List<Recomendacion>(); // Inicializar lista
            _servicioInventario = servicioInventario ?? throw new ArgumentNullException(nameof(servicioInventario));

            // Inicializar usuarios (añadir un Admin)
            _usuarios.Add(new Estudiante(1, "Ana", "Gómez", "ana@mail.com", "Ingeniería"));
            _usuarios.Add(new Docente(2, "Ricardo", "Soto", "ricardo@mail.com", "Humanidades"));
            _usuarios.Add(new Administrador(3, "Admin", "Principal", "admin@biblioteca.com", "Sistemas"));
            _proximoIdUsuario = 4; // Siguiente ID disponible
        }

        // Helper privado para verificar si el usuario es Administrador
        private void VerificarAdmin(Usuario usuario)
        {
            if (usuario.Rol != RolUsuario.Administrador)
            {
                throw new UnauthorizedAccessException("Acción no autorizada. Se requiere rol de Administrador.");
            }
        }

        // --- Gestión de Usuarios (Solo Admin) ---

        public Usuario CrearUsuario(string nombre, string apellido, string correo, RolUsuario rol, string detalleRol, Usuario adminSolicitante)
        {
            VerificarAdmin(adminSolicitante);

            // Validar que el correo no exista
             if (_usuarios.Any(u => u.CorreoElectronico.Equals(correo, StringComparison.OrdinalIgnoreCase)))
             {
                  throw new InvalidOperationException($"Ya existe un usuario con el correo {correo}.");
             }

            Usuario nuevoUsuario;
            int nuevoId = _proximoIdUsuario++;

            switch (rol)
            {
                case RolUsuario.Estudiante:
                    nuevoUsuario = new Estudiante(nuevoId, nombre, apellido, correo, detalleRol); // detalleRol es Carrera
                    break;
                case RolUsuario.Docente:
                    nuevoUsuario = new Docente(nuevoId, nombre, apellido, correo, detalleRol); // detalleRol es Facultad
                    break;
                case RolUsuario.Administrador:
                     nuevoUsuario = new Administrador(nuevoId, nombre, apellido, correo, detalleRol); // detalleRol es Departamento
                    break;
                default:
                    throw new ArgumentException("Rol de usuario inválido.", nameof(rol));
            }

            _usuarios.Add(nuevoUsuario);
            Console.WriteLine($"[Admin] Usuario '{nombre} {apellido}' (Rol: {rol}) creado por {adminSolicitante.Nombre}.");
            return nuevoUsuario;
        }

        public void EliminarUsuario(int idUsuarioEliminar, Usuario adminSolicitante)
        {
            VerificarAdmin(adminSolicitante);

            var usuario = _usuarios.FirstOrDefault(u => u.IdUsuario == idUsuarioEliminar);
            if (usuario == null)
            {
                throw new KeyNotFoundException($"No se encontró usuario con ID {idUsuarioEliminar}.");
            }

            // Validar si el usuario tiene préstamos activos antes de eliminar (importante)
            if (_prestamos.Any(p => p.IdUsuario == idUsuarioEliminar && p.FechaDevolucionReal == null))
            {
                throw new InvalidOperationException($"No se puede eliminar al usuario {usuario.Nombre} porque tiene préstamos activos.");
            }
             // Podríamos añadir validación de multas pendientes también

            _usuarios.Remove(usuario);
            Console.WriteLine($"[Admin] Usuario '{usuario.Nombre} {usuario.Apellido}' eliminado por {adminSolicitante.Nombre}.");
        }

         public void ModificarRolUsuario(int idUsuario, RolUsuario nuevoRol, string nuevoDetalleRol, Usuario adminSolicitante)
        {
             VerificarAdmin(adminSolicitante);

             var usuarioExistente = _usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
             if (usuarioExistente == null)
             {
                 throw new KeyNotFoundException($"No se encontró usuario con ID {idUsuario}.");
             }

            // No podemos simplemente cambiar el rol, porque los tipos son diferentes (Estudiante, Docente, Admin)
            // La forma correcta implica crear un nuevo objeto del tipo correcto con los datos del anterior
            // y reemplazarlo en la lista. Es más complejo.
            // Solución simple (pero menos orientada a objetos): Añadir un método SetRol en Usuario (no recomendado)
            // Solución intermedia: Permitir cambiar solo entre Estudiante/Docente si comparten suficientes datos.
            // Por ahora, lanzaremos una excepción indicando que no está implementado de forma simple.
             throw new NotImplementedException("La modificación de rol requiere recrear el objeto usuario. Implementación pendiente.");

             // Si solo quisiéramos cambiar la propiedad Rol (si fuera posible y tuviera sentido):
             // usuarioExistente.Rol = nuevoRol; // Esto no compila porque Rol tiene protected set
             // Console.WriteLine($"[Admin] Rol del usuario {usuarioExistente.Nombre} cambiado a {nuevoRol} por {adminSolicitante.Nombre}.");
        }


        // --- Lógica de Préstamos (Modificada) ---

        public Prestamo EmitirLibro(int idUsuario, int idLibro, int diasExtraSolicitados = 0)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
            var libro = _servicioInventario.ObtenerPorId(idLibro);

            if (usuario == null) throw new KeyNotFoundException($"Usuario con ID {idUsuario} no encontrado.");
            if (libro == null) throw new KeyNotFoundException($"Libro con ID {idLibro} no encontrado.");

            // Verificar si el usuario puede pedir prestado (Ej: Admins podrían tener límite 0)
            int limitePrestamo = usuario.PedirLibro();
            if (limitePrestamo <= 0)
            {
                 throw new InvalidOperationException($"El usuario {usuario.Nombre} ({usuario.Rol}) no tiene permitido realizar préstamos.");
            }

            int prestamosActuales = _prestamos.Count(p => p.IdUsuario == idUsuario && p.FechaDevolucionReal == null);

            if (prestamosActuales >= limitePrestamo)
            {
                throw new InvalidOperationException($"El usuario {usuario.Nombre} ha alcanzado su límite de {limitePrestamo} préstamos.");
            }

            // Calcular fecha de devolución
            int diasBase = usuario.DiasPrestamoBase();
            int maxExtension = usuario.MaxDiasExtension();
            int diasExtraValidos = Math.Max(0, Math.Min(diasExtraSolicitados, maxExtension)); // Asegura que no sea negativo y no exceda el máximo
            DateTime fechaDevolucionCalculada = DateTime.Today.AddDays(diasBase + diasExtraValidos);

            // Intentar actualizar disponibilidad (lanza excepción si no hay copias)
            libro.ActualizarDisponibilidad(esPrestamo: true);

            // Crear y registrar el nuevo préstamo con la fecha calculada
            var nuevoPrestamo = new Prestamo(_proximoIdPrestamo++, idUsuario, idLibro, fechaDevolucionCalculada);
            _prestamos.Add(nuevoPrestamo);

            Console.WriteLine($"[Préstamo] Libro '{libro.Titulo}' prestado a {usuario.Nombre} hasta {fechaDevolucionCalculada.ToShortDateString()}.");
            return nuevoPrestamo;
        }

        // --- Lógica de Devolución y Multas ---

        public Multa? DevolverLibro(int idPrestamo)
        {
            var prestamo = _prestamos.FirstOrDefault(p => p.IdTransaccion == idPrestamo && p.FechaDevolucionReal == null);

            if (prestamo == null)
            {
                throw new InvalidOperationException("El préstamo no existe o ya fue devuelto.");
            }

            var libro = _servicioInventario.ObtenerPorId(prestamo.IdLibro);
            if (libro == null)
            {
                 // Esto no debería pasar si el préstamo existe, pero es bueno verificar
                 Console.WriteLine($"[WARN] No se encontró el libro con ID {prestamo.IdLibro} asociado al préstamo {idPrestamo} durante la devolución.");
                 // Podríamos decidir si continuar sin actualizar disponibilidad o lanzar error. Continuemos por ahora.
            }

            prestamo.FechaDevolucionReal = DateTime.Now;
            libro?.ActualizarDisponibilidad(esPrestamo: false); // Actualizar si se encontró el libro

            decimal montoMulta = prestamo.CalcularCosto(); // Usa la fecha actual internamente

            if (montoMulta > 0)
            {
                var multa = new Multa(_proximoIdMulta++, prestamo.IdTransaccion, montoMulta);
                _multas.Add(multa);
                 Console.WriteLine($"[Devolución] Libro devuelto tarde. Multa ID:{multa.IdMulta} generada por ${montoMulta}.");
                return multa;
            }
             Console.WriteLine($"[Devolución] Libro devuelto a tiempo.");
            return null;
        }

        // --- Anular Multa (Solo Admin) ---
        public void AnularMulta(int idMulta, Usuario adminSolicitante)
        {
            VerificarAdmin(adminSolicitante);

            var multa = _multas.FirstOrDefault(m => m.IdMulta == idMulta);
            if (multa == null)
            {
                throw new KeyNotFoundException($"No se encontró multa con Id {idMulta}.");
            }

            try
            {
                 multa.Anular(); // Llama al método en Multa.cs
                 Console.WriteLine($"[Admin] Multa {idMulta} anulada por {adminSolicitante.Nombre}.");
            }
            catch (InvalidOperationException ex)
            {
                 Console.WriteLine($"[Admin] No se pudo anular la multa {idMulta}: {ex.Message}");
                 throw; // Relanzamos la excepción para que la capa superior la maneje si es necesario
            }
        }

        // --- Lógica de Recomendaciones ---

        public Recomendacion AgregarRecomendacion(int idUsuario, string titulo, string autor)
        {
            var usuario = _usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
            if (usuario == null) throw new KeyNotFoundException($"Usuario con ID {idUsuario} no encontrado.");

            // Solo Docentes y Estudiantes pueden recomendar
            if (usuario.Rol != RolUsuario.Docente && usuario.Rol != RolUsuario.Estudiante)
            {
                 throw new UnauthorizedAccessException($"El usuario {usuario.Nombre} ({usuario.Rol}) no puede agregar recomendaciones.");
            }

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(autor))
            {
                 throw new ArgumentException("El título y el autor no pueden estar vacíos.");
            }

            var recomendacion = new Recomendacion(_proximoIdRecomendacion++, idUsuario, titulo, autor);
            _recomendaciones.Add(recomendacion);

            Console.WriteLine($"[Recomendación] {usuario.Nombre} recomendó '{titulo}' por '{autor}'.");
            return recomendacion;
        }

         public IEnumerable<Recomendacion> VerRecomendacionesPendientes(Usuario adminSolicitante)
         {
             VerificarAdmin(adminSolicitante);
             return _recomendaciones.Where(r => r.Estado == EstadoRecomendacion.Pendiente);
         }

         public void AprobarRecomendacion(int idRecomendacion, Usuario adminSolicitante)
         {
            VerificarAdmin(adminSolicitante);
            var recomendacion = _recomendaciones.FirstOrDefault(r => r.IdRecomendacion == idRecomendacion);
             if (recomendacion == null) throw new KeyNotFoundException($"Recomendación ID {idRecomendacion} no encontrada.");
             if (recomendacion.Estado != EstadoRecomendacion.Pendiente) throw new InvalidOperationException($"La recomendación ya está en estado {recomendacion.Estado}.");

             recomendacion.Estado = EstadoRecomendacion.Aprobada;
              Console.WriteLine($"[Admin] Recomendación {idRecomendacion} ('{recomendacion.TituloLibro}') aprobada por {adminSolicitante.Nombre}.");
              // Aquí podría ir lógica adicional, como añadir a una lista de compras.
         }

          public void RechazarRecomendacion(int idRecomendacion, string motivo, Usuario adminSolicitante)
         {
            VerificarAdmin(adminSolicitante);
             var recomendacion = _recomendaciones.FirstOrDefault(r => r.IdRecomendacion == idRecomendacion);
             if (recomendacion == null) throw new KeyNotFoundException($"Recomendación ID {idRecomendacion} no encontrada.");
             if (recomendacion.Estado != EstadoRecomendacion.Pendiente) throw new InvalidOperationException($"La recomendación ya está en estado {recomendacion.Estado}.");
             if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("Se requiere un motivo para rechazar.");

             recomendacion.Estado = EstadoRecomendacion.Rechazada;
             recomendacion.MotivoRechazo = motivo;
              Console.WriteLine($"[Admin] Recomendación {idRecomendacion} ('{recomendacion.TituloLibro}') rechazada por {adminSolicitante.Nombre}. Motivo: {motivo}");
         }


        // --- Consultas LINQ ---
        public IEnumerable<Prestamo> ObtenerPrestamosVencidos()
        {
            var hoy = DateTime.Today;
            return _prestamos.Where(p => p.FechaDevolucionReal == null && p.FechaDevolucionEsperada.Date < hoy);
        }

         // NUEVO: Obtener préstamos activos de un usuario
         public IEnumerable<Prestamo> ObtenerPrestamosActivosUsuario(int idUsuario)
         {
              return _prestamos.Where(p => p.IdUsuario == idUsuario && p.FechaDevolucionReal == null);
         }

         // NUEVO: Obtener multas pendientes de un usuario
         public IEnumerable<Multa> ObtenerMultasPendientesUsuario(int idUsuario)
         {
             // Necesitamos unir Prestamos y Multas para saber el usuario de la multa
             return from multa in _multas
                    join prestamo in _prestamos on multa.IdPrestamo equals prestamo.IdTransaccion
                    where prestamo.IdUsuario == idUsuario && multa.Estado == EstadoMulta.Pendiente
                    select multa;
         }
    }
}