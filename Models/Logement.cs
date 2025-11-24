using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
namespace ConciergeAI.Services
{
public class Logement : Supabase.Postgrest.Models.BaseModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Nom { get; set; }
        public string Adresse { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string Wifi { get; set; }
        public string Instructions { get; set; }
    }
   }