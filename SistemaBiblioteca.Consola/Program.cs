// Archivo: SistemaBiblioteca.Consola/Program.cs
using System;
using System.Collections.Generic; // Needed for List<>
using System.Linq;
using SistemaBiblioteca.Aplicacion;
using SistemaBiblioteca.Dominio;

namespace SistemaBiblioteca.Consola
{
    class Program
    {
        // --- Declarar servicios como estáticos para fácil acceso ---
        private static ServicioInventario _servicioInventario = new ServicioInventario();
        private static ServicioPrestamo _servicioPrestamo = new ServicioPrestamo(_servicioInventario);
        private static Usuario? _usuarioActual = null; // Usuario que "inició sesión"

        static void Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("   📚 SISTEMA DE GESTIÓN DE BIBLIOTECA V2.0 📚  ");
            Console.WriteLine("=================================================");

            SeleccionarUsuario(); // Forzar selección de usuario al inicio

            // Bucle principal del menú
            bool salir = false;
            while (!salir && _usuarioActual != null)
            {
                MostrarMenuPrincipal();
                string opcion = Console.ReadLine() ?? "";

                try // Envolver las acciones en un try-catch general
                {
                    switch (_usuarioActual.Rol)
                    {
                        case RolUsuario.Administrador:
                            salir = ProcesarOpcionAdmin(opcion);
                            break;
                        case RolUsuario.Docente:
                        case RolUsuario.Estudiante:
                            salir = ProcesarOpcionUsuario(opcion);
                            break;
                        default: // Caso inesperado
                            Console.WriteLine("Rol de usuario desconocido. Saliendo.");
                            salir = true;
                            break;
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    MostrarError($"Acceso denegado: {ex.Message}");
                }
                catch (KeyNotFoundException ex)
                {
                    MostrarError($"Elemento no encontrado: {ex.Message}");
                }
                catch (InvalidOperationException ex)
                {
                    MostrarError($"Operación inválida: {ex.Message}");
                }
                 catch (ArgumentException ex)
                {
                     MostrarError($"Argumento inválido: {ex.Message}");
                }
                catch (NotImplementedException ex)
                {
                     MostrarAdvertencia($"Funcionalidad no implementada: {ex.Message}");
                }
                catch (Exception ex) // Captura general para otros errores
                {
                    MostrarError($"Error inesperado: {ex.Message}");
                    // Podríamos querer registrar este error en un log más detallado
                }

                if (!salir)
                {
                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                    Console.Clear(); // Limpiar consola para el siguiente menú
                     Console.WriteLine("=================================================");
                     Console.WriteLine($"   📚 MENÚ PRINCIPAL - Usuario: {_usuarioActual.Nombre} ({_usuarioActual.Rol}) 📚");
                     Console.WriteLine("=================================================");
                }
            }

            Console.WriteLine("\nGracias por usar el Sistema de Biblioteca. ¡Adiós!");
        }

        // --- Simulación de Login / Selección de Usuario ---
        static void SeleccionarUsuario()
        {
            Console.WriteLine("\n--- SELECCIONAR USUARIO ---");
            var usuariosDisponibles = _servicioPrestamo.Usuarios;
            if (!usuariosDisponibles.Any())
            {
                Console.WriteLine("No hay usuarios registrados. Saliendo.");
                Environment.Exit(1); // Salir si no hay usuarios
            }

            for (int i = 0; i < usuariosDisponibles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {usuariosDisponibles[i].Nombre} {usuariosDisponibles[i].Apellido} ({usuariosDisponibles[i].Rol})");
            }
            Console.WriteLine("0. Salir");

            int indiceSeleccionado = -1;
            while (indiceSeleccionado < 0 || indiceSeleccionado > usuariosDisponibles.Count)
            {
                Console.Write("Seleccione el número del usuario para actuar como él (o 0 para salir): ");
                if (!int.TryParse(Console.ReadLine(), out indiceSeleccionado) || indiceSeleccionado < 0 || indiceSeleccionado > usuariosDisponibles.Count)
                {
                    MostrarError("Opción inválida.");
                    indiceSeleccionado = -1; // Resetear para volver a pedir
                }
            }

            if (indiceSeleccionado == 0)
            {
                 Console.WriteLine("Saliendo...");
                 Environment.Exit(0);
            }

            _usuarioActual = usuariosDisponibles[indiceSeleccionado - 1];
            Console.Clear(); // Limpiar pantalla después de seleccionar
            Console.WriteLine("=================================================");
            Console.WriteLine($"   📚 BIENVENIDO/A {_usuarioActual.Nombre} ({_usuarioActual.Rol}) 📚");
            Console.WriteLine("=================================================");
        }

