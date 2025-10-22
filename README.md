#  Sistema de Gestión de Biblioteca

##  Descripción
Sistema desarrollado en **C#** para administrar préstamos y devoluciones de libros en una biblioteca. Incluye gestión de usuarios, control de disponibilidad de libros, cálculo de multas por devoluciones tardías y manejo de excepciones.

##  Funcionalidades Principales

- **Gestión de Usuarios con Polimorfismo**  
  Cada tipo de usuario (estudiante, docente) tiene un límite de préstamo diferente.  
  Ejemplo: Estudiantes pueden tomar hasta 3 libros, docentes hasta 5.

- **Préstamos de Libros**  
  Verifica disponibilidad y actualiza el inventario tras un préstamo exitoso.

- **Manejo de Excepciones**  
  Si no hay copias disponibles, se lanza una excepción controlada.

- **Devoluciones y Multas**  
  Calcula multas automáticamente si la devolución es tardía, según los días de retraso.

##  Estructura del Proyecto

- `SistemaBibliotecaAplicacion`: Lógica principal del sistema.
- `SistemaBibliotecaDominio`: Entidades como Libros y Usuarios.
- `SistemaBibliotecaConsola`: Interfaz de línea de comandos para interactuar con el sistema.

##  Ejecución

Para correr el sistema desde la consola:

```bash
dotnet run --project SistemaBibliotecaConsola
