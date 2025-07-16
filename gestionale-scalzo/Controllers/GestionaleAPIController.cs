using AutoMapper;
using gestionale_scalzo.Data;
using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;
using gestionale_scalzo.Repository.IRepository;
using gestionale_scalzo.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace gestionale_scalzo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GestionaleAPIController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ITypologyRepository _typoRepo;
        protected APIResponse _response;

        public GestionaleAPIController(IOrderRepository orderRepo, ITypologyRepository typoRepo)
        {
            _orderRepo = orderRepo;
            _typoRepo = typoRepo;
            _response = new();
        }


        /// <summary>
        /// Aggiunta / Modifica di un ordine
        /// </summary>
        /// <param name="orderDataDTO"></param>
        /// <returns></returns>
        [HttpPost("upsertOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Order" })]
        [Authorize]
        public async Task<IActionResult> UpsertOrder([FromBody] OrderDTO orderDataDTO)
        {
            return Ok(await _orderRepo.UpsertOrder(orderDataDTO));
        }


        /// <summary>
        /// Restistuisce i dettagli dell'ordine o tutta la lista degli ordini
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Order" })]
        [Authorize]
        public async Task<IActionResult> GetOrder(int? id)
        {
            return Ok(await _orderRepo.GetOrder(id));
        }


        /// <summary>
        /// Restituisce le gli ordini in base al filtro impostato
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("getFilteredOrders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Order" })]
        [Authorize]
        public async Task<IActionResult> GetFilteredOrders([FromBody] FilteredOrderDataDTO filter)
        {
            return Ok(await _orderRepo.GetFilteredOrders(filter));
        }
        
        
        /// <summary>
        /// Cancellazione di un ordine
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("deleteOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Order" })]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            return Ok(await _orderRepo.DeleteOrder(id));
        }
        
        
        /// <summary>
        /// Cancellazione di una tipologia
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("deleteTypo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Typo" })]
        [Authorize]
        public async Task<IActionResult> DeleteTypo(int id)
        {
            return Ok(await _typoRepo.DeleteTypology(id));
        }

        /// <summary>
        /// Restistuisce i dettagli della tipologia o tutta la lista delle tipologie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getTypo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Typo" })]
        [Authorize]
        public async Task<IActionResult> GetTypo(int? id)
        {
            return Ok(await _typoRepo.GetTypology(id));
        }
        
        /// <summary>
        /// Cancellazione di una tipologia
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("upsertTypo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Tags = new[] { "Typo" })]
        [Authorize]
        public async Task<IActionResult> UpsertTypo(TypologyDTO typologyDTO)
        {
            return Ok(await _typoRepo.UpsertTypology(typologyDTO));
        }

    }
}
