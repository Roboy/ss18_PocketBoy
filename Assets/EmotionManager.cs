using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pocketboy.Common
{
    public class EmotionManager : Singleton<EmotionManager>
    {

        public List<Sprite> Mouths;
        public float Speed = 0.1f;

        public IEnumerator mouthMoving()
        {
            SpriteRenderer s = this.GetComponent<SpriteRenderer>();
            while (true)
            {
                
                for (int i = 0; i < Mouths.Count; i++)
                {
                    s.sprite = Mouths[i];
                    yield return new WaitForSeconds(Speed);
                }
            }
        }

        public void ResetMouth()
        {
            this.GetComponent<SpriteRenderer>().sprite = Mouths[0];
        }
    }

}
