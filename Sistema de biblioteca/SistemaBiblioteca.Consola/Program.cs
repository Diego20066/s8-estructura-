using System;
using System.Linq;
using SistemaBiblioteca.Aplicacion;
using SistemaBiblioteca.Dominio;

namespace SistemaBiblioteca.Consola
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================================================");
            Console.WriteLine("  📚 SISTEMA DE GESTIÓN DE BIBLIOTECA (C# POO)  ");
            Console.WriteLine("=================================================");

            // 1. Inicialización de Servicios (Composición e Inyección de Dependencias)
            var servicioInventario = new ServicioInventario();
            var servicioPrestamo = new ServicioPrestamo(servicioInventario);

            Console.WriteLine("\n--- 1. USUARIOS Y POLIMORFISMO (Límites de Préstamo) ---");

            // Ahora usamos las propiedades públicas en lugar de campos privados
            var ana = servicioPrestamo.Usuarios.First(u => u.Nombre == "Ana"); // Estudiante
            var ricardo = servicioPrestamo.Usuarios.First(u => u.Nombre.Contains("Ricardo")); // Docente

            Console.WriteLine($"-> {ana.Nombre} ({ana.TipoUsuario}) tiene un límite de: {ana.PedirLibro()} libros.");
            Console.WriteLine($"-> {ricardo.Nombre} ({ricardo.TipoUsuario}) tiene un límite de: {ricardo.PedirLibro()} libros.");

            Console.WriteLine("\n--- 2. PRÉSTAMO EXITOSO (Estudiante pide libro 1) ---");
            try
            {
                var prestamoAna = servicioPrestamo.EmitirLibro(idUsuario: 1, idLibro: 1);
                var libro1 = servicioInventario.ObtenerPorId(1);

                Console.WriteLine($"[OK] Préstamo {prestamoAna.IdTransaccion} emitido para Ana.");
                Console.WriteLine($"[INFO] Libro 1: Copias disponibles restantes: {libro1?.CopiasDisponibles ?? 0}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Falló el préstamo: {ex.Message}");
            }

            Console.WriteLine("\n--- 3. MANEJO DE EXCEPCIONES (Préstamo sin Copias) ---");

            servicioPrestamo.EmitirLibro(idUsuario: 1, idLibro: 2);
            servicioPrestamo.EmitirLibro(idUsuario: 1, idLibro: 2);
            servicioPrestamo.EmitirLibro(idUsuario: 2, idLibro: 2);

            Console.WriteLine($"[INFO] Intentando prestar Libro 2 por quinta vez...");

            try
            {
                servicioPrestamo.EmitirLibro(idUsuario: 2, idLibro: 2);
            }
            catch (InvalidOperationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FAIL] Excepción capturada (Correcto): {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\n--- 4. DEVOLUCIÓN TARDÍA Y CÁLCULO DE MULTA ---");

            int idLibroVencido = 1;
            int idPrestamoVencido = 1;
            var prestamoVencido = servicioPrestamo.Prestamos.First(p => p.IdTransaccion == idPrestamoVencido);

            prestamoVencido.FechaTransaccion = DateTime.Now.AddDays(-10);
            prestamoVencido.FechaDevolucionEsperada = prestamoVencido.FechaTransaccion.AddDays(7);

            Console.WriteLine($"[INFO] Préstamo {idPrestamoVencido} vencido. Fecha esperada: {prestamoVencido.FechaDevolucionEsperada.ToShortDateString()}");

            try
            {
                var multaGenerada = servicioPrestamo.DevolverLibro(idPrestamoVencido);
                var libroDevuelto = servicioInventario.ObtenerPorId(idLibroVencido);

                if (multaGenerada != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[ALERTA] Multa GENERADA: ${multaGenerada.Monto:N2} por {multaGenerada.EstadoPago}.");
                    Console.ResetColor();
                }

                if (libroDevuelto != null)
                {
                    Console.WriteLine($"[INFO] Libro {idLibroVencido}: Copias disponibles después de devolución: {libroDevuelto.CopiasDisponibles}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[WARN] Libro {idLibroVencido} no encontrado al devolver; no se pueden mostrar copias disponibles.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Falló la devolución: {ex.Message}");
            }

            Console.WriteLine("\n=================================================");
            Console.WriteLine("    FIN DE LA SIMULACIÓN Y DEMOSTRACIÓN POO");
            Console.WriteLine("=================================================");
        }
    }
}
