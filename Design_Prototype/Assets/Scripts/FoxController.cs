using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//tdo to avoid collisions vith trees etc add area taht the fox movein

public class FoxController : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    [SerializeField]
    private GameObject _fox;

    private Animator _animator;

    private float dontChangeAnimationFor = 0;
    private string _previousAnimation = "Fox_Sit_Idle_Break";

    //todo 
    //fix bug with rotation angle

    // Start is called before the first frame update
    void Start()
    {
        _fox.transform.rotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), new Vector3(-1, 0, 0));
        _animator = _fox.GetComponent<Animator>();
    }

    private void ChangeAnim(string name)
    {
        if (dontChangeAnimationFor > 0) return;
        if (_previousAnimation != name)
            _animator.CrossFade(name, 0.5f);
        _previousAnimation = name;

    }

    // Update is called once per frame
    void Update()
    {
        dontChangeAnimationFor -= Time.deltaTime;
        Vector3 playerDelta = _player.transform.position - _fox.transform.position;
        Vector3 target = _player.transform.position +
         (playerDelta.x > 0 ? new Vector3(-15, 0, 15) : new Vector3(15, 0, 15));
        Vector3 delta = target - _fox.transform.position;

        var tooClose = playerDelta.magnitude < 9;
        Quaternion targetRotation = Quaternion.FromToRotation(new Vector3(1, 0, 0), new Vector3(-1, 0, 0));

        if (delta.magnitude > 25 || tooClose)
        {
            delta.Normalize();
            _fox.transform.position += (tooClose ? 20f : 7f) * delta * Time.deltaTime;
            targetRotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), delta);
            if (tooClose)
            {
                ChangeAnim("Fox_Somersault_InPlace");
                dontChangeAnimationFor = 1f;
            }
            else
                ChangeAnim("Fox_Walk_InPlace 0");
        }
        else
        {
            ChangeAnim("Fox_Sit_Idle_Break");
        }

        _fox.transform.eulerAngles = new Vector3(0,_fox.transform.eulerAngles.y,0);
        _fox.transform.rotation = Quaternion.RotateTowards(_fox.transform.rotation, targetRotation, Time.deltaTime * 360f * 0.5f);

    }
}