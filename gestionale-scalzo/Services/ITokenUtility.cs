using gestionale_scalzo.Model;

namespace gestionale_scalzo.Services
{
    public interface ITokenUtility
    {
        /// <summary>
        /// Stacca la sessione 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> CreateToken(ApplicationUser user, string roles);

        /// <summary>
        /// RefreshToken viene utilizzato quando scade la durata del token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> GenerateRefreshToken();
    }
}
