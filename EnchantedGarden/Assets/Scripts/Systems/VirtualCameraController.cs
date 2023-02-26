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
		_worldEvents.SpiritSpawned += SpiritSpawned;
		_worldEvents.SpiritBanished += SpiritBanished;
		_worldEvents.FoxAlert += FoxAlert;
		_worldEvents.FoxAlertEnded += FoxAlertEnded;
	}

    private void OnDestroy()
    {
		_worldEvents.SpiritSpawned -= SpiritSpawned;
		_worldEvents.SpiritBanished -= SpiritBanished;
		_worldEvents.FoxAlert -= FoxAlert;
		_worldEvents.FoxAlertEnded -= FoxAlertEnded;
	}

    private void SpiritSpawned(object sender, Spirit e)
	{		
		AddToTargetGroup(e.transform);		
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
		if (_targetGroup.FindMember(t) == -1)
        {
			_targetGroup.AddMember(t, weight == null ? _defaultTargetWeight : weight.Value, radius == null ? _defaultTargetRadius : radius.Value);
		}		
	}

	private void RemoveFromTargetGroup(Transform t)
	{
		_targetGroup.RemoveMember(t);
	}
}
