using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;
using gestionale_scalzo.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace gestionale_scalzo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : Controller
    {

        protected APIResponse _response;
        private readonly IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
            _response = new();
        }

        /// <summary>
        /// Restituisce tutti i clienti filtrati per codice fiscale
        /// </summary>
        /// <param name="idAgent"></param>
        /// <returns></returns>
        [HttpGet("getClients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Clients" })]
        [Authorize]
        public async Task<IActionResult> GetClients(string? codiceFiscale)
        {
            return Ok(await _clientRepository.GetClients(codiceFiscale));
        }

        /// <summary>
        /// Aggiunta/Modifica cliente
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("upsertClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Clients" })]
        [Authorize]
        public async Task<IActionResult> UpsertClient([FromBody] ClientDTO model)
        {
            return Ok(await _clientRepository.UpsertClient(model));
        }

        /// <summary>
        /// Cancellazione del cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("deleteClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Clients" })]
        [Authorize]
        public async Task<IActionResult> DeleteClient(int id)
        {
            return Ok(await _clientRepository.DeleteClient(id));
        }
    }
}
