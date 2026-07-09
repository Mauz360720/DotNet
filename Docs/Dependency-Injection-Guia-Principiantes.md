# Guía para principiantes: Inyección de dependencias en .NET (usando la aplicación de ejemplo)

Generalidades de `IConfiguration` y `appsettings.json`
-----------------------------------------------------
Antes de entrar en la inyección de dependencias, es útil entender cómo .NET maneja la configuración de la aplicación
mediante `IConfiguration` y archivos como `appsettings.json`.

- `IConfiguration` es la abstracción que representa un conjunto de valores de configuración. Está pensado para leer
  valores desde múltiples "proveedores" (providers) —por ejemplo JSON, variables de entorno, argumentos de línea de
  comandos, Azure Key Vault, etc.— y exponerlos de forma uniforme.

- `ConfigurationBuilder` se usa para componer esos proveedores. Un ejemplo típico en `Program.cs`:

```csharp
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();
```

- Orden y sobrescritura: el orden en que agregas proveedores importa. Valores añadidos por proveedores posteriores
  sobrescriben a los anteriores. Por ejemplo, una variable de entorno con la misma clave que una entrada en
  `appsettings.json` reemplazará el valor del JSON.

- `appsettings.json` es un archivo JSON con estructura libre; es común tener secciones como `ConnectionStrings` o
  `Logging`. Ejemplo mínimo:

```json
{
  "ConnectionStrings": {
    "SQLite": "Data Source=reservation.sqlite"
  },
  "MyOptions": {
    "OptionA": "valor",
    "OptionB": 42
  }
}
```

- Acceso a valores: puedes leer valores puntuales con `configuration["MiClave:SubClave"]` o usar métodos convenientes
  como `configuration.GetConnectionString("SQLite")` para la sección `ConnectionStrings`.

- Binding a POCOs: para conveniencia y tipado, puedes enlazar secciones a objetos fuertemente tipados:

```csharp
var options = configuration.GetSection("MyOptions").Get<MyOptions>();
```

- Buenas prácticas rápidas:
    - No pongas secretos (contraseñas, keys) en `appsettings.json` en repositorios públicos. Usa variables de entorno,
      User Secrets (en desarrollo) o servicios de secretos en producción.
    - Marca `reloadOnChange: true` durante desarrollo si quieres recargar configuración al editar el archivo, pero ten
      cuidado en producción.
    - Asegúrate de que `appsettings.json` se copie al output (propiedad CopyToOutputDirectory) o usa `SetBasePath`
      apropiadamente para que el archivo sea localizado en tiempo de ejecución.

Nota: paquetes NuGet requeridos
--------------------------------
Antes de compilar o ejecutar los ejemplos de esta guía asegúrate de instalar los paquetes NuGet que usa el proyecto.
Ejecuta estos comandos desde la carpeta del proyecto donde quieras añadir los paquetes (por ejemplo `Application`):

```bash
cd /Users/afmappe/Downloads/Layer.Solution/Application
dotnet add package Microsoft.Data.Sqlite
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Dapper
dotnet restore
```

Con estos paquetes tendrás disponibles la conexión SQLite (`Microsoft.Data.Sqlite`), la inyección de dependencias (
`Microsoft.Extensions.DependencyInjection`) y Dapper para mapeo ligero de datos.

Nota adicional: `Microsoft.Extensions.DependencyInjection` es un paquete NuGet independiente. Aunque forma parte del
ecosistema de extensiones de .NET, puede no estar referenciado por defecto en todos los tipos de proyecto; por ello en
la guía lo incluimos explícitamente con `dotnet add package Microsoft.Extensions.DependencyInjection`.
Checklist (lo que voy a cubrir):

- [x] ¿Qué es la inyección de dependencias (DI) y por qué usarla
- [x] Cómo registrar servicios con `ServiceCollection` (ejemplos reales del proyecto)
- [x] Los ciclos de vida (lifetimes): Singleton, Scoped, Transient — qué significan y cuándo usar cada uno
- [x] Buenas prácticas con conexiones a bases de datos (ej.: `SqliteConnection`) y repositorios
- [x] Ejemplos de código extraídos y explicados desde `Program.cs`, `ReservationRepository` y `ReservationModule`
- [x] Cómo probar localmente

---

Introducción
------------
La inyección de dependencias (Dependency Injection, DI) es un patrón para desacoplar componentes: en lugar de que una
clase cree o localice sus dependencias, estas se *inyectan* desde el exterior. En .NET la librería más usada es la que
viene en `Microsoft.Extensions.DependencyInjection` (el `ServiceCollection` y `ServiceProvider`).

Ventajas principales:

- Facilita el testing (puedes inyectar mocks)
- Mejora la mantenibilidad y la separación de responsabilidades
- Centraliza la configuración de la aplicación (donde se resuelven dependencias)

