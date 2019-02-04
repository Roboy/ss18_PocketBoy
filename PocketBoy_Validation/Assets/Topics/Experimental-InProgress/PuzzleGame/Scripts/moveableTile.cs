using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.PuzzleGame
{

    public class moveableTile : MonoBehaviour
    {

        public bool Moveable = false;
        public enum State { standard, correct, incorrect };

        private State s;
        private GameObject father;

        private void Start()
        {
            s = State.standard;
            father = null;
        }

        public State getState()
        {
            return s;
        }

        public void changeState(State s_new)
        {
            s = s_new;
        }

        public GameObject getFather()
        {
            return father;
        }

        public void setFather(GameObject go)
        {
            father = go;
        }


    }
}