using System;
using System.Collections.Generic;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Services;

public partial class GeoConnectContext : DbContext
{
    public GeoConnectContext()
    {
    }

    public GeoConnectContext(DbContextOptions<GeoConnectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comentario> Comentarios { get; set; }

    public virtual DbSet<Lugar> Lugares { get; set; }

    public virtual DbSet<AccionLugar> LugaresAcciones { get; set; }

    public virtual DbSet<Municipio> Municipios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- 1. CONFIGURACIÓN DE COMENTARIOS ---
        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.ToTable("Comentarios", t=>
                t.HasCheckConstraint("CK_Comentario_Calificacion","calificacion >= 1 AND calificacion <= 5")
            );

            entity.HasKey(e => e.IdComentario); // EF Core infiere el nombre de la PK automáticamente

            entity.Property(e => e.IdComentario).HasColumnName("idComentario");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario").IsRequired(); // Ahora es obligatorio
            entity.Property(e => e.IdLugar).HasColumnName("idLugar").IsRequired(); // Apunta a la nueva PK
            entity.Property(e => e.Comentario1).HasColumnName("comentario").IsRequired();
            entity.Property(e => e.Calificacion).HasColumnName("calificacion");

            entity.Property(e => e.FechaPublicacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaPublicacion");

            // Nuevas Llaves Foráneas limpias
            entity.HasOne(d => d.Lugar)
                .WithMany()
                .HasForeignKey(d => d.IdLugar)
                .HasConstraintName("FK_Comentarios_Lugares");

            entity.HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_Comentarios_Usuarios");
        });

        // --- 2. CONFIGURACIÓN DE LUGARES ---
        modelBuilder.Entity<Lugar>(entity =>
        {
            entity.ToTable("Lugares");
            entity.HasKey(e => e.IdLugar); // NUEVA LLAVE PRIMARIA

            // Hacemos que GooglePlaceId sea único, pero permitiendo nulos
            entity.HasIndex(e => e.GooglePlaceId).IsUnique();

            entity.Property(e => e.IdLugar).HasColumnName("idLugar");
            entity.Property(e => e.GooglePlaceId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("googlePlaceId"); // Ya no es required

            entity.Property(e => e.IdMunicipio).HasColumnName("idMunicipio");
            entity.Property(e => e.NombreLugar).HasMaxLength(255).HasColumnName("nombreLugar").IsRequired();
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.Coordenadas).HasColumnName("coordenadas");
            entity.Property(e => e.FotoUrl).HasColumnName("fotoUrl");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");

            entity.HasOne(d => d.Municipio)
                .WithMany()
                .HasForeignKey(d => d.IdMunicipio)
                .HasConstraintName("FK_Lugares_Municipios");
        });

        // --- 3. CONFIGURACIÓN DE ACCIONES LUGAR ---
        modelBuilder.Entity<AccionLugar>(entity =>
        {
            // Ajustado al nombre de la tabla en el script SQL nuevo
            entity.ToTable("AccionLugar");
            entity.HasKey(e => e.IdAccion);

            entity.Property(e => e.IdAccion).HasColumnName("idAccion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario").IsRequired(); // Ahora es obligatorio
            entity.Property(e => e.IdLugar).HasColumnName("idLugar").IsRequired(); // Apunta a la nueva PK

            entity.Property(e => e.TipoAccion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tipoAccion")
                .IsRequired();

            entity.Property(e => e.FechaAccion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaAccion");

            // Nuevas Llaves Foráneas limpias
            entity.HasOne(d => d.Lugar)
                .WithMany()
                .HasForeignKey(d => d.IdLugar)
                .HasConstraintName("FK_AccionLugar_Lugares");

            entity.HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_AccionLugar_Usuarios");
        });

        // --- 4. CONFIGURACIÓN DE MUNICIPIOS ---
        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.ToTable("Municipios");
            entity.HasKey(e => e.IdMunicipio);

            entity.Property(e => e.IdMunicipio).HasColumnName("idMunicipio");
            entity.Property(e => e.NombreMunicipio)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombreMunicipio")
                .IsRequired();

            entity.Property(e => e.Departamento)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValue("Antioquia")
                .HasColumnName("departamento");
        });

        // --- 5. CONFIGURACIÓN DE USUARIOS ---
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.IdUsuario);

            // Índice único limpio para el correo
            entity.HasIndex(e => e.Correo).IsUnique().HasDatabaseName("UQ_Usuarios_Correo");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Nombre).HasMaxLength(255).IsUnicode(false).HasColumnName("nombre").IsRequired();
            entity.Property(e => e.Correo).HasMaxLength(255).IsUnicode(false).HasColumnName("correo").IsRequired();
            entity.Property(e => e.Contrasena).HasColumnName("contrasena").IsRequired();

            entity.Property(e => e.Verificado)
                .HasDefaultValue(false)
                .HasColumnName("verificado");

            entity.Property(e => e.TokenVerificacion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tokenVerificacion");

            entity.Property(e => e.TokenExpira)
                .HasColumnType("datetime")
                .HasColumnName("tokenExpira");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
