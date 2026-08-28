using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace RenderManager.System
{
    public static class DiscordWebhook
    {
        public static string WebhookUrl { get; set; } = "";
        public static bool IsWebhookEnabled { get; set; } = false;
        public static bool IsHookOnPartyJoin { get; set; } = false;
        public static bool IsHookOnPartyLeave { get; set; } = false;
        public static bool IsHookOnPartyFill { get; set; } = true;

        private static readonly HttpClient Client = new HttpClient();

        private static async void SendWebhook(string message)
        {
            if (!IsWebhookEnabled || string.IsNullOrWhiteSpace(WebhookUrl))
                return;

            var payload = new
            {
                content = message,
                username = "Party Finder Notifications",
                avatar_url = ""
            };

            try
            {
                var jsonPayload = JsonSerializer.Serialize(payload);
                using var content = new StringContent(
                    jsonPayload,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await Client.PostAsync(WebhookUrl, content);

                if (response.IsSuccessStatusCode) {
                    Service.Log.Info("Webhook sent");
                } else {
                    Service.Log.Error(
                        $"Webhook failed: {(int)response.StatusCode} {response.ReasonPhrase}"
                    );
                }
            }
            catch (Exception ex)
            {
                Service.Log.Error($"Webhook error: {ex}");
            }
        }

        public static void NotifyPartyFull()
        {
            if (!IsHookOnPartyFill)
                return;

            SendWebhook("Cross world party filled");
        }

        public static void NotifyPartyJoin(
            CrossWorldPartyListSystem.CrossWorldMember m)
        {
            if (!IsHookOnPartyJoin)
                return;

            SendWebhook(
                "Cross world party member joined: " +
                $"Name:{m.Name}, " +
                $"Job:{PartyListener.GetJobAbbr(m.JobId)}, " +
                $"Level:{m.Level}"
            );
        }

        public static void NotifyPartyLeave(
            CrossWorldPartyListSystem.CrossWorldMember m)
        {
            if (!IsHookOnPartyLeave)
                return;

            SendWebhook(
                "Cross world party member left: " +
                $"Name:{m.Name}, " +
                $"Job:{PartyListener.GetJobAbbr(m.JobId)}; " +
                "don't forget to refresh party finder slot in case it's locked"
            );
        }
    }
}

