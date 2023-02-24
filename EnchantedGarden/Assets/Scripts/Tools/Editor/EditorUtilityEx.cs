using UnityEditor;
using UnityEngine;

public static class EditorUtilityEx
{
	//	Transform rotation to Tool local in Editor
	public static Quaternion HandleRotation(this Transform transform)
	{
		return Tools.pivotRotation == PivotRotation.Local ? transform.rotation : Quaternion.identity;
	}
}
