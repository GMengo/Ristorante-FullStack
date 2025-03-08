namespace ristorante_backend.Models
{
    public class Ristorante
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public Ristorante() { }

        public Ristorante(int id, string nome)
        {
            Id = id;
            Nome = nome;
        }
    }
}