        // --- Menús ---
        static void MostrarMenuPrincipal()
        {
            Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
            if (_usuarioActual == null) return; // Seguridad

            if (_usuarioActual.Rol == RolUsuario.Administrador)
            {
                Console.WriteLine("1. Gestionar Libros (Agregar/Eliminar)");
                Console.WriteLine("2. Gestionar Usuarios (Agregar/Eliminar/Modificar Rol [Pendiente])");
                Console.WriteLine("3. Gestionar Recomendaciones (Ver/Aprobar/Rechazar)");
                Console.WriteLine("4. Anular Multa");
                Console.WriteLine("5. Ver Todos los Préstamos Activos");
                Console.WriteLine("6. Ver Todas las Multas Pendientes");
                Console.WriteLine("-------------------------");
                Console.WriteLine("S. Cambiar de Usuario");
                Console.WriteLine("0. Salir");
            }
            else // Docente o Estudiante
            {
                Console.WriteLine("1. Buscar Libro");
                Console.WriteLine("2. Listar Todos los Libros");
                Console.WriteLine("3. Pedir Libro Prestado");
                Console.WriteLine("4. Devolver Libro");
                Console.WriteLine("5. Ver Mis Préstamos Activos");
                Console.WriteLine("6. Ver Mis Multas Pendientes");
                Console.WriteLine("7. Recomendar Libro");
                Console.WriteLine("-------------------------");
                Console.WriteLine("S. Cambiar de Usuario");
                Console.WriteLine("0. Salir");
            }
            Console.Write("Seleccione una opción: ");
        }

        // --- Procesadores de Opciones ---
        static bool ProcesarOpcionAdmin(string opcion)
        {
            switch (opcion.ToUpper())
            {
                case "1": AccionGestionarLibros(); break;
                case "2": AccionGestionarUsuarios(); break;
                case "3": AccionGestionarRecomendaciones(); break;
                case "4": AccionAnularMulta(); break;
                case "5": AccionVerTodosPrestamosActivos(); break;
                case "6": AccionVerTodasMultasPendientes(); break;
                case "S": SeleccionarUsuario(); return false; // Volver a seleccionar usuario, no salir
                case "0": return true; // Salir
                default: MostrarError("Opción inválida."); break;
            }
            return false; // No salir por defecto
        }

         static bool ProcesarOpcionUsuario(string opcion)
        {
            switch (opcion.ToUpper())
            {
                case "1": AccionBuscarLibro(); break;
                case "2": AccionListarLibros(); break;
                case "3": AccionPedirPrestamo(); break;
                case "4": AccionDevolverLibro(); break;
                case "5": AccionVerMisPrestamos(); break;
                case "6": AccionVerMisMultas(); break;
                case "7": AccionRecomendarLibro(); break;
                case "S": SeleccionarUsuario(); return false; // Volver a seleccionar usuario, no salir
                case "0": return true; // Salir
                default: MostrarError("Opción inválida."); break;
            }
            return false; // No salir por defecto
        }


        // --- Implementación de Acciones ---

        // Acciones Comunes
        static void AccionBuscarLibro()
        {
            Console.Write("Ingrese Título, Autor o ISBN a buscar: ");
            string criterio = Console.ReadLine() ?? "";
            try
            {
                 var libro = _servicioInventario.BuscarLibro(criterio);
                 Console.WriteLine($"\n--- Libro Encontrado ---");
                 MostrarDetalleLibro(libro);
            }
            catch(InvalidOperationException ex)
            {
                 MostrarAdvertencia(ex.Message);
            }
        }

