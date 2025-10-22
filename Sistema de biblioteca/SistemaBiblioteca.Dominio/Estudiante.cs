// Archivo: SistemaBiblioteca.Dominio/Estudiante.cs

namespace SistemaBiblioteca.Dominio
{
    /// <summary>
    /// Representa a un usuario de tipo Estudiante. Hereda de Usuario (Generalización).
    /// </summary>
    public class Estudiante : Usuario
    {
        public string Carrera { get; set; } // Atributo de especialización

        /// <summary>
        /// Constructor para inicializar un Estudiante. Llama al constructor base.
        /// </summary>
        public Estudiante(int id, string nombre, string apellido, string correo, string carrera)
            : base(id, nombre, apellido, correo, "Estudiante") // Llama al constructor del padre
        {
            Carrera = carrera;
        }

        // ----------------------------------------------------
        // POLIMORFISMO (Sobrescritura - Overriding)
        // ----------------------------------------------------

        /// <summary>
        /// Sobreescribe la regla de préstamo: Estudiantes mantienen el límite base.
        /// </summary>
        /// <returns>Límite de 3 libros.</returns>
        public override int PedirLibro()
        {
            // Regla de Negocio Específica para Estudiantes
            // Aunque coincide con la base, demuestra que la subclase tiene control sobre la regla.
            return 3; 
        }

        /// <summary>
        /// Sobreescritura simple para confirmar la devolución.
        /// </summary>
        public override void DevolverLibro()
        {
            // Podríamos añadir aquí lógica específica del Estudiante si fuera necesario.
            base.DevolverLibro(); // Llama a la lógica base de la clase Usuario
        }
    }
}