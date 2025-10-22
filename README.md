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

## 1. Navegar a la Carpeta Raíz del Proyecto
Primero, el usuario debe abrir la terminal (PowerShell, CMD, o Terminal de VS Code) y navegar al directorio principal de la solución (donde se encuentra el archivo SistemaBiblioteca.sln).

Bash

cd /ruta/a/SistemaBiblioteca
## 2. Restaurar Dependencias (Opcional, pero Recomendado)
Aunque los proyectos están referenciados, es una buena práctica forzar al SDK de .NET a descargar y restaurar cualquier paquete NuGet y dependencia externa que falte.

Bash

dotnet restore
## 3. Ejecutar Pruebas Unitarias (Validación)
Para confirmar que toda la lógica de negocio (clases Libro y Prestamo) funciona como se espera, se ejecutan las pruebas. Este es el resultado clave para la documentación.

Bash

dotnet test
## 4. Ejecutar la Aplicación de Consola (Demo Principal)
Finalmente, este comando compila el proyecto de Consola y lo ejecuta, mostrando el flujo de la simulación (Polimorfismo, Excepciones, Multas).

Bash

dotnet run --project SistemaBiblioteca.Consola