        static void AccionListarLibros()
        {
             Console.WriteLine("\n--- Catálogo de Libros ---");
             var catalogo = _servicioInventario.ObtenerTodo();
             if (!catalogo.Any())
             {
                 MostrarAdvertencia("El catálogo está vacío.");
                 return;
             }
             foreach (var libro in catalogo)
             {
                 MostrarDetalleLibro(libro, simple: true);
             }
        }


        // Acciones de Usuario (Docente/Estudiante)
        static void AccionPedirPrestamo()
        {
            int idLibro = LeerEntero("Ingrese el ID del libro a prestar:");
            int diasExtra = LeerEntero($"¿Cuántos días extra desea solicitar? (Máximo {_usuarioActual?.MaxDiasExtension() ?? 0}, 0 para ninguno):", requerido: false);

            var prestamo = _servicioPrestamo.EmitirLibro(_usuarioActual!.IdUsuario, idLibro, diasExtra); // Usamos ! porque sabemos que no es null aquí
            MostrarExito($"Préstamo del libro ID {idLibro} realizado con éxito. Devolver antes de: {prestamo.FechaDevolucionEsperada.ToShortDateString()}");
        }

        static void AccionDevolverLibro()
        {
             var prestamosActivos = _servicioPrestamo.ObtenerPrestamosActivosUsuario(_usuarioActual!.IdUsuario).ToList();
             if (!prestamosActivos.Any())
             {
                 MostrarAdvertencia("No tienes préstamos activos para devolver.");
                 return;
             }

             Console.WriteLine("\n--- Tus Préstamos Activos ---");
             foreach(var p in prestamosActivos)
             {
                 var libro = _servicioInventario.ObtenerPorId(p.IdLibro);
                 Console.WriteLine($"ID Préstamo: {p.IdTransaccion} - Libro: '{libro?.Titulo ?? "Desconocido"}' (ID: {p.IdLibro}) - Fecha Devolución: {p.FechaDevolucionEsperada.ToShortDateString()}");
             }

             int idPrestamo = LeerEntero("Ingrese el ID del préstamo a devolver:");

             var multa = _servicioPrestamo.DevolverLibro(idPrestamo);
             if (multa != null)
             {
                 MostrarAdvertencia($"Libro devuelto con retraso. Se generó una multa de ${multa.Monto:N2} (ID Multa: {multa.IdMulta}).");
             }
             else
             {
                 MostrarExito("Libro devuelto correctamente y a tiempo.");
             }
        }

        static void AccionVerMisPrestamos()
        {
            Console.WriteLine("\n--- Mis Préstamos Activos ---");
            var prestamos = _servicioPrestamo.ObtenerPrestamosActivosUsuario(_usuarioActual!.IdUsuario);
            if (!prestamos.Any())
            {
                MostrarAdvertencia("No tienes préstamos activos.");
                return;
            }
            foreach (var p in prestamos)
            {
                var libro = _servicioInventario.ObtenerPorId(p.IdLibro);
                 Console.WriteLine($"- ID Préstamo: {p.IdTransaccion}, Libro: '{libro?.Titulo ?? "Desconocido"}', Vence: {p.FechaDevolucionEsperada.ToShortDateString()}{(p.EstaVencido ? " [VENCIDO]" : "")}");
            }
        }

         static void AccionVerMisMultas()
         {
             Console.WriteLine("\n--- Mis Multas Pendientes ---");
             var multas = _servicioPrestamo.ObtenerMultasPendientesUsuario(_usuarioActual!.IdUsuario);
              if (!multas.Any())
             {
                 MostrarAdvertencia("No tienes multas pendientes.");
                 return;
             }
             foreach (var m in multas)
             {
                 Console.WriteLine($"- ID Multa: {m.IdMulta}, Monto: ${m.Monto:N2}, Fecha Generación: {m.FechaGeneracion.ToShortDateString()}, Préstamo ID: {m.IdPrestamo}");
             }
         }

         static void AccionRecomendarLibro()
         {
            Console.WriteLine("\n--- Recomendar un Libro ---");
            string titulo = LeerTexto("Título del libro:", requerido: true);
            string autor = LeerTexto("Autor del libro:", requerido: true);

            _servicioPrestamo.AgregarRecomendacion(_usuarioActual!.IdUsuario, titulo, autor);
            MostrarExito("¡Gracias por tu recomendación!");
         }


