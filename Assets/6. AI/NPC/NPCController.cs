using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [SerializeField] private bool shouldFollowTarget = true;
    [SerializeField] private FieldOfView fovController;

    private NavMeshAgent _agent;
    private ShootingController _shootingController;
    private NPCAnimator _animatorController;
    private BTSelector _rootBT;


    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if(_agent == null)
        {
            Debug.LogError("Missing component: NavMeshAgent");
        }

        _shootingController = GetComponent<ShootingController>();
        if (_shootingController == null)
        {
            Debug.LogError("Missing component: ShootingController");
        }

        _animatorController = GetComponent<NPCAnimator>();
        if (_animatorController == null)
        {
            Debug.LogError("Missing component: NPCAnimator");
        }

        BTSequence followTargetSequence = new BTSequence(new List<BTNode>
        {
            new BTCanSeeTarget(fovController),
            new BTMoveToTarget(fovController, _agent),
            new BTPlayAnimation(_animatorController.rigController, _animatorController.drawAnimationName),
            new BTShootTarget(_shootingController, fovController)
        });

        _rootBT = new BTSelector(new List<BTNode>
        {
            followTargetSequence
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldFollowTarget)
            _rootBT.Evaluate();
    }

    
}
