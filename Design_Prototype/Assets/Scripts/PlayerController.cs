using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float _moveSpeed = 10f;

    private Animator _animator;

    //forward
    private Vector3 _direction = new Vector3(0, 0, -1);

    [SerializeField]
    private GameObject _player;    

    void Start(){
        _animator = _player.GetComponent<Animator>();
    }

    // Update is called once per frame

    private string _previousAnimation = "Idle03";
    private float currentDirection = 0;

    private void walk(int direction){
        if(_previousAnimation != "BattleWalkForward")
                _animator.CrossFade("BattleWalkForward", 0.5f);

            
            if (currentDirection != direction) {
                float step = 800 * Time.deltaTime;
                if (Math.Abs(currentDirection - direction) < 1.5*step || 
                    360-Math.Abs(currentDirection - direction) < 1.5*step) { 
                    currentDirection = direction;
                } else if ((direction - currentDirection) % 360 < 180) {
                    currentDirection += step;
                } else { 
                    currentDirection -= step;
                }
                currentDirection %= 360;
            }

            _player.transform.rotation = Quaternion. Euler(0, currentDirection , 0);
            _player.transform.Translate( _player.transform.rotation*(new Vector3(0,0,1)*_moveSpeed * Time.deltaTime), Space.World);
            _previousAnimation = "BattleWalkForward";
            Debug.Log( _player.transform.rotation*(new Vector3(0,0,1)*_moveSpeed * Time.deltaTime));
    }

    void Update()
    {
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
        else{
            Debug.Log("Idle02");
            if(_previousAnimation != "Idle01")
                _animator.CrossFade("Idle01", 0.5f);
            _previousAnimation = "Idle01";
        }
    }

}
