# BibliotecaApp

Proyecto de ejemplo: Sistema de gestión de biblioteca en .NET 8 (MSTest).
Estructura:
- Biblioteca.Domain (entidades, interfaces, servicios)
- Biblioteca.App (aplicación de consola)
- Biblioteca.Tests (pruebas unitarias con MSTest)

Abrir la solución `BibliotecaApp.sln` en Visual Studio o ejecutar con `dotnet`:

Restaurar paquetes:
dotnet restore

Compilar:
dotnet build

Ejecutar la app:
cd Biblioteca.App
dotnet run

Ejecutar tests:
cd Biblioteca.Tests
dotnet test
