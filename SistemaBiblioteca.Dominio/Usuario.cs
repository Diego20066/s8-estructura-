// Archivo: SistemaBiblioteca.Dominio/Usuario.cs
namespace SistemaBiblioteca.Dominio
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string CorreoElectronico { get; set; }
        // Cambiamos TipoUsuario string por el enum RolUsuario
        public RolUsuario Rol { get; protected set; } // protected set para que solo clases hijas puedan cambiarlo si es necesario
        public DateTime FechaRegistro { get; set; }

        // Modificamos el constructor para aceptar RolUsuario
        public Usuario(int id, string nombre, string apellido, string correo, RolUsuario rol)
        {
            IdUsuario = id;
            Nombre = nombre;
            Apellido = apellido;
            CorreoElectronico = correo;
            Rol = rol; // Asignamos el rol
            FechaRegistro = DateTime.Today;
        }

        // Método virtual para límite de libros (ya existente)
        public virtual int PedirLibro()
        {
            return 3; // Límite base (Estudiante)
        }

        // NUEVO: Método virtual para días base de préstamo
        public virtual int DiasPrestamoBase()
        {
             // Por defecto 15 días para Estudiante o rol no especificado
             return 15;
        }

        // NUEVO: Método virtual para días máximos de extensión
        public virtual int MaxDiasExtension()
        {
             // Por defecto 45 días para Estudiante
             return 45;
        }


        public virtual void DevolverLibro()
        {
            Console.WriteLine($"[Sistema]: Se ha iniciado el proceso de devolución para el usuario {Nombre}.");
        }

        // NUEVO: Método para recomendar (simple por ahora)
        // Lo ponemos virtual por si queremos añadir lógica específica por rol en el futuro
        public virtual void RecomendarLibro(string titulo, string autor)
        {
             // La lógica real estará en ServicioPrestamo o un nuevo ServicioRecomendacion
             Console.WriteLine($"[Usuario {Nombre}]: Recomienda el libro '{titulo}' por '{autor}'.");
        }
    }
}