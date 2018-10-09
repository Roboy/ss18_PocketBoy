using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Pocketboy.Runaround
{
    public class NavigateQuestions : MonoBehaviour, IPointerClickHandler
    {

        public void OnPointerClick(PointerEventData eventData)
        {
            QuestionManager.Instance.NavigateQuestion(gameObject.tag);
        }
    }
}
