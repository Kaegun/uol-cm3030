using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class VirtualCameraController : MonoBehaviour
{
	[Header("Events")]
	[SerializeField]
	private ScriptableWorldEventHandler _worldEvents;

	[Header("Target Group")]
	[SerializeField]
	private CinemachineTargetGroup _targetGroup;

	[SerializeField]
	private float _defaultTargetWeight;

	[SerializeField]
	private float _defaultTargetRadius;

	// Start is called before the first frame update
	private void Start()
	{
		Assert.IsNotNull(_worldEvents, Utility.AssertNotNullMessage(nameof(_worldEvents)));
		_worldEvents.SpiritWaveSpawned += SpiritWaveSpawned;
		_worldEvents.SpiritBanished += SpiritBanished;
		_worldEvents.FoxAlert += FoxAlert;
		_worldEvents.FoxAlertEnded += FoxAlertEnded;
	}

	private void SpiritWaveSpawned(object sender, Spirit[] e)
	{
		foreach (Spirit spirit in e)
		{
			AddToTargetGroup(spirit.transform);
		}
	}

	private void SpiritBanished(object sender, Spirit e)
	{
		RemoveFromTargetGroup(e.transform);
	}

	private void FoxAlert(object sender, GameObject e)
	{
		AddToTargetGroup(e.transform);
	}

	private void FoxAlertEnded(object sender, GameObject e)
	{
		RemoveFromTargetGroup(e.transform);
	}

	private void AddToTargetGroup(Transform t, float? weight = null, float? radius = null)
	{
		_targetGroup.AddMember(t, weight == null ? _defaultTargetWeight : weight.Value, radius == null ? _defaultTargetRadius : radius.Value);
	}

	private void RemoveFromTargetGroup(Transform t)
	{
		_targetGroup.RemoveMember(t);
	}
}
