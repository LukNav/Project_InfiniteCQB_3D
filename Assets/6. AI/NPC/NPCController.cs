using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public bool shouldFollowTarget = true;
    public FieldOfView fovController;
    private NavMeshAgent _agent;

    private BTSelector rootBT;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if(_agent == null)
        {
            Debug.LogError("Missing component: NavMeshAgent");
        }

        BTSequence followTargetSequence = new BTSequence(new List<BTNode>
        {
            new BTCanSeeTarget(fovController),
            new BTMoveToTarget(fovController, _agent)
        });

        rootBT = new BTSelector(new List<BTNode>
        {
            followTargetSequence
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldFollowTarget)
            rootBT.Evaluate();
    }

    
}
