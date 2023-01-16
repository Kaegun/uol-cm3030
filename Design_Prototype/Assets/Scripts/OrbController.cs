using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    [SerializeField]
    private GameObject _orb;

    private GameObject _spirit;

    void PossessingPlant(){
            _orb.GetComponent<ParticleSystem>().startColor = new Color(162, 0, 37, 1.0f);
        }

    void DefaultState(){
            _orb.GetComponent<ParticleSystem>().startColor = new Color(0, 162, 37, 1.0f);
    }        

    void Update()
    {
        
        /*if(_spirit._spiritState == SpiritState.Possessing){
            PossessingPlant()
        }
        ...

        */

    }  

    void Start(){
        DefaultState();
        //_spirit = Game_spirit.GetComponent<Spirit>();
    }

}
