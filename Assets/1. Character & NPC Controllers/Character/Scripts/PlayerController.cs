using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Interactions;
using Main.Camera.Enums;

namespace ICQB.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Weapon UI settings")]
        public Texture2D cursorTexture;
        public CursorMode cursorMode = CursorMode.Auto;

        [Header("Debug")]
        public Transform debugObject;

        public Vector3 MouseTarget { get { return _mouseTarget; } }
        private Vector3 _mouseTarget;
        public float DistanceFromCharacterToMouse { get { return _distanceFromCharacterToMouse; } }
        private float _distanceFromCharacterToMouse;


        void Awake()
        {
            SetCursor();
        }


        private void FixedUpdate()
        {
            UpdateMouseInfo();
        }

        #region Debugging Area
        public void OnTest(InputAction.CallbackContext context) => Test();

        private void Test()
        {

        }

        void OnGUI()
        {
            //GUI.Box(_screenRect, "Box in the middle");
        }
        #endregion

        private void UpdateMouseInfo()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                _mouseTarget = ray.GetPoint(distance);
                _distanceFromCharacterToMouse = Vector3.Distance(transform.position, _mouseTarget);
            }
        }

        private void SetCursor()
        {
            Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), cursorMode);
        }

        #region old methods for reference
        //private void RotateTowardsMouose()
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().y, 0f, Mouse.current.position.ReadValue().x));
        //    if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask))
        //    {
        //        var direction = hitInfo.point - transform.position;
        //        direction.y = 0f;
        //        direction.Normalize();
        //        transform.forward = direction;
        //    }
        //}

        //private void RotatePlayer(Vector3 direction)
        //{
        //    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, characterRotationPower * Time.fixedDeltaTime);
        //}

        //private Vector3 CalculateDirection()
        //{
        //    return new Vector3(
        //            _movementDirection.y * cameraTarget.forward.x + _movementDirection.x * cameraTarget.right.x,
        //            0,
        //            _movementDirection.y * cameraTarget.forward.z + _movementDirection.x * cameraTarget.right.z);
        //}
        #endregion
    }

}