Registro y creación del contenedor (ejemplo del proyecto)
---------------------------------------------------------
En `Application/Program.cs` (fragmento real del proyecto) se hace lo siguiente para configurar DI:

```csharp
// 1. Crear ServiceCollection
var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);

// 2. Build Container (ServiceProvider)
var serviceProvider = serviceCollection.BuildServiceProvider();

// 3. Crear scope y ejecutar la app
using var scope = serviceProvider.CreateScope();
var app = scope.ServiceProvider.GetRequiredService<MyApp>();
app.Run();
```

En `ConfigureServices` del propio `Program.cs` se registran servicios así:

```csharp
var connectionString = $"Data Source={ConnectionString}";
services.AddScoped<SqliteConnection>(_ => new SqliteConnection(connectionString));
services.AddScoped<IReservationRepository, ReservationRepository>();
services.AddScoped<IReservationModule, ReservationModule>();
services.AddScoped<MyApp>();
```

Este registro indica al contenedor cómo construir y con qué ciclo de vida crear cada tipo.

Tipos de ciclo de vida (lifetimes)
---------------------------------
.NET ofrece tres lifetimes primarios en `Microsoft.Extensions.DependencyInjection`:

1) Singleton

- services.AddSingleton<TService, TImplementation>()
- Se crea una única instancia durante toda la vida del `ServiceProvider`.
- Uso recomendado: servicios sin estado que se puedan compartir y sean thread-safe (p. ej. servicios de configuración,
  caches inmutables, loggers que internamente usan sincronización).
- No usar para recursos no thread-safe o que mantengan estado por usuario/por operación (p. ej. conexiones DB sin manejo
  explícito de concurrencia).

2) Scoped

- services.AddScoped<TService, TImplementation>()
- Se crea una instancia por *scope*. En ASP.NET Core un scope es una petición HTTP; en aplicaciones de consola depende
  de cuándo llames `CreateScope()`.
- Uso recomendado: servicios cuyo alcance debe vivir durante una operación/solicitud (repositorios, UoW, servicios
  transaccionales) porque permite compartir la misma instancia dentro de la operación y se garantiza la limpieza al
  finalizar el scope.

3) Transient

- services.AddTransient<TService, TImplementation>()
- Se crea una nueva instancia cada vez que se solicita el servicio.
- Uso recomendado: servicios ligeros, sin estado o que deban ser independientes cada vez que se inyectan.

Qué ciclo elegir — reglas prácticas
----------------------------------

- Repositorios y Unit of Work: normalmente `Scoped` (una instancia por operación/solicitud).
- Servicios de utilidades sin estado (por ejemplo, formatters puros): `Transient` o `Singleton` si son thread-safe.
- Conexiones a base de datos: no uses `Singleton` a menos que sepas exactamente cómo manejar concurrencia; prefiere
  `Scoped` (en solicitudes web) o crear/abrir conexiones por operación (Transient con using) según el patrón de acceso.

Explicación aplicada al proyecto (Program.cs + ReservationRepository)
------------------------------------------------------------------
En la aplicación de ejemplo `Program.cs` se registró `SqliteConnection` y los repositorios como `Scoped`:

- `services.AddScoped<SqliteConnection>(_ => new SqliteConnection(connectionString));`
- `services.AddScoped<IReservationRepository, ReservationRepository>();`

En `ReservationRepository` (extracto real):

```csharp
public class ReservationRepository : IReservationRepository
{
    private readonly SqliteConnection _connection;

    public ReservationRepository(SqliteConnection connection)
    {
        _connection = connection;
        if (connection.State != ConnectionState.Open)
        {
            _connection.Open();
            EnsureTableExists();
        }
    }
    // ... métodos que usan _connection
}
```

Interpretación:

- Como `SqliteConnection` y `ReservationRepository` están registrados `Scoped`, si el programa crea un scope con
  `CreateScope()` (como hace `Program.cs`), ambos se crearán una vez y se compartirán dentro de ese scope. El
  `ReservationRepository` recibirá la instancia de `SqliteConnection` correspondiente a ese scope.
- Para una aplicación de consola donde creas un scope por ejecución de `MyApp.Run()`, `Scoped` funciona bien: la
  conexión se abre al construir el repositorio y se cierra cuando el `ServiceProvider`/scope se libera (siempre que el
  contenedor disponga los disposables). Esto es un patrón válido.

Alternativas y buenas prácticas con conexiones (recomendaciones)
----------------------------------------------------------------

1) Patrón actual (Scoped connection)

- Si el alcance de la operación es claro (por ejemplo: una aplicación que crea un scope por trabajo), `Scoped` para la
  conexión es razonable.
