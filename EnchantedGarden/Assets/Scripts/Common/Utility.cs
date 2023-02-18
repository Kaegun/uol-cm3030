using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static CommonTypes;

public static class Utility
{
	public static Vector3 ZeroZ(this Vector3 v)
	{
		return new Vector3(v.x, v.y, 0.0f);
	}

	public static Vector3 ZeroY(this Vector3 v)
	{
		return new Vector3(v.x, 0.0f, v.z);
	}

	public static Vector3 RandomUnitVec3()
	{
		return new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized;

	}

	public static Quaternion RotateTowards(this Quaternion q, Vector3 position, Vector3 target, float t)
	{
		var vectDirection = (target - position).normalized;
		return Quaternion.Slerp(q, Quaternion.LookRotation(vectDirection), t);
	}

	public static Quaternion RandomizeY(this Quaternion q)
	{
		//	TODO: Validate this approach
		var rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
		//return new Quaternion(q.x, rotation.y, q.z, q.w);
		return rotation;
	}

	public static Quaternion ZeroY(this Quaternion q)
	{
		var rotation = q.eulerAngles.ZeroY();
		return Quaternion.Euler(rotation);
	}

	public static Vector3 GetVector2DFromAngle(float angle)
	{
		//	TODO: Check whether this is correct, since it feels very 2D
		return new Vector3(Mathf.Cos(AngleToRadians(angle)), 0.0f, Mathf.Sin(AngleToRadians(angle)));
	}

	//	Check whether the layer equals the layer enum
	public static bool IsLayer(this GameObject go, Layers layer) => go.layer == (int)layer;

	// Check whether the game object is in a selection of layers
	// Is there a better name for this?
	public static bool IsInLayers(this GameObject go, Layers[] layers) => layers.Contains((Layers)go.layer);

	// Convert layers to layer mask
	public static int LayersAsLayerMask(Layers layer) => 1 << (int)layer;

	public static int LayersAsLayerMask(Layers[] layers)
	{
		int layerMask = 1 << (int)layers[0];
		if (layers.Length > 1)
		{
			for (int i = 1; i < layers.Length; i++)
			{
				layerMask |= 1 << (int)layers[i];
			}
		}
		return layerMask;
	}

	public static float AngleToRadians(float angle) => angle * Mathf.Deg2Rad;

	public static string TraceMessage(string message,
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string caller = "",
		[CallerLineNumber] int lineNumber = 0)
	{
		return $"{message} in ({filePath}:[{caller}]:({lineNumber})).";
	}

	public static string AssertNotNullMessage(string name,
		[CallerFilePath] string filePath = "",
		[CallerMemberName] string caller = "",
		[CallerLineNumber] int lineNumber = 0)
	{
		return TraceMessage($"[{name}] must not be NULL.", filePath, caller, lineNumber);
	}
}
