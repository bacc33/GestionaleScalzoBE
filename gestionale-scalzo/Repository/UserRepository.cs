using AutoMapper;
using gestionale_scalzo.Data;
using gestionale_scalzo.Model;
using gestionale_scalzo.Model.DTO;
using gestionale_scalzo.Repository.IRepository;
using gestionale_scalzo.Services;
using gestionale_scalzo.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace gestionale_scalzo.Repository
{

    

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        JwtSettings _jwtSettings;
        private readonly ITokenUtility _jwtTokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IUserRepository> _logger;
        //private readonly IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, IMapper mapper,
            RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, ILogger<IUserRepository> logger, ITokenUtility tokenService,
            IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            //_mapper = mapper;
            _logger = logger;
            _jwtTokenService = tokenService;
            _configuration = configuration;
            _roleManager = roleManager;
            _jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        #region UTENTI

        public async Task<APIResponse> Upsert(RegisterRequestDTO dto)
        {
            var response = new APIResponse();

            try
            {
                IdentityResult result;
                ApplicationUser user;
                string temporaryPassword = string.Empty;

                // --- UPDATE
                if (!string.IsNullOrEmpty(dto.Id))
                {
                    user = await _userManager.FindByIdAsync(dto.Id);
                    if (user == null)
                    {
                        response.StatusCode = HttpStatusCode.NotFound;
                        response.IsSuccess = false;
                        response.ErrorMessages = new List<string> { "Utente non trovato" };
                        return response;
                    }

                    // Mappa i campi modificabili
                    user.Name = dto.Name;
                    user.Surname = dto.Surname;
                    user.UserName = dto.Username;
                    user.Email = dto.Email;
                    user.PhoneNumber = dto.PhoneNumber;

                    result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.IsSuccess = false;
                        response.ErrorMessages = result.Errors.Select(e => e.Description).ToList();
                        return response;
                    }
                }
                // --- INSERT
                else
                {
                    bool usernameUnique = await IsUniqueUser(dto.Username);
                    var emailExists = await _userManager.FindByEmailAsync(dto.Email) != null;

                    if (!usernameUnique || emailExists)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.IsSuccess = false;
                        response.ErrorMessages = new List<string> { "Esiste già un'utenza associata ai dati forniti" };
                        return response;
                    }

                    user = new ApplicationUser
                    {
                        UserName = dto.Username,
                        Email = dto.Email,
                        NormalizedEmail = dto.Email.ToUpper(),
                        Name = dto.Name,
                        Surname = dto.Surname,
                        PhoneNumber = dto.PhoneNumber,
                        FirstAccess = true,
                        Attivo = true
                    };

                    temporaryPassword = CreateTemporaryPassword();
                    result = await _userManager.CreateAsync(user, temporaryPassword);
                    if (!result.Succeeded)
                    {
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.IsSuccess = false;
                        response.ErrorMessages = result.Errors.Select(e => e.Description).ToList();
                        return response;
                    }
                }

                // Assicuriamoci che i ruoli esistano
                if (!await _roleManager.RoleExistsAsync("Amministratore"))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = "Amministratore", Descrizione = "Amministratore" });
                    await _roleManager.CreateAsync(new ApplicationRole { Name = "Ragioniere", Descrizione = "Ragioniere" });
                    await _roleManager.CreateAsync(new ApplicationRole { Name = "Magazziniere", Descrizione = "Magazziniere" });
                }

                // Rimuovo vecchio ruolo (se presente) e aggiungo il nuovo
                var oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                if (!string.IsNullOrEmpty(oldRole))
                    await _userManager.RemoveFromRoleAsync(user, oldRole);

                await _userManager.AddToRoleAsync(user, dto.Role);

                // Invia email di benvenuto o modifica
                SendEmail(
                    user.Email,
                    string.IsNullOrEmpty(temporaryPassword) ? "Dati utente modificati" : "Benvenuto nel portale ScalzoFruit",
                    string.IsNullOrEmpty(temporaryPassword) ? "EmailModificaDatiUtente" : "EmailBenvenuto",
                    user.UserName,
                    passwordTemporanea: temporaryPassword
                );

                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                response.Result = Costanti.DatabaseOK;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore in Upsert");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.Message };
                return response;
            }
        }


        public async Task<APIResponse> Login(LoginRequestDTO loginRequestDTO)
        {
            APIResponse response = new APIResponse();
            try
            {
                var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

                if (user == null)
                {
                    response.Result = new LoginResponseDTO()
                    {
                        Token = ""
                    };
                    response.ErrorMessages = new List<string>() { "Username non valido" };
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return response;
                }

                if (!user.Attivo)
                {
                    response.Result = new LoginResponseDTO()
                    {
                        Token = ""
                    };
                    response.ErrorMessages = new List<string>() { "Utente non abilitato all'accesso" };
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    response.IsSuccess = false;
                    return response;
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDTO.Password, false);

                if (!result.Succeeded)
                {
                    response.Result = new LoginResponseDTO()
                    {
                        Token = ""
                    };
                    response.ErrorMessages = new List<string>() { "Username non valido e/o password non corretta" };
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return response;
                }

                //if user was found generate JWT Token
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtTokenService.CreateToken(user, roles.FirstOrDefault());
                var refreshToken = _jwtTokenService.GenerateRefreshToken();


                LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
                {
                    Token = accessToken.Result,
                    RefreshToken = refreshToken.Result,
                    ID = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Role = roles.FirstOrDefault(),
                    IsFirstAccess = user.FirstAccess
                };

                user.RefreshToken = refreshToken.Result;
                user.RefreshTokenExpirationDate = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpirationMinutes);
                _db.SaveChanges();

                response.Result = loginResponseDTO;
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                return response;

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message, e.InnerException.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse> Logout(TokenApiModel tokenApiModel)
        {
            APIResponse response = new APIResponse();
            try
            {
                if (tokenApiModel == null)
                {
                    response.Result = new LoginResponseDTO()
                    {
                        Token = "",
                        RefreshToken = ""
                    };
                    response.ErrorMessages = new List<string>() { "Richiesta non valida" };
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return response;
                }

                string accessToken = tokenApiModel.AccessToken;
                string refreshToken = tokenApiModel.RefreshToken;

                var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
                var claimName = token.Claims.First(c => c.Type == "unique_name").Value;

                var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == Int32.Parse(claimName));

                if (user == null)
                {
                    response.Result = new LoginResponseDTO()
                    {
                        Token = "",
                        RefreshToken = ""
                    };
                    response.ErrorMessages = new List<string>() { "Richiesta non valida" };
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return response;
                }
                else
                {
                    user.RefreshToken = "";
                    user.RefreshTokenExpirationDate = DateTime.MinValue;
                    _db.SaveChanges();
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message, e.InnerException.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse> Refresh(TokenApiModel tokenApiModel)
        {
            APIResponse response = new APIResponse();
            try
            {
                if (tokenApiModel == null)
                {
                    response.Result = new LoginResponseDTO()
                    {
                        Token = "",
                        RefreshToken = ""
                    };
                    response.ErrorMessages = new List<string>() { "Richiesta non valida" };
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return response;
                }

                string accessToken = tokenApiModel.AccessToken;
                string refreshToken = tokenApiModel.RefreshToken;

                var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
                var claimName = token.Claims.First(c => c.Type == "unique_name").Value;

                var user = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == Int32.Parse(claimName));

                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpirationDate < DateTime.Now)
                {
                    response.Result = new LoginResponseDTO()
                    {
                        Token = "",
                        RefreshToken = ""
                    };
                    response.ErrorMessages = new List<string>() { "Token di aggiornamento non valido" };
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                    return response;
                }

                var roles = await _userManager.GetRolesAsync(user);
                var newAccessToken = _jwtTokenService.CreateToken(user, roles.FirstOrDefault());
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken.Result;
                user.RefreshTokenExpirationDate = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpirationMinutes);
                await _db.SaveChangesAsync();

                response.Result = new LoginResponseDTO()
                {
                    Token = newAccessToken.Result,
                    RefreshToken = newRefreshToken.Result,
                    ID = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Role = roles.FirstOrDefault(),
                    IsFirstAccess = user.FirstAccess
                };
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message, e.InnerException.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }


        public async Task<APIResponse> GetUsers()
        {
            APIResponse response = new APIResponse();
            try
            {
                var userList = await _db.Users
                                .Join(_db.UserRoles,
                                  a => a.Id,
                                  b => b.UserId,
                                  (a, b) => new { A = a, B = b })
                                .Join(_db.Roles,
                                  ab => ab.B.RoleId,
                                  c => c.Id,
                                  (ab, c) => new
                                  {
                                      User = ab.A,
                                      UserRoles = ab.B,
                                      Roles = c
                                  })
                                .Select(x => new ApplicationUserDTO
                                {
                                    Id = x.User.Id.ToString(),
                                    Username = x.User.UserName,
                                    Name = x.User.Name,
                                    Surname = x.User.Surname,
                                    Email = x.User.Email,
                                    PhoneNumber = x.User.PhoneNumber,
                                    Role = x.Roles.Name,
                                    Enabled = x.User.Attivo
                                })
                                .OrderBy(x => x.Id)
                                .ToListAsync();

                response.Result = userList;
                response.StatusCode = HttpStatusCode.OK;
                response.IsSuccess = true;
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message, e.InnerException.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }


        public async Task<APIResponse> Delete(int id)
        {
            APIResponse response = new APIResponse();
            ApplicationUser? user;
            bool result = false;
            try
            {
                user = _db.Users.FirstOrDefault(x => x.Id == id);

                if (user != null)
                {
                    result = (_db.Users.Remove(user).State.ToString() == "Deleted");
                }
                else
                {
                    response.Result = 0;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsSuccess = false;
                }

                if (result)
                {
                    await _db.SaveChangesAsync();
                    response.Result = this.GetUsers().Result.Result;
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message, e.InnerException.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }

        public async Task<APIResponse> ChangePassword(UserChangePwdDTO userChange)
        {
            APIResponse response = new APIResponse();
            ApplicationUser? user = null;
            string temporaryPassword = string.Empty;
            IdentityResult result = null;
            try
            {
                if (userChange.UserId > 0)
                {
                    string id = userChange.UserId.ToString();
                    user = await _userManager.FindByIdAsync(id);

                    if (user != null)
                    {
                        if (!string.IsNullOrEmpty(userChange.CurrentPassword))
                        {
                            result = await _userManager.ChangePasswordAsync(user, userChange.CurrentPassword, userChange.NewPassword);

                            user.FirstAccess = false;
                            await _userManager.UpdateAsync(user);
                        }
                        else
                        {
                            user.FirstAccess = true;
                            temporaryPassword = CreateTemporaryPassword();
                            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                            await _userManager.ResetPasswordAsync(user, token, temporaryPassword);

                            _logger.LogInformation($"Password temporanea generata per l'utente {user.UserName}: {temporaryPassword}");  

                            result = await _userManager.UpdateAsync(user);
                        }
                    }
                    else
                    {
                        response.Result = 0;
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.IsSuccess = false;
                    }
                }

                if (result.Succeeded)
                {
                    await _db.SaveChangesAsync();
                    if (!string.IsNullOrEmpty(temporaryPassword))
                    {
                        SendEmail(user.Email, "Procedura cambio password completata", "EmailModificaPassword", user.UserName, passwordTemporanea: temporaryPassword);
                    }

                    response.Result = Costanti.DatabaseOK;
                    response.StatusCode = HttpStatusCode.OK;
                    response.IsSuccess = true;

                }
                else
                {
                    response.Result = -1;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages = new List<string>() { "Non è stato possibile effettuare il cambio password" };
                    response.IsSuccess = false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { e.Message, e.InnerException.Message };
                response.StatusCode = HttpStatusCode.InternalServerError;
            }
            return response;
        }

        #endregion


        #region UTILITY

        /// <summary>
        /// return true se username non esiste
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<bool> IsUniqueUser(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.UserName == username) == null ? true : false;
        }


        /// <summary>
        /// crea una password temporanea
        /// </summary>
        /// <returns></returns>
        private string CreateTemporaryPassword()
        {
            string charsUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string charsLower = "abcdefghijklmnopqrstuvwxyz";
            string charsDigit = "0123456789";
            string charsNonAlpha = "!@#$%^&*()_+";
            var random = new Random();

            return new string(Enumerable.Repeat(charsUpper, 2)
              .Select(s => s[random.Next(s.Length)]).ToArray()) +
              new string(Enumerable.Repeat(charsLower, 2)
              .Select(s => s[random.Next(s.Length)]).ToArray()) +
              new string(Enumerable.Repeat(charsDigit, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray()) +
              new string(Enumerable.Repeat(charsNonAlpha, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public int SendEmail(string toEmail, string subject, string template, string userName, string passwordTemporanea = null)
        {
            try
            {
                //Escludi la tua email specifica
                //if (toEmail.Equals("r.marco9633@gmail.com", StringComparison.OrdinalIgnoreCase))
                //{
                //    _logger.LogInformation($"Email esclusa per destinatario: {toEmail}");
                //    return Costanti.DatabaseOK; // Ritorna successo senza inviare
                //}

                string htmlBody = string.Empty;
                string body = string.Empty;
                string agente = string.Empty;
                string cliente = string.Empty;

                using (StreamReader reader = new StreamReader(Path.Combine(_env.WebRootPath, _configuration.GetValue<string>("PATHTEMPLATEMODELLIEMAIL")) + template + ".html"))
                {
                    htmlBody = reader.ReadToEnd().ToString();
                    string portalLink = _configuration.GetValue<string>("URL");

                    switch (template)
                    {
                        case "EmailBenvenuto":
                            body = String.Format(htmlBody, userName, userName, passwordTemporanea, portalLink, DateTime.Now.Year);
                            break;
                        case "EmailModificaPassword":
                            body = String.Format(htmlBody, userName, userName, passwordTemporanea, portalLink, DateTime.Now.Year);
                            break;                        
                    }
                }


                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                      | SecurityProtocolType.Tls11
                                                      | SecurityProtocolType.Tls12;
                }

                var client = new SmtpClient(_configuration.GetValue<string>("SMTP_SERVER"), _configuration.GetValue<int>("SMTP_PORT"))
                {
                    Credentials = new NetworkCredential(_configuration.GetValue<string>("SMTP_USERNAME"), _configuration.GetValue<string>("SMTP_PASSWORD")),
                    EnableSsl = true
                };
                // Create email message
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("gestionale@scalzofruit.it");
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body.ToString();
                // Send email
                client.Send(mailMessage);

                return Costanti.DatabaseOK;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.InnerException);

                return 0;
            }
        }


        #endregion

    }
}
