using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class PermissionController : MonoBehaviour {

    [SerializeField]
    private bool Record_Audio;

	// Use this for initialization
	IEnumerator Start () {
        if (Record_Audio)
        {
            var microphonePermissionName = "android.permission.RECORD_AUDIO";
            var req = AndroidPermissionsManager.RequestPermission(microphonePermissionName);
            yield return req.WaitForCompletion();
            Debug.Log("=====================PERMISSION========================");
            Debug.Log("PERMISSION:" + AndroidPermissionsManager.IsPermissionGranted(microphonePermissionName));
            Debug.Log("=====================PERMISSION========================");
            AndroidPermissionsManager.RequestPermission(microphonePermissionName).ThenAction((grantResult) =>
            {
                Debug.Log("=====================PERMISSION========================");
                for (int i = 0; i < grantResult.PermissionNames.Length; i++)
                {
                    Debug.Log(grantResult.PermissionNames[i] + " : " + grantResult.GrantResults[i]);
                }
                Debug.Log("=====================PERMISSION========================");
            });
        }        
    }
}
