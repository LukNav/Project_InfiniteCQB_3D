using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public float damage = 15f;

    private bool _isActive = false;
    private Vector3 _lastPos;
    //We don't want to destroy and instantiate bullets all the time - we want to enable and disable them, which is more effective
    private void OnEnable()
    {
        trailRenderer.Clear();
        Invoke("DestroySelf", 2f);
        _isActive = true;
        _lastPos = Vector3.down;
    }

    private void OnDisable()
    {
        CancelInvoke("DestroySelf");
        _isActive = false;
    }

    public void Update()
    {
        if (_isActive)// PROBLEM <--- This activates when instantiating it - could affect the performance, could be optimised
        {
            if (_lastPos == Vector3.down)
            {
                _lastPos = transform.position;
                return;
            }

            Vector3 velocityDir = transform.position - _lastPos;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, velocityDir, out hit, 0.3f))
            {
                OnTriggerEnter(hit.collider);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet HIT");
        StatsController statsController = other.GetComponentInParent<StatsController>();
        if(statsController != null)
        {
            statsController.TakeDamage(damage);
        }
        DestroySelf();
    }

    private void DestroySelf()
    {
        gameObject.SetActive(false);
    }

}
