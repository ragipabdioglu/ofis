using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Kill
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class PlayerKillTargetDetector : MonoBehaviour
    {
        private readonly List<KillTargetDummy> _targetsInRange = new();

        public KillTargetDummy CurrentTarget { get; private set; }

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
            KillTargetDummy target = other.GetComponent<KillTargetDummy>();

            if (target == null)
                return;

            if (!_targetsInRange.Contains(target))
                _targetsInRange.Add(target);

            RefreshCurrentTarget();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            KillTargetDummy target = other.GetComponent<KillTargetDummy>();

            if (target == null)
                return;

            _targetsInRange.Remove(target);

            if (CurrentTarget == target)
                CurrentTarget = null;

            RefreshCurrentTarget();
        }

        private void Update()
        {
            RefreshCurrentTarget();
        }

        private void RefreshCurrentTarget()
        {
            _targetsInRange.RemoveAll(target => target == null);

            CurrentTarget = null;

            float bestDistance = float.MaxValue;
            Vector3 origin = transform.position;

            foreach (KillTargetDummy target in _targetsInRange)
            {
                float distance = Vector3.Distance(origin, target.transform.position);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    CurrentTarget = target;
                }
            }
        }
    }
}