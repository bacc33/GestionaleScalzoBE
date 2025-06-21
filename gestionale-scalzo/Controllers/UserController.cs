using gestionale_scalzo.Data;
using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;
using gestionale_scalzo.Repository.IRepository;
using gestionale_scalzo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace gestionale_scalzo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            _response = new();
        }

        /// <summary>
        /// Inserimento/Modifica di un'utenza
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("upsert")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Users" })]
        public async Task<IActionResult> Upsert([FromBody] RegisterRequestDTO model)
        {
            return Ok(await _userRepo.Upsert(model));
        }

        /// <summary>
        /// Login dell'utente nel portale
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [SwaggerOperation(Tags = new[] { "Users" })]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            return Ok(await _userRepo.Login(model));
        }

        /// <summary>
        /// Metodo per fare il refresh del token
        /// </summary>
        /// <param name="tokenApiModel"></param>
        /// <returns></returns>
        [HttpPost("refreshToken")]
        [SwaggerOperation(Tags = new[] { "Users" })]
        public async Task<IActionResult> Refresh([FromBody] TokenApiModel tokenApiModel)
        {
            return Ok(await _userRepo.Refresh(tokenApiModel));
        }

        /// <summary>
        /// Restituisce la lista di tutti gli utenti
        /// </summary>
        /// <returns></returns>
        [HttpGet("getUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Users" })]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userRepo.GetUsers());
        }

        /// <summary>
        /// Cancellazione di un'utenza
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("deleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Users" })]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _userRepo.Delete(id));
        }

        /// <summary>
        /// Metodo per il cambio pw
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("changePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Users" })]
        [Authorize]
        public async Task<IActionResult> ChangePassword(UserChangePwdDTO user)
        {
            return Ok(await _userRepo.ChangePassword(user));
        }

        /// <summary>
        /// Logout dell'utente nel portale
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("logout")]
        [SwaggerOperation(Tags = new[] { "Users" })]
        public async Task<IActionResult> Logout([FromBody] TokenApiModel model)
        {
            return Ok(await _userRepo.Logout(model));
        }

    }
}
