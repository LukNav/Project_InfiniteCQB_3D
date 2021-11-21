using System.Collections;
using UnityEngine;

namespace ICQB.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Animator settings")]
        public Animator animator;
        public float animatorSprintAnimation_Pos = 1.5f; //Blend tree named "Movement" has sprint animation, which y position we place here

        [Header("Weapon Animator settings")]
        public Animator rigController;
        public string drawAnimationName = "Weapon_PistolDraw_Anim";

        //[Header("Dependencies")]
        private PlayerController _playerController;
        private PlayerMovementController _playerMovementController;
        private CameraController _cameraController;

        private void Awake()
        {
            SubscribeToInputSystem();
            GetDependencies();
        }
        private void GetDependencies()
        {
            _cameraController = GetComponent<CameraController>();
            if (_cameraController == null)
                Debug.LogError("CameraController missing");

            _playerController = GetComponent<PlayerController>();
            if (_playerController == null)
                Debug.LogError("PlayerController missing");

            _playerMovementController = GetComponent<PlayerMovementController>();
            if (_playerMovementController == null)
                Debug.LogError("PlayerMovementController missing");

        }

        private void SubscribeToInputSystem()
        {
            PlayerInputController.input_OnDrawWeaponDelegate += DrawWeapon;

        }

        private void FixedUpdate()
        {

            AnimateMovement();


            AnimateCharacterTurning();
        }

        public void DisableMovementAnimations()
        {
            animator.SetFloat("VelocityZ", 0, 0.1f, Time.fixedDeltaTime);
            animator.SetFloat("VelocityX", 0, 0.1f, Time.fixedDeltaTime);
        }

        private void AnimateMovement()
        {
            float speedModifier = _playerMovementController._isSprinting ? animatorSprintAnimation_Pos : 1; // Multiplies the Velocity float sent to Animator Blend Tree. Set 1 if not sprinting. 

            //speedModifier =
            //    _playerMovementController._isMovingTowardsMouse
            //    && _playerController.DistanceFromCharacterToMouse <= _playerMovementController.moveTowardsMouseDampDistance
            //    ? 1 : speedModifier;

            Vector3 movementDirection = new Vector3(_playerMovementController._movementDirection.x, 0f, _playerMovementController._movementDirection.y);
            if (!_playerMovementController._isMovingTowardsMouse)//if not moving towards mouse should get a direction relative to characters rotation
                movementDirection = Quaternion.Euler(0, -transform.rotation.eulerAngles.y + (int)_cameraController.CameraDirection * 90f, 0) * movementDirection;//rotate the movement direction relative to characters rotation (Can't use euler directly 'transform.rotation' since it is not as accurate for some reason)
            animator.SetFloat("VelocityZ", speedModifier * movementDirection.z * speedModifier, 0.1f, Time.fixedDeltaTime);
            animator.SetFloat("VelocityX", speedModifier * movementDirection.x * speedModifier, 0.1f, Time.fixedDeltaTime);
        }

        private void AnimateCharacterTurning()
        {
            animator.SetFloat("Turn", Mathf.Clamp(_playerMovementController.charactersRotationDelta * 1.3f, -1, 1), _playerMovementController.turnDampTime, Time.fixedDeltaTime);
        }

        private void DrawWeapon()
        {
            if(rigController != null)
                rigController.Play(drawAnimationName);
        }
    }
}