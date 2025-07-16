using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;

namespace gestionale_scalzo.Repository.IRepository
{
    public interface IClientRepository
    {
        /// <summary>
        /// Restituisce tutti i clienti associati all'agente indicato
        /// </summary>
        /// <returns></returns>
        Task<APIResponse> GetClients(string? codiceFiscale);

        /// <summary>
        /// Aggiunta/Modifica cliente
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        Task<APIResponse> UpsertClient(ClientDTO client);

        /// <summary>
        /// Cancellazione del cliente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<APIResponse> DeleteClient(int id);
    }
}
