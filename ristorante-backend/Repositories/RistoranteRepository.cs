using Microsoft.Data.SqlClient;
using ristorante_backend.Models;

namespace ristorante_backend.Repositories
{
    public class RistoranteRepository
    {
        private const string connectionString = "Data Source=localhost;Initial Catalog=Ristorante;Integrated Security=True;Trust Server Certificate=True";

        public Ristorante ReadRistorante(SqlDataReader reader)
        {
            Ristorante ristorante = new Ristorante();
            ristorante.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            ristorante.Nome = reader.GetString(reader.GetOrdinal("nome"));
            return ristorante;
        }

        public async Task<List<Ristorante>> GetRistoranti()
        {
            List<Ristorante> listaCategorie = new List<Ristorante>();
            string query = "SELECT * FROM Ristorante";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Ristorante ristorante = ReadRistorante(reader);
                            listaCategorie.Add(ristorante);
                        }
                    }
                }
            }
            return listaCategorie;
        }

        public async Task<List<Ristorante>> GetRistoranteByNome(string nome)
        {
            List<Ristorante> listaCategorie = new List<Ristorante>();
            string query = "SELECT * FROM Ristorante where nome like @nome";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@nome", $"%{nome}%");
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    listaCategorie.Add(ReadRistorante(reader));
                }
            }
            return listaCategorie;
        }

        public async Task<Ristorante> GetRistoranteById(int id)
        {
            string query = "SELECT TOP 1 * FROM Ristorante WHERE id = @id";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                Ristorante ristorante = ReadRistorante(reader);
                return ristorante;
            }
            return null;
        }

        public async Task<int> InsertRistorante(Ristorante ristorante)
        {
            string query = "INSERT INTO Ristorante (nome) VALUES (@nome)" + "SELECT SCOPE_IDENTITY()";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@nome", ristorante.Nome);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync()); ;
        }

        public async Task<int> UpdateRistorante(int id, Ristorante ristorante)
        {
            string query = "UPDATE Ristorante SET nome = @nome WHERE id = @id";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nome", ristorante.Nome);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteRistorante(int id)
        {
            string query = "DELETE FROM Ristorante WHERE id = @id";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
