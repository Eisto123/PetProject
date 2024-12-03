using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patting : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pet")
        {
            var pet = other.GetComponent<PetAI>();
            if (pet != null)
            {
                pet.OnPattingStart();
                // DebugLog.Instance.Log("patting Start");
            }
            else
            {
                // DebugLog.Instance.Log("couldn't find");
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pet")
        {
            var pet = other.GetComponent<PetAI>();
            pet.OnPattingEnd();
            // DebugLog.Instance.Log("patting end");
        }
    }
}
