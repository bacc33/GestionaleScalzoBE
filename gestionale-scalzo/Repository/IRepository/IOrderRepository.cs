using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;

namespace gestionale_scalzo.Repository.IRepository
{
    public interface IOrderRepository
    {

        /// <summary>
        /// Aggiunta / Modifica di un ordine
        /// </summary>
        /// <param name="orderDataDTO"></param>
        /// <returns></returns>
        Task<APIResponse> UpsertOrder(OrderDTO orderDataDTO);

        /// <summary>
        /// Restistuisce i dettagli dell'ordine
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<APIResponse> GetOrder(int? id);

        /// <summary>
        /// Restituisce le gli ordini in base al filtro impostato
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<APIResponse> GetFilteredOrders(FilteredOrderDataDTO filter);

        /// <summary>
        /// Eliminazione dell'ordine
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<APIResponse> DeleteOrder(int id);
    }
}
