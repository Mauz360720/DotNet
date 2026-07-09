# Guía para principiantes: Excepciones y manejo de errores en .NET C#

Checklist de lo que incluye esta guía:

- [x] Qué son las excepciones y por qué importan
- [x] Cómo lanzar (`throw`) y capturar (`try/catch/finally`) excepciones
- [x] Diferencias entre `throw` y `throw ex` (rethrow)
- [x] Excepciones comunes del BCL y cómo tratarlas
- [x] Excepciones personalizadas
- [x] Buenas prácticas (logging, no usar excepciones para control de flujo, granularidad)
- [x] Ejemplos prácticos y cómo probarlos
- [x] 5 ejercicios prácticos al final

---

## Introducción: ¿qué es una excepción?

Una excepción es un mecanismo de notificación de errores o condiciones inesperadas que ocurren durante la ejecución de un programa. En .NET, las excepciones son objetos que derivan de `System.Exception`.

Cuando ocurre una excepción no manejada, la ejecución salta fuera del flujo normal y busca un manejador (`catch`) apropiado en la pila de llamadas; si no se encuentra, la aplicación termina (o en aplicaciones web, la petición falla).

---

## 1) Estructura básica: try / catch / finally

Bloque básico para capturar y manejar excepciones:

```csharp
try
{
    // Código que puede lanzar una excepción
    int a = 10, b = 0;
    var c = a / b; // lanza DivideByZeroException
}
catch (DivideByZeroException ex)
{
    Console.WriteLine("No puedes dividir por cero: " + ex.Message);
}
catch (Exception ex)
{
    // Captura genérica: usar con cuidado
    Console.WriteLine("Error inesperado: " + ex.Message);
}
finally
{
    // Código que siempre se ejecuta (liberar recursos, limpiar estado)
    Console.WriteLine("Fin del intento");
}
```

Puntos clave:
- Los `catch` más específicos deben ir antes que los genéricos.
- `finally` se ejecuta tanto si hubo excepción como si no; ideal para liberar recursos.

---

## 2) Lanzar excepciones: `throw` y crear excepciones personalizadas

Puedes lanzar (throw) excepciones cuando detectes condiciones inválidas en tu código:

```csharp
public void EstablecerEdad(int edad)
{
    if (edad < 0) throw new ArgumentOutOfRangeException(nameof(edad), "La edad no puede ser negativa");
    // asignación...
}
```

Excepciones personalizadas (cuando quieres expresar errores de dominio):

```csharp
public class LibroNoDisponibleException : Exception
{
    public int LibroId { get; }

    public LibroNoDisponibleException(int libroId)
        : base($"El libro con id {libroId} no está disponible")
    {
        LibroId = libroId;
    }
}

// Uso:
// throw new LibroNoDisponibleException(42);
```

Recomendación: hereda de `Exception` (o `ApplicationException` si necesitas distinguir), implementa constructores que acepten `message` y `innerException` si corresponde.

---

## 3) Rethrow: `throw;` vs `throw ex;`

Cuando vuelves a lanzar una excepción dentro de un `catch`, existen dos formas:

- `throw;` — re-lanza la excepción preservando la pila de llamadas original (stack trace).
- `throw ex;` — lanza la excepción como si se originara de nuevo en este punto: la pila de llamadas se reinicia (pierdes la traza original).

Siempre que quieras conservar la traza original usa `throw;`:

```csharp
try
{
    // ...
}
catch(Exception ex)
{
    // registrar o limpiar
    Log(ex);
    throw; // conserva la pila original
}
```

---

## 4) Manejo de recursos: using y finally

El patrón `using` es sintaxis azucarada para garantizar la liberación de recursos que implementan `IDisposable`.

```csharp
using(var fs = File.OpenRead("archivo.txt"))
{
    // leer archivo
}
// fs.Dispose() se llamará automáticamente aunque ocurra excepción
```

Equivalente con try/finally:

```csharp
var fs = File.OpenRead("archivo.txt");
try
{
    // leer
}
finally
{
    fs.Dispose();
}
```

---

## 5) Excepciones asíncronas y `async/await`

En métodos `async`, las excepciones se propagan como en código síncrono: si `await` envuelve una tarea que falló, la excepción es lanzada en el punto del `await`.

```csharp
public async Task<int> DownloadAsync()
{
    using var client = new HttpClient();
    var response = await client.GetAsync("https://no-existe"); // puede lanzar HttpRequestException
    response.EnsureSuccessStatusCode();
    return (int)response.Content.Headers.ContentLength;
}

// Uso:
try
{
    var size = await DownloadAsync();
}
catch(HttpRequestException ex)
{
    Console.WriteLine("Error de red: " + ex.Message);
}
```

Nota: excepciones dentro de `Task` no observadas pueden causar problemas; en general, `await` asegura que las excepciones sean observadas en el hilo que hace el `await`.

---

## 6) Excepciones comunes del BCL y cómo tratarlas

- `ArgumentNullException` — cuando se pasa null a un método que no lo acepta. Validar parámetros al inicio.
- `ArgumentOutOfRangeException` — valor fuera de rango.
- `InvalidOperationException` — llamado inválido para el estado actual del objeto.
- `IOException` / `FileNotFoundException` — operaciones de E/S.
- `HttpRequestException` — errores de red en HttpClient.

