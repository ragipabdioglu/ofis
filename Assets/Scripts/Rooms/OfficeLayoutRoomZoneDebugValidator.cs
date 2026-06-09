using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Rooms
{
    public sealed class OfficeLayoutRoomZoneDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateExpandedRoomZones();
        }

        [ContextMenu("Validate Expanded Room Zones")]
        public void ValidateExpandedRoomZones()
        {
            GameObject layoutRoot = new GameObject("OfficeLayoutRoomZone_TestRoot");
            OfficeLayoutDebugBuilder builder = layoutRoot.AddComponent<OfficeLayoutDebugBuilder>();
            builder.BuildLayout();

            OfficeRoomZone[] zones = layoutRoot.GetComponentsInChildren<OfficeRoomZone>();
            HashSet<OfficeRoomType> foundTypes = new HashSet<OfficeRoomType>();

            foreach (OfficeRoomZone zone in zones)
            {
                if (zone == null)
                    continue;

                foundTypes.Add(zone.RoomType);
            }

            ValidateRoomType(foundTypes, OfficeRoomType.Hallway);
            ValidateRoomType(foundTypes, OfficeRoomType.Reception);
            ValidateRoomType(foundTypes, OfficeRoomType.ManagerOffice);
            ValidateRoomType(foundTypes, OfficeRoomType.MeetingRoom);
            ValidateRoomType(foundTypes, OfficeRoomType.ServerRoom);
            ValidateRoomType(foundTypes, OfficeRoomType.SecurityRoom);
            ValidateRoomType(foundTypes, OfficeRoomType.Accounting);
            ValidateRoomType(foundTypes, OfficeRoomType.HumanResources);
            ValidateRoomType(foundTypes, OfficeRoomType.OfficeSupport);
            ValidateRoomType(foundTypes, OfficeRoomType.Logistics);
            ValidateRoomType(foundTypes, OfficeRoomType.StorageRoom);
            ValidateRoomType(foundTypes, OfficeRoomType.ArchiveRoom);
            ValidateRoomType(foundTypes, OfficeRoomType.BreakRoom);
            ValidateRoomType(foundTypes, OfficeRoomType.PrintRoom);
            ValidateRoomType(foundTypes, OfficeRoomType.Kitchen);

            bool countPassed = zones.Length >= 15;

            if (countPassed)
                Debug.Log($"[OfficeLayoutRoomZoneValidator] PASS RoomZoneCount: Count={zones.Length}");
            else
                Debug.LogError($"[OfficeLayoutRoomZoneValidator] FAIL RoomZoneCount: Count={zones.Length}");
        }

        private static void ValidateRoomType(HashSet<OfficeRoomType> foundTypes, OfficeRoomType expectedType)
        {
            if (foundTypes.Contains(expectedType))
                Debug.Log($"[OfficeLayoutRoomZoneValidator] PASS RoomType_{expectedType}");
            else
                Debug.LogError($"[OfficeLayoutRoomZoneValidator] FAIL RoomType_{expectedType}");
        }
    }
}
