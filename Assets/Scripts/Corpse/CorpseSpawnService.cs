using UnityEngine;

namespace OFIS.Corpse
{
    public sealed class CorpseSpawnService : MonoBehaviour
    {
        [Header("Corpse Spawn")]
        [SerializeField] private GameObject corpsePrefab;
        [SerializeField] private Transform corpseParent;

        public CorpsePlaceholder SpawnCorpse(string victimName, Vector3 position)
        {
            GameObject corpseObject;

            if (corpsePrefab != null)
            {
                corpseObject = Instantiate(corpsePrefab, position, Quaternion.identity, corpseParent);
            }
            else
            {
                corpseObject = CreateRuntimeCorpseObject(position);
            }

            CorpsePlaceholder corpse = corpseObject.GetComponent<CorpsePlaceholder>();

            if (corpse == null)
                corpse = corpseObject.AddComponent<CorpsePlaceholder>();

            corpse.Initialize(victimName);

            Debug.Log($"[CorpseSpawnService] Spawned corpse for {victimName} at {position}");

            return corpse;
        }

        private GameObject CreateRuntimeCorpseObject(Vector3 position)
        {
            GameObject corpseObject = new GameObject("Corpse_Runtime");
            corpseObject.transform.position = position;

            SpriteRenderer spriteRenderer = corpseObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSquareSprite();
            spriteRenderer.sortingOrder = 45;
            corpseObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

            BoxCollider2D collider = corpseObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.7f, 0.7f);

            corpseObject.AddComponent<CorpsePlaceholder>();

            if (corpseParent != null)
                corpseObject.transform.SetParent(corpseParent);

            return corpseObject;
        }

        private static Sprite CreateSquareSprite()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            return Sprite.Create(
                texture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f),
                1f);
        }
    }
}