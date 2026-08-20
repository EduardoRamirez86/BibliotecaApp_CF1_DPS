# Sistema de Gestión de Biblioteca — BibliotecaApp

Proyecto de Cátedra · Fase 1 · Universidad Don Bosco (UDB)  
Asignatura: Desarrollo de Programas Sección 01

---

## Descripción

Aplicación de consola desarrollada en **C# 13 (.NET 10.0)** que simula el sistema de gestión de préstamos de libros físicos de la Biblioteca UDB. Permite registrar usuarios (estudiantes y docentes), consultar el catálogo de libros, realizar préstamos y registrar devoluciones.

El proyecto aplica **Programación Orientada a Objetos (POO)** avanzada, los principios **SOLID** y una arquitectura modular diseñada para escalar a una aplicación web **ASP.NET Core MVC** en la Fase 2.

---

## Tecnologías utilizadas

| Tecnología | Versión | Uso |
|---|---|---|
| C# | 13 | Lenguaje principal |
| .NET | 10.0 (`net10.0`) | Target Framework / Runtime |
| IDE Recomendado | Visual Studio 2022 / VS Code | Entorno de desarrollo |
| Almacenamiento | En memoria (`List<T>`) | Persistencia temporal (Fase 1) |

---

## Instrucciones de ejecución

### Opción 1: Visual Studio (Recomendado)

1. Abrir la carpeta del proyecto en **Visual Studio 2022**.
2. Hacer doble clic en la solución `BibliotecaApp.slnx` o en el archivo de proyecto `BibliotecaApp.csproj`.
3. Presionar **F5** (o la tecla `Ctrl + F5` / clic en el botón de **Iniciar / Run** en la barra superior) para compilar y ejecutar la aplicación de consola.

---

### Opción 2: Desde la Terminal (.NET CLI)

1. Abrir una terminal en la **raíz del proyecto** (donde está ubicado `BibliotecaApp.csproj`).
   > ⚠️ **Nota:** No intentes ejecutar `dotnet run` dentro de subcarpetas como `/Services` o `/Models`, ya que fallará al no encontrar el archivo de proyecto `.csproj`.

2. Ejecutar el siguiente comando:
   ```bash
   dotnet run
   ```

---

## Estructura del proyecto

```
BibliotecaApp_CF1_DPS/
│
├── Program.cs                  # Punto de entrada. Configura servicios, seed data y bucle principal.
│
├── Models/                     # Entidades del dominio (POO: Herencia, Abstracción, Encapsulamiento)
│   ├── EntidadBase.cs          # Clase abstracta base: genera Id (GUID) y FechaCreacion automáticamente.
│   ├── Libro.cs                # Clase abstracta Libro: define la estructura común de cualquier libro.
│   ├── LibroFisico.cs          # Libro físico con stock en estante. Controla préstamos y devoluciones.
│   ├── Usuario.cs              # Clase abstracta Usuario: base para Estudiante y Docente.
│   ├── Estudiante.cs           # Usuario de tipo estudiante. Agrega Carné y Carrera.
│   ├── Docente.cs              # Usuario de tipo docente. Agrega NumeroEmpleado y Departamento.
│   └── Prestamo.cs             # Relación entre Usuario y Libro en un período de tiempo determinado.
│
├── Interfaces/                 # Contratos abstractos (SOLID: DIP — la UI depende de interfaces, no de clases concretas)
│   ├── IBuscable.cs            # Interfaz genérica de búsqueda por texto. Implementada por los servicios.
│   ├── ILibroService.cs        # Contrato del servicio de libros: obtener, agregar, buscar.
│   ├── IUsuarioService.cs      # Contrato del servicio de usuarios: registrar, buscar, verificar.
│   └── IPrestamoService.cs     # Contrato del servicio de préstamos: crear, devolver, consultar.
│
├── Services/                   # Lógica de negocio con almacenamiento en memoria
│   ├── LibroService.cs         # Gestiona el catálogo de libros. Implementa ILibroService.
│   ├── UsuarioService.cs       # Gestiona el registro de usuarios. Implementa IUsuarioService.
│   └── PrestamoService.cs      # Orquesta préstamos y devoluciones. Implementa IPrestamoService.
│
└── UI/                         # Capa de presentación para la consola (SOLID: SRP — solo renderiza)
    ├── ConsoleViews.cs         # Componentes reutilizables: banner, headers, badges, inputs, mensajes.
    ├── LibroView.cs            # Vistas de catálogo, detalle y selección de libros.
    ├── UsuarioView.cs          # Vistas de lista, registro y selección de usuarios.
    └── PrestamoView.cs         # Vistas de registro de préstamos, devoluciones y confirmaciones.
```

---

## Funcionalidades principales

- **Catálogo de libros** — Listado de todos los libros físicos con disponibilidad en tiempo real.
- **Gestión de usuarios** — Registro y consulta de estudiantes y docentes de la UDB.
- **Préstamo de libros** — Valida disponibilidad de stock y registra el préstamo con fecha de devolución esperada (15 días).
- **Devolución de libros** — Lista préstamos activos, registra la devolución y libera el stock.
- **Detalle de libro** — Búsqueda por texto y ficha completa con información del ejemplar.

---

## Principios SOLID aplicados

| Principio | Aplicación en el proyecto |
|---|---|
| **S** — Single Responsibility | Cada clase tiene una única razón de cambio (Services vs UI vs Models). |
| **O** — Open/Closed | Se pueden agregar nuevos tipos de libro sin modificar los servicios existentes. |
| **L** — Liskov Substitution | `LibroFisico` puede usarse en cualquier lugar donde se espera un `Libro`. |
| **I** — Interface Segregation | `IBuscable<T>` es una interfaz específica, no un contrato monolítico. |
| **D** — Dependency Inversion | `Program.cs` declara los servicios con sus interfaces (`ILibroService`, etc.). |

---

## Integrantes del equipo

| # | Apellidos | Nombres | Carné |
|---|---|---|---|
| 1 | Ruiz Hernández | Edgar Antonio | RH201851 |
| 2 | Henriquez Vasquez | Axel Francisco | HV230423 |
| 3 | Varela Linares | Marjorie Daniela | VL261354 |
| 4 | Ramirez Torres | Eduardo Alfredo | RT240549 |
| 5 | Azucena Ayala | Carlos Josue | AA260854 |
| 6 | Ayala Palacios | Marcos Ezequiel | AP260351 |
