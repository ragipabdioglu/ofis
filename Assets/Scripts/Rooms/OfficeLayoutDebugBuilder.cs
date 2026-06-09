using UnityEngine;

namespace OFIS.Rooms
{
    public sealed class OfficeLayoutDebugBuilder : MonoBehaviour
    {
        [SerializeField] private bool buildOnStart = false;
        [SerializeField] private bool clearExistingBeforeBuild = true;

        [Header("Wall Visual")]
        [SerializeField] private Color wallColor = new(0.18f, 0.18f, 0.18f, 1f);
        [SerializeField] private int wallSortingOrder = 20;

        [Header("Floor Visual")]
        [SerializeField] private int floorSortingOrder = -10;

        [SerializeField] private Color hallwayColor = new(0.18f, 0.22f, 0.25f, 1f);
        [SerializeField] private Color meetingRoomColor = new(0.22f, 0.18f, 0.32f, 1f);
        [SerializeField] private Color archiveRoomColor = new(0.24f, 0.20f, 0.14f, 1f);
        [SerializeField] private Color serverRoomColor = new(0.12f, 0.20f, 0.30f, 1f);
        [SerializeField] private Color accountingColor = new(0.14f, 0.26f, 0.18f, 1f);
        [SerializeField] private Color hrColor = new(0.28f, 0.18f, 0.20f, 1f);
        [SerializeField] private Color logisticsColor = new(0.18f, 0.24f, 0.16f, 1f);
        [SerializeField] private Color officeSupportColor = new(0.20f, 0.20f, 0.20f, 1f);
        [SerializeField] private Color managerOfficeColor = new(0.24f, 0.18f, 0.28f, 1f);
        [SerializeField] private Color securityRoomColor = new(0.16f, 0.16f, 0.28f, 1f);
        [SerializeField] private Color breakRoomColor = new(0.28f, 0.24f, 0.16f, 1f);
        [SerializeField] private Color storageRoomColor = new(0.22f, 0.22f, 0.16f, 1f);
        [SerializeField] private Color printRoomColor = new(0.20f, 0.24f, 0.24f, 1f);
        [SerializeField] private Color kitchenColor = new(0.26f, 0.20f, 0.16f, 1f);
        [SerializeField] private Color receptionColor = new(0.18f, 0.26f, 0.26f, 1f);

        [ContextMenu("Build Debug Office Layout")]
        public void BuildLayout()
        {
            if (clearExistingBeforeBuild)
                ClearChildren();

            var floorsRoot = new GameObject("Floors");
            floorsRoot.transform.SetParent(transform);
            floorsRoot.transform.localPosition = Vector3.zero;

            var wallsRoot = new GameObject("Walls");
            wallsRoot.transform.SetParent(transform);
            wallsRoot.transform.localPosition = Vector3.zero;

            BuildFloors(floorsRoot.transform);
            BuildWalls(wallsRoot.transform);

            Debug.Log("[OfficeLayout] Expanded debug office layout with floors, room zones and wall colliders built.");
        }

        private void Start()
        {
            if (buildOnStart)
                BuildLayout();
        }

