##Descripción
Este sistema de gestión de biblioteca en C# está diseñado para demostrar principios clave de la Programación Orientada a Objetos (POO) y validar reglas de negocio mediante un flujo de trabajo simulado en consola.
##1. Usuarios y Polimorfismo

Propósito: Mostrar herencia y polimorfismo.
Ejecución: Se crean instancias de Estudiante (Ana) y Docente (Dr. Ricardo), ambos usan PedirLibro().
Resultado: Ana puede pedir hasta 3 libros, Dr. Ricardo hasta 5.
POO Aplicada: Método sobrescrito (override) en subclases, misma llamada con comportamiento diferente.

##2. Préstamo Exitoso

Propósito: Demostrar encapsulamiento y composición.
Ejecución: Se presta el libro "Cien Años de Soledad".
Resultado: Copias disponibles bajan de 5 a 4.
POO Aplicada: La consola usa ServicioPrestamo, que llama a ActualizarDisponibilidad() del objeto Libro.

##3. Manejo de Excepciones

Propósito: Validar robustez ante estados inválidos.
Ejecución: Se agotan las copias del Libro 2 y se intenta un préstamo adicional.
Resultado: Se lanza y captura una InvalidOperationException.
POO Aplicada: Uso de try-catch para manejar errores de negocio.

##4. Devolución Tardía y Multa

Propósito: Aplicar lógica de negocio y clases abstractas.
Ejecución: Se simula una devolución con 3 días de retraso.
Resultado: Se genera multa de $1.50.
POO Aplicada: Prestamo hereda de TransaccionBase y sobrecarga CalcularCosto() para aplicar la fórmula:
Multa=Dıˊas de retraso×Tarifa diaria\text{Multa} = \text{Días de retraso} \times \text{Tarifa diaria}Multa=Dıˊas de retraso×Tarifa diaria
