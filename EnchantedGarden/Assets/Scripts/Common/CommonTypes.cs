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

	public static class AnimatorActions
	{
		public const string ForwardSpeed = "ForwardSpeed";
		public const string Alert = "Alert";
	}

	public static class Scenes
	{
		public const string UI = "UI Overlay";
		public const string GameOver = "Game Over";
		public const string Level1 = "ju-beta-1";   //	TODO: Name properly
	}
}
