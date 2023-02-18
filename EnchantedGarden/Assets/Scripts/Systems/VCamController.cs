using Cinemachine;
using UnityEngine;

public class VCamController : MonoBehaviour
{
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
        _worldEvents.SpiritWaveSpawned += SpiritWaveSpawned;
        _worldEvents.SpiritBanished += SpiritBanished;
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

    private void AddToTargetGroup(Transform t, float? weight = null, float? radius = null)
    {
        _targetGroup.AddMember(t, weight == null ? _defaultTargetWeight : weight.Value, radius == null ? _defaultTargetRadius : radius.Value);
    }

    private void RemoveFromTargetGroup(Transform t)
    {
        _targetGroup.RemoveMember(t);
    }
}
