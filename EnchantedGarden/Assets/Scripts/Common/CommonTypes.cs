public static class CommonTypes
{
	public enum Layers
	{
		Default = 0,
		TransparentFX = 1,
		IgnoreRaycast = 2,
		Water = 4,
		UI = 5,
		Firewood = 8,
		TrickPlant = 9,
		Plant = 10,
		Cauldron = 11,
		Herb = 12,
		Spirit = 13,
		PlantPatch = 14,
		Tool = 15,
		Player = 16,
		Forest = 17,
	}

	public static bool IsLayer(this UnityEngine.GameObject go, Layers layer)
	{
		//	Check whether the layer equals the layer enum
		return go.layer.Equals(layer);
	}

	public static class AnimatorActions
	{
		public const string ForwardSpeed = "ForwardSpeed";
		public const string Alert = "Alert";
	}
}
