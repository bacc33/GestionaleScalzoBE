using gestionale_scalzo.Data;
using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;
using gestionale_scalzo.Repository.IRepository;
using gestionale_scalzo.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Net;
using System.Security.Claims;

namespace gestionale_scalzo.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<OrderRepository> _logger;
        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _env;
        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;


        public OrderRepository(ApplicationDbContext db, UserManager<ApplicationUser> userManager, ILogger<OrderRepository> logger, 
            IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

        

        public async Task<APIResponse> DeleteOrder(int id)
        {
            APIResponse response = new APIResponse();
            Order? order;
            bool result = false;
            try
            {
                _logger.LogDebug("Cancellazione dell'ordine con ID: {0}", id);

                order = _db.Orders.FirstOrDefault(x => x.Id == id);
                                
                result = (_db.Orders.Remove(order).State.ToString() == "Deleted");

                if (result)
                {
                    await _db.SaveChangesAsync();
                    response.Result = this.GetOrder(0).Result;
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    response.Result = Costanti.RichiestaNonValida;
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

        public async Task<APIResponse> GetFilteredOrders(FilteredOrderDataDTO filter)
        {
            APIResponse response = new APIResponse();
            try
            {

                ApplicationUser loggedUser = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.Name));
                var role = User.FindFirstValue(ClaimTypes.Role);

                var contracts = await _db.Orders
                .Select(c => new
                {
                    c.Id,
                    c.NumeroOrdine,
                    c.NumeroColli,
                    c.Varieta,
                    TipologiaPedane = c.TipologiaPedaneId,
                    c.NumeroPedane,
                    TipologiaCassette = c.TipologiaCassetteId,
                    c.NumeroCassette,
                    c.PesoCassetta,
                    c.KiliNetti,
                    c.KiliTotali,
                    NomeCliente = c.Client.Name,
                    CognomeCliente = c.Client.Surname,
                    CodiceFiscaleCliente = c.Client.TaxCode,
                    c.DataInserimento,
                    c.DataPartenza,
                    c.PrezzoOrdine,
                    c.CompagniaTrasporto,
                    TipologiaBancale = c.TipologiaBancaleId,
                    c.Peso
                })
                .Distinct()
                .Where
                (c =>
                (String.IsNullOrEmpty(filter.NumeroOrdine) ? true : c.NumeroOrdine == filter.NumeroOrdine)
                //&& (role == Costanti.Agente ? c.IdAgente == loggedUser.Id : role == Costanti.UtenteBO ? c.ApplicationUser.IdSede == loggedUser.IdSede : true)
                //&& (role == Costanti.Agente ? c.IdAgente == loggedUser.Id : (filter.IdAgente == 0 ? (role == Costanti.UtenteBO ? c.ApplicationUser.IdSede == loggedUser.IdSede : true) : c.IdAgente == filter.IdAgente))
                && (String.IsNullOrEmpty(filter.CfCliente) ? true : c.CodiceFiscaleCliente == filter.CfCliente)
                && (c.DataInserimento >= filter.DataInserimentoInizio  && c.DataInserimento < filter.DataInserimentoFine)
                && (c.DataPartenza >= filter.DataPartenzaInizio  && c.DataPartenza < filter.DataPartenzaFine)
                )
                .OrderByDescending(c => c.DataInserimento)
                .ToListAsync();

                response.Result = contracts;
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

        public async Task<APIResponse> GetOrder(int? id)
        {
            APIResponse response = new APIResponse();
            try
            {
                if (id == 0)
                {
                    response.Result = await _db.Orders
                    .Select(c => new
                    {
                        c.Id,
                        c.NumeroOrdine,
                        c.NumeroColli,
                        c.Varieta,
                        TipologiaPedane = c.TipologiaPedaneId,
                        c.NumeroPedane,
                        TipologiaCassette = c.TipologiaCassetteId,
                        c.NumeroCassette,
                        c.PesoCassetta,
                        c.KiliNetti,
                        c.KiliTotali,
                        NomeCliente = c.Client.Name,
                        CognomeCliente = c.Client.Surname,
                        CodiceFiscaleCliente = c.Client.TaxCode,
                        c.DataInserimento,
                        c.DataPartenza,
                        c.PrezzoOrdine,
                        c.CompagniaTrasporto,
                        TipologiaBancale = c.TipologiaBancaleId,
                        c.Peso,
                        c.Scarico
                    })
                    .Distinct()
                    .OrderByDescending(o => o.DataInserimento)
                    .ToListAsync();
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    var order = await _db.Orders
                    .Select(c => new
                    {
                        c.Id,
                        c.NumeroOrdine,
                        c.NumeroColli,
                        c.Varieta,
                        TipologiaPedane = c.TipologiaPedaneId,
                        c.NumeroPedane,
                        TipologiaCassette = c.TipologiaCassetteId,
                        c.NumeroCassette,
                        c.PesoCassetta,
                        c.KiliNetti,
                        c.KiliTotali,
                        NomeCliente = c.Client.Name,
                        CognomeCliente = c.Client.Surname,
                        CodiceFiscaleCliente = c.Client.TaxCode,
                        c.DataInserimento,
                        c.DataPartenza,
                        c.PrezzoOrdine,
                        c.CompagniaTrasporto,
                        TipologiaBancale = c.TipologiaBancaleId,
                        c.Peso,
                        c.Scarico
                    })
                    .Distinct()
                    .Where(o => o.Id == id)
                    .SingleOrDefaultAsync();

                    response.Result = order;
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

        public async Task<APIResponse> UpsertOrder([FromBody] OrderDTO orderDataDTO)
        {
            APIResponse response = new APIResponse();
            bool result = false;
            Order? order = null;
            try
            {
                _logger.LogDebug("UPSERT dell'ordine {0}", orderDataDTO.ToString());

                if (orderDataDTO.Id > 0)
                {
                    //si tratta di un update
                    order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == orderDataDTO.Id);

                    if (order != null)
                    {
                        order.NumeroOrdine = orderDataDTO.NumeroOrdine;
                        order.NumeroColli = (int)orderDataDTO.NumeroColli;
                        order.Varieta = orderDataDTO.Varieta; 
                        order.TipologiaBancaleId = orderDataDTO.TipologiaBancale;
                        order.NumeroPedane = orderDataDTO.NumeroPedane;
                        order.TipologiaCassetteId = orderDataDTO.TipologiaCassette;
                        order.NumeroCassette= orderDataDTO.NumeroCassette;
                        order.TipologiaPedaneId = orderDataDTO.TipologiaPedane;
                        order.PesoCassetta= orderDataDTO.PesoCassetta;
                        order.Peso = orderDataDTO.Peso;
                        order.KiliNetti = orderDataDTO.KiliNetti;
                        order.KiliTotali = orderDataDTO.KiliTotali;
                        order.Cliente = orderDataDTO.CodiceFiscaleCliente;
                        order.DataInserimento = orderDataDTO.DataInserimento;
                        order.DataPartenza = orderDataDTO.DataPartenza;
                        order.PrezzoOrdine = orderDataDTO.PrezzoOrdine;
                        order.CompagniaTrasporto = orderDataDTO.CompagniaTrasporto;
                        order.Scarico = orderDataDTO.Scarico;                        

                        result = (_db.Orders.Update(order).State.ToString() == "Modified");
                    }

                }
                else
                {
                    //si tratta di una insert
                    order = new Order()
                    {
                        NumeroOrdine = "IT" + orderDataDTO.DataInserimento.ToString("yyyy") +
                                        orderDataDTO.DataInserimento.ToString("MM") +
                        orderDataDTO.DataInserimento.ToString("dd"),
                        NumeroColli = orderDataDTO.NumeroColli,
                        Varieta = orderDataDTO.Varieta,
                        TipologiaPedaneId = orderDataDTO.TipologiaPedane,
                        NumeroPedane = orderDataDTO.NumeroPedane,
                        TipologiaCassetteId = orderDataDTO.TipologiaCassette,
                        NumeroCassette = orderDataDTO.NumeroCassette,
                        PesoCassetta = orderDataDTO.PesoCassetta,
                        TipologiaBancaleId = orderDataDTO.TipologiaBancale,
                        KiliNetti = orderDataDTO.KiliNetti,
                        KiliTotali = orderDataDTO.KiliTotali,
                        Cliente = orderDataDTO.CodiceFiscaleCliente,
                        DataInserimento = orderDataDTO.DataInserimento,
                        DataPartenza = orderDataDTO.DataPartenza,
                        PrezzoOrdine = orderDataDTO.PrezzoOrdine,
                        CompagniaTrasporto = orderDataDTO.CompagniaTrasporto,
                        Scarico = orderDataDTO.Scarico,
                        Peso = orderDataDTO.Peso
                    };

                    result = (_db.Orders.AddAsync(order).Result.State.ToString() == "Added");

                }
                if (result)
                {
                    await _db.SaveChangesAsync();
                                        
                    if (orderDataDTO.Id == 0)
                    {
                        //this.InsertMessage(contract.Id, "EmailAggiuntaContratto");
                        response.IsSuccess = true;
                        response.StatusCode = HttpStatusCode.Created;
                        response.Result = order.Id;
                    }
                    else
                    {
                        _db.SaveChanges();
                        //invio un messaggio a tutti gli operatori del backoffice
                        //this.InsertMessage(contract.Id, "EmailModificaContratto");
                        response.IsSuccess = true;
                        response.StatusCode = HttpStatusCode.Created;
                        response.Result = order.Id;
                    }

                }

                else
                {
                    response.Result = Costanti.RichiestaNonValida;
                    response.StatusCode = HttpStatusCode.OK;
                    response.ErrorMessages = new List<string>() { "Esiste già una ordine con l'id = " + orderDataDTO.NumeroOrdine };
                    response.IsSuccess = false;
                    return response;
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

        //public async void InsertMessage(int idContratto, string template)
        //{
        //    try
        //    {
        //        var contratto = _db.Contracts.Select(c => new {
        //            c.Id,
        //            c.IdPratica,
        //            c.ApplicationUsers,
        //            c.IdAgente,
        //            c.Cliente,
        //            c.Client,
        //            c.DataInserimento,
        //            c.DataScadenza,
        //            c.DataValidazione,
        //            c.Servizio,
        //            c.IdCategoria,
        //            c.Filename,
        //            c.Note,
        //            c.Prodotto,
        //            c.OrderStates,
        //            Agente = c.ApplicationUsers.Name + " " + c.ApplicationUsers.Surname
        //        })
        //        .Distinct()
        //        .Where(c => c.Id == idContratto)
        //        .SingleOrDefault();

        //        List<ApplicationUser> destinatari = new List<ApplicationUser>();

        //        //mando la comunicazione sempre all'agente e all'amministratore se lo stato è nuovo o inviato 
        //        if (contratto.OrderStates.Id == Costanti.PraticaNuova || contratto.OrderStates.Id == Costanti.PraticaInviata)
        //        {
        //            destinatari.AddRange(_userManager.GetUsersInRoleAsync(Costanti.Amministratore.ToString()).Result.ToList());
        //        }

        //        destinatari.Add(_userManager.FindByIdAsync(contratto.IdAgente.ToString()).Result);

        //        List<Comunicazione> messages = new List<Comunicazione>();

        //        foreach (ApplicationUser user in destinatari)
        //        {
        //            var intestazioneMessaggio = contratto.IdPratica != null ? contratto.IdPratica : idContratto.ToString();
        //            var cliente = !string.IsNullOrEmpty(contratto.Client.Name) && !string.IsNullOrEmpty(contratto.Client.Surname)
        //                    ? $"{contratto.Client.Name} {contratto.Client.Surname}" : contratto.Client.TaxCode;
        //            Comunicazione messageToAdd = new Comunicazione();
        //            messageToAdd.DataInserimento = DateTime.Now;
        //            messageToAdd.Oggetto = (template == "EmailAggiuntaContratto") ? "Nuovo contratto aggiunto" : "Contratto modificato";
        //            messageToAdd.Messaggio = (template == "EmailAggiuntaContratto") ?
        //                "Dati contratto " +
        //                "</br> Id: " + intestazioneMessaggio +
        //                "</br> Data Inserimento: " + contratto.DataInserimento.ToString("dd-MM-yyyy") +
        //                "</br> Agente: " + contratto.Agente +
        //                "</br> Cliente: " + cliente
        //                :
        //                "Dati contratto " +
        //                "</br> Id: " + intestazioneMessaggio +
        //                "</br> Data Inserimento: " + contratto.DataInserimento.ToString("dd-MM-yyyy") +
        //                "</br> Agente: " + contratto.Agente +
        //                "</br> Cliente: " + cliente +
        //                "</br> Prodotto: " + contratto.Prodotto.Descrizione +
        //                "</br> Note: " + contratto.Note +
        //                "</br> Stato pratica: " + contratto.OrderStates.Descrizione.ToString();
        //            messageToAdd.Template = template;
        //            messageToAdd.IdContratto = idContratto;
        //            messageToAdd.Stato = Costanti.ComunicazioneNonElaborata;
        //            messageToAdd.StatoLettura = Costanti.ComunicazioneNonLetta;
        //            messageToAdd.Destinatario = user.Id;
        //            messages.Add(messageToAdd);
        //        }

        //        await _db.Messaggi.AddRangeAsync(messages);

        //        var result = _db.SaveChanges();

        //        if (result > 0)
        //        {
        //            BackgroundJob.Enqueue<IMailSender>(x => x.ExecuteSync());
        //        }
        //        else
        //        {
        //            _logger.LogError("Rilevato problema durante l'inserimento delle comunicazioni");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, e.Message, e.InnerException);
        //    }
        //}
    }
}
