using System.Linq;
using Lumina.Excel.Sheets;

namespace RenderManager.System
{
    public static class PartyListener
    {
        public static void On() {
            CrossWorldPartyListSystem.OnJoin += OnJoin;
            CrossWorldPartyListSystem.OnLeave += OnLeave;
        }


        public static void Off() {
            CrossWorldPartyListSystem.OnJoin -= OnJoin;
            CrossWorldPartyListSystem.OnLeave -= OnLeave;
        }

        public static string GetJobAbbr(uint jobId) {
            var jobEnum = Service.DataManager.GetExcelSheet<ClassJob>().Where(a => a.RowId == jobId);
            return jobEnum.FirstOrDefault().Abbreviation.ToString();
        }
        private static void OnJoin(CrossWorldPartyListSystem.CrossWorldMember m) {
            if (m.MemberIndex == 8 && DiscordWebhook.IsHookOnPartyFill) {
                DiscordWebhook.NotifyPartyFull();
            } else if (DiscordWebhook.IsHookOnPartyJoin) {
                DiscordWebhook.NotifyPartyJoin(m);
            }
        }


        private static void OnLeave(CrossWorldPartyListSystem.CrossWorldMember m) {
            if (DiscordWebhook.IsHookOnPartyLeave)
                DiscordWebhook.NotifyPartyLeave(m);
        }
    }
}
