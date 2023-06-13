using API.FurnitureStore.API.Configuration;
using API.FurnitureStore.Shared.Auth;
using API.FurnitureStore.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.FurnitureStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        //IdentityUser es un tipo de usuario, se puede reemplazar por una
        //clase usuario creada por default
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        //se injecta con patron iOptions
        //trae elementos desde el archivo de configuracion
        public AuthenticationController(UserManager<IdentityUser> userManager,
                                        IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;

            //va el .Value porque es una propiedad IOption
            _jwtConfig = jwtConfig.Value;

        }

        //
        public async Task<IActionResult> Register ([FromBody] UserRegistrationRequestDto request)
        {
            //model state verifica los data anotations ([required])
            if (!ModelState.IsValid) return BadRequest();

            //Verificar si el email existe
            var emailExist = await _userManager.FindByEmailAsync(request.EmailAdress);

            if (emailExist != null)
            
                return BadRequest(new AuthResult()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "El email ya existe"
                    }
                });


            //Create usuario
            var user = new IdentityUser()
            {
                Email = request.EmailAdress,
                UserName = request.EmailAdress

            };

            //Devuelve un identity result que es una clase con varias propiedades.
            //si tuvo exito se crea el usuario devuelve el token 
            var isCreated = await _userManager.CreateAsync(user);

            if (isCreated.Succeeded)
            {
                var token = GenerateToken(user);
                return Ok(new AuthResult()
                {
                    Result = true,
                    Token = token
                });
            }
            else
            {
                //Lista de errores
                var errors = new List<string>();

                //Se recorre el enumerable del identity result que tiene al lista de errores
                //Se addean los errores a la lista creada de errores
                foreach (var err in isCreated.Errors)
                    errors.Add(err.Description);


                return BadRequest(new AuthResult
                {
                    Result = false,
                    Errors = errors
                });
            }

            return BadRequest(new AuthResult
            {
                Result = false,
                Errors = new List<string> { "El usuario no puede ser creado" }
            });

        }

        //Generador de token
        private string GenerateToken (IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            //Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                //informacion pertinente del usuario que se va a autenticar
                Subject = new ClaimsIdentity(new ClaimsIdentity(new[]
                {
                    //JwtRegisteredClaimNames es una estructura que 
                    //define los nombres de las declaraciones que se
                    //consideran estándar para los tokens JWT.

                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    //el ID del token es el GUID
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    //Identifica la hora y el dia que fue emitido el token
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())

                })),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }
        
    }
}
