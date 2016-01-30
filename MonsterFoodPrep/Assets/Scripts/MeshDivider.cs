using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//SORRY ABOUT THE MESS!
//Based very roughly on Randy Gaul's Sutherland-Hodgman implementation.

public class MeshDivider : MonoBehaviour
{
    private class Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }

    public bool test = true;

    List<Vector3> negativeVertices = new List<Vector3>();
    List<int> negativeTriangles = new List<int>();
    List<Vector3> positiveVertices = new List<Vector3>();
    List<int> positiveTriangles = new List<int>();

    void Awake()
    {
        if (test)
        {
            test = false;
            Divide(new Plane(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.0f, -0.25f, 0.0f)));
        }
    }

    /*
    IEnumerator Start()
    {
    
        //GetComponent<Rigidbody>().detectCollisions = false;
        yield return new WaitForEndOfFrame();
        //GetComponent<Rigidbody>().detectCollisions = true; ;
    }
    */

    public void Divide2(Plane plane)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 a = mesh.vertices[mesh.triangles[i]];
            Vector3 b = mesh.vertices[mesh.triangles[i + 1]];
            Vector3 c = mesh.vertices[mesh.triangles[i + 2]];

            if(!plane.SameSide(a, b) || !plane.SameSide(b, c))
            {
           
            }
        }
    }

    bool TrianglePlaneIntersection(Triangle triangle, Plane plane)
    {
        Ray ray;
        float distance;

        ray = new Ray(triangle.a, triangle.b - triangle.a);
        if (plane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
        }

        return false;
    }

    public void Divide(Plane plane)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 a = mesh.vertices[mesh.triangles[i    ]];
            Vector3 b = mesh.vertices[mesh.triangles[i + 1]];
            Vector3 c = mesh.vertices[mesh.triangles[i + 2]];
            float da = plane.GetDistanceToPoint(a);
            float db = plane.GetDistanceToPoint(b);
            float dc = plane.GetDistanceToPoint(c);
            if (da * db < 0.0f)
            {
                Triangle[] tris;
                DivideTriangle(a, b, c, da, db, dc, out tris);
                foreach (Triangle tri in tris)
                {     
                    Face(tri, plane);
                }
            }
            else if (da * dc < 0.0f)
            {
                Triangle[] tris;
                DivideTriangle(c, a, b, dc, da, db, out tris);
                foreach (Triangle tri in tris)
                {
                    Face(tri, plane);
                }
            }
            else if (db * dc < 0.0f)
            {
                Triangle[] tris;
                DivideTriangle(b, c, a, db, dc, da, out tris);
                foreach (Triangle tri in tris)
                {
                    Face(tri, plane);
                }
            }
            else
            {
                Face(new Triangle(a, b, c), plane);
            }
        }

        Mesh positiveMesh = new Mesh();
        positiveMesh.vertices = positiveVertices.ToArray();
        positiveMesh.triangles = positiveTriangles.ToArray();
        positiveMesh.RecalculateBounds();
        positiveMesh.RecalculateNormals();

        Mesh negativeMesh = new Mesh();
        negativeMesh.vertices = negativeVertices.ToArray();
        negativeMesh.triangles = negativeTriangles.ToArray();
        negativeMesh.RecalculateBounds();
        negativeMesh.RecalculateNormals();

        positiveVertices = null;
        positiveTriangles = null;
        negativeVertices = null;
        negativeTriangles = null;

        GameObject positive = Instantiate(gameObject);
        meshFilter = positive.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = positiveMesh;
        // meshCollider = positive.GetComponent<MeshCollider>();

        //meshCollider.sharedMesh = positiveMesh;

        GameObject negative = Instantiate(gameObject);
        meshFilter = negative.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = negativeMesh;
        // meshCollider = negative.GetComponent<MeshCollider>();

        //meshCollider.sharedMesh = negativeMesh;
     

        Destroy(gameObject);
    }

    private void Face(Triangle triangle, Plane plane)
    {
        if (plane.GetSide((triangle.a + triangle.b + triangle.c) / 3))
        {
            List<Vector3> vertices = positiveVertices;
            List<int> triangles = positiveTriangles;
            triangles.Add(vertices.Count);
            triangles.Add(vertices.Count + 1);
            triangles.Add(vertices.Count + 2);
            vertices.Add(triangle.a);
            vertices.Add(triangle.b);
            vertices.Add(triangle.c);
        }
        else
        {
            List<Vector3> vertices = negativeVertices;
            List<int> triangles = negativeTriangles;
            triangles.Add(vertices.Count);
            triangles.Add(vertices.Count + 1);
            triangles.Add(vertices.Count + 2);
            vertices.Add(triangle.a);
            vertices.Add(triangle.b);
            vertices.Add(triangle.c);
        }
    }

    static void DivideTriangle(Vector3 a, Vector3 b, Vector3 c, float da, float db, float dc, out Triangle[] triangles)
    {
        List<Triangle> output = new List<Triangle>();
        Vector3 ab = a + (da / (da - db)) * (b - a);
        if(da < 0.0f)
        {
            if(dc < 0.0f)
            {
                Vector3 bc = b + (db / (db - dc)) * (c - b);
                output.Add(new Triangle(b, bc, ab));
                output.Add(new Triangle(bc, c, a));
                output.Add(new Triangle(ab, bc, a));
            }
            else
            {
                Vector3 ac = a + (da / (da - dc)) * (c - a);
                output.Add(new Triangle(a, ab, ac));
                output.Add(new Triangle(ab, b, c));
                output.Add(new Triangle(ac, ab, c));     
            }
        }
        else
        {
            if(dc < 0.0f)
            {
                Vector3 ac = a + (da / (da - dc)) * (c - a);
                output.Add(new Triangle(a, ab, ac));
                output.Add(new Triangle(ac, ab, b));
                output.Add(new Triangle(b, c, ac));
            }
            else
            {
                Vector3 bc = b + (db / (db - dc)) * (c - b);
                output.Add(new Triangle(b, bc, ab));
                output.Add(new Triangle(a, ab, bc));
                output.Add(new Triangle(c, a, bc));
            }
        }
        triangles = output.ToArray();
    }
}
