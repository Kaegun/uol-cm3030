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
		SpiritWall = 18,
		CauldronVat = 19,
	}

	public static class AnimatorActions
	{
		public const string ForwardSpeed = "ForwardSpeed";
		public const string Alert = "Alert";
		public const string PickUp = "PickUp";
		public const string Digging = "Digging";
	}

	public static class Scenes
	{
		public const string Launcher = "Launcher";
		public const string UI = "UI Overlay";
		public const string GameOver = "Game Over";
		public const string LevelFailed = "Level Failed";
		public const string Level0 = "Level 0";
		public const string Level1 = "Level 1";
		public const string Level2 = "Level 2";
		public const string Level3 = "Level 3";
		public const string Level4 = "Level 4";
		public const string Options = "Options";
		public const string Loading = "Loading";
		public const string Victory = "Victory";
		public const string TemplateLevel = "Template";
		public const string Credits = "Credits";
	}

	public enum Levels
	{
		Level0,
		Level1,
		Level2,
		Level3,
		Level4,
		Template
	}

	public static class ScoreValues
	{
		public const int BanishedSpirit = 1000;
		public const int BanishedWall = 1000;

		//	TODO: Define other scoring events
	}

	public static class Constants
	{
		public const float UsesThreshold = 0.5f;
	}
}
