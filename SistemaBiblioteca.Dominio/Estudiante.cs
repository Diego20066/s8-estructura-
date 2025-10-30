// Archivo: SistemaBiblioteca.Dominio/Estudiante.cs
namespace SistemaBiblioteca.Dominio
{
    public class Estudiante : Usuario
    {
        public string Carrera { get; set; }

        public Estudiante(int id, string nombre, string apellido, string correo, string carrera)
             // Pasamos RolUsuario.Estudiante al constructor base
            : base(id, nombre, apellido, correo, RolUsuario.Estudiante)
        {
            Carrera = carrera;
        }

        // Límite de libros (ya existente, coincide con la base pero es bueno dejarlo explícito)
        public override int PedirLibro()
        {
            return 3;
        }

        // No necesitamos sobrescribir DiasPrestamoBase ni MaxDiasExtension
        // porque los valores por defecto de la clase Usuario ya son los correctos para Estudiante (15 y 45).

        public override void DevolverLibro()
        {
            base.DevolverLibro();
        }

        // Podríamos sobrescribir RecomendarLibro si fuera necesario
        // public override void RecomendarLibro(string titulo, string autor) { ... }
    }
}