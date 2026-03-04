using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Models;

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
        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.ToTable("Comentarios");
            entity.HasKey(e => e.IdComentario).HasName("PK__Comentar__C74515DA092105FE");

            entity.Property(e => e.IdComentario).HasColumnName("idComentario");
            entity.Property(e => e.Comentario1).HasColumnName("comentario");
            entity.Property(e => e.FechaPublicacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaPublicacion");
            entity.Property(e => e.GooglePlaceId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("googlePlaceId");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

            entity.HasOne(d => d.GooglePlace).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.GooglePlaceId)
                .HasConstraintName("FK__Comentari__googl__5BE2A6F2");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Comentarios)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Comentari__idUsu__5AEE82B9");
        });

        modelBuilder.Entity<Lugar>(entity =>
        {
            entity.ToTable("Lugares");
            entity.HasKey(e => e.GooglePlaceId).HasName("PK__Lugares__FD6A056AB8A76094");

            entity.Property(e => e.GooglePlaceId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("googlePlaceId");
            entity.Property(e => e.Coordenadas).HasColumnName("coordenadas");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.FotoUrl).HasColumnName("fotoUrl");
            entity.Property(e => e.IdMunicipio).HasColumnName("idMunicipio");
            entity.Property(e => e.NombreLugar)
                .HasMaxLength(255)
                .HasColumnName("nombreLugar");

            entity.HasOne(d => d.IdMunicipioNavigation).WithMany(p => p.Lugares)
                .HasForeignKey(d => d.IdMunicipio)
                .HasConstraintName("FK__Lugares__idMunic__5165187F");
        });

        modelBuilder.Entity<AccionLugar>(entity =>
        {
            entity.ToTable("LugaresAcciones");
            entity.HasKey(e => e.IdAccion).HasName("PK__LugaresA__E0B207A4DF513223");

            entity.Property(e => e.IdAccion).HasColumnName("idAccion");
            entity.Property(e => e.FechaAccion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaAccion");
            entity.Property(e => e.GooglePlaceId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("googlePlaceId");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.TipoAccion)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipoAccion");

            entity.HasOne(d => d.GooglePlace).WithMany(p => p.LugaresAcciones)
                .HasForeignKey(d => d.GooglePlaceId)
                .HasConstraintName("FK__LugaresAc__googl__5629CD9C");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.LugaresAcciones)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__LugaresAc__idUsu__5535A963");
        });

        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.ToTable("Municipios");
            entity.HasKey(e => e.IdMunicipio).HasName("PK__Municipi__FD10E400375E83FA");

            entity.Property(e => e.IdMunicipio).HasColumnName("idMunicipio");
            entity.Property(e => e.Departamento)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("Antioquia")
                .HasColumnName("departamento");
            entity.Property(e => e.NombreMunicipio)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreMunicipio");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__645723A627F384E6");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__2A586E0B854D0267").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Contrasena).HasColumnName("contrasena");
            entity.Property(e => e.Correo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.TokenExpira)
                .HasColumnType("datetime")
                .HasColumnName("tokenExpira");
            entity.Property(e => e.TokenVerificacion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tokenVerificacion");
            entity.Property(e => e.Verificado)
                .HasDefaultValue(false)
                .HasColumnName("verificado");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
