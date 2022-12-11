using UnityEngine;

public static class Utility
{
	//	From: https://answers.unity.com/questions/446540/calculating-the-angle-of-a-vector2-from-zero.html
	public static float CalculateAngle(this Vector2 vector2)
	{
		return 360.0f - (Mathf.Atan2(-vector2.x, vector2.y) * Mathf.Rad2Deg);
	}
}
