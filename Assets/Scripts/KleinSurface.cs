using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class KleinSurface : MonoBehaviour
{
    private Mesh _mesh;

    [SerializeField]
    private int _thetaResolution = 10;

    [SerializeField]
    private int _vResolution = 10;

    [SerializeField]
    private float _r = 10f;

    [SerializeField]
    private float _w = 10f;

    [SerializeField]
    private Vector3 _rotateAxis = Vector3.up;

    [SerializeField]
    private float _rotateSpeed = 10f;

    private void Awake()
    {
        _mesh = GetComponent<MeshFilter>().mesh = new Mesh();

        Build();
    }

    private void Update()
    {
        Build();

        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(_rotateAxis, _rotateSpeed * Time.deltaTime);
    }

    private void Build()
    {
        int verts = _thetaResolution * _vResolution;

        Vector3[] vertices = new Vector3[verts];
        int[] indices = new int[_thetaResolution * (_vResolution + 1) * 6];

        float vDiff = Mathf.PI * 2f / _vResolution;
        float thetaDiff = Mathf.PI * 2f / _thetaResolution;
        
        for (int vCount = 0; vCount < _vResolution; vCount++)
        {
            for (int thetaCount = 0; thetaCount < _thetaResolution; thetaCount++)
            {
                float v = vCount * vDiff;
                float theta = thetaCount * thetaDiff;

                float x = (_r + Mathf.Cos(theta / 2f) * Mathf.Cos(v) - Mathf.Sin(theta / 2f) * Mathf.Sin(2f * v)) * Mathf.Cos(theta);
                float y = (_r + Mathf.Cos(theta / 2f) * Mathf.Cos(v) - Mathf.Sin(theta / 2f) * Mathf.Sin(2f * v)) * Mathf.Sin(theta);
                float z = _w * Mathf.Sin(theta / 2f) * Mathf.Cos(v) + Mathf.Cos(theta / 2f) * Mathf.Sin(2f * v);

                vertices[thetaCount * _vResolution + vCount] = new Vector3(x, y, z);
            }

            if (vCount > 0)
            {
                for (int thetaCount = 0; thetaCount < _thetaResolution; thetaCount++)
                {
                    int vertexBufferIndex = vCount * _thetaResolution + thetaCount;
                    int indexBufferIndex = ((vCount - 1) * _thetaResolution + thetaCount) * 6;

                    indices[indexBufferIndex] = vertexBufferIndex;
                    indices[indexBufferIndex + 1] = (vCount - 1) * _thetaResolution + thetaCount;
                    indices[indexBufferIndex + 2] = (vCount - 1) * _thetaResolution + (thetaCount + 1) % _thetaResolution;

                    indices[indexBufferIndex + 3] = vertexBufferIndex;
                    indices[indexBufferIndex + 4] = (vCount - 1) * _thetaResolution + (thetaCount + 1) % _thetaResolution;
                    indices[indexBufferIndex + 5] = vCount * _thetaResolution + (thetaCount + 1) % _thetaResolution;
                }
            }
        }

        for (int thetaCount = 0; thetaCount < _thetaResolution; thetaCount++)
        {
            int vertexBufferIndex = _vResolution * _thetaResolution + thetaCount;
            int indexBufferIndex = ((_vResolution - 1) * _thetaResolution + thetaCount) * 6;

            indices[indexBufferIndex] = vertexBufferIndex % verts;
            indices[indexBufferIndex + 1] = ((_vResolution - 1) * _thetaResolution + thetaCount) % verts;
            indices[indexBufferIndex + 2] = ((_vResolution - 1) * _thetaResolution + (thetaCount + 1) % _thetaResolution) % verts;

            indices[indexBufferIndex + 3] = vertexBufferIndex % verts;
            indices[indexBufferIndex + 4] = ((_vResolution - 1) * _thetaResolution + (thetaCount + 1) % _thetaResolution) % verts;
            indices[indexBufferIndex + 5] = (_vResolution * _thetaResolution + (thetaCount + 1) % _thetaResolution) % verts;
        }

        _mesh.Clear();
        _mesh.vertices = vertices;
        _mesh.triangles = indices;
        _mesh.RecalculateNormals();
    }
}
