using UnityEngine;

namespace OFIS.Company
{
    public sealed class CompanyHealthService : MonoBehaviour
    {
        [SerializeField] private int startingHealth = 100;
        [SerializeField] private int minHealth = 0;
        [SerializeField] private int maxHealth = 100;

        public int CurrentHealth { get; private set; }

        private void Awake()
        {
            CurrentHealth = Mathf.Clamp(startingHealth, minHealth, maxHealth);
            Debug.Log($"[CompanyHealth] Initialized. Health={CurrentHealth}");
        }

        public void ApplyDelta(int delta, string reason)
        {
            int previousHealth = CurrentHealth;
            CurrentHealth = Mathf.Clamp(CurrentHealth + delta, minHealth, maxHealth);

            Debug.Log(
                $"[CompanyHealth] {previousHealth} -> {CurrentHealth}. " +
                $"Delta={delta}, Reason={reason}");
        }

        public void ApplyTaskCompleted(string taskName)
        {
            ApplyDelta(+2, $"Task completed: {taskName}");
        }

        public void ApplyFaultyTaskCompleted(string taskName)
        {
            ApplyDelta(-8, $"Faulty task completed: {taskName}");
        }
    }
}