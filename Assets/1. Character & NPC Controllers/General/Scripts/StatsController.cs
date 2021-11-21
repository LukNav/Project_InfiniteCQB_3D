using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    public float health = 100;

    public Animator animator;// move this to other script, e.g. rigid body controller and subscribe to Die event
    public Collider collider;
    public GameObject weaponCollider; // This is not pretty, but works for demo - Has a task to fix in "Mid-Late Phase" epic
    public Rigidbody rigidBody;
    public Rigidbody rigsRigidBody;
    public GameObject hips;
    public MonoBehaviour controller;

    public delegate void DeathDelegate();
    public DeathDelegate deathDelegate;

    bool isDead { get { return health <= 100; } }


    public void Start()
    {
        deathDelegate += Die;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            deathDelegate();
            return;
        }
    }

    private void Die()
    {
        animator.enabled = false;
        collider.enabled = false;
        rigidBody.isKinematic = true;
        
        hips.SetActive(true);
        //rigsRigidBody.constraints = RigidbodyConstraints.None;
        controller.enabled = false;
        //weaponCollider.SetActive(true);

        
        //Destroy(gameObject);
    }
}
