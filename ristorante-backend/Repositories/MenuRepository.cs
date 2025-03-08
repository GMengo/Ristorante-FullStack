using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ristorante_backend.Models;

namespace ristorante_backend.Repositories
{
    public class MenuRepository
    {
        private const string connectionString = "Data Source=localhost;Initial Catalog=Ristorante;Integrated Security=True;TrustServerCertificate=True";
        private PiattoRepository _piattoRepository; 

        public MenuRepository(PiattoRepository piattoRepository)
        {
            _piattoRepository = piattoRepository;
        }
        private Menu ReadMenu(SqlDataReader reader)
        {
            var id = reader.GetInt32(reader.GetOrdinal("id"));
            var nome = reader.GetString(reader.GetOrdinal("nome"));
            int ristoranteId = reader.GetInt32(reader.GetOrdinal("ristoranteId"));
            string ristoranteNome = reader.GetString(reader.GetOrdinal("ristoranteNome"));
            Ristorante ristorante = new Ristorante(ristoranteId, ristoranteNome);   
            var menu = new Menu(id, nome, ristoranteId, ristorante);
            return menu;
        }

        public async Task<List<Menu>> GetAllMenuAsync()
        {
            string query = "SELECT m.*, r.Id as ristoranteId, r.Nome as ristoranteNome FROM menu m INNER JOIN ristorante r ON m.ristoranteId = r.Id";
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            List<Menu> menu = new List<Menu>();
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        menu.Add(ReadMenu(reader));
                    }
                }
            }
            return menu;
        }

        public async Task<Menu> GetMenuByIdAsync(int id)
        {
            string query = "SELECT m.*, r.Id as ristoranteId, r.Nome as ristoranteNome FROM menu m INNER JOIN ristorante r ON m.ristoranteId = r.Id where m.id = @id";
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    Menu menu = ReadMenu(reader);
                    return menu;
                }

            }
            return null;
        }

        public async Task<List<Piatto>> GetAllPiattoFromMenuId(int id)
        {
            Dictionary<int, Piatto> piatti = new Dictionary<int, Piatto>();

            string query = @$"SELECT p.*, C.Id as Id_Categoria, C.nome as nome_Categoria, M.Id as Id_Menu, M.nome as nome_Menu, R.Id as Id_Ristorante, R.Nome as Nome_Ristorante
                             FROM piatto p
                             LEFT JOIN Categoria C on p.CategoriaId = C.Id
                             LEFT JOIN PiattoMenu PM on PM.piattoId = P.Id
                             LEFT JOIN Menu M on PM.MenuId = M.Id
                             INNER JOIN Ristorante R on M.ristoranteId = R.Id
                             WHERE M.Id = @id;
                            ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            _piattoRepository.ReadPiatto(reader, piatti);
                        }
                    }
                }
                return piatti.Values.ToList();
            }
        }

        public async Task<List<Piatto>> GetAllPiattoFromMenuNome(string nome)
        {
            Dictionary<int, Piatto> piatti = new Dictionary<int, Piatto>();

            string query = @$"SELECT p.*, C.Id as Id_Categoria, C.nome as nome_Categoria, M.Id as Id_Menu, M.nome as nome_Menu, R.Id as Id_Ristorante, R.Nome as Nome_Ristorante
                             FROM piatto p
                             LEFT JOIN Categoria C on p.CategoriaId = C.Id
                             LEFT JOIN PiattoMenu PM on PM.piattoId = P.Id
                             LEFT JOIN Menu M on PM.MenuId = M.Id
                             INNER JOIN Ristorante R on M.ristoranteId = R.Id
                             WHERE M.nome like @nome;
                            ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nome", $"%{nome}%");
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            _piattoRepository.ReadPiatto(reader, piatti);
                        }
                    }
                }
                return piatti.Values.ToList();
            }
        }

        public async Task<int> InsertMenu(Menu menu)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            string query = $"INSERT INTO menu (nome, ristoranteId) VALUES (@nome,@ristoranteId);" +
                        $"SELECT SCOPE_IDENTITY();";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@nome", menu.Nome));
                cmd.Parameters.Add(new SqlParameter("@ristoranteId", menu.RistoranteId));

                menu.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return menu.Id;
            }
        }

        public async Task<int> UpdateMenuAsync(int id, Menu menu)
        {
            string query = "UPDATE Menu SET nome = @nome WHERE Id = @id";
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nome", menu.Nome);

            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteMenuAsync(int id)
        {
            string query = "DELETE FROM menu WHERE Id = @id";
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
