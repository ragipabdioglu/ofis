using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Corpse
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class CorpseDetector : MonoBehaviour
    {
        private readonly List<CorpsePlaceholder> _corpsesInRange = new();

        public CorpsePlaceholder CurrentCorpse { get; private set; }

        private void Reset()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void Awake()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            CorpsePlaceholder corpse = other.GetComponent<CorpsePlaceholder>();

            if (corpse == null)
                return;

            if (!_corpsesInRange.Contains(corpse))
                _corpsesInRange.Add(corpse);

            RefreshCurrentCorpse();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            CorpsePlaceholder corpse = other.GetComponent<CorpsePlaceholder>();

            if (corpse == null)
                return;

            _corpsesInRange.Remove(corpse);

            if (CurrentCorpse == corpse)
                CurrentCorpse = null;

            RefreshCurrentCorpse();
        }

        private void Update()
        {
            RefreshCurrentCorpse();
        }

        private void RefreshCurrentCorpse()
        {
            _corpsesInRange.RemoveAll(corpse => corpse == null);

            CurrentCorpse = null;

            float bestDistance = float.MaxValue;
            Vector3 origin = transform.position;

            foreach (CorpsePlaceholder corpse in _corpsesInRange)
            {
                float distance = Vector3.Distance(origin, corpse.transform.position);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    CurrentCorpse = corpse;
                }
            }
        }
    }
}