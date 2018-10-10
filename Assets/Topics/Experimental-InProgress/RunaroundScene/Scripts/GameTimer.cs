using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameTimer : MonoBehaviour {

    public float Time = 5.0f;
    public TextMeshPro Text;
    public Button StartButton;

    public IEnumerator Countdown(float time)
    {
        StartButton.enabled = false;
        while (time > 0.0f)
        {
            Text.text = time.ToString("f1");
            time -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        StartButton.enabled = true;

    }
}
