using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Common.Utils;
public class FieldOfView : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("FOV Settings")]
    public float fov = 90f;
    public int rayCount = 20;
    public float viewDistance = 7f;
    public LayerMask obstacleLayers;
    public GameObject fovPrefab;//prefab with mesh filter and mesh renderer components

    [Header("Target tracking")]
    public LayerMask targetsLayers;

    [Header("Dependency Settings")]
    public bool useLocalTransform = true;//If false: use SetOrigin and SetAimDirection methods to update the transform values

    [Header("Debugging. Don't touch")]
    public List<Transform> visibleTargets = new List<Transform>();
    private int lastVisibleTargetsListCount = 0;

    Vector3 _origin;
    float _startingAngle = 0f;
    float _angle;
    float _angleIncrease;
    Mesh mesh;
    float dotAngleProduct;
    //public GameObject targetLockPrefab;
    //private GameObject _targetLockImage;

    void Start()
    {
        if(mesh == null)
        {
            CreateFieldOfViewMesh();
        }

        _angleIncrease = fov / rayCount;
        _origin = Vector3.zero;

        dotAngleProduct = Mathf.Abs(Vector3.Dot(transform.forward, Utils.GetVectorFromAngle((360 - fov) / 2)));

        //_targetLockImage = Instantiate(targetLockPrefab);
    }

    private void CreateFieldOfViewMesh()
    {
        mesh = new Mesh();
        GameObject fovGameObject = Instantiate(fovPrefab);
        MeshFilter meshFilter = fovGameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            Debug.LogError("fovPrefab MUST have Mesh filter and Mesh renderer");
        meshFilter.mesh = mesh;
    }

    private void Update()
    {
        if(useLocalTransform)
        {
            SetOrigin(transform.position);
            SetAimDirection(transform.forward);
        }

        VisualiseFOVCone();
        //if(lastVisibleTargetsListCount != visibleTargets.Count)
        SetVisibleTargets();

    }


    private void VisualiseFOVCone()
    {
        _angle = _startingAngle;


        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = _origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;

            RaycastHit raycastHit;
            Vector3 direction = Utils.GetVectorFromAngle(_angle);
            if (!Physics.Raycast(_origin, direction, out raycastHit, viewDistance, obstacleLayers))
            {
                vertex = _origin + Utils.GetVectorFromAngle(_angle) * viewDistance;
            }
            else
            {
                vertex = raycastHit.point;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)//We run it only after the first loop, because we need origin vertex to connect to.
            {
                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = 0;
                triangleIndex += 3;
            }
            vertexIndex++;


            _angle -= _angleIncrease;//-= for clockwise rotation
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    private void SetVisibleTargets()
    {
        visibleTargets = new List<Transform>();

        Collider[] targets = Physics.OverlapSphere(_origin, viewDistance, targetsLayers);
        foreach (Collider target in targets)
        {
            Vector3 direction = target.transform.position - _origin;
            RaycastHit hitInfo;
            bool didRaycastHitObstacle = Physics.Raycast(_origin, direction, out hitInfo, viewDistance, obstacleLayers);
            float distanceToTarget = Vector3.Distance(_origin, target.transform.position);

            if (!didRaycastHitObstacle || distanceToTarget < hitInfo.distance)
            {
                Transform targetTransform = target.transform;
                if (IsTargetInFOVField(targetTransform.position))
                    visibleTargets.Add(targetTransform);
            }
        }

        lastVisibleTargetsListCount = visibleTargets.Count;
    }

    private bool IsTargetInFOVField(Vector3 target)
    {
        Vector3 direction = target - _origin;
    
        float dotProduct = Vector3.Dot(transform.forward, direction.normalized);// If confused what dot is - google Dot Product
        //Debug.Log("dotAngleProduct: " + dotAngleProduct + " : DotProduct: " + dotProduct);
        bool isTargetInsideFOVCone = dotProduct > dotAngleProduct;

        if (isTargetInsideFOVCone)
            return true;

        return false;
    }

    //private void HighlightVisibleTargets()
    //{
    //    if (visibleTargets.Count == 0)
    //        return;

    //    _targetLockImage.transform.position = visibleTargets[0].position;
    //}

    public void SetOrigin(Vector3 origin)
    {
        _origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        _startingAngle = Utils.GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }

    /// <summary>
    /// Returns target's transform
    /// </summary>
    /// <returns>Transform of target. If there are no visible targets - returns null</returns>
    public Transform GetTarget()
    {
        if (visibleTargets.Count > 0)
            return visibleTargets[0];
        return null;
    }


}
