using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompostBin : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Compost _compost;

    [SerializeField]
    private float _cooldownDuration;

    private float _cooldownProgress;

    [SerializeField]
    private MeshRenderer _compostMesh;

    [SerializeField]
    private Transform _spawnTransform;

    // Start is called before the first frame update
    void Start()
    {
        _cooldownProgress = _cooldownDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (OnCooldown())
        {
            _cooldownProgress += Time.deltaTime;
            if (_cooldownProgress > _cooldownDuration)
            {
                _cooldownProgress = _cooldownDuration;
            }
        }

        // Alter opacity of compost in compost bin to signify cooldown
        var color = _compostMesh.material.color;
        color.a = OnCooldown() ? 0 : 1;
        _compostMesh.material.color = color;
    }

    public bool OnCooldown()
    {
        return _cooldownProgress < _cooldownDuration;
    }

    public bool CanBeInteractedWith()
    {
        return !OnCooldown();
    }

    public void OnPlayerInteract(PlayerController player)
    {
        Instantiate(_compost, _spawnTransform.position, Quaternion.identity);
        _cooldownProgress = 0;
    }
}
