using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace ConciergeAI.Services
{
public class Message : Supabase.Postgrest.Models.BaseModel
    {
        public Message() {}
        private int v1;
        private string v2;
        private string v3;
        private DateTime now;

        public Message(int v1, string v2, string v3, DateTime now)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.now = now;
        }
       [PrimaryKey("Id")]
       public int Id { get; set; }
        public int LogementId { get; set; }
        public string Type { get; set; }
        public string Contenu { get; set; }
        public System.DateTime Timestamp { get; set; }
    }


    }


  
