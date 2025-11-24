using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
namespace ConciergeAI.Services
{
public class ClientModel : Supabase.Postgrest.Models.BaseModel
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Contact { get; set; }
        public string WhatsappNumber { get; set; }
    }
}