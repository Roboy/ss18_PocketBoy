using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour {

    [SerializeField]
    private Transform TeleportEnd;

    [SerializeField]
    private bool InstantTeleport;

    public Vector3 TeleportGoal { get { return TeleportEnd.position; } }

    public bool ShouldTeleportInstantly { get { return InstantTeleport; } }

    public void ShowGoal()
    {
        TeleportEnd.parent.gameObject.SetActive(true);
    }

    public void HideGoal()
    {
        TeleportEnd.parent.gameObject.SetActive(false);
    }
}
