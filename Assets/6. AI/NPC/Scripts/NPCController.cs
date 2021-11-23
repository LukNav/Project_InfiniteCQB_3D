using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public bool shouldFollowTarget = true;
    public FieldOfView fovController;

    [Header("Scan settings")]
    public float rotationDuration = 0.5f;
    public float scanDelay = 2f;


    private NavMeshAgent _agent;
    private ShootingController _shootingController;
    private NPCAnimator _animatorController;
    private BTSelector _rootBT;
    private Timer scanTimer;

    public bool isRotating = false;
    private float _elapsedTime = 0f;
    public float angle = 0f;


    // Start is called before the first frame update
    void Start()
    {
        angle = transform.eulerAngles.y;

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
            new BTRotateToTarget(this, fovController),
            new BTMoveToTarget(fovController, _agent),
            new BTPlayAnimation(_animatorController.rigController, _animatorController.drawAnimationName),
            new BTShootTarget(_shootingController, fovController)
        });

        scanTimer = new Timer(scanDelay);
        BTSelector scanSelector = new BTSelector(new List<BTNode>
        {
            new BTSequence (new List<BTNode>
            {
                new BTTimer_HasEnded(scanTimer),
                new BTRotateToRandomAngle(this),
                new BTTimer_Stop(scanTimer)
            }),
            new BTTimer_Start(scanTimer)
        });

        _rootBT = new BTSelector(new List<BTNode>
        {
            followTargetSequence,
            scanSelector
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFollowTarget)
            _rootBT.Evaluate();

        scanTimer.Update();

        RotateToSetAngle();
    }

    private void RotateToSetAngle()
    {
        if(!isRotating)
            return;

        float currentRotation_y = transform.eulerAngles.y;
        if (Mathf.Abs(currentRotation_y - angle) < 0.1f)//is npc rotated close enough to the angle
        {
            isRotating = false;
            _elapsedTime = 0f;
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(currentRotation_y, angle, _elapsedTime / rotationDuration), 0f);
            _elapsedTime += Time.deltaTime;
        }
    }

    public void OnDisable()
    {
        fovController.enabled = false;
        _agent.enabled = false;
        _animatorController.enabled = false;
    }

    
}
