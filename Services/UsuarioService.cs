using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BibliotecaApp.Data;
using BibliotecaApp.DTOs.Usuarios;
using BibliotecaApp.Interfaces;
using BibliotecaApp.Models;

namespace BibliotecaApp.Services;

/// <summary>
/// Gestiona la consulta y persistencia de usuarios en SQL Server mediante Entity Framework Core.
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly BibliotecaDbContext _context;

    public UsuarioService(BibliotecaDbContext context)
    {
        _context = context;
    }

    public IReadOnlyList<Usuario> ObtenerTodos() => _context.Usuarios.AsNoTracking().ToList().AsReadOnly();

    public IReadOnlyList<UsuarioResumenDto> ObtenerResumenTodos()
        => _context.Usuarios
            .AsNoTracking()
            .Select(u => new UsuarioResumenDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Identificacion = u.Identificacion,
                Email = u.Email,
                Rol = u.ObtenerRol()
            })
            .ToList()
            .AsReadOnly();

    public Usuario? ObtenerPorEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return _context.Usuarios.FirstOrDefault(u => u.Email == emailNormalizado);
    }

    public void Registrar(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        if (ExisteIdentificacion(usuario.Identificacion))
            throw new InvalidOperationException(
                $"Ya existe un usuario con la identificación '{usuario.Identificacion}'.");

        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    public bool ExisteIdentificacion(string identificacion)
        => _context.Usuarios.Any(u => u.Identificacion.ToLower() == identificacion.ToLower());

    public IEnumerable<Usuario> BuscarPorTexto(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino)) return ObtenerTodos();

        return _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Nombre.Contains(termino) ||
                        u.Identificacion.Contains(termino) ||
                        u.Email.Contains(termino))
            .ToList();
    }

    public Usuario? ObtenerPorId(Guid id) => _context.Usuarios.FirstOrDefault(u => u.Id == id);
}
