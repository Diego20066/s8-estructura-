// Archivo: SistemaBiblioteca.Dominio/Docente.cs

namespace SistemaBiblioteca.Dominio
{
    /// <summary>
    /// Representa a un usuario de tipo Docente. Hereda de Usuario (Generalización).
    /// </summary>
    public class Docente : Usuario
    {
        public string Facultad { get; set; } // Atributo de especialización

        /// <summary>
        /// Constructor para inicializar un Docente. Llama al constructor base.
        /// </summary>
        public Docente(int id, string nombre, string apellido, string correo, string facultad)
            : base(id, nombre, apellido, correo, "Docente") // Llama al constructor del padre
        {
            Facultad = facultad;
        }

        // ----------------------------------------------------
        // POLIMORFISMO (Sobrescritura - Overriding)
        // ----------------------------------------------------

        /// <summary>
        /// Sobreescribe la regla de préstamo: Docentes tienen un límite superior.
        /// </summary>
        /// <returns>Límite de 5 libros.</returns>
        public override int PedirLibro()
        {
            // Regla de Negocio Específica para Docentes:
            // Tienen un límite más alto debido a necesidades de investigación/clase.
            return 5; 
        }
    }
}