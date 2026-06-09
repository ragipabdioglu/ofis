using UnityEngine;

namespace OFIS.PlayerControl
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    public sealed class TopDownPlayerMotor : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4.5f;

        private Rigidbody2D _rigidbody;
        private PlayerInputReader _inputReader;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _inputReader = GetComponent<PlayerInputReader>();

            _rigidbody.gravityScale = 0f;
            _rigidbody.freezeRotation = true;
            _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void FixedUpdate()
        {
            Vector2 velocity = _inputReader.MoveInput * moveSpeed;
            _rigidbody.linearVelocity = velocity;
        }
    }
}