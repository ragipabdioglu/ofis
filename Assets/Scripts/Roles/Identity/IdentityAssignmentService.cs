using System;
using System.Collections.Generic;
using System.Linq;
using OFIS.Core.Ids;
using OFIS.Roles.Departments;

namespace OFIS.Roles.Identity
{
    public sealed class IdentityAssignmentService
    {
        private readonly Random _random;

        public IdentityAssignmentService(int seed = 0)
        {
            _random = seed == 0 ? new Random() : new Random(seed);
        }

        public IdentityAssignmentResult AssignIdentities(IReadOnlyList<PlayerId> playerIds)
        {
            if (playerIds == null || playerIds.Count == 0)
                return IdentityAssignmentResult.Failed("Player list is empty.");

            var pool = BuildDefaultPool();

            if (playerIds.Count > pool.Count)
                return IdentityAssignmentResult.Failed($"Not enough identity pool entries. Players={playerIds.Count}, Pool={pool.Count}");

            var shuffledPool = pool.ToList();
            Shuffle(shuffledPool);

            var identities = new List<PlayerPublicIdentity>();

            for (int i = 0; i < playerIds.Count; i++)
            {
                var entry = shuffledPool[i];

                identities.Add(new PlayerPublicIdentity(
                    playerIds[i],
                    entry.CharacterName,
                    entry.Department,
                    entry.DepartmentDisplayName,
                    entry.JobTitle));
            }

            return IdentityAssignmentResult.Completed(identities);
        }

        public PlayerPublicIdentity GetIdentityForPlayer(
            PlayerId playerId,
            IReadOnlyList<PlayerPublicIdentity> identities)
        {
            if (identities == null)
                return null;

            return identities.FirstOrDefault(x => x.PlayerId == playerId);
        }

        private static List<IdentityPoolEntry> BuildDefaultPool()
        {
            return new List<IdentityPoolEntry>
            {
                new("Ayhan Demir", DepartmentType.Accounting, "Muhasebe", "Finans Uzmanı"),
                new("Elif Kaya", DepartmentType.HumanResources, "İnsan Kaynakları", "Personel Uzmanı"),
                new("Mert Yalçın", DepartmentType.Logistics, "Lojistik", "Operasyon Sorumlusu"),
                new("Selin Aras", DepartmentType.Archive, "Arşiv", "Arşiv Sorumlusu"),
                new("Burak Şahin", DepartmentType.Server, "Sunucu", "Sistem Teknisyeni"),
                new("Deniz Aydın", DepartmentType.OfficeSupport, "Ofis Destek", "Ofis Asistanı"),
                new("Ceren Koç", DepartmentType.MeetingOperations, "Toplantı Operasyon", "Toplantı Koordinatörü"),
                new("Kerem Aksoy", DepartmentType.Accounting, "Muhasebe", "Muhasebe Uzmanı"),
                new("Zeynep Eren", DepartmentType.HumanResources, "İnsan Kaynakları", "İK Asistanı"),
                new("Emre Polat", DepartmentType.Logistics, "Lojistik", "Kargo Sorumlusu"),
                new("Derya Tunç", DepartmentType.Archive, "Arşiv", "Dosya Kontrol Uzmanı"),
                new("Onur Kaplan", DepartmentType.Server, "Sunucu", "Ağ Destek Uzmanı")
            };
        }

        private void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = _random.Next(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}