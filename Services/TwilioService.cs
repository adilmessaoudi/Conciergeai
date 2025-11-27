using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ConciergeAI.Services
{
    public class TwilioService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromNumber;

        public TwilioService(IConfiguration configuration)
        {
            _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID")
                ?? throw new Exception("TWILIO_ACCOUNT_SID missing");

            _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN")
                ?? throw new Exception("TWILIO_AUTH_TOKEN missing");

            _fromNumber = Environment.GetEnvironmentVariable("TWILIO_SMS_NUMBER")
                ?? throw new Exception("TWILIO_SMS_NUMBER missing");

            TwilioClient.Init(_accountSid, _authToken);
        }

        // ✅ Méthode publique pour envoyer un SMS
        public void SendMessage(string toNumber, string message)
        {
            MessageResource.Create(
                from: new PhoneNumber(_fromNumber),
                to: new PhoneNumber(toNumber),
                body: message
            );
        }

        // ✅ Méthode pour envoyer un message test
        public async Task SendTestMessage(string to)
        {
            try
            {
                var message = await MessageResource.CreateAsync(
                    from: new PhoneNumber(_fromNumber),
                    to: new PhoneNumber(to),
                    body: "🔔 Test Twilio SMS : votre MVP fonctionne bien !"
                );

                Console.WriteLine("Message Twilio envoyé : " + message.Sid);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur Twilio : " + ex.Message);
                throw;
            }
        }
    }
}
