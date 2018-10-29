using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public class ForceScreenOrientation
    {
        private static ScreenOrientation m_OriginalOrientation = ScreenOrientation.AutoRotation;
        
        public static void SetScreenOrientation(ScreenOrientation orientation)
        {
            Screen.orientation = orientation;
        }

        public static void ResetScreenOrientation()
        {
            Screen.orientation = m_OriginalOrientation;
        }
    }
}

