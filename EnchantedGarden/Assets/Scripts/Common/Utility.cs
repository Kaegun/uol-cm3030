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

	public static Vector3 GetVector2DFromAngle(float angle)
	{
		//	TODO: Check whether this is correct, since it feels very 2D
		return new Vector3(Mathf.Cos(AngleToRadians(angle)), 0.0f, Mathf.Sin(AngleToRadians(angle)));
	}

	//	Check whether the layer equals the layer enum
	public static bool IsLayer(this UnityEngine.GameObject go, Layers layer) => go.layer == (int)layer;

	public static float AngleToRadians(float angle) => angle * Mathf.Deg2Rad;
}
