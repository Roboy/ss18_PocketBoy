using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Pocketboy.ModelCategorization
{
    public class CategorizationModel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private GameObject NameContainer;

        [SerializeField]
        private TextMeshProUGUI Name;

        public void OnPointerDown(PointerEventData eventData)
        {
            ShowName();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            HideName();
        }

        public void ShowName()
        {
            NameContainer.gameObject.SetActive(true);
        }

        public void HideName()
        {
            NameContainer.gameObject.SetActive(false);
        }

        public void SetName(string name)
        {
            Name.text = name;
        }
    }
}
