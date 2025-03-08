using Microsoft.Data.SqlClient;
using ristorante_backend.Models;

namespace ristorante_backend.Repositories
{
    public class CategoriaRepository
    {
        private const string connectionString = "Data Source=localhost;Initial Catalog=Ristorante;Integrated Security=True;Trust Server Certificate=True";

        public Categoria ReadCategoria(SqlDataReader reader)
        {
            Categoria c = new Categoria();
            c.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            c.Nome = reader.GetString(reader.GetOrdinal("nome"));
            return c;
        }

        public async Task<List<Categoria>> GetCategorie()
        {
            List<Categoria> listaCategorie = new List<Categoria>();
            string query = "SELECT * FROM Categoria";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Categoria c = ReadCategoria(reader);
                            listaCategorie.Add(c);
                        }
                    }
                }
            }
            return listaCategorie;
        }

        public async Task<List<Categoria>> GetCategorieByNome(string nome)
        {
            List<Categoria> listaCategorie = new List<Categoria>();
            string query = "SELECT * FROM Categoria where nome like @nome";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@nome", $"%{nome}%");
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    listaCategorie.Add(ReadCategoria(reader));
                }
            }
            return listaCategorie;
        }

        public async Task<Categoria> GetCategoriaById(int id)
        {
            string query = "SELECT TOP 1 * FROM Categoria WHERE id = @id";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                Categoria categoria = ReadCategoria(reader);
                return categoria;
            }
            return null;
        }

        public async Task<int> InsertCategoria(Categoria categoria)
        {
            string query = "INSERT INTO Categoria (nome) VALUES (@nome)" + "SELECT SCOPE_IDENTITY()";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@nome", categoria.Nome);

            return Convert.ToInt32(await cmd.ExecuteScalarAsync()); ;
        }

        public async Task<int> UpdateCategoria(int id, Categoria categoria)
        {
            string query = "UPDATE Categoria SET nome = @nome WHERE id = @id";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nome", categoria.Nome);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteCategoria(int id)
        {
            string query = "DELETE FROM Categoria WHERE id = @id";
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync();
        }

    }
}
