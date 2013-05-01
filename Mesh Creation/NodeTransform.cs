using UnityEngine;
using System.Collections;

public class NodeTransform : MonoBehaviour
{
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		foreach (Transform t in UnityEditor.Selection.transforms)
			if (this.transform == t) return;
		
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere(this.transform.position, 0.20f);
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.transform.position, 0.25f);
	}	
#endif
}
