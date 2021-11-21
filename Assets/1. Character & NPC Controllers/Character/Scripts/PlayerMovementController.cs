﻿using System.Collections;
using UnityEngine;

namespace ICQB.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        public float walkSpeed = 1.5f;
        public float sprintSpeed = 5f;//not using multiplier because, if player walks slowly with a controller, and clicks sprint - see, see the problem? DO YOU SEE IT?
        public float backwardsSpeedMultiplier = 0.75f;
        public LayerMask aimLayerMask;
        public float walkRotationSmoothTime = 0.5f;
        public float sprintRotationSmoothTime = 0.2f;
        public float moveTowardsMouseDampDistance = 1f; //When moving towards mouse, and distance to mouse from character is small - movement speed has to decrease. This is the distance from which the speed decreasing should occur
        public float moveTowardsMouseDeadzone = 0.4f;
        public float turnDampTime = 0.1f;

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
            MoveCharacter();
            RotateCharacter();
        }


        public void OnMove(Vector2 movementDirection)
        {
            _movementDirection = movementDirection;
        }
        public void OnMoveTowardsMouse(bool isMovingTowardsMouse)
        {
            //_isMovingTowardsMouse = isMovingTowardsMouse;
        }
        public void OnSprint(bool isSprinting)
        {
            _isSprinting = isSprinting;
        }

        private void MoveCharacter()
        {
            float speed = _isSprinting ? sprintSpeed : walkSpeed;
            speed *= _movementDirection.y <= 0 ? backwardsSpeedMultiplier : 1;//if moving backwards - multiply the speed by backwardsSpeedMultiplier to slow character down

            Vector3 movementDirection = new Vector3(_movementDirection.x, 0f, _movementDirection.y);
            movementDirection.Normalize();
            movementDirection = Quaternion.Euler(0f, (int)_cameraController.CameraDirection * 90f, 0f) * movementDirection;//rotate movement vector towards relative screen rotation
            transform.Translate(movementDirection * speed * Time.fixedDeltaTime, Space.World);

        }

        private void RotateCharacter()
        {
            float lastRotationAngle_Y = transform.rotation.y;

            Vector3 direction = _playerController.MouseTarget - transform.position;
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float speedModifier = 1f;

            float rotationSmoothTime = _isSprinting ? sprintRotationSmoothTime : walkRotationSmoothTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotation, 0), rotationSmoothTime * speedModifier);

            float currentAngle = transform.rotation.eulerAngles.y;
            charactersRotationDelta = rotation < 0 ? rotation + 360 - currentAngle : rotation - currentAngle;
         
        }
    }
}