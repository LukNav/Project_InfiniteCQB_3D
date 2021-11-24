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
    public float angleChangeDelay = 3;


    private NavMeshAgent _agent;
    private ShootingController _shootingController;
    private NPCAnimator _animatorController;
    public StatsController statsController { get; private set; }
    private BTSelector _rootBT;
    private Timer scanTimer;
    private Timer angleChangeTimer;

    public bool isRotating = false;
    public float elapsedRotationTime = 0f;
    public float rotationAngle;
    


    // Start is called before the first frame update
    void Start()
    {
        rotationAngle = transform.eulerAngles.y;

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

        statsController = GetComponent<StatsController>();
        if (statsController == null)
        {
            Debug.LogError("Missing component: StatsController");
        }


        scanTimer = new Timer(scanDelay);
        angleChangeTimer = new Timer(angleChangeDelay);


        BTSequence followTargetSequence = new BTSequence(new List<BTNode>
        {
            new BTCanSeeTarget(fovController),
            new BTTimer_Stop(scanTimer),
            new BTTimer_Stop(angleChangeTimer),
            new BTRotateToTarget(this, fovController),
            new BTMoveToTarget(fovController, _agent),
            new BTPlayAnimation(_animatorController.rigController, _animatorController.drawAnimationName),
            new BTShootTarget(_shootingController, fovController)
        });


        BTSequence isHitSequence = new BTSequence(new List<BTNode>
        {
            new BTIsHit(statsController),
            new BTRotateToHitDirection(this),
            new BTSelector(new List<BTNode>//yEd Graph name: HasRotated
            {
                new BTSequence(new List<BTNode>
                {
                    new BTIsNotRotating(this),
                    new BTTimer_Stop(scanTimer),
                    new BTTimer_Stop(angleChangeTimer),
                    new BTResetHitInfo(statsController)
                }),
                new BTSuccess()
            })
        });

        BTSequence scanSequence = new BTSequence(new List<BTNode>
        {
            new BTTimer_Start(scanTimer),
            new BTSelector(new List<BTNode>//yEd Graph name: DelayTheScanningStart
            {
                new BTSequence(new List<BTNode>
                {
                    new BTTimer_HasEnded(scanTimer),
                    new BTTimer_Start(angleChangeTimer),
                    new BTSelector(new List<BTNode> //yEd Graph name: AngleChangeDelay
                    {
                        new BTSequence(new List<BTNode>
                        {
                            new BTTimer_HasEnded(angleChangeTimer),
                            new BTRotateToRandomAngle(this, 90f, true),
                            new BTTimer_Restart(angleChangeTimer)
                        })
                    }),
                    new BTSuccess()
                }),
                new BTSuccess()
            })
        });

        _rootBT = new BTSelector(new List<BTNode>
        {
            followTargetSequence,
            isHitSequence,
            scanSequence
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldFollowTarget)
            _rootBT.Evaluate();

        scanTimer.Update();
        angleChangeTimer.Update();

        RotateToSetAngle();
    }

    private void RotateToSetAngle()
    {
        if(!isRotating)
            return;

        float currentRotation_y = transform.eulerAngles.y;
        if (Mathf.Abs(Mathf.DeltaAngle(currentRotation_y, rotationAngle)) < 0.1f)//is npc rotated close enough to the angle
        {
            isRotating = false;
            elapsedRotationTime = 0f;
        }
        else
        {
            transform.eulerAngles = new Vector3(0f, Mathf.LerpAngle(currentRotation_y, rotationAngle, elapsedRotationTime / rotationDuration), 0f);
            elapsedRotationTime += Time.deltaTime;
        }
    }

    public void OnDisable()
    {
        fovController.enabled = false;
        _agent.enabled = false;
        _animatorController.enabled = false;
    }

    
}
