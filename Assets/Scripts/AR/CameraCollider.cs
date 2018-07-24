using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision happened.");
        Debug.Log(other.gameObject.name.ToString());
    }
    
}
