using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadDebug : MonoBehaviour {

    public Button b;
    public string Scene;
	// Use this for initialization
	void Start () {
		b.onClick.AddListener(() => SceneManager.LoadScene(Scene));
	}
}
