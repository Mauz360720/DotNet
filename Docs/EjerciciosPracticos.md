# Ejercicios Prácticos: Unit Testing y Bogus

Pon a prueba tus conocimientos sobre el patrón **AAA** y la generación de datos con **Bogus** con estos ejercicios.

---

## Ejercicio 1: El Patrón AAA Básico
**Objetivo:** Crear una prueba unitaria para una lógica simple de conversión de temperatura.

1. Imagina que tienes una clase `ConvertidorTemperatura` con un método `CelsiusAFahrenheit(double celsius)`.
2. La fórmula es: `(celsius * 9 / 5) + 32`.
3. Escribe una prueba unitaria siguiendo estrictamente los comentarios `// Arrange`, `// Act` y `// Assert`.

**Reto:** Prueba qué sucede cuando el valor de entrada es `0`.

---

## Ejercicio 2: Generación de Objetos con Bogus
**Objetivo:** Practicar la configuración de reglas (`RuleFor`) para objetos complejos.

1. Define una clase `Producto` con las siguientes propiedades:
    - `Guid Id`
    - `string Nombre`
    - `string Categoria`
    - `decimal Precio`
    - `int Stock`
2. Crea un `Faker<Producto>` que genere datos realistas:
    - El `Precio` debe estar entre 1 y 1000.
    - El `Stock` debe ser un número entero entre 0 y 50.
    - Usa las categorías de Bogus adecuadas para `Nombre` y `Categoria`.

---

## Ejercicio 3: Lógica de Negocio y Bogus
**Objetivo:** Combinar una clase con lógica y pruebas que usen datos aleatorios.

**Escenario:** Tienes una clase `ValidadorSuscripcion` con un método `EsElite(Usuario usuario)`. 
- Un usuario es "Elite" si su `Edad` es mayor a 18 **y** su `Saldo` es mayor a 10,000.

**Instrucciones:**
1. Crea la clase `Usuario` con `Nombre`, `Edad` y `Saldo`.
2. Escribe una prueba unitaria donde uses Bogus para generar un usuario que cumpla AMBAS condiciones.
3. Verifica con `Assert.True` que el método funciona correctamente.

---

## Ejercicio 4: Uso Avanzado de f.Random
**Objetivo:** Usar selecciones de listas y enums.

1. Crea un Enum `TipoVehiculo { Coche, Moto, Camion }`.
2. Crea una clase `Vehiculo` con `Marca` y `Tipo`.
3. Configura un `Faker<Vehiculo>` donde:
    - La `Marca` se elija aleatoriamente de una lista manual: `new[] { "Toyota", "Ford", "Tesla" }`.
    - El `Tipo` se elija aleatoriamente del Enum `TipoVehiculo`.

---

## Ejercicio 5: El "Bug" Escondido
**Objetivo:** Aprender por qué los Unit Tests son importantes.

1. Implementa este método:
```csharp
public int Dividir(int a, int b) {
    return a / b;
}
```
2. Crea una prueba unitaria que use Bogus para generar el valor `a` (cualquier número), pero fija el valor `b` en `0`.
3. ¿Qué sucede al ejecutar la prueba? ¿Cómo podrías mejorar el código original para manejar este caso?

---

# Ejercicios de LINQ (Consultas de Datos)

Utiliza la librería **Bogus** para generar los datos iniciales de estos ejercicios y **LINQ** para resolver las preguntas.

## Ejercicio 6: Filtrado y Conteo
**Objetivo:** Practicar `Where` y `Count`.

1. Crea una clase `Pedido` con `Id`, `Cliente` y `Estado` (string: "Pendiente", "Enviado", "Cancelado").
2. Genera una lista de 100 pedidos con Bogus usando `f.PickRandom` para el estado.
3. **Misión:** ¿Cuántos pedidos están en estado "Cancelado"?

---

## Ejercicio 7: Transformación y Proyección
**Objetivo:** Practicar `Select` y objetos anónimos.

1. Crea una clase `Empleado` con `Nombre`, `Salario` y `Departamento`.
2. Genera una lista de 20 empleados.
3. **Misión:** Crea una nueva lista que contenga solo el `Nombre` y el `Salario` aumentado en un 10% (Bono de productividad) de todos los empleados.

---

## Ejercicio 8: El Ranking (Ordenamiento)
**Objetivo:** Practicar `OrderByDescending` y `Take`.

1. Crea una clase `Videojuego` con `Titulo` y `Calificacion` (double entre 0 y 100).
2. Genera una lista aleatoria de videojuegos.
3. **Misión:** Obtén los 5 videojuegos con las mejores calificaciones, ordenados de mayor a menor.

---

## Ejercicio 9: Agrupamiento por Categoría
**Objetivo:** Practicar `GroupBy`.

1. Usa la misma lista de `Empleado` del Ejercicio 7.
2. **Misión:** Agrupa a los empleados por `Departamento` y muestra en consola el nombre del departamento y cuántos empleados pertenecen a él.

---

## Ejercicio 10: Combinación de Todo
**Objetivo:** Encadenamiento de métodos LINQ.

1. Crea una clase `Venta` con `Producto`, `Cantidad`, `PrecioUnitario` y `Fecha`.
2. Genera datos para el último mes.
3. **Misión:** Calcula el **Total de Ingresos** (Suma de Cantidad * PrecioUnitario) únicamente de las ventas del producto "Laptop" realizadas en la última semana.

---
*¡Buena suerte! Recuerda que la clave de un buen Unit Test es que sea fácil de leer y que pruebe una sola cosa a la vez.*
