using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace Pocketboy.Common {

    public class EmotionMenu : MonoBehaviour, IPointerDownHandler {

        public GameObject DropDownMenu;

        public void OnPointerDown(PointerEventData eventData)
        {
            DropDownMenu.gameObject.SetActive(!DropDownMenu.gameObject.activeInHierarchy);
        }


    }
}
