using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsuarios()
    {
        var usuarios = new[]
        {
            new
            {
                Id = 1,
                Nombre = "Administrador",
                Email = "admin@demo.com",
                Activo = true
            }
        };

        return Ok(usuarios);
    }
}