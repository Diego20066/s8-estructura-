// Archivo: SistemaBiblioteca.Dominio/Usuario.cs

namespace SistemaBiblioteca.Dominio
{
    /// <summary>
    /// Representa al usuario base del sistema de biblioteca.
    /// Define propiedades y métodos comunes (generalización).
    /// </summary>
    public class Usuario
    {
        // Propiedades comunes (Encapsulamiento con auto-properties)
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CorreoElectronico { get; set; }
        public string TipoUsuario { get; private set; } // Solo se define en el constructor o por herencia
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Constructor principal para inicializar un objeto Usuario.
        /// </summary>
        public Usuario(int id, string nombre, string apellido, string correo, string tipo)
        {
            IdUsuario = id;
            Nombre = nombre;
            Apellido = apellido;
            CorreoElectronico = correo;
            TipoUsuario = tipo;
            FechaRegistro = DateTime.Today;
        }

        // ----------------------------------------------------
        // POLIMORFISMO (Métodos Virtuales)
        // ----------------------------------------------------

        /// <summary>
        /// Define la capacidad máxima de libros que el usuario puede pedir.
        /// Este método es virtual, permitiendo que las subclases implementen reglas específicas.
        /// </summary>
        /// <returns>El límite máximo de libros que puede pedir.</returns>
        public virtual int PedirLibro()
        {
            // Regla de Negocio Base (regla por defecto del sistema)
            // Cualquier usuario genérico tiene un límite predeterminado.
            return 3;
        }

        /// <summary>
        /// Simula la acción de devolver un libro.
        /// Es virtual para permitir que las subclases añadan lógica de seguimiento específica.
        /// </summary>
        public virtual void DevolverLibro()
        {
            // Lógica común de devolución (ej: notificar al sistema para cerrar el préstamo)
            Console.WriteLine($"[Sistema]: Se ha iniciado el proceso de devolución para el usuario {Nombre}.");
        }
    }
}