Buenas prácticas: maneja sólo las excepciones que puedes resolver. No captures excepciones genéricas (`catch(Exception)`) para ignorarlas; al menos regístralas y vuelve a lanzar si no puedes manejarlas.

---

## 7) Buenas prácticas y anti-patrones

Qué hacer:
- Valida argumentos al inicio del método y lanza `ArgumentNullException`/`ArgumentException` cuando tenga sentido.
- Usa excepciones personalizadas para expresar errores de dominio claros.
- Registra (log) las excepciones con suficiente contexto (métodos, parámetros importantes) para poder depurar.
- Usa `throw;` para re-lanzar y preservar stack trace.
- Usa `using`/`IDisposable` o `finally` para liberar recursos.
- Mantén los bloques `try` lo más pequeños posible (solo alrededor del código que puede fallar).

Qué evitar:
- No uses excepciones para controlar el flujo normal (por ejemplo, validar si un elemento existe usando excepciones).
- No atrapes y silencies excepciones (empty catch) — al menos registra o documenta por qué se ignora.
- Evita `catch (Exception)` sin re-lanzar o registrar; dificulta el diagnóstico.

---

## 8) Ejemplos prácticos combinados

Ejemplo: validación + lanzamiento + manejo + logging simulado

```csharp
public class ServicioCuentas
{
    public void Transferir(string deCuenta, string aCuenta, decimal cantidad)
    {
        if (string.IsNullOrEmpty(deCuenta)) throw new ArgumentNullException(nameof(deCuenta));
        if (string.IsNullOrEmpty(aCuenta)) throw new ArgumentNullException(nameof(aCuenta));
        if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser positiva", nameof(cantidad));

        try
        {
            // Operaciones de débito/crédito que pueden lanzar excepciones de E/S o dominio
            Debitar(deCuenta, cantidad);
            Acreditar(aCuenta, cantidad);
        }
        catch(Exception ex)
        {
            // registrar con contexto y re-lanzar para que la capa superior pueda actuar
            Console.WriteLine($"Error al transferir de {deCuenta} a {aCuenta}: {ex.Message}");
            throw; // importante conservar la pila
        }
    }

    private void Debitar(string cuenta, decimal cantidad)
    {
        // simulación
        if (cuenta == "0000") throw new InvalidOperationException("Cuenta bloqueada");
    }

    private void Acreditar(string cuenta, decimal cantidad)
    {
        // simulación
    }
}
```

---

## 9) Cómo probar los ejemplos rápidamente

1. Crear un proyecto de consola:

```bash
dotnet new console -o ExceptionsExample
cd ExceptionsExample
```

2. Reemplaza `Program.cs` por un archivo que contenga pequeñas pruebas de las secciones anteriores (por ejemplo, probar `Transferir`, `DownloadAsync`, lanzar excepciones personalizadas, etc.).

3. Ejecutar:

```bash
dotnet run
```

---

## Ejercicios prácticos

1) Validación y lanzamiento (Fácil)
   - Crea una clase `Usuario` con propiedades `Nombre` y `Email`. Implementa un método `SetEmail(string email)` que valide formato básico (contiene `@`) y lance `ArgumentException` si es inválido. Prueba en `Program.cs` con casos válidos e inválidos.

2) Excepción personalizada (Medio)
   - Implementa una excepción `ProductoAgotadoException : Exception` que incluya `ProductoId` y `CantidadSolicitada`. Crea una clase `Inventario` con método `Retirar(productoId, cantidad)` que lance la excepción cuando no haya stock.

3) Rethrow y logging (Medio)
   - Crea un servicio que llame a otro método que lanza una excepción. Captura la excepción, registra (Console.WriteLine) y vuelve a lanzar con `throw;`. Observa la pila antes y después (usa `Exception.StackTrace`).

4) Manejo de recursos (Fácil)
   - Escribe código que abra un archivo y lo lea usando `using`. Forzar una excepción dentro del bloque y comprobar que el archivo se cierra correctamente (puedes intentar abrir el archivo para escritura después para verificar que no está bloqueado).

5) Lectura segura de archivos con reintentos simples (Intermedio)
   - Objetivo: Implementar un método `LeerArchivoConReintentos(string path, int reintentos = 3)` que intente abrir y leer el contenido de un archivo. En caso de `IOException` debe reintentar hasta el número de intentos indicado; si el archivo no existe (`FileNotFoundException`) debe notificarlo inmediatamente.
   - Implementación mínima:
     - Usar `using` para abrir el `StreamReader` y garantizar el cierre del recurso.
     - Capturar `FileNotFoundException` y `IOException` por separado.
     - Reintentar en caso de `IOException` hasta `reintentos` veces con un pequeño delay (por ejemplo, 200ms, 400ms...).
     - Registrar (Console.WriteLine) cada intento y el motivo si falla.
   - Salida esperada: Mostrar el contenido del archivo si se leyó correctamente; si falla tras los reintentos, mostrar un mensaje de error indicando intentos realizados.
   - Pistas: Mantén el bloque `try` lo más pequeño posible (solo la lectura); usa `Thread.Sleep` para el delay si haces una versión síncrona; en versiones `async` usa `Task.Delay`.

Consejos para los ejercicios:
- Escribe pruebas rápidas en `Program.cs` y ejecuta con `dotnet run`.
- Añade mensajes claros cuando captures excepciones para entender qué ocurrió.

---

Fin de la guía sobre excepciones y manejo de errores.

