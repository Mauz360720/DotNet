# Guía Básica de Unit Testing para Principiantes

Esta guía se centra en lo más fundamental de las pruebas unitarias, utilizando el patrón **AAA (Arrange, Act, Assert)**.

## ¿Qué es un Unit Test?

Es una prueba automatizada que verifica el funcionamiento de una "unidad" de código (generalmente un método) de forma
aislada. El objetivo es asegurar que cada pieza individual funcione correctamente.

---

## El Patrón AAA (Arrange, Act, Assert)

Este patrón es una de las mejores formas de organizar tus pruebas para que sean legibles y fáciles de mantener.

### 1. **Arrange** (Preparar)

En este primer paso, preparas todo lo necesario para que la prueba se ejecute.

- Creas instancias de las clases.
- Defines las variables de entrada.
- Estableces cuál es el resultado esperado.

### 2. **Act** (Actuar)

Aquí es donde ejecutas el código que realmente quieres probar. Generalmente consiste en llamar a un método.

### 3. **Assert** (Afirmar / Verificar)

Finalmente, compruebas si el resultado de la acción fue el esperado. Si la afirmación es cierta, la prueba pasa. Si no,
falla.

---

## Ejemplo Práctico (C#)

Imagina que tienes una clase `Calculadora`:

```csharp
public class Calculadora 
{
    public int Sumar(int a, int b) 
    {
        return a + b;
    }
}
```

### La Prueba Unitaria

Aquí aplicamos AAA:

```csharp
[Fact]
public void Sumar_CuandoSeSumanDosNumeros_RetornaLaSumaCorrecta() 
{
    // Arrange (Preparar)
    var calculadora = new Calculadora();
    int valor1 = 5;
    int valor2 = 10;
    int resultadoEsperado = 15;

    // Act (Actuar)
    int resultadoReal = calculadora.Sumar(valor1, valor2);

    // Assert (Afirmar)
    Assert.Equal(resultadoEsperado, resultadoReal);
}
```

---

## Atributos Principales: [Fact] y [Theory]

En xUnit (uno de los frameworks de pruebas más usados en .NET), existen dos tipos principales de pruebas:

### 1. [Fact]

Se utiliza para pruebas que **siempre deben ser verdaderas**. No aceptan parámetros. Es una prueba única y específica.

* **Ejemplo:** Probar que `2 + 2` siempre es `4`.

### 2. [Theory]

Se utiliza para pruebas que deben ser verdaderas para un **conjunto de datos**. Permite ejecutar el mismo método de
prueba varias veces con diferentes entradas.
Normalmente se acompaña del atributo **`[InlineData]`**.

#### Ejemplo de [Theory]:

Imagina que quieres probar que un método `EsPar` funciona con varios números:

```csharp
[Theory]
[InlineData(2)]
[InlineData(4)]
[InlineData(10)]
[InlineData(100)]
public void EsPar_CuandoSePasanNumerosPares_RetornaTrue(int numero) 
{
    // Arrange
    var calculadora = new Calculadora();

    // Act
    bool resultado = calculadora.EsPar(numero);

    // Assert
    Assert.True(resultado);
}
```

**Ventaja:** En lugar de escribir 4 pruebas separadas con `[Fact]`, escribes una sola `[Theory]` que se ejecuta 4 veces
con datos distintos.

---

## Asserts más comunes (Verificaciones)

El objeto `Assert` es el que decide si tu prueba pasa o falla. Aquí tienes los métodos más utilizados en el día a día:

### 1. Igualdad y Diferencia

- **`Assert.Equal(esperado, real)`**: Comprueba que el resultado es exactamente el que esperas.
- **`Assert.NotEqual(valor, real)`**: Comprueba que el resultado NO sea un valor específico.

### 2. Valores Booleanos

- **`Assert.True(condicion)`**: Pasa si la condición es verdadera (`true`).
- **`Assert.False(condicion)`**: Pasa si la condición es falsa (`false`).

### 3. Nulidad

- **`Assert.Null(objeto)`**: Verifica que un objeto sea nulo.
- **`Assert.NotNull(objeto)`**: Verifica que un objeto haya sido instanciado (no sea nulo).

### 4. Colecciones y Texto (Empty/Contains)

- **`Assert.Empty(coleccion)`**: Comprueba que una lista, array o string esté vacío (longitud 0).
- **`Assert.NotEmpty(coleccion)`**: Comprueba que tenga al menos un elemento o carácter.
- **`Assert.Contains("texto", cadenaTotal)`**: Verifica que una subcadena exista dentro de otra.
- **`Assert.Contains(elemento, lista)`**: Verifica que un objeto exista dentro de una lista.

### 5. Rangos

