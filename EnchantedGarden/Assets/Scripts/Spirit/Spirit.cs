using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spirit : MonoBehaviour, IInteractable
{
    enum SpiritState
    {
        Spawning,
        Searching,
        StartingPossession,
        Possessing,
        Trapped,
    }

    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _turnSpeed = 10f;

    [SerializeField]
    private GameObject _bodyObj;

    [Header("Audio")]
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private ScriptableAudioClip _spawnAudio;
    [SerializeField]
    private ScriptableAudioClip _beginPossessingAudio;
    [SerializeField]
    private ScriptableAudioClip _completePossessionAudio;
    [SerializeField]
    private ScriptableAudioClip _bansihAudio;

    private Vector3 _moveDirection;
    //private float _moveTime = 0;
    private float _moveSpeedMultiplier = 1f;
    private float _possessionRateMultiplier = 1f;

    public float PossessionRateMultiplier => _possessionRateMultiplier;

    private SpiritState _spiritState;
    private IPossessable _possessedPossessable;
    private Vector3 _spawnPos;
    private IPossessable _targetPossessable = null;

    public bool CanBeBanished => _spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession;

    public void SetPropsOnSpawn(float moveSpeedMultiplier, float possessionRateMultiplier)
    {
        _moveSpeedMultiplier = moveSpeedMultiplier;
        _possessionRateMultiplier = possessionRateMultiplier;
    }

    public void Banish()
    {
        if (_possessedPossessable != null)
        {
            _possessedPossessable.OnDispossess();
        }
        // Spirit is destroyed before audio plays
        // TODO: Fix audio not playing due to spirit object being destroyed
        AudioController.PlayAudioDetached(_bansihAudio, transform.position);
        GameManager.Instance.ScorePoints(50);
        Destroy(gameObject);
    }

    //  Start is called before the first frame update
    private void Start()
    {
        _spawnPos = transform.position;
        _spiritState = SpiritState.Spawning;
        AudioController.PlayAudio(_audioSource, _spawnAudio);
        StartCoroutine(SpawnCoroutine());
    }

    //  Update is called once per frame
    private void Update()
    {
        switch (_spiritState)
        {
            case SpiritState.StartingPossession:
                _possessedPossessable.WhileCompletingPossession(this);
                if (_possessedPossessable.PossessionCompleted)
                {
                    _possessedPossessable.OnPossessionCompleted(this);
                    _spiritState = SpiritState.Possessing;
                    AudioController.PlayAudio(_audioSource, _completePossessionAudio);
                }

                break;
            case SpiritState.Possessing:
                if (_possessedPossessable is Plant plant)
                {
                    plant.transform.position = transform.position;
                    _moveDirection = (_spawnPos - transform.position).normalized;
                    Move(0.5f);
                }
                break;
            default:
                break;
        }
    }

    private bool IsPossessingPlant => (_spiritState == SpiritState.Possessing || _spiritState == SpiritState.StartingPossession) && _possessedPossessable is Plant;

    public Transform Transform => transform;

    public GameObject GameObject => gameObject;

    private void StealPossessedPlant()
    {
        GameManager.Instance.ScorePoints(-100);
        Destroy(_possessedPossessable.GameObject);
        Destroy(gameObject);
    }

    private void Move(float speedFactor = 1.0f)
    {
        _moveDirection.y = 0;
        _moveDirection = _moveDirection.normalized;
        transform.rotation = transform.rotation.RotateTowards(transform.position, transform.position + _moveDirection, _turnSpeed * Time.deltaTime);
        transform.position += _moveSpeed * Time.deltaTime * _moveDirection * _moveSpeedMultiplier * speedFactor;

    }

    private void DeactivateBody()
    {
        _bodyObj.SetActive(false);
    }

    // Not sure there is any instance we'll need this but will keep it for now
    private void ActivateBody()
    {
        _bodyObj.SetActive(true);
    }

    private IEnumerator SpawnCoroutine()
    {
        // Set move direction towards origin with random roation applied
        _moveDirection = Quaternion.Euler(new Vector3(0, Random.Range(-60, 60), 0)) * transform.position.normalized * -1;
        float t = 0;
        while (t < 2f)
        {
            Move(0.5f);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        _spiritState = SpiritState.Searching;
        StartCoroutine(SearchCoroutine());
    }

    private IEnumerator SearchCoroutine()
    {
        float searchDuration = 0;
        while (_spiritState == SpiritState.Searching)
        {
            // Capture elapsed time at start of loop execution
            float time = Time.time;
            // Add possessable layers based on how long the spirit has been searching for something to possess
            int layerMask = Utility.LayersAsLayerMask(CommonTypes.Layers.Plant);
            if (searchDuration > 10)
            {
                layerMask = Utility.LayersAsLayerMask(new[] { CommonTypes.Layers.Plant, CommonTypes.Layers.SpiritWall });
            }
            // If target is not valid carry out searching behaviour to find vaild possession target
            if (_targetPossessable == null || !_targetPossessable.CanBePossessed)
            {
                // For 2 seconds rotate spirit and use raycast to search for valid possessable
                // Spirits target closest possessable found
                float t = 0;
                HashSet<IPossessable> hitPossessables = new HashSet<IPossessable>();
                while (t < 2)
                {
                    if (Physics.Raycast(transform.position, transform.rotation * Vector3.forward, out RaycastHit hit, 100f, layerMask, QueryTriggerInteraction.Collide)
                        && hit.collider.TryGetComponent(out IPossessable hitPossessable)
                        && hitPossessable.CanBePossessed)
                    {
                        Debug.DrawRay(transform.position, (transform.rotation * Vector3.forward).normalized * hit.distance, Color.yellow);
                        hitPossessables.Add(hitPossessable);
                    }
                    float bounce = Mathf.PingPong(Time.time, 1.6f) - 0.8f;
                    transform.rotation = transform.rotation.RotateTowards(transform.position,
                        transform.position + transform.rotation * Vector3.right * bounce,
                        Time.deltaTime * _turnSpeed / 8);
                    t += Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }
                _targetPossessable = hitPossessables.OrderBy(p => Vector3.Distance(p.Transform.position, transform.position)).FirstOrDefault();
                // For three seconds if vaild possessable has not been found, move towards origin
                _moveDirection = Quaternion.Euler(new Vector3(0, Random.Range(-60, 60), 0)) * transform.position.normalized * -1;
                t = 0;
                while (t < 3 && (_targetPossessable == null || !_targetPossessable.CanBePossessed))
                {
                    Move();
                    t += Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                _moveDirection = (_targetPossessable.Transform.position - transform.position).normalized;
                Move();
            }
            // Use captured time to increment search duration
            // There is probably a better way to do this
            searchDuration += Time.time - time;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //	TODO: Handle all layers for possession
        if (other.gameObject.IsInLayers(new[] { CommonTypes.Layers.Plant, CommonTypes.Layers.SpiritWall })
            && other.TryGetComponent(out IPossessable possessable)
            && possessable.CanBePossessed
            && possessable == _targetPossessable
            && _spiritState == SpiritState.Searching)
        {
            // Stop searching coroutine
            StopAllCoroutines();
            _possessedPossessable = possessable;
            _possessedPossessable.OnPossessionStarted(this);
            transform.position = new Vector3(_possessedPossessable.Transform.position.x, transform.position.y, _possessedPossessable.Transform.position.z);
            _spiritState = SpiritState.StartingPossession;
            AudioController.PlayAudio(_audioSource, _beginPossessingAudio);

            if (possessable is SpiritWall)
            {
                DeactivateBody();
            }
        }
        else if (other.gameObject.IsLayer(CommonTypes.Layers.Forest) && IsPossessingPlant)
        {
            //	handle - we're off the edge of the map Jim
            StealPossessedPlant();
        }

        // TODO: Rework trick plants
        //else if (other.gameObject.IsLayer(CommonTypes.Layers.TrickPlant)
        //    && other.TryGetComponent(out TrickPlant trickPlant)
        //    && trickPlant.CanTrapSpirit)
        //{
        //    //  handle trick plants
        //    trickPlant.TrapSpirit(this);
        //    DeactivateBody();
        //    _spiritState = SpiritState.Trapped;
        //    transform.position = trickPlant.transform.position;
        //}

    }

    public bool CanInteractWith(IInteractor interactor)
    {
        switch (interactor)
        {
            case Flask _:
                return CanBeBanished && interactor.CanInteractWith(this);
            default:
                return false;
        }
    }

    public void OnInteractWith(IInteractor interactor)
    {
        switch (interactor)
        {
            case Flask _:
                Banish();
                break;
            default:
                break;
        }
    }

    public bool DestroyOnInteract(IInteractor interactor)
    {
        switch (interactor)
        {
            case Flask _:
                return true;
            default:
                return false;
        }
    }
}
