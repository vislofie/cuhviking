using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    [SerializeField]
    private GameObject _FOVVisualizer;

    [SerializeField]
    private LayerMask _mask;
    [SerializeField]
    private float _viewDistance = 20.0f;
    [SerializeField]
    private float _viewAngle = 90.0f;
    [SerializeField]
    private int _rayCount = 2;

    private Mesh _mesh;
    private float _startingAngle;
    Vector3 _origin;

    Ray ray;

    private void Start()
    {
        _mesh = new Mesh();
        _FOVVisualizer.GetComponent<MeshFilter>().mesh = _mesh;
        _origin = Vector3.zero;
    }

    private void Update()
    {
        float angle = _startingAngle;
        float angleIncrease = _viewAngle / _rayCount;

        Vector3[] vertices = new Vector3[_rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[_rayCount * 3];

        vertices[0] = _origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= _rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit hit;
            Physics.Raycast(_origin, Utils.GetVectorFromAngleXZ(angle), out hit, _viewDistance, _mask);
            if (hit.collider == null)
            {
                // no hit
                vertex = _origin + Utils.GetVectorFromAngleXZ(angle) * _viewDistance;
            }
            else
            {
                // hit object
                vertex = hit.point;
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex += 1;
            angle -= angleIncrease;
        }

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
       
        _mesh.RecalculateBounds();  
    }

    public void SetOrigin(Vector3 origin)
    {
        _origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        _startingAngle = Utils.GetAngleFromVectorXZ(aimDirection) + _viewAngle / 2.0f;
    }
}
