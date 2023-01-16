using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{

    
    [SerializeField]
    public GameObject[] _grasses;

    void instantiateObjects(int range, GameObject[] arr, float minX, float maxX, float minY, float maxY){
        for (int i=0; i < range; i++) {
            int objectIndex = Random.Range(0,arr.Length);
            GameObject obj = arr[objectIndex];
            float randX = Random.Range(minX, maxX);                                          
            float randY = Random.Range(minY, maxY);
            Instantiate(obj, obj.transform.position + new Vector3(randX,0,randY), Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instantiateObjects(100, _grasses, -40.0f, 60.0f, -40.0f, 100.0f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
