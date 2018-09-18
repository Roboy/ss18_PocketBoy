using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour {

    public float Time = 5.0f;
    public TextMeshPro Text;


    public IEnumerator Countdown(float time)
    {
        while (time > 0.0f)
        {
            Text.text = time.ToString("f1");
            time -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

    }
}
