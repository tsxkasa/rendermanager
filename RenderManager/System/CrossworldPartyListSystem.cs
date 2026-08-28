using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Info;

namespace RenderManager.System
{
    public static class CrossWorldPartyListSystem
    {
        public delegate void CrossWorldJoinDelegate(CrossWorldMember m);

        public delegate void CrossWorldLeaveDelegate(CrossWorldMember m);

        private static readonly List<CrossWorldMember> Members = new();
        private static List<CrossWorldMember> OldMembers = new();
        public static event CrossWorldJoinDelegate? OnJoin;
        public static event CrossWorldLeaveDelegate? OnLeave;

        public static void Start() {
            Service.Framework.Update += Update;
        }

        public static void Stop() {
            Service.Framework.Update -= Update;
        }

        private static unsafe void Update(IFramework fw) {
            if (!Service.ClientState.IsLoggedIn) {
                return;
            }

            if (!InfoProxyCrossRealm.IsCrossRealmParty()) {
                return;
            }

            Members.Clear();
            var memberCount = InfoProxyCrossRealm.GetPartyMemberCount();

            for (uint i = 0; i < memberCount; i++){
                var mem = InfoProxyCrossRealm.GetGroupMember(i);
                var mObj = new CrossWorldMember {
                    Name = mem->NameString,
                    MemberIndex = memberCount,
                    Level = mem->Level,
                    JobId = mem->ClassJobId
                };

                Members.Add(mObj);
            }

            if (Members.Count != OldMembers.Count) {
                foreach (var i in Members) {
                    if (!OldMembers.Any(a => a.Name == i.Name)) {
                        OnJoin?.Invoke(i);
                    }
                }

                foreach (var i in Members) {
                    if (!Members.Any(a => a.Name == i.Name)) {
                        OnLeave?.Invoke(i);
                    }
                }
            }

            OldMembers = Members.ToList();
        }

        public struct CrossWorldMember
        {
            public string Name;

            public uint MemberIndex;
            public uint Level;
            public uint JobId;
        }
    }
}
