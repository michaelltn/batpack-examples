using UnityEngine;
using System.Collections;

public static class PlaneCreator
{
	// http://games.deozaan.com/unity/MeshTutorial.pdf
	public static GameObject CreateNewPlane(float width, float height, string name = "BackgroundPlane")
	{
		GameObject newPlane = new GameObject(name);
		Mesh newMesh = new Mesh();
		newPlane.AddComponent(typeof(MeshFilter));
		newPlane.AddComponent(typeof(MeshRenderer));
		
		Vector3 bottomLeft = new Vector3(-width/2, -height/2, 0);
		Vector3 bottomRight = new Vector3(width/2, -height/2, 0);
		Vector3 topLeft = new Vector3(-width/2, height/2, 0);
		Vector3 topRight = new Vector3(width/2, height/2, 0);
		
		newMesh.vertices = new Vector3[4] {bottomLeft, bottomRight, topLeft, topRight};
		
		Vector2[] uvs = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		newMesh.uv = uvs;
		
		newMesh.triangles = new int[6] {1, 0, 2, 2, 3, 1};
		newMesh.RecalculateNormals();
		
		newPlane.GetComponent<MeshFilter>().mesh = newMesh;
		
		return newPlane;
	}

}
