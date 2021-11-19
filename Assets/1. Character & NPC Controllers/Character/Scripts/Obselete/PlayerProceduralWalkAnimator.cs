using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProceduralWalkAnimator : MonoBehaviour
{
    public Transform pivotPoint;
    public Transform leftLegTarget;
    public Transform rightLegTarget;

    public float widthBetweenLegs = 2f;
    public Vector2 circleSize = new Vector2(2f, 1f);
    public float rotationSpeedMultiplier = 2f;
    [Range(0f, 2f)]
    public float offsetOfValuePI = 1;
    public bool shouldInvertRotationAxis = false;
    private float _currentRotationTime = 0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_currentRotationTime > 2 * Mathf.PI)
            _currentRotationTime -= 2 * Mathf.PI;

        float offsetVal = Mathf.PI * offsetOfValuePI / 2;
        float rightLegRotationPos_X = circleSize.x * Mathf.Cos(_currentRotationTime - offsetVal);
        float rightLegRotationPos_Y = circleSize.y * Mathf.Sin(_currentRotationTime - offsetVal);

        float leftLegRotationPos_X = circleSize.x * Mathf.Cos(_currentRotationTime + offsetVal) ;
        float leftLegRotationPos_Y = circleSize.y * Mathf.Sin(_currentRotationTime + offsetVal) ;

        if (!shouldInvertRotationAxis)
        {
            leftLegTarget.position = new Vector3(leftLegRotationPos_X, leftLegRotationPos_Y, -widthBetweenLegs / 2) + pivotPoint.position;
            rightLegTarget.position = new Vector3(rightLegRotationPos_X, rightLegRotationPos_Y, widthBetweenLegs / 2) + pivotPoint.position;
        }
        else
        {
            leftLegTarget.position = new Vector3(-widthBetweenLegs / 2, leftLegRotationPos_Y, leftLegRotationPos_X) + pivotPoint.position;
            rightLegTarget.position = new Vector3(widthBetweenLegs / 2, rightLegRotationPos_Y, rightLegRotationPos_X) + pivotPoint.position;
        }
        _currentRotationTime += Time.fixedDeltaTime * rotationSpeedMultiplier;

        Debug.Log(pivotPoint.localPosition);
    }
}
