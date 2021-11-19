using System.Collections;
using UnityEngine;

namespace ICQB.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 1.5f;
        [SerializeField] private float sprintSpeed = 5f;//not using multiplier because, if player walks slowly with a controller, and clicks sprint - see, see the problem? DO YOU SEE IT?
        [SerializeField] private float backwardsSpeedMultiplier = 0.75f;
        [SerializeField] private LayerMask aimLayerMask;
        [SerializeField] private float walkRotationSmoothTime = 0.5f;
        [SerializeField] internal float sprintRotationSmoothTime = 0.2f;
        [SerializeField] internal float moveTowardsMouseDampDistance = 1f; //When moving towards mouse, and distance to mouse from character is small - movement speed has to decrease. This is the distance from which the speed decreasing should occur
        [SerializeField] private float moveTowardsMouseDeadzone = 0.4f;
        [SerializeField] internal float turnDampTime = 0.1f;

        internal Vector2 _movementDirection;
        internal bool _isSprinting = false;
        internal bool _isMovingTowardsMouse = false;
        internal float charactersRotationDelta;//used to animate character's rotation


        private CameraController _cameraController;
        private PlayerController _playerController;

        void Awake()
        {
            SubscribeToInputSystem();
            GetDependencies();
        }

        private void SubscribeToInputSystem()
        {
            PlayerInputController.input_OnMoveDelegate += OnMove;
            PlayerInputController.input_OnMoveTowardsMouseDelegate += OnMoveTowardsMouse;
            PlayerInputController.input_OnSprintDelegate += OnSprint;
        }
        private void GetDependencies()
        {
            _cameraController = GetComponent<CameraController>();
            if (_cameraController == null)
                Debug.LogError("CameraController missing");

            _playerController = GetComponent<PlayerController>();
            if (_playerController == null)
                Debug.LogError("PlayerController missing");
        }

        private void FixedUpdate()
        {
            if (_playerController.DistanceFromCharacterToMouse > moveTowardsMouseDeadzone)
            {
                MoveCharacter();
            }

            RotateCharacter();

        }


        public void OnMove(Vector2 movementDirection)
        {
            _movementDirection = movementDirection;
        }
        public void OnMoveTowardsMouse(bool isMovingTowardsMouse)
        {
            _isMovingTowardsMouse = isMovingTowardsMouse;
        }
        public void OnSprint(bool isSprinting)
        {
            _isSprinting = isSprinting;
        }

        private void MoveCharacter()
        {
            float speed = _isSprinting ? sprintSpeed : walkSpeed;
            speed *= _movementDirection.y <= 0 ? backwardsSpeedMultiplier : 1;//if moving backwards - multiply the speed by backwardsSpeedMultiplier to slow character down

            if (_isMovingTowardsMouse
                && _playerController.DistanceFromCharacterToMouse <= moveTowardsMouseDampDistance)// when moving towards mouse the speed is decreased if mouse is close to character
            {
                speed *= (_playerController.DistanceFromCharacterToMouse * _playerController.DistanceFromCharacterToMouse) / moveTowardsMouseDampDistance;
            }

            Vector3 movementDirection = new Vector3(_movementDirection.x, 0f, _movementDirection.y);
            movementDirection.Normalize();
            if (!_isMovingTowardsMouse)
            {
                movementDirection = Quaternion.Euler(0f, (int)_cameraController.CameraDirection * 90f, 0f) * movementDirection;//rotate movement vector towards relative screen rotation
                transform.Translate(movementDirection * speed * Time.fixedDeltaTime, Space.World);
            }
            else
            {
                transform.Translate(movementDirection * speed * Time.fixedDeltaTime);
            }
        }

        private void RotateCharacter()
        {
            float lastRotationAngle_Y = transform.rotation.y;

            //transform.rotation *= Quaternion.AngleAxis(_lookDirection.x, Vector3.up); old implementation
            Vector3 direction = _playerController.MouseTarget - transform.position;
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float speedModifier = 1f;

            if (_isMovingTowardsMouse
                && _playerController.DistanceFromCharacterToMouse <= moveTowardsMouseDampDistance)// when moving towards mouse the speed is decreased if mouse is close to character
            {
                speedModifier = (_playerController.DistanceFromCharacterToMouse * _playerController.DistanceFromCharacterToMouse) / moveTowardsMouseDampDistance;
            }

            float rotationSmoothTime = _isSprinting ? sprintRotationSmoothTime : walkRotationSmoothTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotation, 0), rotationSmoothTime * speedModifier);

            float currentAngle = transform.rotation.eulerAngles.y;
            charactersRotationDelta = rotation < 0 ? rotation + 360 - currentAngle : rotation - currentAngle;
            //_isTurning = Mathf.Abs(charactersRotationDelta) > 0.1 ? true : false;
            Debug.Log(charactersRotationDelta + " : " + currentAngle);
        }
    }
}