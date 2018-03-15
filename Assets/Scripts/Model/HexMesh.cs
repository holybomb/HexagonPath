using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    /// <summary>
    /// 六边形外径
    /// </summary>
	public const float outerRadius = 1f;
    /// <summary>
    /// 六边形内径
    /// 内径等于外径*cos(30)
    /// </summary>
	public const float innerRadius = outerRadius * 0.866025404f;

	public static Vector3[] corners = {
        new Vector3(-0.5f*outerRadius,0,0),
        new Vector3(-0.25f*outerRadius,0,0.5f*innerRadius),
        new Vector3(0.25f*outerRadius,0,0.5f*innerRadius),
        new Vector3(0.5f*outerRadius,0,0),
        new Vector3(0.25f*outerRadius,0,-0.5f*innerRadius),
        new Vector3(-0.25f*outerRadius,0,-0.5f*innerRadius),
        new Vector3(-0.5f*outerRadius,0,0),
	};

	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
    List<Color> colors;
//	// Use this for initialization
//	void Awake ()
//	{
//		CreateMesh ();
//	}
	public void CreateMesh()
	{
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        var center = transform.position;
		for (int i = 0; i < 6; i++) {
			AddTriangle(
				center,
				center + corners[i],
				center + corners[i + 1]
			);
		}
		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
        SetColor(Color.white);
	}
    public void SetColor(Color color)
	{
        colors.Clear();
        for (int i = 0; i < hexMesh.vertexCount; i++)
        {
            colors.Add(color);
        }
        hexMesh.colors = colors.ToArray();
	}
	void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
	// Update is called once per frame
	void Update ()
	{
	
	}
}

