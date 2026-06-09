using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OFIS.PlayerControl
{
    public sealed class PlayerInputReader : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }

        private void Update()
        {
            MoveInput = ReadMoveInput();
        }

        private static Vector2 ReadMoveInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null)
                return Vector2.zero;

            float x = 0f;
            float y = 0f;

            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                x -= 1f;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                x += 1f;

            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                y -= 1f;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                y += 1f;

            var input = new Vector2(x, y);

            if (input.sqrMagnitude > 1f)
                input.Normalize();

            return input;
#else
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            var input = new Vector2(x, y);

            if (input.sqrMagnitude > 1f)
                input.Normalize();

            return input;
#endif
        }
    }
}