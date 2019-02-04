using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{

    [CreateAssetMenu(fileName = "SoundClip", menuName = "Pocketboy/Audio/SoundClip")]
    public class SoundClip : ScriptableObject
    {
        public string ID;
        public AudioClip AudioFile;
    }

}