using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelChunk : MonoBehaviour
{
	public Material chunkMaterial;
	public NodeTransform textureOrigin;
	public float textureWidth = 1f;
	public float textureHeight = 1f;
	
	public bool addMeshCollider = true;
	public PhysicMaterial physicsMaterial;
	public float outlineThickness = 0.15f;
	public float drawDepth = 0.5f;
	public float outlineDepth = 0.25f;
	public Color outlineColor = Color.white;
	private LineRenderer outlineRenderer;
	private TrackCreator trackCreator;
	
	public bool flattenNodesToLocalZPlane = true;
	public bool deactivateNodesOnStart = true;
	public bool addNodesFromChildren = false;
	public NodeTransform[] nodes;
	
	void Awake ()
	{
		if (this.isValid)
		{
			if (flattenNodesToLocalZPlane)
				foreach (NodeTransform n in nodes)
					n.transform.localPosition = new Vector3(n.transform.localPosition.x, n.transform.localPosition.y, 0);
			
			Mesh msh = new Mesh();
			msh.vertices = this.getVertices();
			msh.triangles = this.getIndices();
			msh.uv = this.getUV();
			msh.RecalculateNormals();
			msh.RecalculateBounds();
			
			if (chunkMaterial != null)
			{
				this.gameObject.AddComponent(typeof(MeshRenderer));
				this.renderer.material = chunkMaterial;
			}
			
			MeshFilter filter = this.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
			filter.mesh = msh;
			
			if (addMeshCollider)
			{
				//(this.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider).sharedMesh = msh;
				(this.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider).sharedMesh = MeshExtrusion.ExtrudeZ(msh, 1f);
				if (physicsMaterial != null)
				{
					this.gameObject.collider.material = physicsMaterial;
				}
				
				if (outlineThickness > 0 && nodes.Length > 1)
				{
					GameObject outlineGO = new GameObject("outline");
					outlineGO.transform.parent = this.transform;
					outlineGO.transform.localPosition = Vector3.zero;
					trackCreator = outlineGO.AddComponent(typeof(TrackCreator)) as TrackCreator;
					trackCreator.nodes = new NodeTransform[this.nodes.Length + 1];
					for (int i = 0; i < nodes.Length + 1; i++)
					{
						trackCreator.nodes[i] = this.nodes[i % nodes.Length];
					}
					trackCreator.drawDepth = outlineDepth;
					trackCreator.addMeshCollider = false;
					trackCreator.chunkMaterial = new Material(Resources.Load("FlatMat") as Material);
					trackCreator.chunkMaterial.SetColor("_TintColor", outlineColor);
					trackCreator.trackWidth = 0.1f;
					trackCreator.uvBottomLeft = Vector2.zero;
					trackCreator.uvSize = new Vector2(1f, 1f);
					trackCreator.stretchPieces = true;
				}
				
//				if (outlineThickness > 0 && nodes.Length > 1)
//				{
//					outlineRenderer = this.gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
//					outlineRenderer.SetVertexCount(nodes.Length + 2);
//					for (int i = 0; i < nodes.Length + 2; i++)
//					{
//						outlineRenderer.SetPosition(i, new Vector3(
//							nodes[i % nodes.Length].transform.localPosition.x,
//							nodes[i % nodes.Length].transform.localPosition.y,
//							outlineDepth)
//							);
//					}
//					outlineRenderer.SetWidth(outlineThickness, outlineThickness);
//					outlineRenderer.castShadows = false;
//					outlineRenderer.receiveShadows = false;
//					outlineRenderer.useWorldSpace = false;
//					outlineRenderer.material = Resources.Load("FlatMat") as Material;
//				}
			}
			
			if (deactivateNodesOnStart)
				foreach (NodeTransform n in nodes)
					n.gameObject.active = false;
		}
	}
	
	public bool isValid
	{
		get
		{
			for (int n = 0; n < nodes.Length; n++)
			{
				if (nodes[n] == null)
					return false;
			}
			return true;
		}
	}
	
	private int[] getIndices()
	{
		Vector2[] vectors = new Vector2[nodes.Length];
		for (int n = 0; n < nodes.Length; n++)
		{
			vectors[n] = new Vector2(
				nodes[n].transform.localPosition.x,
				nodes[n].transform.localPosition.y);
		}
		
		Triangulator triangulator = new Triangulator(vectors);
		int[] indices = triangulator.Triangulate();
		
		return indices;
	}
	
	public Vector3[] getVertices()
	{
		Vector3[] vectors = new Vector3[nodes.Length];
		for (int n = 0; n < nodes.Length; n++)
		{
			vectors[n] = new Vector3(
				nodes[n].transform.localPosition.x,
				nodes[n].transform.localPosition.y,
				drawDepth);
				//nodes[n].transform.localPosition.z);
		}
		return vectors;
	}
	
	Vector3 uvOrigin;
	public Vector2[] getUV()
	{
		if (textureOrigin != null)
			uvOrigin = textureOrigin.transform.localPosition;
		else
			uvOrigin = Vector3.zero;
		
		Vector2[] vectors = new Vector2[nodes.Length];
		for (int n = 0; n < nodes.Length; n++)
		{
			vectors[n] = new Vector2(
				(nodes[n].transform.localPosition.x - uvOrigin.x) / textureWidth,
				(nodes[n].transform.localPosition.y - uvOrigin.y) / textureHeight);
		}
		return vectors;
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (addNodesFromChildren)
		{
			nodes = this.gameObject.GetComponentsInChildren<NodeTransform>();
			addNodesFromChildren = false;
		}
		
		foreach (Transform t in UnityEditor.Selection.transforms)
			if (this.transform == t) return;
		
		if (nodes != null && nodes.Length > 0)
		{
			Gizmos.color = Color.magenta;
			for (int n = 0; n < nodes.Length; n++)
			{
				int o = n==0 ? nodes.Length - 1 : n-1;
				if (nodes[n] && nodes[o])
				{
					Gizmos.DrawLine(nodes[n].transform.position, nodes[o].transform.position);
				}
			}
		}
	}
	
	void OnDrawGizmosSelected()
	{
		if (nodes != null && nodes.Length > 0)
		{
			Gizmos.color = Color.green;
			for (int n = 0; n < nodes.Length; n++)
			{
				int o = n==0 ? nodes.Length - 1 : n-1;
				if (nodes[n] && nodes[o])
				{
					Gizmos.DrawLine(nodes[n].transform.position, nodes[o].transform.position);
				}
			}
		}
	}
#endif
}
