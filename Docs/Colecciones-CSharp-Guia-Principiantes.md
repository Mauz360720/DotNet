# Guía para principiantes: Colecciones en .NET C#

Checklist de lo que incluye esta guía:

- [x] Explicación de colecciones básicas: arrays, `List<T>`
- [x] Interfaces de colecciones: `IEnumerable<T>`, `ICollection<T>`, `IList<T>`
- [x] Colecciones asociativas: `Dictionary<TKey, TValue>`, `SortedDictionary`, `Lookup`
- [x] Colecciones de conjunto: `HashSet<T>`, `SortedSet<T>`
- [x] Estructuras FIFO/LIFO: `Queue<T>`, `Stack<T>`
- [x] Colecciones reactivas/observables: `ObservableCollection<T>`
- [x] Notas de rendimiento y cuándo usar cada una
- [x] Ejemplos en C# y recomendaciones de uso
- [x] Ejercicios prácticos al final

Esta guía está pensada para principiantes y muestra con ejemplos simples cómo y cuándo usar las colecciones más comunes en C#.

---

## Contenido (sugerido para aprender en orden)

1. Arrays
2. `IEnumerable<T>`, `ICollection<T>`, `IList<T>`
3. `List<T>`
4. `Dictionary<TKey, TValue>` y colecciones asociativas
5. `HashSet<T>` y `SortedSet<T>`
6. `Queue<T>` y `Stack<T>`
7. Otras colecciones útiles (`ObservableCollection<T>`, `Concurrent`)
8. LINQ y colecciones
9. Ejercicios prácticos

---

## 1) Arrays

- Un array (`T[]`) tiene tamaño fijo y acceso por índice en O(1).
- Útil cuando conoces el tamaño de antemano o buscas máximo rendimiento y menor overhead.

Ejemplo:

```csharp
int[] numeros = new int[3];
numeros[0] = 10;
numeros[1] = 20;
numeros[2] = 30;
Console.WriteLine(numeros[1]); // 20
```

Cuando no conoces el tamaño o necesitas añadir/eliminar elementos frecuentemente, usa `List<T>`.

---

## 2) Interfaces: `IEnumerable<T>`, `ICollection<T>`, `IList<T>`

- `IEnumerable<T>`: la interfaz más básica; permite iterar con `foreach`. No garantiza operaciones de conteo rápidas ni mutabilidad.
- `ICollection<T>`: hereda de `IEnumerable<T>` y añade `Count`, `Add`, `Remove`, `Contains`.
- `IList<T>`: hereda de `ICollection<T>` y añade acceso por índice, `Insert` y `RemoveAt`.

Ejemplo:

```csharp
void Imprimir<T>(IEnumerable<T> secuencia)
{
    foreach(var item in secuencia)
        Console.WriteLine(item);
}

var lista = new List<string> { "a", "b", "c" };
Imprimir(lista); // List<T> implementa IEnumerable<T>
```

Consejo: acepta y devuelve `IEnumerable<T>` cuando sólo necesitas iterar; acepta `IList<T>` o `ICollection<T>` cuando necesites mutar o conocer `Count` eficientemente.

---

## 3) `List<T>`

- Implementa `IList<T>` y es la colección dinámica más usada.
- Inserción al final es amortizada O(1). Inserción o eliminación en medio es O(n).
- Buen balance entre facilidad de uso y rendimiento.

Ejemplo:

```csharp
var list = new List<int> {1,2,3};
list.Add(4);
list.Insert(1, 99); // inserta en índice 1
list.Remove(2); // elimina el elemento 2 (por valor)
Console.WriteLine(string.Join(", ", list));
```

Uso típico: cuando construyes colecciones en memoria, agregas y recorres elementos frecuentemente.

---

## 4) Colecciones asociativas: `Dictionary<TKey, TValue>` y variantes

