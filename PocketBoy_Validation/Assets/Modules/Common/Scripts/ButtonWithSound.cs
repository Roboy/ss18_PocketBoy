using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Pocketboy.Common
{
    [RequireComponent(typeof(Selectable))]
    public class ButtonWithSound : MonoBehaviour, ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            AudioSourcesManager.Instance.PlaySound("ButtonClick");
        }
    }
}


