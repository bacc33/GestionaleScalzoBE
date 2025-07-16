using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;

namespace gestionale_scalzo.Repository.IRepository
{
    public interface ITypologyRepository
    {
        /// <summary>
        /// Aggiunta / Modifica di una tipologia
        /// </summary>
        /// <param name="orderDataDTO"></param>
        /// <returns></returns>
        Task<APIResponse> UpsertTypology(TypologyDTO typologyDTO);

        /// <summary>
        /// Restistuisce i dettagli della tipologia o tutte le tipologie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<APIResponse> GetTypology(int? id);

        /// <summary>
        /// Eliminazione della tipologia
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<APIResponse> DeleteTypology(int id);
    }
}
