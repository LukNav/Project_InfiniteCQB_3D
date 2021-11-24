using UnityEngine;

public class BTRotateToHitDirection : BTNode
{
    private NPCController _npcController;
    private StatsController _statsController;
    private Vector3 _hitDirection;
    public BTRotateToHitDirection(NPCController npcController)
    {
        _npcController = npcController;
        _statsController = npcController.statsController;
    }

    public override BTNodeStates Evaluate()
    {
        if(IsAlreadyRotatingToDirection())
        {
            currentNodeState = BTNodeStates.FAILURE;
            return currentNodeState;
        }

        SetAngleToTarget();
        _npcController.isRotating = true;
        currentNodeState = BTNodeStates.SUCCESS;
        return currentNodeState;
    }

    /// <summary>
    /// Sets Rotation angle on NPCController to the direction of target
    /// </summary>
    /// <returns>return false if angle is already set</returns>
    private void SetAngleToTarget()
    {
        _hitDirection = _statsController.hitDirection;
        float angle = Mathf.Atan2(_hitDirection.x, _hitDirection.z) * Mathf.Rad2Deg;

        _npcController.rotationAngle = angle;
    }

    private bool IsAlreadyRotatingToDirection()
    {
        return _hitDirection == _statsController.hitDirection;
    }
}
