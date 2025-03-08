using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ristorante_backend.Models;

namespace ristorante_backend.Repositories
{
    public class PiattoRepository
    {
        private const string connectionString = "Data Source=localhost;Initial Catalog=Ristorante;Integrated Security=True;TrustServerCertificate=True";

        public void ReadPiatto(SqlDataReader r, Dictionary<int, Piatto> piatti)
        {
            int id = r.GetInt32(r.GetOrdinal("Id"));

            if (piatti.TryGetValue(id, out Piatto? piatto) == false)
            {
                string nome = r.GetString(r.GetOrdinal("nome"));
                string descrizione = r.GetString(r.GetOrdinal("descrizione"));
                decimal prezzo = (decimal)r.GetDouble(r.GetOrdinal("prezzo"));
                piatto = new Piatto(id, nome, descrizione, prezzo);
                piatti.Add(id, piatto);

            }
            if (r.IsDBNull(r.GetOrdinal("Id_Categoria")) == false)
            {
                Categoria c = new();
                c.Id = r.GetInt32(r.GetOrdinal("Id_Categoria"));
                c.Nome = r.GetString(r.GetOrdinal("nome_Categoria"));
                piatto.CategoriaId = c.Id;
                piatto.Categoria = c;
            }

            if (r.IsDBNull(r.GetOrdinal("Id_Menu")) == false)
            {
                int menuId = r.GetInt32(r.GetOrdinal("Id_Menu"));
                string menuNome = r.GetString(r.GetOrdinal("nome_Menu"));
                int ristoranteId = r.GetInt32(r.GetOrdinal("Id_Ristorante"));
                string ristoranteNome = r.GetString(r.GetOrdinal("Nome_Ristorante"));
                Ristorante ristorante = new Ristorante(ristoranteId, ristoranteNome);
                Menu m = new Menu(menuId, menuNome, ristoranteId, ristorante);
                piatto.MenuId.Add(menuId);
                piatto.Menu.Add(m);
            }
        }

