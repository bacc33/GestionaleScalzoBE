using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;

namespace gestionale_scalzo.Repository.IRepository
{
    public interface IUserRepository
    {
        /// <summary>
        /// Login dell'utente nel portale
        /// </summary>
        /// <param name="loginRequestDTO"></param>
        /// <returns></returns>
        Task<APIResponse> Login(LoginRequestDTO loginRequestDTO);

        /// <summary>
        /// Logout dell'utente nel portale
        /// </summary>
        /// <param name="okenApiModel"></param>
        /// <returns></returns>
        Task<APIResponse> Logout(TokenApiModel okenApiModel);

        /// <summary>
        /// Metodo per fare il refresh del token
        /// </summary>
        /// <param name="tokenApiModel"></param>
        /// <returns></returns>
        Task<APIResponse> Refresh(TokenApiModel tokenApiModel);

        /// <summary>
        /// Inserimento/Modifica di un'utenza
        /// </summary>
        /// <param name="registerationRequestDTO"></param>
        /// <returns></returns>
        Task<APIResponse> Upsert(RegisterRequestDTO registerationRequestDTO);

        /// <summary>
        /// Cancellazione di un'utenza
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<APIResponse> Delete(int id);

        /// <summary>
        /// Restituisce la lista di tutti gli utenti
        /// </summary>
        /// <returns></returns>
        Task<APIResponse> GetUsers();

        /// <summary>
        /// Metodo per il cambio pw
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<APIResponse> ChangePassword(UserChangePwdDTO user);

    }
}
