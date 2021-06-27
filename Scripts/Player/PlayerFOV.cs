using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFOV : MonoBehaviour
{
    [SerializeField]
    private GameObject _FOVVisualizer;
    [SerializeField]
    private GameObject _FOVShadowReceiver;

    [SerializeField]
    private LayerMask _mask;
    [SerializeField]
    private LayerMask _shadowReceiverMask;
    [SerializeField]
    private float _viewDistance = 20.0f;
    [SerializeField]
    private float _viewAngle = 90.0f;
    [SerializeField]
    private int _rayCount = 2;

    private Mesh _mesh;
    private Mesh _shadowReceiverMesh;
    private float _startingAngle;
    Vector3 _origin;

    Ray ray;

    private void Start()
    {
        _mesh = new Mesh();
        _shadowReceiverMesh = new Mesh();
        _FOVVisualizer.GetComponent<MeshFilter>().mesh = _mesh;
        _FOVShadowReceiver.GetComponent<MeshFilter>().mesh = _shadowReceiverMesh;
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

        Vector3[] shadowReceiverVertices = vertices;
        RaycastHit SRHit;
        Physics.Raycast(_origin, Vector3.down, out SRHit, _shadowReceiverMask);
        if (SRHit.collider != null)
            shadowReceiverVertices[0] = SRHit.point;

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
                Debug.Log("obj was hit");
                // hit object
                vertex = hit.point;
            }
            vertices[vertexIndex] = vertex;

            Physics.Raycast(vertex, Vector3.down, out SRHit, _shadowReceiverMask);
            if (SRHit.collider != null)
                shadowReceiverVertices[vertexIndex] = SRHit.point;

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

        _shadowReceiverMesh.vertices = shadowReceiverVertices;
        _shadowReceiverMesh.uv = uv;
        _shadowReceiverMesh.triangles = triangles;
        

        _mesh.RecalculateBounds();
        _shadowReceiverMesh.RecalculateBounds();    
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
