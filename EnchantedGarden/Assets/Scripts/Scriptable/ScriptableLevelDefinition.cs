using System.Runtime.Remoting.Messaging;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelDefinition", menuName = "Scriptable/LevelDefinition")]
public class ScriptableLevelDefinition : ScriptableObject
{
	[SerializeField]
	private CommonTypes.Levels _level;
	public CommonTypes.Levels Level => _level;

	[Header("Level")]
	//	The duration of a level
	[SerializeField]
	[Range(15f, 600f)]
	private float _levelDuration = 90f;
	public float LevelDuration => _levelDuration;

	[SerializeField]
	private bool _infiniteLevel = false;
	public bool InfiniteLevel => _infiniteLevel;

	[Header("Spirits and Possession")]
	//	The definition of the spirit spawning frequency
	[SerializeField]
	private SpiritWave[] _waves;
	public SpiritWave[] Waves => _waves;

	//  Factor by how much easier it is to possess the plant
	[SerializeField]
	private float _unplantedFactor = 5f;
	public float UnplantedFactor => _unplantedFactor;

	//  Threshold for amount of time plant must be BeingPossessed state before becoming Possessed
	[SerializeField]
	private float _possessionThreshold = 10f;
	public float PossessionThreshold => _possessionThreshold;

	[Header("Plants")]
	[SerializeField]
	private int _startNumberOfPlants;
	public int StartNumberOfPlants => _startNumberOfPlants;

	public int CurrentNumberOfPlants { get; set; }

	//  Threshold for amount of time taken to replant a plant
	[SerializeField]
	private float _replantingThreshold = 2f;
	public float ReplantingThreshold => _replantingThreshold;

	//	Settings pertaining to the cauldron
	[SerializeField]
	private ScriptableBackgroundMusic _backgroundMusic;
	public ScriptableBackgroundMusic BackgroundMusic { get { return _backgroundMusic; } }

	[SerializeField]
	private CauldronSettings _cauldronSettings;
	public CauldronSettings CauldronSettings => _cauldronSettings;
}
