using System.ComponentModel.DataAnnotations;

namespace ristorante_backend.Models
{
    public class Piatto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il nome non può avere più di 50 caratteri")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        [StringLength(50, ErrorMessage = "La descrizione non può avere più di 255 caratteri")]
        public string Descrizione { get; set; }
        [Required(ErrorMessage = "Il prezzo è obbligatorio")]
        [Range(0.1, 1000)]
        public decimal Prezzo { get; set; }
        public int? CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
        public List<int> MenuId { get; set; } = new List<int>();
        public List<Menu> Menu { get; set; } = new List<Menu>();

        public Piatto(int id, string nome, string descrizione, decimal prezzo)
        {
            Id = id;
            Nome = nome;
            Descrizione = descrizione;
            Prezzo = prezzo;
        }
    }
}
