using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{

    
    [SerializeField]
    public GameObject[] _grasses;

    [SerializeField]
    public GameObject[] _plants;

    void instantiateObject(GameObject[] arr, float minX, float maxX, float minY, float maxY){
        int objectIndex = Random.Range(0,arr.Length);
        GameObject obj = arr[objectIndex];
        float randX = Random.Range(minX, maxX);                                          
        float randY = Random.Range(minY, maxY);

        Instantiate(obj, obj.transform.position + new Vector3(randX,0,randY), Quaternion.identity);

    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i < 100; i++) {
            instantiateObject(_plants, 0.0f, 25.0f, 0.0f, 15.0f);
        }

        for (int i=0; i < 250; i++) {
            instantiateObject(_plants, -40.0f, 25.0f, -40.0f, 25.0f);
        }

        for (int i=0; i < 100; i++) {
            instantiateObject(_plants, -30.0f, -15.0f, -30.0f, 5.0f);
        }

        for (int i=0; i < 3000; i++) {
            instantiateObject(_grasses, -40.0f, 30.0f, -30.0f, 30.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
