using ConciergeAI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------
// 1️⃣ Charger les variables d'environnement (Azure App Service)
// ---------------------------------------------------
builder.Configuration.AddEnvironmentVariables();

// ---------------------------------------------------
// 2️⃣ Récupération des valeurs
// ---------------------------------------------------
var twilioAccountSid   = builder.Configuration["TWILIO_ACCOUNT_SID"];
var twilioAuthToken    = builder.Configuration["TWILIO_AUTH_TOKEN"];
var twilioWhatsappFrom = builder.Configuration["TWILIO_WHATSAPP_NUMBER"];

var openAiApiKey = builder.Configuration["OPENAI_API_KEY"];

var supabaseUrl = builder.Configuration["SUPABASE_URL"];
var supabaseKey = builder.Configuration["SUPABASE_API_KEY"];
// ---------------------------
// 1️⃣ Ajouter les services
// ---------------------------
builder.Services.AddControllers();

// Ajouter SupabaseService, OpenAiService, TwilioService
builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddHttpClient<OpenAiService>();
builder.Services.AddSingleton<TwilioService>();


// Swagger pour tests (optionnel)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
Console.WriteLine("---- CHECK ENV ----");
Console.WriteLine("Twilio SID : " + twilioAccountSid);
Console.WriteLine("OpenAI Key : " + (string.IsNullOrEmpty(openAiApiKey) ? "❌ MISSING" : "OK"));
Console.WriteLine("Supabase URL : " + supabaseUrl);
Console.WriteLine("-------------------");

// ---------------------------
// 2️⃣ Middleware
// ---------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/*
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using ConciergeAI.Services;

var builder = Host.CreateApplicationBuilder(args);

// Charger configuration (appsettings.json)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Déclarer les services
builder.Services.AddSingleton<SupabaseService>();

builder.Services.AddSingleton<TwilioService>(sp => 
    new TwilioService(
        sp.GetRequiredService<IConfiguration>()["Twilio:AccountSID"],
        sp.GetRequiredService<IConfiguration>()["Twilio:AuthToken"],
        sp.GetRequiredService<IConfiguration>()["Twilio:WhatsappNumber"]
    )
);

builder.Services.AddHttpClient<OpenAiService>();

var app = builder.Build();

// Récupérer les services
var supabase = app.Services.GetRequiredService<SupabaseService>();
var twilio = app.Services.GetRequiredService<TwilioService>();
var openai = app.Services.GetRequiredService<OpenAiService>();

Console.WriteLine("------ TEST SUPABASE ------");
try
{
    await supabase.TestConnection();
    Console.WriteLine("[OK] Supabase fonctionne !");
}
catch (Exception ex)
{
    Console.WriteLine("[ERREUR SUPABASE] " + ex.Message);
}

Console.WriteLine("------ TEST TWILIO ------");
try
{
    await twilio.SendTestMessage("whatsapp:+33624880420");
    Console.WriteLine("[OK] Twilio fonctionne !");
}
catch (Exception ex)
{
    Console.WriteLine("[ERREUR TWILIO] " + ex.Message);
}


Console.WriteLine("------ TEST OPENAI ------");
try
{
    var result = await openai.GenerateResponse("donne moi les resto a fez");
    Console.WriteLine("[OK] OpenAI fonctionne ! Réponse : " + result);
}
catch (Exception ex)
{
    Console.WriteLine("[ERREUR OPENAI] " + ex.Message);
}




Console.WriteLine("----- FIN DES TESTS -----");
await Task.Delay(100); // Pour laisser le temps aux logs d'apparaître
*/