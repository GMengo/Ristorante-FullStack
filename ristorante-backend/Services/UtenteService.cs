using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using ristorante_backend.Models;

namespace ristorante_backend.Services
{
    public class UtenteService
    {
        public const string connectionString = "Data Source=localhost;Initial Catalog=Ristorante;Integrated Security=True;Trust Server Certificate=True";

        private readonly IPasswordHasher<UtenteModel> _passwordHasher;

        public UtenteService(IPasswordHasher<UtenteModel> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> RegisterAsync(UtenteModel utente)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            string ricercaUtente = "SELECT * FROM Utente where Email = @Email";
            using (SqlCommand commandRicerca = new SqlCommand(ricercaUtente, connection))
            {
                commandRicerca.Parameters.AddWithValue("@Email", utente.Email);
                SqlDataReader reader = await commandRicerca.ExecuteReaderAsync();
                if (reader.Read())
                    throw (new Exception(message: "Esiste già un utente registrato con l' email inserita"));
                await reader.CloseAsync();
            }
            bool validatoreNumero = false;
            bool validatoreMaiuscola = false;

            if (utente.Password.Length < 8)
                return false;

            foreach (char carattere in utente.Password)
            {
                try
                {
                    string daCharaString = Convert.ToString(carattere);
                    int tester = Convert.ToInt32(daCharaString);
                    validatoreNumero = true;
                }
                catch (Exception ex) { }

                if (char.IsLetter(carattere) && carattere.ToString() == carattere.ToString().ToUpper())
                {
                    validatoreMaiuscola = true;
                }
            }
            if (validatoreMaiuscola == false || validatoreNumero == false)
            {
                return false;
            }

            string passwordHash = _passwordHasher.HashPassword(utente, utente.Password);

            string query = "INSERT INTO Utente (Email, PasswordHash) VALUES (@Email, @PasswordHash)";
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", utente.Email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            return await command.ExecuteNonQueryAsync() > 0;

        }

        public async Task<Utente> AuthenticateAsync(string email, string password)
        {
            string query = "SELECT * FROM Utente WHERE Email = @Email";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue($"@Email", email);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                int id = reader.GetInt32(reader.GetOrdinal("Id"));
                string passwordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                UtenteModel model = new UtenteModel() { Email = email, Password = password };
                if (_passwordHasher.VerifyHashedPassword(model, passwordHash, password) != PasswordVerificationResult.Success)
                {
                    return null;
                }
                return new Utente() { Id = id, Email = email };
            }
            return null;
        }

        public async Task<List<string>> GetUserRolesAsync(int utenteId)
        {
            List<string> ruoli = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(
                "SELECT r.Nome " +
                "FROM Ruolo r " +
                "INNER JOIN UtenteRuolo ur ON r.Id = ur.RuoloId " +
                "WHERE ur.UtenteId = @UtenteId", connection);
                command.Parameters.AddWithValue("@UtenteId", utenteId);
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ruoli.Add(reader.GetString(0));
                }
            }
            return ruoli;
        }

        public async Task<Utente> GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "select * from utente where id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    Utente u = new Utente();
                    u.Id = reader.GetInt32(reader.GetOrdinal("id"));
                    u.Email = reader.GetString(reader.GetOrdinal("email"));
                    return u;
                }
            }
            return null;
        }

        public async Task<Utente> GetUserByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "select * from utente where email = @email";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);
                var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    Utente u = new Utente();
                    u.Id = reader.GetInt32(reader.GetOrdinal("id"));
                    u.Email = reader.GetString(reader.GetOrdinal("email"));
                    return u;
                }
            }
            return null;
        }

        public async Task<int> PostVotoPiatto(int piattoId, int utenteId, int voto)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "INSERT INTO PiattoUtenteVoto (piattoId, utenteId, voto) values (@piattoId, @utenteId, @voto)";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@piattoId", piattoId);
            cmd.Parameters.AddWithValue("@utenteId", utenteId);
            cmd.Parameters.AddWithValue("@voto", voto);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> PutVotoPiatto(int piattoId, int utenteId, int voto)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "UPDATE PiattoUtenteVoto SET voto = @voto WHERE piattoId = @piattoId and utenteId = @utenteId";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@piattoId", piattoId);
            cmd.Parameters.AddWithValue("@utenteId", utenteId);
            cmd.Parameters.AddWithValue("@voto", voto);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteVotoPiatto(int piattoId, int utenteId)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "DELETE PiattoUtenteVoto WHERE piattoId = @piattoId and utenteId = @utenteId";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@piattoId", piattoId);
            cmd.Parameters.AddWithValue("@utenteId", utenteId);
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<object>> GetPiattiVotati(int utenteId)
        {
            List<object> list = new List<object>();
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "Select p.Id as piattoId, p.Nome as piattoNome, puv.Voto as voto from piatto p inner join PiattoUtenteVoto puv on p.id = puv.piattoId where utenteId = @utenteId";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@utenteId", utenteId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (reader.Read())
            {
                int piattoId = reader.GetInt32(reader.GetOrdinal("piattoId"));
                string nome = reader.GetString(reader.GetOrdinal("piattoNome"));
                int voto = reader.GetInt32(reader.GetOrdinal("voto"));
                list.Add(new
                {
                    PiattoId = piattoId,
                    Nome = nome,
                    Voto = voto
                });
            }
            return list;

        }
    }
}
