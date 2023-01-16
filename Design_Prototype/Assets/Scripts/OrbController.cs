using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    [SerializeField]
    private GameObject _spirit;

    void ChangeColorOfElements(int r, int g, int b, float alpha){
        var _spiritElements = GetComponentsInChildren<ParticleSystem>();
        foreach(var element in _spiritElements){
            element.startColor = new Color( r,g,b,alpha);
        }
    }

    //green
    void PossessingPlant(){
        ChangeColorOfElements(0, 255, 0, 0.5f);
    }

    //red
    void Trapped(){
        ChangeColorOfElements(162, 0, 37, 0.5f);
        //_orb.GetComponent<ParticleSystem>().startColor = new Color(162, 0, 37, 1.0f);
    }

    //blue or white
    void Repelled(){
        ChangeColorOfElements(10, 10, 255, 1f);
        //_orb.GetComponent<ParticleSystem>().startColor = new Color(186, 85, 211, 1.0f);
    }
    
    //yellow
    void DefaultState(){
        ChangeColorOfElements(255, 255, 0, 0.5f);
    }    

    void Update()
    {

        //todo
        /*if(_spirit._spiritState == SpiritState.Possessing){
            PossessingPlant()
        }
        ...

        */

         if(Input.GetKey(KeyCode.W)){
            PossessingPlant();
        }
        else if(Input.GetKey(KeyCode.D)){
            Trapped();
        }
        else if(Input.GetKey(KeyCode.A)){
            Repelled();
        }
        else if(Input.GetKey(KeyCode.S)){
            DefaultState();
        }
    
    }

    void Start(){
        DefaultState();
        //_spirit = Game_spirit.GetComponent<Spirit>();
    }

}
