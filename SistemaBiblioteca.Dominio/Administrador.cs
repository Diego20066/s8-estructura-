// Archivo: SistemaBiblioteca.Dominio/Administrador.cs
namespace SistemaBiblioteca.Dominio
{
    public class Administrador : Usuario
    {
        // Podríamos añadir propiedades específicas del admin si las hubiera, ej: NivelAcceso
        public string DepartamentoAdmin { get; set; }

        public Administrador(int id, string nombre, string apellido, string correo, string departamento = "General")
            // Pasamos RolUsuario.Administrador al constructor base
            : base(id, nombre, apellido, correo, RolUsuario.Administrador)
        {
            DepartamentoAdmin = departamento;
        }

        // Decidimos qué reglas aplican a los Admins para préstamos
        // Opción 1: No pueden pedir prestado
        /*
        public override int PedirLibro() => 0;
        public override int DiasPrestamoBase() => 0;
        public override int MaxDiasExtension() => 0;
        */

        // Opción 2: Tienen los mismos privilegios que un Docente (o más)
        public override int PedirLibro() => 5; // O un número más alto
        public override int DiasPrestamoBase() => 30;
        public override int MaxDiasExtension() => 75;

        // Los administradores no recomiendan libros usualmente
        // public override void RecomendarLibro(string titulo, string autor) { ... }
    }
}