        private void BuildFloors(Transform parent)
        {
            CreateRoomFloor(parent, OfficeRoomType.Hallway, "Floor_Hallway", "Hallway", new Vector2(0f, 0f), new Vector2(17.5f, 2.6f), hallwayColor);

            CreateRoomFloor(parent, OfficeRoomType.Reception, "Floor_Reception", "Reception", new Vector2(-7.1f, 4.8f), new Vector2(3.0f, 3.2f), receptionColor);
            CreateRoomFloor(parent, OfficeRoomType.ManagerOffice, "Floor_ManagerOffice", "Manager Office", new Vector2(-3.9f, 4.8f), new Vector2(3.0f, 3.2f), managerOfficeColor);
            CreateRoomFloor(parent, OfficeRoomType.MeetingRoom, "Floor_MeetingRoom", "Meeting Room", new Vector2(0f, 4.8f), new Vector2(4.4f, 3.2f), meetingRoomColor);
            CreateRoomFloor(parent, OfficeRoomType.ServerRoom, "Floor_ServerRoom", "Server Room", new Vector2(4.5f, 4.8f), new Vector2(3.0f, 3.2f), serverRoomColor);
            CreateRoomFloor(parent, OfficeRoomType.SecurityRoom, "Floor_SecurityRoom", "Security Room", new Vector2(7.3f, 4.8f), new Vector2(2.4f, 3.2f), securityRoomColor);

            CreateRoomFloor(parent, OfficeRoomType.Accounting, "Floor_Accounting", "Accounting", new Vector2(-7.1f, -4.8f), new Vector2(3.0f, 3.2f), accountingColor);
            CreateRoomFloor(parent, OfficeRoomType.HumanResources, "Floor_HumanResources", "Human Resources", new Vector2(-3.9f, -4.8f), new Vector2(3.0f, 3.2f), hrColor);
            CreateRoomFloor(parent, OfficeRoomType.OfficeSupport, "Floor_OfficeSupport", "Office Support", new Vector2(0f, -4.8f), new Vector2(4.4f, 3.2f), officeSupportColor);
            CreateRoomFloor(parent, OfficeRoomType.Logistics, "Floor_Logistics", "Logistics", new Vector2(4.5f, -4.8f), new Vector2(3.0f, 3.2f), logisticsColor);
            CreateRoomFloor(parent, OfficeRoomType.StorageRoom, "Floor_StorageRoom", "Storage Room", new Vector2(7.3f, -4.8f), new Vector2(2.4f, 3.2f), storageRoomColor);

            CreateRoomFloor(parent, OfficeRoomType.ArchiveRoom, "Floor_ArchiveRoom", "Archive Room", new Vector2(-6.0f, 7.9f), new Vector2(4.2f, 2.2f), archiveRoomColor);
            CreateRoomFloor(parent, OfficeRoomType.BreakRoom, "Floor_BreakRoom", "Break Room", new Vector2(0f, 7.9f), new Vector2(4.2f, 2.2f), breakRoomColor);
            CreateRoomFloor(parent, OfficeRoomType.PrintRoom, "Floor_PrintRoom", "Print Room", new Vector2(3.8f, 7.9f), new Vector2(2.6f, 2.2f), printRoomColor);
            CreateRoomFloor(parent, OfficeRoomType.Kitchen, "Floor_Kitchen", "Kitchen", new Vector2(6.8f, 7.9f), new Vector2(2.8f, 2.2f), kitchenColor);
        }

