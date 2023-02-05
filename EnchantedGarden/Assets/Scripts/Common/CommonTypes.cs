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
	}

	public static bool IsLayer(this int a, Layers b)
	{
		return a.Equals(b);
	}

	public static class AnimatorVariable
	{
		public const string ForwardSpeed = "ForwardSpeed";
	}
}