- `Dictionary<TKey, TValue>`: mapa hash que ofrece acceso O(1) promedio por clave.
- `SortedDictionary<TKey, TValue>`: mapa ordenado por clave (árbol balanceado), acceso O(log n).
- `Lookup<TKey, TElement>`: parecido a `Dictionary<TKey, List<T>>` pero inmutable y creado con LINQ `ToLookup()`.

Ejemplo `Dictionary`:

```csharp
var dict = new Dictionary<string, int>();
dict["uno"] = 1;
dict.Add("dos", 2);
if (dict.TryGetValue("tres", out int val)) Console.WriteLine(val);
else Console.WriteLine("Clave no encontrada");
```

Consejos:
- Usa `TryGetValue` para evitar excepciones al buscar claves.
- Preferir `Dictionary` cuando necesitas búsqueda rápida por clave.

---

## 5) Colecciones de conjunto: `HashSet<T>` y `SortedSet<T>`

- `HashSet<T>`: conjunto basado en hash con operaciones O(1) promedio para `Add`, `Contains`, `Remove`. No permite duplicados.
- `SortedSet<T>`: mantiene elementos ordenados y operaciones O(log n).

Ejemplos:

```csharp
var hs = new HashSet<int> {1,2,3};
hs.Add(2); // no se añade, ya existe
Console.WriteLine(hs.Contains(3)); // true

var sset = new SortedSet<int> {5,1,3};
foreach(var x in sset) Console.WriteLine(x); // 1,3,5 (ordenado)
```

Operaciones útiles en `HashSet<T>`: `UnionWith`, `IntersectWith`, `ExceptWith`.

---

## 6) `Queue<T>` y `Stack<T>`

- `Queue<T>`: FIFO (First-In-First-Out). Operaciones: `Enqueue` (O(1)), `Dequeue` (O(1)), `Peek`.
- `Stack<T>`: LIFO (Last-In-First-Out). Operaciones: `Push`, `Pop`, `Peek`.

Ejemplo:

```csharp
var q = new Queue<string>();
q.Enqueue("a"); q.Enqueue("b");
Console.WriteLine(q.Dequeue()); // "a"

var st = new Stack<int>();
st.Push(1); st.Push(2);
Console.WriteLine(st.Pop()); // 2
```

Usa `Queue` para procesar tareas en orden; `Stack` para backtracking o expresiones (por ejemplo, convertir infijo a postfijo).

---

## 7) Otras colecciones útiles

- `ObservableCollection<T>`: útil en aplicaciones XAML (WPF, Xamarin) porque notifica cambios (`INotifyCollectionChanged`).
- `ConcurrentDictionary<TKey,TValue>`, `ConcurrentQueue<T>`, `ConcurrentBag<T>`: colecciones seguras para acceso concurrente sin bloqueo manual.
- `LinkedList<T>`: lista doblemente enlazada; operaciones O(1) para inserción/eliminación si ya tienes el nodo pero navegación O(n).

Ejemplo `ObservableCollection` mínimo:

```csharp
using System.Collections.ObjectModel;

var oc = new ObservableCollection<string>();
oc.CollectionChanged += (s,e) => Console.WriteLine($"Cambio: {e.Action}");
oc.Add("hola");
```

---

## 8) LINQ y colecciones

LINQ (Language Integrated Query) permite consultar y transformar colecciones con una sintaxis declarativa. Se puede usar en dos estilos:

- Method syntax (sintaxis de método): encadena métodos de extensión (`Where`, `Select`, `OrderBy`, ...).
- Query syntax (sintaxis de consulta): similar a SQL, usando `from`, `where`, `select`, `join`, `group`.

Estructura general (method syntax):

```csharp
var resultado = fuente
    .Where(x => /* filtro */)
    .OrderBy(x => /* clave */)
    .Select(x => /* proyección */);
```

Estructura general (query syntax):

```csharp
var resultado = from x in fuente
                where /* condición */
                orderby /* clave */
                select /* proyección */;
```

Ambas sintaxis son equivalentes en capacidad; la method syntax es más habitual con operadores avanzados.

