using API.FurnitureStore.API.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
        
    }
}
