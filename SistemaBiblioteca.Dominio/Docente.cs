// Archivo: SistemaBiblioteca.Dominio/Docente.cs
namespace SistemaBiblioteca.Dominio
{
    public class Docente : Usuario
    {
        public string Facultad { get; set; }

        public Docente(int id, string nombre, string apellido, string correo, string facultad)
            // Pasamos RolUsuario.Docente al constructor base
            : base(id, nombre, apellido, correo, RolUsuario.Docente)
        {
            Facultad = facultad;
        }

        // Límite de libros (ya existente)
        public override int PedirLibro()
        {
            return 5;
        }

        // NUEVO: Sobrescribimos días base de préstamo
        public override int DiasPrestamoBase()
        {
             return 30; // Docentes tienen más tiempo base
        }

        // NUEVO: Sobrescribimos días máximos de extensión
        public override int MaxDiasExtension()
        {
             return 75; // Docentes pueden extender más
        }

         // Podríamos sobrescribir DevolverLibro o RecomendarLibro si fuera necesario
         // public override void DevolverLibro() { ... }
         // public override void RecomendarLibro(string titulo, string autor) { ... }
    }
}