        // Acciones de Administrador
        static void AccionGestionarLibros()
        {
            Console.WriteLine("\n--- Gestión de Libros ---");
            Console.WriteLine("1. Agregar Libro");
            Console.WriteLine("2. Eliminar Libro");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Seleccione una opción: ");
            string subOpcion = Console.ReadLine() ?? "";

            switch (subOpcion)
            {
                case "1":
                    string titulo = LeerTexto("Título:", requerido: true);
                    string autor = LeerTexto("Autor:", requerido: true);
                    string isbn = LeerTexto("ISBN:", requerido: true);
                    int copias = LeerEntero("Número total de copias:", min: 1);
                    var nuevoLibro = new Libro(0, titulo, autor, isbn, copias); // ID se asigna en el servicio
                    _servicioInventario.AgregarLibro(nuevoLibro, _usuarioActual!);
                    MostrarExito("Libro agregado correctamente.");
                    break;
                case "2":
                    AccionListarLibros(); // Mostrar libros para que el admin vea el ID
                    int idEliminar = LeerEntero("\nIngrese el ID del libro a eliminar:");
                    _servicioInventario.EliminarLibro(idEliminar, _usuarioActual!);
                    MostrarExito("Libro eliminado correctamente.");
                    break;
                case "0":
                    break;
                default:
                    MostrarError("Opción inválida.");
                    break;
            }
        }

        static void AccionGestionarUsuarios()
        {
            Console.WriteLine("\n--- Gestión de Usuarios ---");
            Console.WriteLine("1. Agregar Usuario");
            Console.WriteLine("2. Eliminar Usuario");
            Console.WriteLine("3. Modificar Rol (Pendiente)");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("Seleccione una opción: ");
            string subOpcion = Console.ReadLine() ?? "";

             switch (subOpcion)
            {
                case "1":
                    string nombre = LeerTexto("Nombre:", requerido: true);
                    string apellido = LeerTexto("Apellido:", requerido: true);
                    string correo = LeerTexto("Correo Electrónico:", requerido: true);
                    RolUsuario rol = LeerRolUsuario();
                    string detalle = "";
                    if (rol == RolUsuario.Estudiante) detalle = LeerTexto("Carrera:", requerido: true);
                    else if (rol == RolUsuario.Docente) detalle = LeerTexto("Facultad:", requerido: true);
                    else if (rol == RolUsuario.Administrador) detalle = LeerTexto("Departamento:", requerido: false);

                    _servicioPrestamo.CrearUsuario(nombre, apellido, correo, rol, detalle, _usuarioActual!);
                    MostrarExito("Usuario creado correctamente.");
                    break;
                case "2":
                     Console.WriteLine("\n--- Lista de Usuarios ---");
                     var usuarios = _servicioPrestamo.Usuarios;
                     foreach(var u in usuarios) Console.WriteLine($"ID: {u.IdUsuario} - {u.Nombre} {u.Apellido} ({u.Rol})");
                    int idEliminar = LeerEntero("\nIngrese el ID del usuario a eliminar:");
                    _servicioPrestamo.EliminarUsuario(idEliminar, _usuarioActual!);
                    MostrarExito("Usuario eliminado correctamente.");
                    break;
                 case "3":
                      MostrarAdvertencia("La funcionalidad para modificar roles directamente aún no está implementada.");
                     // Si se implementara, aquí iría la lógica para pedir ID, nuevo rol, nuevo detalle y llamar al servicio.
                     // _servicioPrestamo.ModificarRolUsuario(idUsuario, nuevoRol, nuevoDetalle, _usuarioActual!);
                     break;
                case "0":
                    break;
                default:
                    MostrarError("Opción inválida.");
                    break;
            }
        }

