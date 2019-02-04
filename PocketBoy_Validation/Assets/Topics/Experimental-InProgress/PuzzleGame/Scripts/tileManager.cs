using System.Collections.Generic;
using UnityEngine;
using Pocketboy.Common;

namespace Pocketboy.PuzzleGame
{

    public class tileManager : Singleton<tileManager>

    {
        public Material std;
        public Material green;
        public Material red;

        private Vector3 dist;
        private float posX;
        private float posY;


        private GameObject m_hittedObject;
        private bool m_dragging = false;
        private bool m_winning = false;
        private List<FindingSource> m_Tiles;

        private void Start()
        {
            //Creates a list of a the puzzle pieces and stores it in m_Tiles
            m_Tiles = new List<FindingSource>();
            foreach (Object o in GameObject.FindObjectsOfType<FindingSource>())
            {
                FindingSource tmp = (FindingSource)o;
                m_Tiles.Add(tmp);
            }

        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseDown();

            }
            if (m_dragging)
            {
                OnMouseDrag();
            }

        }

        void OnMouseDown()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            int layer_mask = LayerMask.GetMask("RoboyParts");

            // Checking if a puzzle part is clicked on by the user
            if (Physics.Raycast(ray, out hit, 100, layer_mask))
            {
                m_hittedObject = hit.transform.gameObject;
                m_dragging = true;
                //Debug.Log(m_hittedObject.gameObject.name);
                dist = Camera.main.WorldToScreenPoint(m_hittedObject.transform.position);
                posX = Input.mousePosition.x - dist.x;
                posY = Input.mousePosition.y - dist.y;
            }


        }

        void OnMouseDrag()
        {
            Vector3 curPos = new Vector3(Input.mousePosition.x - posX, Input.mousePosition.y - posY, dist.z);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);

            if (m_hittedObject != null)
            {
                if (m_hittedObject.GetComponent<moveableTile>().Moveable)
                {
                    m_hittedObject.transform.position = worldPos;
                }
            }
            // User releases the mouse button/ stops the touch motion:
            // If the tile is at the correct spot, it gets placed, then it sticks and is not moveable anymore.
            if (Input.GetMouseButtonUp(0))
            {
                m_dragging = false;
                if (m_hittedObject.GetComponent<moveableTile>().getState() == moveableTile.State.correct)
                {
                    m_hittedObject.GetComponent<moveableTile>().Moveable = false;
                    Vector3 destination = m_hittedObject.GetComponent<moveableTile>().getFather().transform.position;
                    m_hittedObject.transform.position = destination;
                    m_hittedObject.GetComponent<FindingSource>().setSourceFound(true);
                    m_hittedObject.GetComponent<moveableTile>().changeState(moveableTile.State.standard);
                    checkState(m_hittedObject);
                    checkForWin();
                }


            }

        }
        /// <summary>
        /// Updates the visible colour of the Puzzle piece depending of its state.
        /// The state depends on the collision detection system and indicates whether
        /// it can be placed at this position or not.
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="state"></param>
        public void changeTileColour(GameObject tile, moveableTile.State state)
        {
            if (state == moveableTile.State.incorrect)
            {
                tile.GetComponent<Renderer>().material = red;
            }
            if (state == moveableTile.State.correct)
            {
                tile.GetComponent<Renderer>().material = green;
            }
            if (state == moveableTile.State.standard)
            {
                tile.GetComponent<Renderer>().material = std;
            }
        }

        public void checkState(GameObject go)
        {
            changeTileColour(go, go.GetComponent<moveableTile>().getState());
        }

        public bool checkForWin()
        {

            foreach (FindingSource m in m_Tiles)
            {
                // Even if one single piece is not at the final position, the game is not yet won.
                if (!m.getSourceFound())
                {
                    Debug.Log("Still (a) piece(s) missing.");
                    m_winning = false;
                    return false;
                }
            }

            // If all pieces are at the right position, the user wins.
            Debug.Log("You win!!");
            m_winning = true;
            return true;
        }
    }

}
