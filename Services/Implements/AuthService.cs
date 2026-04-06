using BCrypt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Implements
{
    public class AuthService : IAuthService
    {
        private readonly GeoConnectContext _context;
        private readonly IConfiguration _config;

        public AuthService(GeoConnectContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<(bool Exito, string Mensaje, string? Token)> LoginAsync(LoginDto dto)
        {
            var usuarioPorLoggear = await _context.Usuarios.FirstOrDefaultAsync( u => u.Correo == dto.Correo);
            
            if (usuarioPorLoggear == null)
            {
                return (false, "El correo no existe", null);
            }

            bool validPwd = BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuarioPorLoggear.Contrasena);
            
            if (!validPwd)
            {
                return (false, "Contraseña incorrecta.", null);
            }

            string token = generarJwtToken(usuarioPorLoggear);

            return (true,"Inicio de sesión exitoso", token);
        }

        public async Task<(bool Exito, string Mensaje)> VerificarCuentaAsync(VerificarCuentaDto dto)
        {
            var usuarioBd = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (usuarioBd == null)
                return (false, "Usuario no encontrado.");

            if (usuarioBd.Verificado == true)
                return (false, "La cuenta ya se encuentra verificada.");

            if (usuarioBd.TokenVerificacion != dto.Token)
                return (false, "El código de verificación es incorrecto.");

            if (usuarioBd.TokenExpira < DateTime.Now)
                return (false, "El código de verificación ha expirado. Por favor solicita uno nuevo.");

            // Si pasa todo, activamos la cuenta y limpiamos el token de la DB
            usuarioBd.Verificado = true;
            usuarioBd.TokenVerificacion = null;
            usuarioBd.TokenExpira = null;

            await _context.SaveChangesAsync();

            return (true, "Cuenta verificada correctamente. Ya puedes iniciar sesión.");
        }

        private string generarJwtToken(Usuario usuario)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            //condicional para prevenir cualquier caso de olvidar colocar los secretos
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("CRÍTICO: La Key del JWT no está configurada");
            }

            //pasar llave a criptografía
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //claim es el paquete que acompaña al token, NO METER CONTRASEÑAS
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
                new Claim("nombre", usuario.Nombre)
            };

            //token con fecha de vencimiento y firma
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
