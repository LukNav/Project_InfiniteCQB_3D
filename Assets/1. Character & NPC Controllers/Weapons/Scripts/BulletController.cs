using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public TrailRenderer trailRenderer;

    //We don't want to destroy and instantiate bullets all the time - we want to enable and disable them, which is more effective
    private void OnEnable()
    {
        trailRenderer.Clear();
        Invoke("DestroySelf", 2f);
    }

    private void OnDisable()
    {
        CancelInvoke("DestroySelf");
    }

    private void DestroySelf()
    {
        gameObject.SetActive(false);
    }

}
