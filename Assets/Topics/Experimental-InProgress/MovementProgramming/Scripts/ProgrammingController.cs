using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pocketboy.Common;
using TMPro;

namespace Pocketboy.MovementProgramming
{

    public class ProgrammingController : Singleton<ProgrammingController>
    {
       
        [SerializeField]
        private GameObject m_PlayerPrefab;
        [SerializeField]
        private List<GameObject> m_MazePrefabs;
        [SerializeField]
        private GameObject m_Player;
        [SerializeField]
        private GameObject m_Maze;
        [SerializeField]
        private TMP_Dropdown m_DifficultyLevel;


        // Use this for initialization
        void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            LevelManager.Instance.RegisterGameObjectWithRoboy(this.gameObject, Vector3.zero, Quaternion.identity);
            LoadMaze();
            SpawnPlayer();
            
        }
        public void LoadMaze()
        {
            if (m_Maze != null)
                Destroy(m_Maze);

            int levelNumber = m_DifficultyLevel.value;

            var maze = GameObject.Instantiate(m_MazePrefabs[levelNumber]);
            maze.name = m_MazePrefabs[levelNumber].name;
            maze.transform.parent = this.transform;
            Vector3 pos = this.transform.position;
            pos.x -= 0.25f;
            pos.z -= 0.25f;
            maze.transform.position = pos;
            m_Maze = maze;
            CodeManager.Instance.m_NumberOfTries = 0;
            CodeManager.Instance.UpdateAttemptCounter(0);
            
        }

        public void SpawnPlayer()
        {
            //if (m_Player != null)
            //    Destroy(m_Player);


            if (m_Player == null)
            {
                var player = GameObject.Instantiate(m_PlayerPrefab);
                player.name = m_PlayerPrefab.name;
                player.transform.parent = this.transform;
                m_Player = player;
            }
            Transform playerspawn = m_Maze.GetComponent<Maze>().m_PlayerSpawn.transform;

            m_Player.transform.localScale = playerspawn.lossyScale;
            m_Player.transform.position = playerspawn.position;
            m_Player.transform.rotation = playerspawn.rotation;
            m_Player.GetComponent<MazeRunner>().SetInitPose();
            CodeManager.Instance.ExchangeRoboyInMaze(m_Player);
        }
    }
}

