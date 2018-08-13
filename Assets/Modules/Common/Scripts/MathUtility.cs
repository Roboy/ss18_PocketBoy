using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.Common
{
    public static class MathUtility
    {

        /// <summary>
        /// Wraps the given index from 0 to arrayLength.
        /// In Unity "%" is the mathematical rem not mod, they behave differentely for different signs of a,b so we need to transform rem to mod.
        /// Source: http://answers.unity.com/answers/1120641/view.html
        /// </summary>
        /// <param name="index"></param>
        /// <param name="arrayLength"></param>
        /// <returns></returns>
        public static int WrapArrayIndex(int index, int arrayLength)
        {
            return (index % arrayLength + arrayLength) % arrayLength;
        }
    }
}