         static void AccionGestionarRecomendaciones()
         {
             Console.WriteLine("\n--- Gestión de Recomendaciones Pendientes ---");
             var pendientes = _servicioPrestamo.VerRecomendacionesPendientes(_usuarioActual!).ToList();

             if (!pendientes.Any())
             {
                 MostrarAdvertencia("No hay recomendaciones pendientes.");
                 return;
             }

             for(int i = 0; i < pendientes.Count; i++)
             {
                  var reco = pendientes[i];
                  var user = _servicioPrestamo.Usuarios.FirstOrDefault(u => u.IdUsuario == reco.IdUsuarioRecomienda);
                  Console.WriteLine($"{i + 1}. ID: {reco.IdRecomendacion}, Libro: '{reco.TituloLibro}' por '{reco.AutorLibro}', Recomendado por: {user?.Nombre ?? "Desconocido"} ({user?.Rol})");
             }
             Console.WriteLine("-------------------------");
             Console.WriteLine("A. Aprobar recomendación");
             Console.WriteLine("R. Rechazar recomendación");
             Console.WriteLine("0. Volver");
             Console.Write("Seleccione una opción o número de recomendación para ver detalle (no implementado): ");
             string subOpcion = Console.ReadLine()?.ToUpper() ?? "";

             switch(subOpcion)
             {
                 case "A":
                     int idAprobar = LeerEntero("Ingrese el ID de la recomendación a APROBAR:");
                     _servicioPrestamo.AprobarRecomendacion(idAprobar, _usuarioActual!);
                     MostrarExito("Recomendación aprobada.");
                     break;
                 case "R":
                     int idRechazar = LeerEntero("Ingrese el ID de la recomendación a RECHAZAR:");
                     string motivo = LeerTexto("Ingrese el motivo del rechazo:", requerido: true);
                     _servicioPrestamo.RechazarRecomendacion(idRechazar, motivo, _usuarioActual!);
                      MostrarExito("Recomendación rechazada.");
                     break;
                 case "0":
                     break;
                 default:
                     // Podríamos añadir lógica para ver detalles si se ingresa un número
                     MostrarError("Opción inválida.");
                     break;
             }
         }

        static void AccionAnularMulta()
        {
             Console.WriteLine("\n--- Anular Multa ---");
             // Mostrar multas pendientes podría ser útil aquí
             AccionVerTodasMultasPendientes(incluirPagadas: false); // Mostrar solo pendientes para anular

             var multasPendientes = _servicioPrestamo.Multas.Where(m => m.Estado == EstadoMulta.Pendiente).ToList();
             if(!multasPendientes.Any()) return; // Mensaje ya mostrado por AccionVerTodasMultasPendientes

             int idMulta = LeerEntero("Ingrese el ID de la multa a anular:");
             _servicioPrestamo.AnularMulta(idMulta, _usuarioActual!);
             MostrarExito("Multa anulada correctamente.");
        }

         static void AccionVerTodosPrestamosActivos()
        {
             Console.WriteLine("\n--- Todos los Préstamos Activos ---");
             var prestamos = _servicioPrestamo.Prestamos.Where(p => p.FechaDevolucionReal == null);
             if (!prestamos.Any())
             {
                 MostrarAdvertencia("No hay préstamos activos en el sistema.");
                 return;
             }
             foreach (var p in prestamos)
             {
                 var libro = _servicioInventario.ObtenerPorId(p.IdLibro);
                 var usuario = _servicioPrestamo.Usuarios.FirstOrDefault(u => u.IdUsuario == p.IdUsuario);
                 Console.WriteLine($"- ID Préstamo: {p.IdTransaccion}, Libro: '{libro?.Titulo ?? "?"}', Usuario: {usuario?.Nombre ?? "?"} ({usuario?.Rol}), Vence: {p.FechaDevolucionEsperada.ToShortDateString()}{(p.EstaVencido ? " [VENCIDO]" : "")}");
             }
        }