        public async Task<List<Piatto>> GetAllPiatto(int? limit = null)
        {
            Dictionary<int, Piatto> piatti = new Dictionary<int, Piatto>();

            string query = @$"SELECT {(limit == null ? "" : $"TOP {limit}")} p.*, C.Id as Id_Categoria, C.nome as nome_Categoria, M.Id as Id_Menu, M.nome as nome_Menu, R.Id as Id_Ristorante, R.Nome as Nome_Ristorante
                             FROM piatto p
                             LEFT JOIN Categoria C on p.CategoriaId = C.Id
                             LEFT JOIN PiattoMenu PM on PM.piattoId = P.Id
                             LEFT JOIN Menu M on PM.MenuId = M.Id
                             LEFT JOIN Ristorante R on M.ristoranteId = R.Id
                            ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReadPiatto(reader, piatti);
                        }
                    }
                }
                return piatti.Values.ToList();
            }
        }
        public async Task<List<Piatto>> GetPiattoByName(string nome)
        {
            Dictionary<int, Piatto> piatti = new Dictionary<int, Piatto>();

            string query = @$"SELECT p.*, C.Id as Id_Categoria, C.nome as nome_Categoria, M.Id as Id_Menu, M.nome as nome_Menu, R.Id as Id_Ristorante, R.Nome as Nome_Ristorante
                             FROM piatto p
                             LEFT JOIN Categoria C on p.CategoriaId = C.Id
                             LEFT JOIN PiattoMenu PM on PM.piattoId = P.Id
                             LEFT JOIN Menu M on PM.MenuId = M.Id
                             LEFT JOIN Ristorante R on M.ristoranteId = R.Id
                             WHERE p.nome like @nome
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
                            ReadPiatto(reader, piatti);

                        }
                    }
                }
                return piatti.Values.ToList();
            }
        }

        public async Task<Piatto> GetPiattoByIdAsync(int id)
        {
            Dictionary<int, Piatto> piatti = new Dictionary<int, Piatto>();

            string query = @$"SELECT p.*, C.Id as Id_Categoria, C.nome as nome_Categoria, M.Id as Id_Menu, M.nome as nome_Menu, R.Id as Id_Ristorante, R.Nome as Nome_Ristorante
                             FROM piatto p
                             LEFT JOIN Categoria C on p.CategoriaId = C.Id
                             LEFT JOIN PiattoMenu PM on PM.piattoId = P.Id
                             LEFT JOIN Menu M on PM.MenuId = M.Id
                             LEFT JOIN Ristorante R on M.ristoranteId = R.Id
                             WHERE p.id = @id
                            ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            ReadPiatto(reader, piatti);

                        }
                    }
                }
            }
            return piatti.Values.FirstOrDefault();
        }

        public async Task<(int, Piatto)> CreatePiatto(Piatto p)
        {
            string query = $"INSERT INTO Piatto (nome, descrizione, prezzo, categoriaId) VALUES (@nome, @descrizione, @prezzo, @categoriaId);" +
                        $"SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@nome", p.Nome);
                    command.Parameters.AddWithValue("@descrizione", p.Descrizione);
                    command.Parameters.AddWithValue("@prezzo", p.Prezzo);
                    command.Parameters.AddWithValue("@categoriaId", p.CategoriaId ?? (object)DBNull.Value);

                    int piattoId = Convert.ToInt32(await command.ExecuteScalarAsync());

                    p.Id = piattoId;

                    await GestisciMenu(p.MenuId, piattoId, conn);
                    return (piattoId, p);
                }
            }
        }

        public async Task<int> UpdatePiatto(int id, Piatto p)
        {
            string query = "UPDATE Piatto SET nome = @nome, descrizione = @descrizione, prezzo = @prezzo, categoriaId = @categoriaId WHERE Id = @Id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@nome", p.Nome);
                    command.Parameters.AddWithValue("@descrizione", p.Descrizione);
                    command.Parameters.AddWithValue("@prezzo", p.Prezzo);
                    command.Parameters.AddWithValue("@categoriaId", p.CategoriaId ?? (object)DBNull.Value);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    await GestisciMenu(p.MenuId, id, connection);

                    return rowsAffected;
                }
            }

        }

        public async Task<int> DeletePiatto(int id)
        {
            string query = "DELETE FROM piatto where id = @id";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> ClearPiattoMenu(int pizzaId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            string query = $"DELETE FROM PiattoMenu WHERE piattoId = @id";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add(new SqlParameter("@id", pizzaId));
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> AddPiattoMenu(int piattoId, List<int> menu)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            Piatto p = await GetPiattoByIdAsync(piattoId);
            int inserted = 0;
            foreach (int menuId in menu)
            {
                if (p.MenuId.Contains(menuId))
                    continue;
                string query = $"INSERT INTO PiattoMenu (piattoId, menuId) VALUES (@piattoId, @menuId)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@piattoId", piattoId);
                    cmd.Parameters.AddWithValue("@menuId", menuId);
                    inserted += await cmd.ExecuteNonQueryAsync();

                }

            }
            return inserted;
        }

        public async Task<int> DeletePiattoMenu(int piattoId, int menuId)
        {
            using SqlConnection conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            Piatto p = await GetPiattoByIdAsync(piattoId);
            int deleted = 0;
            string query = $"DELETE FROM PiattoMenu WHERE piattoId = @piattoId and menuId = @menuId";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@piattoId", piattoId);
                cmd.Parameters.AddWithValue("@menuId", menuId);
                deleted = await cmd.ExecuteNonQueryAsync();
            }
            return deleted;
        }

        private async Task GestisciMenu(List<int> menu, int piattoId, SqlConnection conn)
        {
            if (menu == null)
                return;

            // Rimuoviamo gli Ingredient relativi a questo Piatto
            await ClearPiattoMenu(piattoId);

            // Inseriamo i nuovi Ingredient
            await AddPiattoMenu(piattoId, menu);
        }

        public async Task<Tuple<double, int>> GetVoti(int piattoId)
        {
            Tuple<double, int> tupla = new Tuple<double, int>(0,0);
            using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            string query = "SELECT COUNT(*) as c, AVG(convert(float,VOTO)) as a FROM PiattoUtenteVoto where piattoId = @piattoId";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@piattoId", piattoId);
            SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (reader.Read())
            {
                double mediaVoti = reader.GetDouble(reader.GetOrdinal("a"));
                int totaleVoti = reader.GetInt32(reader.GetOrdinal("c"));
                tupla = new Tuple<double, int>(mediaVoti,totaleVoti);
            }

            return tupla;
        }
    }
}
