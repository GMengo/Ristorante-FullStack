using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ristorante_frontend.Models
{
    public class Jwt
    {
        public string Token { get; set; } // stringa base64
        public DateTime ExpirationUtc { get; set; }
        public List<string> Roles { get; set; } // Lo decodificheremo dal token
    }
}
