using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    /// <summary>
    /// The text inside the string field InstructionText will be read out load by roboy and shown next to him in a speech bubble."
    /// </summary>
    public class Instruction : MonoBehaviour
    {
        [SerializeField, TextArea, Tooltip("This text will be shown and read out loud by Roboy when pressed on the Help button.")]
        private string InstructionText;

        private void Awake()
        {
            if (string.IsNullOrEmpty(InstructionText))
                return;

            InstructionManager.Instance.PushInstruction(InstructionText);
        }
    }
}

