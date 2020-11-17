using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebService.Helpers;
using WebService.Models;
using WebService.Models.Request;
using WebService.Models.Response;

namespace WebService.Services
{
    public interface IUserService
    {
        Task<UserResponse> Auth(UserRequest model, string ipAddress);
        User GetById(int id);

        List<User> GetAll();
        UserResponse RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
    }

    public class UserService : IUserService
    {
        private TaskAppDBContext _context;
        private readonly AppSettings _appSettings;

        public UserService(TaskAppDBContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public List<User> GetAll()
        {
            return _context.User.ToList();
        }

        public User GetById(int id)
        {
            return _context.User.Find(id);
        }

        public async Task<UserResponse> Auth(UserRequest model, string ipAddress)
        {
            string spassword = Encrypt.GetSHA256(model.Password);

            var user = await _context.User.Where(x => x.Email == model.Email && x.Password == spassword).FirstOrDefaultAsync();

            if (user == null) return null;

            var token = GetToken(user);
            var refreshToken = GetRefreshToken(ipAddress);

            user.RefreshTokens.Add(refreshToken);
            _context.Update(user);
            await _context.SaveChangesAsync();

            return new UserResponse
            {
                Email = user.Email,
                Token = token,
                RefreshToken = refreshToken.Token
            };
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var user = _context.User.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsActive) return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();

            return true;
        }

        public UserResponse RefreshToken(string token, string ipAddress)
        {
            UserResponse response = new UserResponse();

            var user = _context.User.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive) return null;

            // Reemplaze el refresh token antiguo con el nuevo
            var newRefreshToken = GetRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _context.Update(user);
            _context.SaveChanges();

            var jwtToken = GetToken(user);

            return new UserResponse
            {
                Email = user.Email,
                Token = jwtToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        private RefreshToken GetRefreshToken(string ipAddress)
        {
            using(var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

        public string GetToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
