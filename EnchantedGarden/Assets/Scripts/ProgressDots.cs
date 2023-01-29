using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressDots : MonoBehaviour
{
    // Total duration of the loop
    [SerializeField]
    private float _duration;

    [SerializeField]
    private float _bounceDuration;

    [SerializeField]
    private float _bounceHeight;

    [SerializeField]
    private List<GameObject> _dots;

    private void OnEnable()
    {
        foreach (var dot in _dots)
        {
            dot.transform.position = new Vector3(dot.transform.position.x, transform.position.y, dot.transform.position.z);
        }
        InvokeRepeating("StartBouncing", 0.1f, _duration);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void StartBouncing()
    {
        for (int i = 0; i < _dots.Count; i++)
        {
            StartCoroutine(Bounce(_dots[i], i, _bounceDuration, _bounceHeight));
        }
    }

    IEnumerator Bounce(GameObject dot, float num, float duration, float height)
    {
        var startPos = dot.transform.position;
        var endPos = dot.transform.position + Vector3.up * height;
        yield return new WaitForSeconds(num * duration);

        float t = 0;
        while (t < duration)
        {
            dot.transform.position = Vector3.Lerp(startPos, endPos, t / duration);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        dot.transform.position = endPos;

        t = 0;
        while (t < duration)
        {
            dot.transform.position = Vector3.Lerp(endPos, startPos, t / duration);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        dot.transform.position = startPos;
    }
}
