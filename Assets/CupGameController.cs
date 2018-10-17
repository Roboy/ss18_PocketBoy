using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

public class CupGameController : MonoBehaviour {


    [SerializeField]
    GameObject ShuffleMaster;



	void Start () {

        if (LevelManager.Instance.Roboy != null)
        {
            PositionCups();
        }
        
	}
	


    public void PositionCups()
    {
        Quaternion locRot = ShuffleMaster.transform.localRotation;

        //ShuffleMaster.transform.SetParent(LevelManager.Instance.GetAnchorTransform());
        
        ShuffleMaster.transform.position = LevelManager.Instance.Roboy.transform.TransformPoint(new Vector3(0.6f, 0f, 0f));
        
        

    }
}