Conceptos importantes
- Deferred execution (ejecución diferida): muchas consultas LINQ no se ejecutan hasta que se itera el `IEnumerable` o se materializa con `ToList()`, `ToArray()`, `ToDictionary()`, etc.
- Immediate execution: métodos como `ToList()`, `ToArray()`, `Count()` materializan o ejecutan la consulta inmediatamente.
- `IQueryable<T>` vs `IEnumerable<T>`: `IQueryable` representa consultas que pueden traducirse a otro motor (por ejemplo, Entity Framework -> SQL). En `IQueryable`, las expresiones se traducen antes de ejecutar.

Ejemplos prácticos y métodos más usados (method syntax)

1) Where y Select (filtrado y proyección)

```csharp
var numeros = new List<int> { 1,2,3,4,5,6 };
var pares = numeros.Where(n => n % 2 == 0).Select(n => n * 10);
// pares: 20, 40, 60 (deferred)
var listaPares = pares.ToList(); // materializa
```

2) SelectMany (aplanar colecciones)

```csharp
var listas = new List<List<int>> { new(){1,2}, new(){3,4} };
var aplanado = listas.SelectMany(l => l); // 1,2,3,4
```

3) OrderBy / OrderByDescending / ThenBy

```csharp
var productos = new[] {
    new { Nombre = "B", Precio = 20 },
    new { Nombre = "A", Precio = 30 },
    new { Nombre = "A", Precio = 10 }
};
var ordenados = productos.OrderBy(p => p.Nombre).ThenBy(p => p.Precio);
```

4) GroupBy

```csharp
var palabras = new[] { "uno", "dos", "tres", "dos", "uno" };
var agrupado = palabras.GroupBy(w => w)
    .Select(g => new { Palabra = g.Key, Cuenta = g.Count() });
// devuelve objetos con Palabra y Cuenta
```

5) Join (inner join) y GroupJoin (left/grouped join)

```csharp
var clientes = new[] { new { Id = 1, Nombre = "Ana" }, new { Id = 2, Nombre = "Luis" } };
var pedidos = new[] { new { ClienteId = 1, Total = 10 }, new { ClienteId = 1, Total = 5 } };

var join = clientes.Join(pedidos,
                         c => c.Id,
                         p => p.ClienteId,
                         (c,p) => new { c.Nombre, p.Total });

// GroupJoin para agrupar pedidos por cliente
var groupJoin = clientes.GroupJoin(pedidos,
                                   c => c.Id,
                                   p => p.ClienteId,
                                   (c, ps) => new { Cliente = c.Nombre, TotalPedidos = ps.Sum(x => x.Total) });
```

6) Agregaciones: Sum, Max, Min, Average, Aggregate

```csharp
var nums = new[] {1,2,3,4};
var suma = nums.Sum();
var max = nums.Max();
var media = nums.Average();
// Aggregate para reducir con lógica personalizada
var producto = nums.Aggregate(1, (acc, x) => acc * x); // 24
```

7) Existencia y conteo: Any, All, Count

```csharp
bool hayPares = numeros.Any(n => n % 2 == 0);
bool todosPositivos = numeros.All(n => n > 0);
int cantidad = numeros.Count(n => n % 2 == 0);
```

8) Elementos: First / FirstOrDefault / Single / SingleOrDefault / ElementAt

```csharp
var primero = numeros.First();
var primeroPar = numeros.FirstOrDefault(n => n % 2 == 0); // 0 si none
```

9) Partición: Take / Skip / TakeWhile / SkipWhile

```csharp
var primerosTres = numeros.Take(3);
var pagina2 = numeros.Skip(3).Take(3); // paginación simple
```

10) Conjuntos y combinaciones: Distinct / Union / Intersect / Except

```csharp
var a = new[]{1,2,3};
var b = new[]{3,4};
var union = a.Union(b); // 1,2,3,4
var inter = a.Intersect(b); // 3
var diff = a.Except(b); // 1,2
```