- **`Assert.InRange(valor, minimo, maximo)`**: Verifica que un número esté entre un mínimo y un máximo (ambos
  inclusive). Ideal para validar edades, precios o fechas.

### 6. Excepciones

- **`Assert.Throws<NombreExcepcion>(() => accion)`**: Se usa para asegurar que tu código "explota" correctamente cuando
  recibe datos inválidos. Por ejemplo, una división por cero.

---

## Generación de Datos con Bogus

A medida que tus pruebas crecen, escribir datos manualmente (como "Nombre 1", "Prueba", "123") se vuelve tedioso. Aquí
es donde entra **Bogus**.

### ¿Qué es Bogus?

Bogus es una librería para .NET que te permite generar datos falsos (fake data) de forma masiva y realista: nombres,
direcciones, correos electrónicos, fechas, etc.

### ¿Para qué se usa?

Se usa principalmente en la fase de **Arrange**:

- Para evitar "números mágicos" o cadenas de texto repetitivas.
- Para probar cómo se comporta tu código con una gran variedad de datos realistas.
- Para crear objetos complejos rápidamente sin escribir cada propiedad a mano.

### ¿Cómo funciona?

Se basa en reglas. Tú le dices qué tipo de dato quieres para cada propiedad de tu clase.

#### Ejemplo de uso:

Imagina que quieres probar un sistema que procesa usuarios. Primero, define tu clase:

```csharp
public class Usuario 
{
    public string Nombre { get; set; }
    public string Email { get; set; }
    public int Edad { get; set; }
}
```

Ahora, usa Bogus para crear datos aleatorios en tu prueba:

```csharp
[Fact]
public void ProcesarUsuario_ConDatosAleatorios() 
{
    // Arrange
    var faker = new Faker<Usuario>()
        .RuleFor(u => u.Nombre, f => f.Name.FirstName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Edad, f => f.Random.Int(18, 90));

    Usuario usuarioFalso = faker.Generate(); // Crea un usuario con datos realistas

    // Ahora puedes usar 'usuarioFalso' en tu prueba Act y Assert
}
```

### ¿Qué puede generar Faker? (Capacidades)

La potencia de Bogus reside en la enorme cantidad de categorías de datos que puede simular. Aquí tienes algunas de las
más comunes que puedes usar en tus reglas:

- **`f.Name`**: Nombres, apellidos, prefijos, títulos, nombres completos.
- **`f.Internet`**: Emails, nombres de usuario, contraseñas, dominios, URLs, direcciones IP.
- **`f.Address`**: Ciudades, nombres de calles, códigos postales, países, latitud/longitud.
- **`f.Date`**: Fechas pasadas, futuras, recientes, o en un rango específico.
- **`f.Commerce`**: Nombres de productos, departamentos, precios, colores, materiales.
- **`f.Finance`**: Números de cuenta, nombres de bancos, tipos de transacción, moneda, IBAN.
- **`f.Phone`**: Números de teléfono con diferentes formatos.
- **`f.Lorem`**: Párrafos, oraciones o palabras de texto de relleno (Lorem Ipsum).
- **`f.Random`**: Es quizás la herramienta más flexible. Permite generar:
    - **`f.Random.Int(1, 100)`**: Un número entero entre 1 y 100.
    - **`f.Random.Decimal(0, 1000)`**: Un número decimal (ideal para precios).
    - **`f.Random.Double()` o `f.Random.Float()`**: Números de punto flotante.
    - **`f.Random.Bool()`**: Un valor `true` o `false` aleatorio.
    - **`f.Random.Guid()`**: Un identificador único (GUID).
    - **`f.Random.ListItem(miLista)`**: Selecciona un elemento al azar de una lista propia.
    - **`f.Random.Enum<NombreEnum>()`**: Selecciona un valor aleatorio de un enumerado (Enum).

#### Ejemplo rápido de múltiples categorías:

```csharp
var facturaFaker = new Faker<Factura>()
    .RuleFor(f => f.Id, f => f.Random.Guid())
    .RuleFor(f => f.Cliente, f => f.Name.FullName())
    .RuleFor(f => f.Email, f => f.Internet.Email())
    .RuleFor(f => f.Monto, f => f.Finance.Amount(10, 500))
    .RuleFor(f => f.Fecha, f => f.Date.Past(1));
```

---

## Consejos para Principiantes

1. **Nombre descriptivo:** El nombre del método de prueba debe explicar claramente qué está probando y cuál es el
   resultado esperado.
2. **Una sola cosa a la vez:** Intenta que cada prueba verifique solo un comportamiento.
3. **Independencia:** Las pruebas no deben depender unas de otras. Deben poder ejecutarse en cualquier orden.
4. **Rapidez:** Las pruebas unitarias deben ser muy rápidas de ejecutar.
