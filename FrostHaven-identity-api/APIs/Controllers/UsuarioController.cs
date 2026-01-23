using Application.DTOs.Requests.User;
using Application.DTOs.Responses.Users;
using Application.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace APIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly CreateUserUseCase _createUser;

    public UsuarioController(CreateUserUseCase  createUser)
    {
        _createUser = createUser;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreatePlayer([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        try
        {
            var cmd = new CreateUserCommand(
                Email: request.Email,
                Name: request.Name,
                Surname: request.Surname,
                DisplayName: request.DisplayName,
                Password: request.Password,
                Role: request.Role);

            var result = await _createUser.ExecuteAsync(cmd, ct);

            var response = new CreateUserResponse
            {
                Id = result.Id,
                Email = result.Email,
                DisplayName = result.DisplayName,
                Active = result.Active,
                Roles = result.Roles
            };

            // Location header: /api/users/{id}
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Ya existe un usuario", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Usuario duplicado",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Solicitud inválida",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
    
    // Endpoint placeholder para CreatedAtAction
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int id)
    {
        // Lo implementamos después (query + include roles)
        return Ok(new { id });
    }
}