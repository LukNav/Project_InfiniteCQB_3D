using UnityEngine;

public class BTRotateToRandomAngle : BTNode
{
    private NPCController _npcController;
    private bool _randomiseDirection;
    private float _angleIncrease;
    public BTRotateToRandomAngle(NPCController npcController, float angleIncrease = 90f, bool randomiseDirection = true)
    {
        _npcController = npcController;
        _randomiseDirection = randomiseDirection;
        _angleIncrease = angleIncrease;
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
        float direction = _randomiseDirection ? Random.Range(-1, 2) : 1;
       
        float angle = _npcController.transform.eulerAngles.y + _angleIncrease * direction;
        angle -= angle > 360f ? 360f : 0f;
        _npcController.rotationAngle = angle;
    }
}