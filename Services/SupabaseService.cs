using SupabaseClientAlias = Supabase.Client;
using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System;
using System.Threading.Tasks;

namespace ConciergeAI.Services
{
    public class SupabaseService
    {
        private readonly SupabaseClientAlias _client;
         private readonly string url;
    private readonly string key;

             public SupabaseService(IConfiguration configuration)
        {
            url = Environment.GetEnvironmentVariable("SUPABASE_URL") 
               ?? throw new Exception("SUPABASE_URL missing");

        key = Environment.GetEnvironmentVariable("SUPABASE_API_KEY") 
               ?? throw new Exception("SUPABASE_API_KEY missing");

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
                throw new Exception("Supabase credentials not found in environment variables.");

            _client = new SupabaseClientAlias(
                url,
                key,
                new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = false
                }
            );
        }

       public async Task<Logement?> GetLogementAsync(int clientId)
{
    await _client.InitializeAsync();
    return await _client
        .From<Logement>()
        .Where(l => l.ClientId == clientId)
        .Single();
}


public async Task<ClientModel?> GetClientByPhoneNumber(string phoneNumber)
{
    await _client.InitializeAsync();

    // Utiliser SingleOrDefaultAsync au lieu de SingleOrDefault
    var client = await _client
        .From<ClientModel>()
        .Where(c => c.WhatsappNumber == phoneNumber)
        .Single();

    return client;
}

public async Task TestConnection()
{
   var client = new Supabase.Client(url, key);
await client.InitializeAsync();

// Exemple simple pour tester la connexion
var table = client.From<ClientModel>();
var data = await table.Get();
Console.WriteLine($"Nombre de clients : {data.Count}");

}



        public async Task InsertMessageAsync<T>(T entity)
            where T : BaseModel, new()
        {
            try{ 
            await _client.InitializeAsync();
      
            await _client
                .From<T>()
                .Insert(entity);

                }
        catch (Exception ex)
            {
                var typeProperty = typeof(T).GetProperty("Type");
                var value = typeProperty.GetValue(entity);
                Console.WriteLine($"[InsertMessageAsync] ERREUR {value}: {ex.Message}");
            }
        }
        }
    }



/*
using Supabase;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConciergeAI.Services
{
    public class SupabaseService
    {
        private readonly Supabase.Client _client;

        public SupabaseService(string url, string apiKey)
        {
            _client = new Supabase.Client(url, apiKey);
        }

        public async Task InitializeAsync()
        {
            await _client.InitializeAsync();
        }
private readonly string _url;
private readonly string _apiKey;
public SupabaseService(IConfiguration config)
{
    _url = config["Supabase:Url"];
    _apiKey = config["Supabase:ApiKey"];
}

        public async Task TestConnection()
{
    try
    {
        var client = new Supabase.Client(_url, _apiKey);
        Console.WriteLine("Connexion Supabase OK : client instancié !");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Erreur Supabase : " + ex.Message);
        throw;
    }
}


        // Insérer un enregistrement dans Supabase
        public async Task InsertAsync<T>(T record) where T : Supabase.Postgrest.Models.BaseModel, new()
        {
            await _client.From<T>().Insert(record);
        }

        // Récupérer tous les enregistrements d'une table
        public async Task<List<T>> GetAllAsync<T>() where T : Supabase.Postgrest.Models.BaseModel, new()
        {
            var response = await _client.From<T>().Get();
            return response.Models;
        }
    }
}
*/