11) Materialización: ToList, ToArray, ToDictionary, ToLookup

```csharp
var dict = palabras.ToLookup(w => w); // ToLookup crea un Lookup<TKey,IEnumerable<T>>
var dic = palabras.ToDictionary(w => w + "_key", w => w.Length);
```

12) Ejemplo combinado — buscar y proyectar

```csharp
var empleados = new[] {
    new { Id = 1, Nombre = "Ana", Departamento = "IT" },
    new { Id = 2, Nombre = "Luis", Departamento = "Ventas" },
    new { Id = 3, Nombre = "Marta", Departamento = "IT" }
};

var resultado = empleados
    .Where(e => e.Departamento == "IT")
    .OrderBy(e => e.Nombre)
    .Select(e => new { e.Id, e.Nombre });

foreach(var r in resultado) Console.WriteLine(r.Nombre);
```

Query syntax equivalente:

```csharp
var resultadoQuery = from e in empleados
                     where e.Departamento == "IT"
                     orderby e.Nombre
                     select new { e.Id, e.Nombre };
```

Buenas prácticas y notas finales
- Prefiere method syntax para consultas complejas y cuando necesitas utilizar operadores que no están disponibles en la query syntax (por ejemplo, `SelectMany` con índices, `Zip`, `Aggregate`).
- Sé consciente de la ejecución diferida: si la fuente subyacente cambia (p. ej. una lista a la que sigues agregando), la consulta diferida al materializarla reflejará esos cambios.
- Para consultas sobre bases de datos usa `IQueryable<T>` (Entity Framework). Ten cuidado con los métodos que no pueden traducirse a SQL (por ejemplo, métodos locales); en ese caso EF puede lanzar una excepción o ejecutar parte en memoria.
- Usa `ToList()` cuando necesites una snapshot inmutable de los resultados antes de realizar operaciones adicionales o cuando quieras evitar múltiples enumeraciones.
---

## 9) Notas de rendimiento y consideraciones

- `List<T>` vs `LinkedList<T>`: `List<T>` tiene mejor localidad de referencia y suele ser más rápida en la práctica para la mayoría de casos; usa `LinkedList<T>` sólo si necesitas inserciones/eliminaciones O(1) en nodos conocidos.
- `Dictionary` y `HashSet` usan hashing; para tipos personalizados, implementa `GetHashCode()` y `Equals()` correctamente.
- Para grandes volúmenes de datos, evita operaciones que copian toda la colección frecuentemente (`ToList()`, `OrderBy()`), o hazlo de forma controlada.
- Usa colecciones concurrentes en contextos multihilo en lugar de bloquear manualmente con `lock` si es posible.

---

## 10) Ejercicios prácticos

1) Array vs List (Fácil)
   - Crea un array de 5 nombres y luego conviértelo a `List<string>`. Añade y elimina elementos y muestra los resultados.

2) Filtrado con LINQ (Fácil)
   - Dada una `List<int>`, usa LINQ para obtener los números impares mayores que 10 y multiplicarlos por 2.

3) Uso de Dictionary (Medio)
   - Lee una lista de palabras y construye un `Dictionary<string,int>` que cuente ocurrencias por palabra (word count). Muestra las 5 palabras más frecuentes.

4) Conjuntos y operaciones (Medio)
   - Dadas dos listas de enteros, usa `HashSet<int>` para calcular la intersección, la unión y la diferencia.

5) Cola de tareas simple (Medio)
   - Implementa una `Queue<Action>` que simule ejecución de tareas: encola varias acciones (por ejemplo, escribir mensajes) y luego procesa la cola hasta vaciarla.

Consejos:
- Empieza por escribir pruebas pequeñas en `Program.cs`.
- Para el ejercicio de palabras (3), considera normalizar (lowercase) y eliminar signos de puntuación.

---

Fin de la guía sobre colecciones en C#.

