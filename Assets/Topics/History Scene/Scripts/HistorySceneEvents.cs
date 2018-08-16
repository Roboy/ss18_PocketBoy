using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.HistoryScene
{
    public class EventManager
    {
        public delegate void OnNextContentDelegate();
        public static event OnNextContentDelegate NextContentDelegate;

        public delegate void OnPreviousContentDelegate();
        public static event OnPreviousContentDelegate PreviousContentDelegate;       

        public delegate void OnRepeatContentDelegate();
        public static event OnRepeatContentDelegate RepeatContentDelegate;

        public static void OnNextContent()
        {
            if (NextContentDelegate != null)
            {
                NextContentDelegate();
            }
        }

        public static void OnPreviousContent()
        {
            if (PreviousContentDelegate != null)
            {
                PreviousContentDelegate();
            }
        }

        public static void OnRepeatContent()
        {
            if (RepeatContentDelegate != null)
            {
                RepeatContentDelegate();
            }
        }

    }   
}