          static void AccionVerTodasMultasPendientes(bool incluirPagadas = false)
        {
             Console.WriteLine("\n--- Todas las Multas " + (incluirPagadas ? "" : "Pendientes") + " ---");
             var multas = incluirPagadas ? _servicioPrestamo.Multas : _servicioPrestamo.Multas.Where(m => m.Estado == EstadoMulta.Pendiente);
              if (!multas.Any())
             {
                 MostrarAdvertencia("No hay multas " + (incluirPagadas ? "en el sistema." : "pendientes."));
                 return;
             }
             foreach (var m in multas)
             {
                 var prestamoAsociado = _servicioPrestamo.Prestamos.FirstOrDefault(p => p.IdTransaccion == m.IdPrestamo);
                 var usuarioAsociado = prestamoAsociado != null ? _servicioPrestamo.Usuarios.FirstOrDefault(u => u.IdUsuario == prestamoAsociado.IdUsuario) : null;
                 Console.WriteLine($"- ID Multa: {m.IdMulta}, Monto: ${m.Monto:N2}, Estado: {m.Estado}, Usuario: {usuarioAsociado?.Nombre ?? "?"}, Préstamo ID: {m.IdPrestamo}");
             }
        }


        // --- Métodos de Utilidad (Entrada/Salida) ---

        static void MostrarError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {mensaje}");
            Console.ResetColor();
        }

        static void MostrarAdvertencia(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[AVISO] {mensaje}");
            Console.ResetColor();
        }

         static void MostrarExito(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[ÉXITO] {mensaje}");
            Console.ResetColor();
        }

        static int LeerEntero(string mensaje, bool requerido = true, int? min = null, int? max = null)
        {
            int valor = 0;
            bool valido = false;
            while (!valido)
            {
                Console.Write($"{mensaje} ");
                string? input = Console.ReadLine();

                if (!requerido && string.IsNullOrWhiteSpace(input))
                {
                    return 0; // Valor por defecto si no es requerido y se deja vacío
                }

                if (int.TryParse(input, out valor))
                {
                     valido = true; // Es un número
                     if (min.HasValue && valor < min.Value)
                     {
                          MostrarError($"El valor debe ser mayor o igual a {min.Value}.");
                          valido = false;
                     }
                     if (max.HasValue && valor > max.Value)
                     {
                          MostrarError($"El valor debe ser menor o igual a {max.Value}.");
                          valido = false;
                     }
                }
                else
                {
                    MostrarError("Entrada inválida. Por favor, ingrese un número entero.");
                }
            }
            return valor;
        }

        static string LeerTexto(string mensaje, bool requerido = true)
        {
            string? valor = null;
            while (string.IsNullOrWhiteSpace(valor))
            {
                Console.Write($"{mensaje} ");
                valor = Console.ReadLine();
                if (requerido && string.IsNullOrWhiteSpace(valor))
                {
                    MostrarError("Este campo es requerido.");
                }
                 else if (!requerido && string.IsNullOrWhiteSpace(valor))
                 {
                      return ""; // Devolver vacío si no es requerido y no se ingresa nada
                 }
            }
            return valor!; // Sabemos que no es null aquí si es requerido
        }

         static RolUsuario LeerRolUsuario()
        {
            Console.WriteLine("Seleccione el Rol:");
            var roles = Enum.GetValues(typeof(RolUsuario)).Cast<RolUsuario>().ToList();
            for(int i = 0; i < roles.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {roles[i]}");
            }

            int indice = -1;
            while(indice < 1 || indice > roles.Count)
            {
                indice = LeerEntero("Número del Rol:", min: 1, max: roles.Count);
                 if (indice < 1 || indice > roles.Count) MostrarError("Selección inválida.");
            }
            return roles[indice - 1];
        }

        static void MostrarDetalleLibro(Libro libro, bool simple = false)
        {
             if (simple)
             {
                Console.WriteLine($"- ID: {libro.IdLibro}, Título: '{libro.Titulo}', Autor: {libro.Autor}, Disponibles: {libro.CopiasDisponibles}/{libro.TotalCopias}");
             }
             else
             {
                Console.WriteLine($"  ID: {libro.IdLibro}");
                Console.WriteLine($"  Título: {libro.Titulo}");
                Console.WriteLine($"  Autor: {libro.Autor}");
                Console.WriteLine($"  ISBN: {libro.ISBN}");
                Console.WriteLine($"  Copias Totales: {libro.TotalCopias}");
                Console.WriteLine($"  Copias Disponibles: {libro.CopiasDisponibles}");
             }
        }
    }
}