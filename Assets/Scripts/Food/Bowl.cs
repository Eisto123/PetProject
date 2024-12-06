using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Bowl : MonoBehaviour
{
    public GameObject meat;
    public GameObject meatPrefab;
    void Update()
    {
        if(meat == null){
            meat = Instantiate(meatPrefab,this.transform.position+Vector3.up*0.2f,quaternion.identity,this.transform);
        }
    }
}
