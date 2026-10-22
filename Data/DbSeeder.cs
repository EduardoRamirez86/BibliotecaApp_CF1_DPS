using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Enums;
using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;

namespace BibliotecaApp.Data;

/// <summary>
/// Mecanismo de siembra inicial (Data Seeding) para arranque operativo del sistema.
/// </summary>
public static class DbSeeder
{
    public static async Task InicializarAsync(BibliotecaDbContext context, IAuthService authService)
    {
        // 1. Asegurar que la base de datos esté creada y las migraciones aplicadas
        await context.Database.MigrateAsync();

        // 2. Si ya existen usuarios, no volver a sembrar
        if (await context.Usuarios.AnyAsync())
        {
            return;
        }

        // 3. Semilla de Usuarios Iniciales
        var passAdmin = authService.HashearPassword("Admin1234!");
        var passEstudiante = authService.HashearPassword("Estudiante1234!");
        var passDocente = authService.HashearPassword("Docente1234!");

        var admin = new Bibliotecario(
            nombre: "Administrador General",
            identificacion: "EMP-BIB-001",
            email: "biblioteca@udb.edu.sv",
            passwordHash: passAdmin,
            codigoPersonal: "BIB-01");

        var estudiante = new Estudiante(
            nombre: "Carlos Martínez Rivas",
            identificacion: "07891234-5",
            email: "carlos.martinez@udb.edu.sv",
            passwordHash: passEstudiante,
            carne: "UDB-2021-087",
            carrera: "Licenciatura en Administracion de Empresas");

        var docente = new Docente(
            nombre: "Dr. Roberto Mejía Fuentes",
            identificacion: "01234567-8",
            email: "roberto.mejia@udb.edu.sv",
            passwordHash: passDocente,
            numeroEmpleado: "EMP-5023",
            departamento: "Departamento de Ingenieria Informatica");

        await context.Usuarios.AddRangeAsync(admin, estudiante, docente);

        // 4. Semilla de Libros Iniciales
        if (!await context.Libros.AnyAsync())
        {
            var libros = new List<LibroFisico>
            {
                new(
                    titulo: "Introducción a los Algoritmos",
                    autor: "Cormen, Leiserson, Rivest, Stein",
                    isbn: "978-0-262-03384-8",
                    anioPublicacion: 2009,
                    genero: "Ciencias de la Computación",
                    stockTotal: 5,
                    ubicacionEstante: "Estante A-12"),
                new(
                    titulo: "Clean Code: Manual de desarrollo ágil",
                    autor: "Robert C. Martin",
                    isbn: "978-0-13-235088-4",
                    anioPublicacion: 2008,
                    genero: "Ingeniería de Software",
                    stockTotal: 4,
                    ubicacionEstante: "Estante B-07"),
                new(
                    titulo: "Designing Data-Intensive Applications",
                    autor: "Martin Kleppmann",
                    isbn: "978-1-491-90308-1",
                    anioPublicacion: 2017,
                    genero: "Bases de Datos",
                    stockTotal: 6,
                    ubicacionEstante: "Estante C-03")
            };

            await context.LibrosFisicos.AddRangeAsync(libros);
        }

        await context.SaveChangesAsync();
    }
}
