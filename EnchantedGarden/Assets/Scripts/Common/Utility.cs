using UnityEngine;

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

	public static Quaternion RotateTowards(this Quaternion q, Vector3 position, Vector3 target, float t)
	{
		var vectDirection = (target - position).normalized;
		return Quaternion.Slerp(q, Quaternion.LookRotation(vectDirection), t);
	}

	public static Vector3 GetVector2DFromAngle(float angle)
	{
		//	TODO: Check whether this is correct, since it feels very 2D
		return new Vector3(Mathf.Cos(AngleToRadians(angle)), 0.0f, Mathf.Sin(AngleToRadians(angle)));
	}

	public static float AngleToRadians(float angle) => angle * Mathf.Deg2Rad;
}
