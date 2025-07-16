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
    public class TypologyRepository : ITypologyRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<TypologyRepository> _logger;

        public TypologyRepository(ApplicationDbContext db, ILogger<TypologyRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<APIResponse> DeleteTypology(int id)
        {
            APIResponse response = new APIResponse();
            Tipologia? tipologia;
            bool result = false;
            try
            {
                _logger.LogDebug("Cancellazione della tipologia con ID: {0}", id);

                tipologia = _db.Tipologie.FirstOrDefault(x => x.Id == id);

                result = (_db.Tipologie.Remove(tipologia).State.ToString() == "Deleted");

                if (result)
                {
                    await _db.SaveChangesAsync();
                    response.Result = this.GetTypology(0).Result;
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

        public async Task<APIResponse> GetTypology(int? id)
        {
            APIResponse response = new APIResponse();
            try
            {
                if (id == 0)
                {
                    response.Result = await _db.Tipologie
                    .Distinct()
                    .OrderByDescending(o => o.Id)
                    .ToListAsync();
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
                else
                {
                    var tipologia = await _db.Tipologie
                    .Distinct()
                    .Where(o => o.Id == id)
                    .SingleOrDefaultAsync();

                    response.Result = tipologia;
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

        public async Task<APIResponse> UpsertTypology(TypologyDTO typologyDTO)
        {
            APIResponse response = new APIResponse();
            bool result = false;
            Tipologia? tipologia= null;
            try
            {
                _logger.LogDebug("UPSERT della tipologia {0}", typologyDTO.ToString());

                if (typologyDTO.Id > 0)
                {
                    //si tratta di un update
                    tipologia = await _db.Tipologie.FirstOrDefaultAsync(x => x.Id == typologyDTO.Id);

                    if (tipologia != null)
                    {
                        tipologia.Type = typologyDTO.Type;
                        tipologia.Name = typologyDTO.Name;

                        result = (_db.Tipologie.Update(tipologia).State.ToString() == "Modified");
                    }

                }
                else
                {
                    //si tratta di una insert
                    tipologia = new Tipologia()
                    {
                        Type = typologyDTO.Type,
                        Name = typologyDTO.Name
                    };

                    result = (_db.Tipologie.AddAsync(tipologia).Result.State.ToString() == "Added");

                }
                if (result)
                {
                    await _db.SaveChangesAsync();

                    response.IsSuccess = true;
                    response.StatusCode = HttpStatusCode.Created;
                    response.Result = tipologia.Id;                   

                }

                else
                {
                    response.Result = Costanti.RichiestaNonValida;
                    response.StatusCode = HttpStatusCode.OK;
                    response.ErrorMessages = new List<string>() { "Esiste già una tipologia con l'id = " + tipologia.Id};
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
    }
}
