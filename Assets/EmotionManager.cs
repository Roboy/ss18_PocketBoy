using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionManager : MonoBehaviour {

    public List<Sprite> Mouths;
    public float Speed = 0.1f;

    

    private IEnumerator mouthMoving()
    {
        for (int i = 0; i < Mouths.Count; i++)
        {
            this.GetComponent<SpriteRenderer>().sprite = Mouths[i];
            yield return new WaitForSeconds(Speed);
        }

        yield return null;
    }
}
