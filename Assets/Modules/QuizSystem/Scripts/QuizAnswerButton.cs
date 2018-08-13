using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pocketboy.QuizSystem
{
    public class QuizAnswerButton : MonoBehaviour
    {
        [SerializeField]
        private Image ImageComponent;

        [SerializeField]
        private TextMeshProUGUI TextComponent;

        [SerializeField]
        private Button ButtonComponent;

        [SerializeField]
        private Color DefaultButtonColor;

        private Animator m_Animator;

        private bool m_IsImage;

        private void Awake()
        {
            m_Animator = GetComponentInChildren<Animator>();
        }

        public void LoadText(string text)
        {
            Reset();
            TextComponent.text = text;
            ShowText();
        }

        public void LoadImage(Sprite sprite)
        {
            Reset();
            ImageComponent.sprite = sprite;
            ShowImage();
        }

        public void CorrectAnimation()
        {
            StartCoroutine(ButtonBlinkAnimation(ButtonComponent, Color.green));
        }

        public void IncorrectAnimation()
        {
            ImageComponent.color = Color.red;
            m_Animator.SetTrigger("Shake");
        }

        public void Deactivate()
        {
            ButtonComponent.interactable = false;
        }

        private void ShowText()
        {
            ImageComponent.enabled = false;
            TextComponent.enabled = true;
            m_IsImage = false;
        }

        private void ShowImage()
        {
            ImageComponent.enabled = true;
            TextComponent.enabled = false;
            m_IsImage = true;
        }

        private void Reset()
        {
            ButtonComponent.interactable = true;
            ButtonComponent.image.color = DefaultButtonColor;
        }

        private IEnumerator ButtonBlinkAnimation(Button button, Color blinkColor)
        {
            Color defaultColor = button.image.color;
            float currentTime = 0f;
            float stepTime = 0.2f;
            float animTime = 1f;
            bool isColored = false;
            while (currentTime < animTime)
            {
                if (isColored)
                {
                    button.image.color = defaultColor;
                }
                else
                {
                    button.image.color = blinkColor;
                }
                isColored = !isColored;
                currentTime += stepTime;
                yield return new WaitForSeconds(stepTime);
            }
            button.image.color = blinkColor;
        }
    }
}


