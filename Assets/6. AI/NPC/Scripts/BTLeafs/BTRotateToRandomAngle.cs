using UnityEngine;

public class BTRotateToRandomAngle : BTNode
{
    private NPCController _npcController;

    public BTRotateToRandomAngle(NPCController npcController)
    {
        _npcController = npcController;
        _npcController.angle = npcController.transform.eulerAngles.y;
    }

    public override BTNodeStates Evaluate()
    {
        if (_npcController.isRotating)
        {
            currentNodeState = BTNodeStates.SUCCESS;//return SUCCESS if npc is rotating
            return currentNodeState;
        }
        else
        {
            SetNewRandomAngle();
            _npcController.isRotating = true;

            currentNodeState = BTNodeStates.FAILURE;//return failure if the npc is not rotating
            return currentNodeState;
        }
    }

    private void SetNewRandomAngle()
    {
        Vector3 target = new Vector3(Random.Range(-10, 11), Random.Range(-10, 11));
        float angle = Mathf.Atan2(target.x, target.y) * Mathf.Rad2Deg;
        if (angle >= 180f) angle = -180f + angle;
        if (angle <= -180f) angle = 180f + angle;
        _npcController.angle = _npcController.transform.eulerAngles.y + 90f;

        _npcController.angle -= _npcController.angle > 360f ? 360f : 0f;
    }
}