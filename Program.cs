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
var twilioAccountSid = builder.Configuration["TWILIO_ACCOUNT_SID"];
var twilioAuthToken = builder.Configuration["TWILIO_AUTH_TOKEN"];
var twilioSmsFrom = builder.Configuration["TWILIO_SMS_NUMBER"]; // <-- SMS au lieu de WhatsApp

var openAiApiKey = builder.Configuration["OPENAI_API_KEY"];

var supabaseUrl = builder.Configuration["SUPABASE_URL"];
var supabaseKey = builder.Configuration["SUPABASE_API_KEY"];

// ---------------------------
// 3️⃣ Ajouter les services
// ---------------------------
builder.Services.AddControllers();

// Ajouter SupabaseService, OpenAiService, TwilioService
builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddHttpClient<OpenAiService>();

// TwilioService est maintenant pour SMS
builder.Services.AddSingleton<TwilioService>();

// Swagger pour tests (optionnel)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

Console.WriteLine("---- CHECK ENV ----");
Console.WriteLine("Twilio SID : " + twilioAccountSid);
Console.WriteLine("Twilio From (SMS) : " + (string.IsNullOrEmpty(twilioSmsFrom) ? "❌ MISSING" : twilioSmsFrom));
Console.WriteLine("OpenAI Key : " + (string.IsNullOrEmpty(openAiApiKey) ? "❌ MISSING" : "OK"));
Console.WriteLine("Supabase URL : " + supabaseUrl);
Console.WriteLine("-------------------");

// ---------------------------
// 4️⃣ Middleware
// ---------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// ---------------------------
// 5️⃣ Optionnel : tests rapides au démarrage
// ---------------------------
using (var scope = app.Services.CreateScope())
{
    var supabase = scope.ServiceProvider.GetRequiredService<SupabaseService>();
    var twilio = scope.ServiceProvider.GetRequiredService<TwilioService>();
    var openai = scope.ServiceProvider.GetRequiredService<OpenAiService>();

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

    Console.WriteLine("------ TEST TWILIO SMS ------");
    try
    {
        // Envoie un SMS test à ton numéro
        await twilio.SendTestMessage("+33XXXXXXXXX"); 
        Console.WriteLine("[OK] Twilio SMS fonctionne !");
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
}

app.Run();
