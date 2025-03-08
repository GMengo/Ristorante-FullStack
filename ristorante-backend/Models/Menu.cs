namespace ristorante_backend.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int RistoranteId { get; set; }
        public Ristorante Ristorante { get; set; }

        public Menu() { }

        public Menu(int id, string nome, int ristoranteId, Ristorante ristorante)
        {
            this.Id = id;
            this.Nome = nome;
            this.RistoranteId = ristoranteId;
            this.Ristorante = ristorante;
        }
    }
}
