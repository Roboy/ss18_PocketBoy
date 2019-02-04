using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.PuzzleGame
{

    public class FindingSource : MonoBehaviour
    {

        private bool m_sourceFound = false;

        private void OnTriggerStay(Collider other)
        {
            if (!m_sourceFound)
            {

                if (this.gameObject.tag == other.gameObject.tag)
                {
                    this.gameObject.GetComponent<moveableTile>().changeState(moveableTile.State.correct);
                    this.gameObject.GetComponent<moveableTile>().setFather(other.gameObject);
                    tileManager.Instance.checkState(this.gameObject);
                }
                else
                {
                    this.gameObject.GetComponent<moveableTile>().changeState(moveableTile.State.incorrect);
                    this.gameObject.GetComponent<moveableTile>().setFather(null);
                    tileManager.Instance.checkState(this.gameObject);

                }
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (!m_sourceFound)
            {
                this.gameObject.GetComponent<moveableTile>().changeState(moveableTile.State.standard);
                this.gameObject.GetComponent<moveableTile>().setFather(null);
                tileManager.Instance.checkState(this.gameObject);
            }
        }

        public void setSourceFound(bool val)
        {
            m_sourceFound = val;
        }

        public bool getSourceFound()
        {
            return m_sourceFound;
        }
    }
}
