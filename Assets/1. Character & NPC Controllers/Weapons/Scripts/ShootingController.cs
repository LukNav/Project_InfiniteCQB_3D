using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    [Header("GameObject references")]
    public Transform firePoint;
    public GameObject bullet;
    // Start is called before the first frame update

    [Header("Optimization settings")]
    public int bulletPoolSize = 15;// maximum count of pooled bullet objects
    private List<GameObject> _bulletPool; // currently instantiated bullets

    [Header("Weapon Settings")]
    //public float rateOfFire = 0.7f; //M1911 pistol Rate of fire is 85 rounds/min, thus 60s/85 = 0.7s delay
    public float bulletSpeed = 10f;

    private bool isSprinting = false;

    void Awake()
    {
        _bulletPool = new List<GameObject>();
        for (int i = 0; i < bulletPoolSize; i++)
        {
            GameObject go = Instantiate(bullet);
            go.SetActive(false);
            _bulletPool.Add(go);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.started || context.performed;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log($"{context.phase} : {Time.time}");
        if (InputActionPhase.Started == context.phase)
        {
            for (int i = 0; i < _bulletPool.Count; i++)
            {
                if (!_bulletPool[i].activeInHierarchy)
                {
                    _bulletPool[i].transform.position = firePoint.position;
                    _bulletPool[i].transform.rotation = firePoint.rotation;
                    _bulletPool[i].SetActive(true);

                    if (!isSprinting)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());//---THIS COULD BE REUSED FROM PlayerController!!!!!!!!
                        Plane plane = new Plane(Vector3.up, Vector3.zero);
                        float distance;
                        if (plane.Raycast(ray, out distance))
                        {
                            Vector3 target = ray.GetPoint(distance);
                            Vector3 direction = target - firePoint.position;
                            direction.Normalize();
                            _bulletPool[i].GetComponent<Rigidbody>().velocity = direction * bulletSpeed;
                        }
                        break;
                    }

                    _bulletPool[i].GetComponent<Rigidbody>().velocity = firePoint.forward * bulletSpeed;
                    break;
                }
            }
        }
    }
}
