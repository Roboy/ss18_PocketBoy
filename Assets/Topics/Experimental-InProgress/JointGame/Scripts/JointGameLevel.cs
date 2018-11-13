using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pocketboy.JointGame
{
    public class JointGameLevel : MonoBehaviour
    {
        [SerializeField]
        private Joint CorrectJoint;

        [SerializeField]
        private RoboticArmController RoboticArm;

        public bool CheckJoint(Joint joint)
        {
            return joint == CorrectJoint;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            RoboticArm.StartMotion();
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            RoboticArm.StopMotion();
            gameObject.SetActive(false);
        }
        
    }
}