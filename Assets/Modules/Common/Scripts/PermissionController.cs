using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

/// <summary>
/// Helper class to request critical permissions. The requested permissions must be included in the AndroidManifest file in the Assets/Plugins folder, otherwise the
/// dialog will not be shown.
/// </summary>
public class PermissionController : MonoBehaviour {

    [SerializeField]
    private bool Record_Audio;

	// Use this for initialization
	void Awake () {
        if (Record_Audio)
        {
            var microphonePermissionName = "android.permission.RECORD_AUDIO";
            AndroidPermissionsManager.RequestPermission(microphonePermissionName);         
        }
    }
}
