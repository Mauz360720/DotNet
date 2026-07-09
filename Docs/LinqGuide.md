# Guía de LINQ para Principiantes (con Bogus)

**LINQ** (Language Integrated Query) es una herramienta poderosa en C# que te permite realizar consultas sobre colecciones de datos (listas, arrays, etc.) de una manera legible y eficiente, de forma muy parecida a como harías consultas SQL pero directamente en tu código.

---

## 1. Conceptos Básicos

Para usar LINQ, asegúrate de tener el namespace:
```csharp
using System.Linq;
```

Las operaciones de LINQ no suelen modificar la lista original, sino que devuelven una nueva secuencia o un valor específico.

---

## 2. Preparando nuestros datos con Bogus

Para estos ejemplos, usaremos **Bogus** para generar una lista de objetos `Estudiante` realistas.

```csharp
public class Estudiante
{
    public string Nombre { get; set; }
    public string Carrera { get; set; }
    public int Edad { get; set; }
    public double Promedio { get; set; }
}

// Generamos 50 estudiantes aleatorios
var faker = new Faker<Estudiante>()
    .RuleFor(e => e.Nombre, f => f.Name.FullName())
    .RuleFor(e => e.Carrera, f => f.PickRandom(new[] { "Sistemas", "Derecho", "Medicina", "Arte" }))
    .RuleFor(e => e.Edad, f => f.Random.Int(18, 30))
    .RuleFor(e => e.Promedio, f => f.Random.Double(0, 10));

List<Estudiante> estudiantes = faker.Generate(50);
```

---

## 3. Operaciones más comunes de LINQ

### A. Filtrado: `Where`
Se usa para obtener elementos que cumplan una condición.

```csharp
// Obtener solo estudiantes de "Sistemas"
var ingenieros = estudiantes.Where(e => e.Carrera == "Sistemas").ToList();

// Estudiantes mayores de 25 años
var mayoresDe25 = estudiantes.Where(e => e.Edad > 25).ToList();
```

### B. Selección: `Select`
Se usa para transformar los elementos o extraer solo una parte de ellos (proyección).

```csharp
// Solo queremos los nombres de los estudiantes (proyectar a una lista de strings)
var nombres = estudiantes.Select(e => e.Nombre).ToList();

// Crear un objeto anónimo con solo Nombre y Promedio
var resumen = estudiantes.Select(e => new { e.Nombre, e.Promedio }).ToList();
```

### C. Ordenamiento: `OrderBy` y `OrderByDescending`
Para organizar los datos.

```csharp
// Ordenar por promedio de menor a mayor
var rankingAsc = estudiantes.OrderBy(e => e.Promedio).ToList();

// Ordenar por edad de mayor a menor
var veteranos = estudiantes.OrderByDescending(e => e.Edad).ToList();
```

### D. Búsqueda de un solo elemento: `First`, `FirstOrDefault`, `Single`
- `First()`: Devuelve el primer elemento. Explota si no hay ninguno.
- `FirstOrDefault()`: Devuelve el primero o `null` si la lista está vacía. **(El más recomendado)**.

```csharp
// Obtener el primer estudiante que se llame "Juan"
var juan = estudiantes.FirstOrDefault(e => e.Nombre.Contains("Juan"));
```

### E. Cuantificadores: `Any` y `All`
- `Any()`: Devuelve `true` si al menos un elemento cumple la condición.
- `All()`: Devuelve `true` solo si TODOS cumplen la condición.

```csharp
// ¿Hay algún estudiante con promedio perfecto (10)?
bool hayExcelentes = estudiantes.Any(e => e.Promedio == 10);

// ¿Todos son mayores de edad?
bool todosAdultos = estudiantes.All(e => e.Edad >= 18);
```

### F. Agregación: `Count`, `Sum`, `Average`, `Max`, `Min`
Para realizar cálculos matemáticos sobre la colección.

```csharp
// ¿Cuántos estudiantes hay en Sistemas?
int totalSistemas = estudiantes.Count(e => e.Carrera == "Sistemas");

// Promedio de edad de todos los estudiantes
double promedioEdad = estudiantes.Average(e => e.Edad);

// El promedio de calificación más alto
double mejorPromedio = estudiantes.Max(e => e.Promedio);
```

### G. Agrupamiento: `GroupBy`
Se usa para agrupar elementos que comparten una característica común (como una categoría o departamento). El resultado es una colección de grupos, donde cada grupo tiene una **`Key`** (el valor por el cual agrupaste).

```csharp
// Agrupar estudiantes por su Carrera
var gruposPorCarrera = estudiantes.GroupBy(e => e.Carrera);

foreach (var grupo in gruposPorCarrera)
{
    Console.WriteLine($"Carrera: {grupo.Key}"); // La "Key" es el nombre de la carrera
    Console.WriteLine($"Cantidad de alumnos: {grupo.Count()}");
    
    // También puedes iterar dentro de cada grupo
    foreach (var estudiante in grupo)
    {
        Console.WriteLine($" - {estudiante.Nombre}");
    }
}
```

---

## 4. Encadenamiento (Fluent Syntax)

Lo mejor de LINQ es que puedes combinar varias operaciones en una sola línea (cadena):

```csharp
// Queremos los nombres de los 3 mejores estudiantes de Sistemas, ordenados por promedio
var top3Sistemas = estudiantes
    .Where(e => e.Carrera == "Sistemas")
    .OrderByDescending(e => e.Promedio)
    .Take(3)
    .Select(e => e.Nombre)
    .ToList();
```

## Resumen Rápido para Principiantes
1. **`Where`**: Filtrar.
2. **`Select`**: Transformar / Elegir columnas.
3. **`OrderBy`**: Ordenar.
4. **`FirstOrDefault`**: Buscar uno solo de forma segura.
5. **`Any`**: Verificar si algo existe.
6. **`Count/Average`**: Cálculos.

---
*Nota: Siempre usa `.ToList()` o `.ToArray()` al final de tu consulta si necesitas que los datos se procesen en ese mismo instante y se guarden en una lista.*
