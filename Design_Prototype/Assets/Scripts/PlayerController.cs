using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float _moveSpeed = 10f;

    private Animator _animator;

    [SerializeField]
    private GameObject _player;    

    private float dontChangeAnimationFor = 0;

    void Start(){
        _animator = _player.GetComponent<Animator>();
    }

    // Update is called once per frame

    private string _previousAnimation = "Idle03";

    private void ChangeAnim(string name)
    {
        if (dontChangeAnimationFor > 0) return;
        if (_previousAnimation != name)
            _animator.CrossFade(name, 0.5f);
        _previousAnimation = name;

    }

    private void walk(int direction){

            Quaternion targetRotation = Quaternion. Euler(0, direction, 0);
            _player.transform.rotation = Quaternion.RotateTowards(_player.transform.rotation, targetRotation, Time.deltaTime * 360f * 0.5f);
            _player.transform.Translate( _player.transform.rotation*(new Vector3(0,0,1)*_moveSpeed * Time.deltaTime), Space.World);
            ChangeAnim("WalkForward");
    }

    void Update()
    {
        dontChangeAnimationFor -= Time.deltaTime;
        if(Input.GetKey(KeyCode.W)){
            walk(0);
        }
        else if(Input.GetKey(KeyCode.D)){
            walk(90);
        }
        else if(Input.GetKey(KeyCode.A)){
            walk(270);
        }
        else if(Input.GetKey(KeyCode.S)){
            walk(180);
        }
        else if(Input.GetKey(KeyCode.Space)){
            ChangeAnim("Attack04");
            if(dontChangeAnimationFor<=0)
                dontChangeAnimationFor = 0.5f;
        }

        else if(Input.GetKey(KeyCode.Return)){
            ChangeAnim("PickUp");
        }
        else{
            ChangeAnim("idle01");
        }
    }

}
