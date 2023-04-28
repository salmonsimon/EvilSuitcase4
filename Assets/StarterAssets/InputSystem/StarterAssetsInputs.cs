using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
        public bool shoot;
		public bool reload;
		public bool pause;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("Keyboard/Mouse Only Input Values")]
		public int weaponShortcut = -1;
        public Vector2 scrollWheel;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

        public void OnShoot(InputValue value)
        {
			if (aim)
				ShootInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}

		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}

        public void OnPause(InputValue value)
        {
			PauseInput(value.isPressed);
        }

		public void OnShortcut1(InputValue value)
		{
			ShortcutInput(value.isPressed, 0);
		}

        public void OnShortcut2(InputValue value)
        {
            ShortcutInput(value.isPressed, 1);
        }

        public void OnShortcut3(InputValue value)
        {
            ShortcutInput(value.isPressed, 2);
        }

        public void OnShortcut4(InputValue value)
        {
            ShortcutInput(value.isPressed, 3);
        }

        public void OnShortcut5(InputValue value)
        {
            ShortcutInput(value.isPressed, 4);
        }

        public void OnShortcut6(InputValue value)
        {
            ShortcutInput(value.isPressed, 5);
        }

        public void OnShortcut7(InputValue value)
        {
            ShortcutInput(value.isPressed, 6);
        }

        public void OnShortcut8(InputValue value)
        {
            ShortcutInput(value.isPressed, 7);
        }

		public void OnScrollWheel(InputValue value)
		{
            ScrollWheelInput(value.Get<Vector2>());
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

        public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }

        public void ReloadInput(bool newReloadState)
        {
            reload = newReloadState;
        }

        public void PauseInput(bool newPauseState)
        {
            pause = newPauseState;
        }

		public void ShortcutInput(bool newShortcutInput, int shortcutIndex)
		{
			if (newShortcutInput && weaponShortcut != shortcutIndex)
				weaponShortcut = shortcutIndex;
		}

        public void ScrollWheelInput(Vector2 newScrollWheelInput)
        {
            scrollWheel = newScrollWheelInput;
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorLockState(cursorLocked);
		}

		public void SetCursorLockState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}