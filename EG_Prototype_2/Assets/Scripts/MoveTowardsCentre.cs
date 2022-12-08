using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsCentre : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    private Vector3 _direction;

    // Start is called before the first frame update
    void Start()
    {
        _direction = transform.position.normalized * -1;
        _direction.y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _moveSpeed * Time.deltaTime * _direction;
    }
}
