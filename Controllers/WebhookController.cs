using Microsoft.AspNetCore.Mvc;
using ConciergeAI.Services;
using System;
using System.Threading.Tasks;

namespace ConciergeAI.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly SupabaseService _supabase;
        private readonly OpenAiService _openAi;
        private readonly TwilioService _twilio;

        public WebhookController(
            SupabaseService supabase, 
            OpenAiService openAi,
            TwilioService twilio)
        {
            _supabase = supabase;
            _openAi = openAi;
            _twilio = twilio;
        }

        [HttpPost]
        public async Task<IActionResult> Receive(
            [FromForm] string From, 
            [FromForm] string Body)
        {
            try
            {
                Console.WriteLine($"[Webhook] Message reçu de {From} : {Body}");

                // 1️⃣ Retrouver le guest dans Supabase
                var guest = await _supabase.GetClientByWhatsApp(From);

                if (guest == null)
                {
                    Console.WriteLine("[Webhook] Aucun client trouvé pour ce numéro.");
                    _twilio.SendMessage(From, 
                        "Désolé, je ne trouve pas votre réservation avec ce numéro.");
                    return Ok();
                }

                // 2️⃣ Charger les infos du logement
var logement = await _supabase.GetLogementAsync(guest.Id);
                if (logement == null)
                {
                    _twilio.SendMessage(From, 
                        "Erreur interne : logement introuvable.");
                    return Ok();
                }

                // 3️⃣ Sauvegarder le message entrant
                await _supabase.InsertMessageAsync(new Message
                {
                    LogementId = logement.Id,
                    Type = "incoming",
                    Contenu = Body,
                    Timestamp = DateTime.UtcNow
                });

                // 4️⃣ Construire le prompt pour OpenAI
                string prompt = $@"
Tu es un assistant de conciergerie Airbnb.
Voici les infos du logement :

Nom : {logement.Nom}
Adresse : {logement.Adresse}
Check-in : {logement.CheckIn}
Check-out : {logement.CheckOut}
Règles : {logement.Instructions}
Wifi: {logement.Wifi}

Le voyageur dit : '{Body}'

Réponds de façon professionnelle, utile et sympathique.
";

                // 5️⃣ Générer une réponse avec OpenAI
                string aiResponse = await _openAi.GenerateResponse(prompt);

                // 6️⃣ Envoyer la réponse via Twilio
                _twilio.SendMessage(From, aiResponse);

                // 7️⃣ Sauvegarder la réponse sortante
                await _supabase.InsertMessageAsync(new Message
                {
                    LogementId = logement.Id,
                    Type = "outgoing",
                    Contenu = aiResponse,
                    Timestamp = DateTime.UtcNow
                });

                return Ok();
            }
            catch (Exception ex)
            {
        Console.WriteLine($"[Webhook] ERREUR : {ex.StackTrace} {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}


/*using Microsoft.AspNetCore.Mvc;
using ConciergeAI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

using System;
using System.Threading.Tasks;

namespace ConciergeAI.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly SupabaseService _supabase;
        private readonly OpenAiService _openAi;
        private readonly TwilioService _twilio;

        public WebhookController(SupabaseService supabase, OpenAiService openAi, TwilioService twilio)
        {
            _supabase = supabase;
            _openAi = openAi;
            _twilio = twilio;
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromForm] string From, [FromForm] string Body)
        {
            try
            {
                // 1️⃣ Sauvegarder le message entrant dans Supabase
                var msg = new Message
                {
                    LogementId = 1, // à adapter selon ton workflow réel
                    Type = "incoming",
                    Contenu = Body,
                    Timestamp = DateTime.UtcNow
                };
                await _supabase.InsertAsync(msg);

                // 2️⃣ Générer la réponse via OpenAI
                var responseText = await _openAi.GenerateResponse(Body);

                // 3️⃣ Envoyer la réponse via Twilio
                _twilio.SendMessage(From, responseText);

                // 4️⃣ Sauvegarder la réponse sortante dans Supabase
                var responseMsg = new Message
                {
                    LogementId = 1,
                    Type = "outgoing",
                    Contenu = responseText,
                    Timestamp = DateTime.UtcNow
                };
                await _supabase.InsertAsync(responseMsg);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}

*/