using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hyperbolic.Math;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject meshObj;
    [SerializeField] int hypIndex;
    [SerializeField] float step;
    [SerializeField] float amount;
    [SerializeField] float amountDiscrete;
    private Vector3[] originalVertices;
    private Vector3[] currentVertices;

    // Start is called before the first frame update
    void Start()
    {
        amountDiscrete = (float)Math.HypotenuseOfFundamentalRegionEndPoint(4, 5)[1].x;
        CentralizeMeshObject(meshObj);
        hypIndex = GetOriginalHypotenuseIndex(meshObj);
        originalVertices = meshObj.GetComponent<MeshFilter>().mesh.vertices;
        currentVertices = originalVertices;
        MeshKleinToMinkowski(meshObj, out currentVertices);
    }

    // Update is called once per frame
    void Update()
    {
        Move(1f, meshObj);
        if(Input.GetKeyDown(KeyCode.X)) MoveDiscrete(meshObj, amountDiscrete * 4, 0f, true);
        if (Input.GetKeyDown(KeyCode.Z)) MoveDiscrete(meshObj, -amountDiscrete * 4, 0f, true);
    }

    public void CentralizeMeshObject(GameObject meshObject)
    {
        meshObject.gameObject.transform.position = new Vector3(0, 0, 0);
    }

    public int GetOriginalHypotenuseIndex(GameObject meshObject)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3 currentHypotenuse = new Vector3(0,0,0);
        int vertexIndex = -1;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldV = LocalToWorld(mesh.vertices[i], meshObject.transform.localScale);
            float currentDistance = Vector3.Distance(currentHypotenuse, new Vector3(0, 0, 0));
            float newDistance = Vector3.Distance(worldV, new Vector3(0, 0, 0));
            bool isFirstQuadrant = worldV.x >= 0 && worldV.y >= 0 && worldV.z >= 0;
            if((newDistance > currentDistance) && isFirstQuadrant)
            {
                currentHypotenuse = worldV;
                vertexIndex = i;
            }
        }
        return vertexIndex;
    }

    public Vector3 LocalToWorld(Vector3 u, Vector3 scale)
    {
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        Vector3 normalized = localToWorld.MultiplyPoint3x4(u);
        Matrix4x4 localNormalized = new Matrix4x4();
        localNormalized.m00 = normalized.x;
        localNormalized.m11 = normalized.y;
        localNormalized.m22 = normalized.z;
        return localNormalized * scale;
    }

    public Vector3 WorldToLocal(Vector3 u, Vector3 scale)
    {
        Vector3 inverseScale = new Vector3(1f/scale.x, 1f/scale.y, 1f/scale.z);
        Matrix4x4 localToWorld = transform.localToWorldMatrix;
        Vector3 normalized = localToWorld.MultiplyPoint3x4(u);
        Matrix4x4 localNormalized = new Matrix4x4();
        localNormalized.m00 = normalized.x;
        localNormalized.m11 = normalized.y;
        localNormalized.m22 = normalized.z;
        return localNormalized * inverseScale;
    }

    /*public Vector3 NormalizedHypotenuse(GameObject meshObject)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3 hypotenuseAtMinkowski = LocalToWorld(vertices[hypIndex], meshObject.transform.localScale);
        VectorD3 doubleHyp = Math.NormalizeToCartesianCoordinates(hypotenuseAtMinkowski);
        VectorD3 invertedHyp = new VectorD3(doubleHyp.x, doubleHyp.z, doubleHyp.y);
        MeshKleinToMinkowski(meshObject, out currentVertices);  
        return Math.MinkowskiToPoincare(invertedHyp).ToUnity();
    }*/

    public void MeshKleinToMinkowski(GameObject meshObject, out Vector3[] currentVertices)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldV = LocalToWorld(mesh.vertices[i], meshObject.transform.localScale);
            Vector3 worldVUp = new Vector3(worldV.x, worldV.z, worldV.y);
            VectorD3 worldVCartesian = Math.NormalizeToCartesianCoordinates(worldVUp);
            VectorD3 result = Math.BeltramiKleinToMinkowski(worldVCartesian); 
            Vector3 res = result.ToUnity();
            vertices[i] = WorldToLocal(new Vector3(res.x, res.z, res.y), meshObject.transform.localScale);
        }
        mesh.vertices = vertices;
        currentVertices = vertices;
    }

    void MoveDiscrete(GameObject meshObject, float x, float y, bool xFirst)
    {
        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        for (var i = 0; i < vertices.Length; i++)
        {
            MatrixD4x4 result = MatrixD4x4.Identity();
            if (x == 0 && y == 0) result = MatrixD4x4.Identity();
            if (x == 0) result = Math.HyperTranslateY(y);
            else if (y == 0) result = Math.HyperTranslateX(x);
            else if (xFirst) result = Math.HyperTranslateX(x) * Math.HyperTranslateY(y);
            else if (!xFirst) result = Math.HyperTranslateY(y) * Math.HyperTranslateX(x);
            VectorD3 currentPos = new VectorD3(vertices[i].x, vertices[i].y, vertices[i].z);
            VectorD3 resultPos = result * currentPos;
            Vector3 resultFloat = resultPos.ToUnity();
            vertices[i] = new Vector3(resultFloat.x, resultFloat.z, resultFloat.y);
        }
        mesh.vertices = vertices;
    }

    private void Move(float step, GameObject meshObject)
    {
        float x = (float)Input.GetAxis("Horizontal") * -step * (float)Time.deltaTime;
        float y = (float)Input.GetAxis("Vertical") * -step * (float)Time.deltaTime;
        amount += x;

        Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            MatrixD4x4 result;
            Vector3 worldV = LocalToWorld(mesh.vertices[i], meshObj.transform.localScale);
            result = Math.RotateZ(x * 2) * Math.HyperTranslateY(y);
            VectorD3 currentPos = new VectorD3(vertices[i].x, vertices[i].y, vertices[i].z);
            VectorD3 resultPos = result * currentPos;
            Vector3 resultFloat = resultPos.ToUnity();
            vertices[i] = new Vector3(resultFloat.x, resultFloat.z, resultFloat.y);
        }
        meshObj.GetComponent<MeshFilter>().mesh.vertices = vertices;
        currentVertices = vertices;
    }
}
