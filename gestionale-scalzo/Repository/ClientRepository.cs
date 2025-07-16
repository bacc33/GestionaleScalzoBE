using AutoMapper;
using gestionale_scalzo.Data;
using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;
using gestionale_scalzo.Repository.IRepository;
using gestionale_scalzo.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace gestionale_scalzo.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;
        private readonly ILogger<IUserRepository> _logger;
        private readonly IMapper _mapper;

        public ClientRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILogger<IUserRepository> logger, IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<APIResponse> DeleteClient(int id)
        {
            APIResponse response = new APIResponse();
            Client? client;
            bool result = false;

            try
            {
                client = _db.Clients.FirstOrDefault(x => x.Id == id);

                if (client != null)
                {
                    _logger.LogDebug("Cancellazione del cliente con ID: {0}", id);

                    var contracts = _db.Orders.Where(x => x.Client == client);

                    if (!contracts.Any())
                    {
                        result = (_db.Clients.Remove(client).State.ToString() == "Deleted");
                    }
                    else
                    {
                        response.Result = Costanti.KO;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.ErrorMessages = new List<string> { "Impossibile eliminare il cliente, esistono ordini associati" };
                        response.IsSuccess = false;
                    }

                }
                else
                {
                    response.Result = Costanti.RichiestaNonValida;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                }

                if (result)
                {
                    await _db.SaveChangesAsync();
                    response.Result = Costanti.DatabaseOK;
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);
                response.Result = Costanti.KO;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message, e.InnerException.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse> GetClients(string? codiceFiscale)
        {
            APIResponse response = new APIResponse();
            try
            {
                response.Result =  await _db.Clients
                    .GroupJoin(
                        _db.Orders,
                        client => client.TaxCode,
                        order => order.Cliente,
                        (client, orders) => new { client, orders }
                    )
                    .SelectMany(
                        joined => joined.orders.DefaultIfEmpty(),
                        (joined, order) => new
                        {
                            joined.client.Id,
                            joined.client.Name,
                            joined.client.Surname,
                            joined.client.Email,
                            joined.client.PhoneNumber,
                            joined.client.PIva,
                            joined.client.ResidenceAddress,
                            joined.client.DateOfBirth,
                            joined.client.TaxCode
                        }
                    )
                    .Where(x =>
                        string.IsNullOrEmpty(codiceFiscale)    
                        || x.TaxCode == codiceFiscale
                    )
                    .Distinct()
                    .ToListAsync();


                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;


            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);
                response.Result = Costanti.KO;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message, e.InnerException.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse> UpsertClient(ClientDTO clientDTO)
        {
            APIResponse response = new APIResponse();
            Client? client;
            bool result = false;

            try
            {
                _logger.LogDebug("UPSERT del cliente {0}", clientDTO.ToString());

                //si tratta di una modifica cliente
                if (clientDTO.Id > 0)
                {

                    client = await _db.Clients.FirstOrDefaultAsync(x => x.Id == clientDTO.Id);

                    client.Name = clientDTO.Name;
                    client.Surname = clientDTO.Surname;
                    client.Email = clientDTO.Email;
                    client.PhoneNumber = clientDTO.PhoneNumber;
                    client.TaxCode = clientDTO.TaxCode;
                    client.PIva = clientDTO.PIva;

                    result = _db.Clients.Update(client).State.ToString() == "Modified";
                }
                else
                {
                    //email e nome associati al cliente devono essere unici
                    if (IsUniqueClient(clientDTO.TaxCode).Result)
                    {
                        client = new Client()
                        {
                            Name = clientDTO.Name,
                            Surname = clientDTO.Surname,
                            Email = clientDTO.Email,
                            PhoneNumber = clientDTO.PhoneNumber,
                            PIva = clientDTO.PIva,
                            TaxCode = clientDTO.TaxCode
                        };

                        result = (_db.Clients.AddAsync(client).Result.State.ToString() == "Added");

                    }
                    else
                    {
                        response.Result = Costanti.KO;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.ErrorMessages = new List<string>() { "Esiste già un cliente associato ai dati forniti" };
                        response.IsSuccess = false;
                        return response;
                    }
                }

                if (result)
                {
                    await _db.SaveChangesAsync();
                    response.Result = Costanti.DatabaseOK;
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Result = Costanti.RichiestaNonValida;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);
                response.Result = Costanti.KO;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message, e.InnerException.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }

        /// <summary>
        /// return true se il codice fiscale non è registrato tra i clienti
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> IsUniqueClient(string taxCode)
        {
            return (await _db.Clients.FirstOrDefaultAsync(x => x.TaxCode == taxCode) == null ? true : false);
        }
    }
}
