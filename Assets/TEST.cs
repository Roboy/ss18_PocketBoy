using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TEST : MonoBehaviour {

    public TMP_Text Text;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(Text.textInfo.pageInfo[0].lastCharacterIndex);	
	}
}
