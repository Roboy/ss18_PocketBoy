using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pocketboy.MovementProgramming
{

    public class LineOfCode : MonoBehaviour, IPointerClickHandler
    {

        public string operation = "";
        [SerializeField]
        private bool m_Selected = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            m_Selected = !m_Selected;
            CodeManager.Instance.ChangeInstructionColour(gameObject.GetComponent<RectTransform>(), m_Selected);
            CodeManager.Instance.CalculateSelectedLines(gameObject.GetComponent<RectTransform>(), m_Selected);
        }

        public bool IsSelected()
        {
            return m_Selected;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}