# Sistema de Gestión de Biblioteca — BibliotecaApp

Proyecto de Cátedra · Universidad Don Bosco (UDB)  
Asignatura: Desarrollo de Programas Sección 01

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

---

## Descripción General

Aplicación Web desarrollada en **ASP.NET Core MVC (.NET 10.0)** con **Entity Framework Core** y **SQL Server**. El proyecto implementa una arquitectura en capas (N-Layer / MVC) con principios **SOLID**, inyección de dependencias y componentes reutilizables.

Actualmente se encuentra implementado y verificado el **Módulo de Seguridad y Autenticación**:
- **Inicio de Sesión (Login)**: Pantalla fiel a los lineamientos institucionales UDB con correo institucional (`@udb.edu.sv`), toggle interactivo para visibilidad de contraseña y opción "Recordarme".
- **Registro de Usuarios (Register)**: Alta de usuarios con validaciones de contraseñas seguras (mínimo 8 caracteres, confirmación coincidente) y asignación de roles.
- **Roles del Sistema**:
  - `Estudiante`: Alumno de la institución.
  - `Docente`: Profesor o académico.
  - `Bibliotecario`: Personal administrativo y gestor del sistema bibliotecario.
- **Seguridad Criptográfica**: Cifrado y verificación de contraseñas mediante **PBKDF2/SHA256** con salt criptográfica aleatoria de 128 bits e iteraciones de seguridad (100,000 iteraciones).
- **Manejo de Sesión**: Autenticación nativa basada en **Cookies** (`CookieAuthenticationDefaults`).
- **Página de Bienvenida (Post-Login)**: Redirección inmediata a `Home/Index` que despliega una confirmación ("¡Hola Mundo!"), el usuario autenticado, su rol activo y opción de cierre de sesión.

---

## Tecnologías Utilizadas

| Tecnología | Versión | Uso |
|---|---|---|
| C# | 13 | Lenguaje principal |
| .NET SDK | 10.0 (`net10.0`) | Framework de ejecución |
| ASP.NET Core MVC | 10.0 | Arquitectura web y controladores |
| Entity Framework Core | 10.0.12 | ORM y mapeo relacional TPH |
| SQL Server | Express / LocalDB | Motor de base de datos |
| Criptografía | PBKDF2 (SHA256) | Hashing seguro de credenciales |

---

## Cómo Ejecutar el Proyecto

### 1. Clonar el repositorio
```bash
git clone https://github.com/EduardoRamirez86/BibliotecaApp_CF1_DPS.git
cd BibliotecaApp_CF1_DPS
```

### 2. Configurar la Cadena de Conexión (si aplica)
Revisar `appsettings.json` para verificar la cadena de conexión hacia tu instancia de SQL Server:
```json
{
  "ConnectionStrings": {
    "ConexionSQL": "Server=localhost;Database=BibliotecaUDB_DPS;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Aplicar Migraciones a la Base de Datos (opcional)
Si se desea sincronizar la base de datos con las tablas:
```bash
dotnet ef database update
```

### 4. Compilar y Ejecutar la Aplicación

#### Opción A: Desde Terminal / Consola
```bash
dotnet run
```
Abre tu navegador en la URL indicada en la consola (usualmente `https://localhost:7xxx` o `http://localhost:5xxx`). La aplicación abrirá directamente la pantalla de **Iniciar Sesión**.

#### Opción B: Desde Visual Studio 2022
1. Abrir la solución `BibliotecaApp.slnx` o el archivo `BibliotecaApp.csproj`.
2. Presionar **F5** o `Ctrl + F5` para iniciar.

---

## Flujo de Prueba del Módulo de Seguridad

1. **Pantalla inicial:** El navegador se redirige automáticamente a `/Auth/Login`.
2. **Crear una cuenta:** Haz clic en *"¿No tienes cuenta? Regístrate aquí"*.
3. **Completar el registro:**
   - Nombre: `Eduardo Ramírez`
   - Correo: `eduardo.ramirez@udb.edu.sv`
   - Rol: Selecciona `Estudiante`, `Docente` o `Bibliotecario`.
   - Contraseña: (mínimo 8 caracteres, ej. `Prueba1234!`).
   - Prueba el botón del ojo para verificar o esconder la contraseña.
4. **Iniciar Sesión:** Ingresa con el correo y contraseña registrados.
5. **Verificación:** Accederás a la pantalla de bienvenida con el *"¡Hola Mundo!"*, tu rol asignado y el botón de *"Cerrar Sesión"*.
