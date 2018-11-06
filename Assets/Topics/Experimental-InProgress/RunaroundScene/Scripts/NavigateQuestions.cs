﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Pocketboy.Runaround
{
    public class NavigateQuestions : MonoBehaviour, IPointerClickHandler
    {

        public void OnPointerClick(PointerEventData eventData)
        {
            if (gameObject.GetComponent<Button>().enabled)
            {
                QuestionManager.Instance.NavigateQuestion(gameObject.tag);
            }
        }
    }
}