- Ventaja: una única conexión compartida dentro del scope y tablas pueden crearse en el constructor (como hace
  `EnsureTableExists`).
- Riesgo: si accidentalmente expones el `ServiceProvider` globalmente o reutilizas el mismo scope para muchas
  operaciones concurrentes, podrías tener problemas con concurrencia.

2) Crear/abrir conexión por operación (más seguro y común en repositorios)

- No registrar `SqliteConnection` directamente; en su lugar, crear la conexión en cada método del repositorio con
  `using var conn = new SqliteConnection(connectionString); conn.Open(); ...`.
- Ventaja: evita problemas de concurrencia y hace más explícito el alcance de la conexión. Es el patrón más usado en
  aplicaciones que manejan muchas operaciones pequeñas.

3) Registrar una fábrica o `IDbConnection` transient

- `services.AddTransient<SqliteConnection>(_ => new SqliteConnection(connectionString));`
- O registrar `Func<SqliteConnection>` o un `IDbConnectionFactory` para que cada petición obtenga una nueva instancia.
- Servicio repositorio puede solicitar la fábrica y crear/abrir la conexión cuando necesite.

4) Nunca usar Singleton para `SqliteConnection` (salvo casos muy específicos)

- Compartir la misma instancia de conexión entre muchos hilos sin una capa de sincronización puede provocar errores.

Ejemplos concretos de registro alternativo
-----------------------------------------
A) Registrar fábrica y usar en repositorio (pattern recomendado si quieres control más fino):

```csharp
// Registro
services.AddTransient<Func<SqliteConnection>>(_ => () => new SqliteConnection(connectionString));
services.AddScoped<IReservationRepository, ReservationRepository>();

// Repositorio (constructor recibe la fábrica)
public class ReservationRepository : IReservationRepository
{
    private readonly Func<SqliteConnection> _connectionFactory;
    public ReservationRepository(Func<SqliteConnection> connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IEnumerable<ReservationInfo> GetAll()
    {
        using var conn = _connectionFactory();
        conn.Open();
        return conn.Query<ReservationInfo>("SELECT * FROM Reservation").ToList();
    }
}
```

B) Abrir conexión por método (más simple)

```csharp
public IEnumerable<ReservationInfo> GetAll()
{
    using var conn = new SqliteConnection(_connectionString);
    conn.Open();
    return conn.Query<ReservationInfo>("SELECT * FROM Reservation").ToList();
}
```

Inyección por constructor — ejemplo real
--------------------------------------
El módulo de negocio (`ReservationModule`) define sus dependencias mediante el constructor (record syntax en el archivo
real):

```csharp
public class ReservationModule(IReservationRepository repository) : IReservationModule
{
    // usa 'repository' para implementar la lógica de negocio
}
```

Esto demuestra el patrón clásico: la clase declara lo que necesita y el contenedor se encarga de proveerlo.

Disposal / limpieza
-------------------

- Si registras objetos que implementan `IDisposable` (p. ej. `SqliteConnection`) en el contenedor, el `ServiceProvider`
  se encargará de *disponerlos* cuando se disponga el provider o el scope que los creó.
- Por eso es importante crear scopes bien delimitados. En el `Program.cs` del ejemplo se hace
  `using var scope = serviceProvider.CreateScope();` lo cual asegura que los `Scoped` disposables se limpien al salir
  del bloque.

Patrones comunes (resumen rápido)
---------------------------------

- Repositorios: Scoped (en web) o crear nueva conexión por llamada (con `using`) para evitar problemas.
- Servicios sin estado: Transient o Singleton (si son thread-safe).
- Servicios con estado por petición: Scoped.

Ergonomía y testabilidad
------------------------

- Programa en función de interfaces (ej.: `IReservationRepository`) para que en pruebas puedas inyectar implementaciones
  falsas.
- Evita el Service Locator (buscar servicios desde `IServiceProvider` dentro de la lógica de negocio). Prefiere la
  inyección por constructor.

Cómo probar la aplicación localmente
-----------------------------------
Desde la carpeta `Application` puedes compilar y ejecutar la app. En macOS / zsh:

```bash
cd /Users/afmappe/Downloads/Layer.Solution/Application
dotnet build
dotnet run
```

Notas finales y recomendaciones
-------------------------------

- Para aplicaciones pequeñas o prototipos la estrategia `Scoped` para conexión + scope por ejecución (como en este
  proyecto) es aceptable y simple.
- Para aplicaciones con concurrencia o alta carga, prefiere abrir conexiones por operación (using) o implementar una
  factoría y usar pooling adecuado.
- Siempre registra por interfaces (p. ej. `IReservationRepository`) para facilitar pruebas.
- Delimita scopes y respeta el ciclo de vida recomendado para cada tipo de servicio.

Fin de la guía.

