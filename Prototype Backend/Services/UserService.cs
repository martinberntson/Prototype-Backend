using Prototype_Backend.Database;
using Prototype_Backend.Models;
using static Prototype_Backend.Records.Records;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Prototype_Backend.Services
{
    public class UserService
    {
        private UserIdentityContext _context;
        private IConfiguration _config;
        public UserService(UserIdentityContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        internal UserRegistrationResponse RegisterUser(UserRegistrationQuery query)
        {
            User newUser = new()
            {
                Fullname = query.FullName,
                Username = query.Username,
                Email = query.Email,
                PhoneNumber = query.PhoneNumber
            };
            string uri = RegisterProfilePic();
            newUser.ProfileUri = uri;

            Identity newIdentity = new();

            newIdentity.UserEmail = newUser.Email;

            byte[] salt = new byte[128 / 8];
            using (var rngCsp = RandomNumberGenerator.Create())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            newIdentity.Salt = Convert.ToBase64String(salt);

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            newIdentity.PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: query.Password,
                salt: newIdentity.Salt.ToCharArray().Select(s => Convert.ToByte(s)).ToArray(),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));


            _context.Add<User>(newUser);
            _context.Add<Identity>(newIdentity);
            _context.SaveChanges();

            return new($"{newUser.Username}", "User registered!");

        }

        internal bool CheckUsername(string username)
        {
            var existingUser = _context.Users.Where(x => x.Username == username).FirstOrDefault();
            if (existingUser is null) return false;
            return true;
        }

        internal object CheckEmail(string email)
        {
            var existingUser = _context.Find<User>(email);
            if (existingUser is null) return false;
            return true;
        }

        internal string CheckPassword(UserLoginRequest request)
        {
            var existingUser = _context.Users.Where(x => x.Username == request.Username).FirstOrDefault();
            var identity = _context.Identities.Where(x => x.UserEmail == existingUser.Email).FirstOrDefault();
            if (identity is null) return "Identity not found";

            var salt = identity.Salt;
            var hashToTest = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: request.Password,
                salt: salt.ToCharArray().Select(s => Convert.ToByte(s)).ToArray(),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            if (hashToTest.CompareTo(identity.PasswordHash) == 0) return GenerateJWT(existingUser);
            return "Not identical";
        }

        private string RegisterProfilePic()
        {
            return "Not Implemented Exception";
        }
        private string GenerateJWT(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JwtTokenString"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Email), new Claim("User", "value") }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JwtTokenString"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                string userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
