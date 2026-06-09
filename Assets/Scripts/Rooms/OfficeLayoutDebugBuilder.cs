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
        [SerializeField] private Color officeSupportColor = new(0.20f, 0.20f, 0.20f, 1f);

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

            Debug.Log("[OfficeLayout] Debug office layout with floors and door gaps built.");
        }

        private void Start()
        {
            if (buildOnStart)
                BuildLayout();
        }

        private void BuildFloors(Transform parent)
        {
            CreateFloor(parent, "Floor_Hallway", new Vector2(0f, 0f), new Vector2(15.5f, 3.1f), hallwayColor);

            CreateFloor(parent, "Floor_ArchiveRoom", new Vector2(-5.9f, 4.3f), new Vector2(4.0f, 5.1f), archiveRoomColor);
            CreateFloor(parent, "Floor_MeetingRoom", new Vector2(0f, 4.3f), new Vector2(7.5f, 5.1f), meetingRoomColor);
            CreateFloor(parent, "Floor_ServerRoom", new Vector2(5.9f, 4.3f), new Vector2(4.0f, 5.1f), serverRoomColor);

            CreateFloor(parent, "Floor_Accounting", new Vector2(-5.9f, -4.3f), new Vector2(4.0f, 5.1f), accountingColor);
            CreateFloor(parent, "Floor_OfficeSupport", new Vector2(0f, -4.3f), new Vector2(7.5f, 5.1f), officeSupportColor);
            CreateFloor(parent, "Floor_HumanResources", new Vector2(5.9f, -4.3f), new Vector2(4.0f, 5.1f), hrColor);
        }

        private void BuildWalls(Transform parent)
        {
            // Outer bounds
            CreateWall(parent, "Wall_Top", new Vector2(0f, 7f), new Vector2(16f, 0.35f));
            CreateWall(parent, "Wall_Bottom", new Vector2(0f, -7f), new Vector2(16f, 0.35f));
            CreateWall(parent, "Wall_Left", new Vector2(-8f, 0f), new Vector2(0.35f, 14f));
            CreateWall(parent, "Wall_Right", new Vector2(8f, 0f), new Vector2(0.35f, 14f));

            // Top room bottom walls with door gaps to hallway
            CreateWall(parent, "Wall_Archive_Bottom_Left", new Vector2(-6.65f, 1.6f), new Vector2(2.7f, 0.25f));
            CreateWall(parent, "Wall_Archive_Bottom_Right", new Vector2(-3.95f, 1.6f), new Vector2(1.2f, 0.25f));

            CreateWall(parent, "Wall_Meeting_Bottom_Left", new Vector2(-1.85f, 1.6f), new Vector2(2.2f, 0.25f));
            CreateWall(parent, "Wall_Meeting_Bottom_Right", new Vector2(1.85f, 1.6f), new Vector2(2.2f, 0.25f));

            CreateWall(parent, "Wall_Server_Bottom_Left", new Vector2(3.95f, 1.6f), new Vector2(1.2f, 0.25f));
            CreateWall(parent, "Wall_Server_Bottom_Right", new Vector2(6.65f, 1.6f), new Vector2(2.7f, 0.25f));

            // Bottom room top walls with door gaps to hallway
            CreateWall(parent, "Wall_Accounting_Top_Left", new Vector2(-6.65f, -1.6f), new Vector2(2.7f, 0.25f));
            CreateWall(parent, "Wall_Accounting_Top_Right", new Vector2(-3.95f, -1.6f), new Vector2(1.2f, 0.25f));

            CreateWall(parent, "Wall_HR_Top_Left", new Vector2(3.95f, -1.6f), new Vector2(1.2f, 0.25f));
            CreateWall(parent, "Wall_HR_Top_Right", new Vector2(6.65f, -1.6f), new Vector2(2.7f, 0.25f));

            // Vertical separators between top rooms
            CreateWall(parent, "Wall_Archive_Meeting_Separator", new Vector2(-3.8f, 4.3f), new Vector2(0.25f, 5.1f));
            CreateWall(parent, "Wall_Meeting_Server_Separator", new Vector2(3.8f, 4.3f), new Vector2(0.25f, 5.1f));

            // Vertical separators between bottom rooms and lower middle area
            CreateWall(parent, "Wall_Accounting_Left_Separator", new Vector2(-3.8f, -4.3f), new Vector2(0.25f, 5.1f));
            CreateWall(parent, "Wall_HR_Right_Separator", new Vector2(3.8f, -4.3f), new Vector2(0.25f, 5.1f));

            // Lower middle boundary, with door-like side access
            CreateWall(parent, "Wall_LowerMiddle_LeftStub", new Vector2(-1.25f, -1.6f), new Vector2(2.5f, 0.25f));
            CreateWall(parent, "Wall_LowerMiddle_RightStub", new Vector2(1.25f, -1.6f), new Vector2(2.5f, 0.25f));
        }

        private void CreateFloor(
            Transform parent,
            string objectName,
            Vector2 position,
            Vector2 size,
            Color color)
        {
            var floor = new GameObject(objectName);
            floor.transform.SetParent(parent);
            floor.transform.position = new Vector3(position.x, position.y, 0.1f);
            floor.transform.localScale = new Vector3(size.x, size.y, 1f);

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