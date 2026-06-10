using UnityEngine;

namespace OFIS.Interactions
{
    public sealed class SceneInteractionPlacementDebugBuilder : MonoBehaviour
    {
        [SerializeField] private bool buildOnStart = false;
        [SerializeField] private bool clearExistingBeforeBuild = true;
        [SerializeField] private float interactionColliderRadius = 0.35f;

        [ContextMenu("Build Debug Scene Interactions")]
        public void BuildInteractions()
        {
            if (clearExistingBeforeBuild)
                ClearChildren();

            CreateInteraction("Interaction_Task_Accounting", WorldInteractionType.Task, "Review invoices", new Vector2(-7.1f, -4.8f));
            CreateInteraction("Interaction_Task_Server", WorldInteractionType.Task, "Check server logs", new Vector2(4.5f, 4.8f));
            CreateInteraction("Interaction_MeetingJoin", WorldInteractionType.MeetingJoin, "Join meeting", new Vector2(0f, 4.8f));
            CreateInteraction("Interaction_SabotageRepair_Server", WorldInteractionType.SabotageRepair, "Repair server sabotage", new Vector2(4.8f, 4.6f));
            CreateInteraction("Interaction_Corpse_Debug", WorldInteractionType.CorpseInspectOrCarry, "Inspect corpse", new Vector2(-6f, 7.9f));
            CreateInteraction("Interaction_VictimNote_Debug", WorldInteractionType.VictimNote, "Read note", new Vector2(-3.9f, 4.8f));
            CreateInteraction("Interaction_DoorPanel_Debug", WorldInteractionType.DoorPanel, "Use door panel", new Vector2(6.8f, 7.9f));

            Debug.Log("[SceneInteractionPlacement] Debug scene interactions built.");
        }

        private void Start()
        {
            if (buildOnStart)
                BuildInteractions();
        }

        private void CreateInteraction(string objectName, WorldInteractionType type, string displayName, Vector2 position)
        {
            GameObject interaction = new GameObject(objectName);
            interaction.transform.SetParent(transform);
            interaction.transform.position = new Vector3(position.x, position.y, 0f);

            CircleCollider2D collider = interaction.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = interactionColliderRadius;

            WorldInteractionCandidateProvider provider = interaction.AddComponent<WorldInteractionCandidateProvider>();
            provider.Configure(type, displayName, true);
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
