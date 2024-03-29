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
		public const string Alert = "Alert";
		public const string Digging = "Digging";
		public const string Drop = "Drop";
		public const string ForwardSpeed = "ForwardSpeed";
		public const string PickUp = "PickUp";
	}

	public static class Scenes
	{
		public const string Launcher = "Launcher";
		public const string LauncherUI = "Launcher UI";
		public const string UI = "UI Overlay";
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
		public const string Story = "Story";
	}

	public enum Levels
	{
		Launcher,
		Level0,
		Level1,
		Level2,
		Level3,
		Level4,
		Template        
    }

	public static class ScoreValues
	{
		public const int BanishedSpiritBeforeFinishedPossession = 1000;
		public const int BanishedSpiritAfterFinishedPossession = 500;
		public const int ReplantedPlant = 500;
		public const int BanishedWall = 500;
	}

	public static class Constants
	{
		public const float UsesThreshold = 0.5f;
	}

	public enum VolumeChannel
    {
		Master,
		Music,
		SFX
    }
	public static class Volume
    {
		public const string MasterVolume = "Master Volume";
		public const string MusicVolume = "Music Volume";
		public const string SFXVolume = "SFX Volume";
    }
}