        private void BuildWalls(Transform parent)
        {
            CreateWall(parent, "Wall_Top", new Vector2(0f, 9.2f), new Vector2(18f, 0.35f));
            CreateWall(parent, "Wall_Bottom", new Vector2(0f, -6.7f), new Vector2(18f, 0.35f));
            CreateWall(parent, "Wall_Left", new Vector2(-9f, 1.25f), new Vector2(0.35f, 15.9f));
            CreateWall(parent, "Wall_Right", new Vector2(9f, 1.25f), new Vector2(0.35f, 15.9f));

            CreateWall(parent, "Wall_TopRooms_Bottom_Left", new Vector2(-6.3f, 3.0f), new Vector2(4.0f, 0.25f));
            CreateWall(parent, "Wall_TopRooms_Bottom_Mid", new Vector2(0f, 3.0f), new Vector2(3.2f, 0.25f));
            CreateWall(parent, "Wall_TopRooms_Bottom_Right", new Vector2(6.1f, 3.0f), new Vector2(4.6f, 0.25f));

            CreateWall(parent, "Wall_BottomRooms_Top_Left", new Vector2(-6.3f, -3.0f), new Vector2(4.0f, 0.25f));
            CreateWall(parent, "Wall_BottomRooms_Top_Mid", new Vector2(0f, -3.0f), new Vector2(3.2f, 0.25f));
            CreateWall(parent, "Wall_BottomRooms_Top_Right", new Vector2(6.1f, -3.0f), new Vector2(4.6f, 0.25f));

            CreateWall(parent, "Wall_Reception_Manager_Separator", new Vector2(-5.55f, 4.8f), new Vector2(0.25f, 3.2f));
            CreateWall(parent, "Wall_Manager_Meeting_Separator", new Vector2(-2.2f, 4.8f), new Vector2(0.25f, 3.2f));
            CreateWall(parent, "Wall_Meeting_Server_Separator", new Vector2(2.2f, 4.8f), new Vector2(0.25f, 3.2f));
            CreateWall(parent, "Wall_Server_Security_Separator", new Vector2(5.95f, 4.8f), new Vector2(0.25f, 3.2f));

            CreateWall(parent, "Wall_Accounting_HR_Separator", new Vector2(-5.55f, -4.8f), new Vector2(0.25f, 3.2f));
            CreateWall(parent, "Wall_HR_Support_Separator", new Vector2(-2.2f, -4.8f), new Vector2(0.25f, 3.2f));
            CreateWall(parent, "Wall_Support_Logistics_Separator", new Vector2(2.2f, -4.8f), new Vector2(0.25f, 3.2f));
            CreateWall(parent, "Wall_Logistics_Storage_Separator", new Vector2(5.95f, -4.8f), new Vector2(0.25f, 3.2f));

            CreateWall(parent, "Wall_UpperUtility_Bottom_Left", new Vector2(-6f, 6.65f), new Vector2(4.2f, 0.25f));
            CreateWall(parent, "Wall_UpperUtility_Bottom_Mid", new Vector2(0f, 6.65f), new Vector2(4.2f, 0.25f));
            CreateWall(parent, "Wall_UpperUtility_Bottom_Right", new Vector2(5.3f, 6.65f), new Vector2(5.4f, 0.25f));

            CreateWall(parent, "Wall_Archive_Break_Separator", new Vector2(-2.9f, 7.9f), new Vector2(0.25f, 2.2f));
            CreateWall(parent, "Wall_Break_Print_Separator", new Vector2(2.05f, 7.9f), new Vector2(0.25f, 2.2f));
            CreateWall(parent, "Wall_Print_Kitchen_Separator", new Vector2(5.3f, 7.9f), new Vector2(0.25f, 2.2f));
        }

        private void CreateRoomFloor(
            Transform parent,
            OfficeRoomType roomType,
            string objectName,
            string displayName,
            Vector2 position,
            Vector2 size,
            Color color)
        {
            var floor = new GameObject(objectName);
            floor.transform.SetParent(parent);
            floor.transform.position = new Vector3(position.x, position.y, 0.1f);
            floor.transform.localScale = new Vector3(size.x, size.y, 1f);

            var collider = floor.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = Vector2.one;

            var zone = floor.AddComponent<OfficeRoomZone>();
            zone.ConfigureForDebug(roomType, displayName);

            var visual = floor.AddComponent<SpriteRenderer>();
            visual.sprite = CreateRuntimeSquareSprite();
            visual.color = color;
            visual.sortingOrder = floorSortingOrder;
        }

        private void CreateWall(
            Transform parent,
            string objectName,
            Vector2 position,
            Vector2 size)
        {
            var wall = new GameObject(objectName);
            wall.transform.SetParent(parent);
            wall.transform.position = new Vector3(position.x, position.y, 0f);
            wall.transform.localScale = new Vector3(size.x, size.y, 1f);

            var collider = wall.AddComponent<BoxCollider2D>();
            collider.isTrigger = false;
            collider.size = Vector2.one;

            wall.AddComponent<OfficeWall>();

            var visual = wall.AddComponent<SpriteRenderer>();
            visual.sprite = CreateRuntimeSquareSprite();
            visual.color = wallColor;
            visual.sortingOrder = wallSortingOrder;
        }

        private Sprite CreateRuntimeSquareSprite()
        {
            var texture = Texture2D.whiteTexture;

            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                1f);
        }

        private void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(transform.GetChild(i).gameObject);
                else
                    Destroy(transform.GetChild(i).gameObject);
#else
                Destroy(transform.GetChild(i).gameObject);
#endif
            }
        }
    }
}
