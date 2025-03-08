using ristorante_frontend.Models;
using System.Net.Http.Json;
using System.Net.Http;
using System.Security.Claims;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Net.Http.Headers;

namespace ristorante_frontend.Services
{

    public enum ApiServiceResultType
    {
        Success,
        Error
    }
    public static class ApiService
    {
        public const string API_URL = "http://localhost:5000";
        public static string Email { get; set; }
        public static string Password { get; set; }
        public static async Task<ApiServiceResult<bool>> Register()
        {
            try
            {
                using HttpClient client = new HttpClient();
                var response = await client.PostAsync($"{API_URL}/Account/Register",
                    JsonContent.Create(new { Email = Email, Password = Password }));

                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new ApiServiceResult<bool>(true);
                }
                else
                {
                    // Tentativo di estrarre il messaggio d'errore direttamente
                    string errorMessage = "Registrazione fallita";
                    try
                    {
                        var errorObject = JsonDocument.Parse(responseBody);
                        if (errorObject.RootElement.TryGetProperty("message", out JsonElement messageProp))
                        {
                            errorMessage = messageProp.GetString() ?? errorMessage;
                        }
                    }
                    catch { /* Se il parsing fallisce, mantieni il messaggio di default */ }

                    return new ApiServiceResult<bool>(new Exception(errorMessage));
                }
            }
            catch (Exception ex)
            {
                return new ApiServiceResult<bool>(ex);
            }
        }

        public static async Task<ApiServiceResult<Jwt>> GetJwtToken()
        {
            try
            {
                using HttpClient client = new HttpClient();
                var httpResult = await client.PostAsync($"{API_URL}/Account/Login",
                    JsonContent.Create(new { Email = Email, Password = Password }));

                var resultBody = await httpResult.Content.ReadAsStringAsync();

                if (!httpResult.IsSuccessStatusCode)
                {
                    // Gestione errori dal backend
                    string errorMessage = "Login fallito";
                    try
                    {
                        var errorObject = JsonDocument.Parse(resultBody);
                        if (errorObject.RootElement.TryGetProperty("message", out JsonElement messageProp))
                        {
                            errorMessage = messageProp.GetString() ?? errorMessage;
                        }
                    }
                    catch { /* Ignora errori di parsing */ }

                    return new ApiServiceResult<Jwt>(new Exception(errorMessage));
                }

                var data = JsonConvert.DeserializeObject<Jwt>(resultBody);

                if (string.IsNullOrEmpty(data?.Token))
                {
                    return new ApiServiceResult<Jwt>(new Exception("Token non valido"));
                }

                AddRolesToJwt(data);
                return new ApiServiceResult<Jwt>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<Jwt>(e);
            }
        }
        private static void AddRolesToJwt(Jwt jwt)
        {
            try
            {
                // Decodifichiamo il JWT
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(jwt.Token);

                // Vediamo se ci sono ruoli nel JWT
                var roles = jwtToken.Claims
                    .Where((Claim c) => c.Type == "role")
                    .Select(c => c.Value).ToList();

                // Aggiungiamoli nel nostro DTO (data transfer object) rappresentante il JWT
                jwt.Roles = roles;
            }
            catch { } // Se succede qualcosa non facciamo nulla
        }

        public static async Task<ApiServiceResult<List<Piatto>>> GetPiatto()
        {
            try
            {
                using HttpClient client = new HttpClient();
                var httpResult = await client.GetAsync($"{API_URL}/Piatto");
                var resultBody = await httpResult.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<Piatto>>(resultBody);
                return new ApiServiceResult<List<Piatto>>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<List<Piatto>>(e);
            }
        }

        /// <summary>
        /// Richiama l'API per creare un piatto e ne ritorna, in caso di successo, un interno che rappresenta l'ID del nuovo piatto
        /// </summary>
        /// <param name="newPiatto"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public static async Task<ApiServiceResult<int>> CreatePiatto(Piatto newPiatto, Jwt jwt)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                // Devo aggiungere il JWT
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.Token);

                var httpResult = await httpClient.PostAsync($"{API_URL}/Piatto", JsonContent.Create(newPiatto));
                var resultBody = await httpResult.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<int>(resultBody); // L'API ritorna un intero che rappresenta il nuovo ID del piatto
                return new ApiServiceResult<int>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<int>(e);
            }
        }
        /// <summary>
        /// Richiamo l'API per aggiornare un piatto. Ritorna il numero di righe interessate
        /// </summary>
        public static async Task<ApiServiceResult<int>> UpdatePiatto(Piatto piatto, Jwt token)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

                var httpResult = await httpClient.PutAsync($"{API_URL}/Piatto/{piatto.Id}", JsonContent.Create(piatto));
                var resultBody = await httpResult.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<int>(resultBody);
                return new ApiServiceResult<int>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<int>(e);
            }
        }
        public static async Task<ApiServiceResult<int>> DeletePiatto(int piattoId, Jwt token)
        {
            try
            {
                using HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

                var httpResult = await httpClient.DeleteAsync($"{API_URL}/Piatto/{piattoId}");
                var resultBody = await httpResult.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<int>(resultBody);
                return new ApiServiceResult<int>(data);
            }
            catch (Exception e)
            {
                return new ApiServiceResult<int>(e);
            }
        }
    }
}