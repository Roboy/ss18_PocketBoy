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
            SpawnPlayer();
            LoadMaze();
        }
        public void LoadMaze()
        {
            if (m_Maze != null)
                Destroy(m_Maze);

            int levelNumber = m_DifficultyLevel.value;

            var maze = GameObject.Instantiate(m_MazePrefabs[levelNumber]);
            maze.name = m_MazePrefabs[levelNumber].name;
            m_Maze = maze;

            m_Player.transform.localScale = maze.GetComponent<Maze>().m_PlayerSpawn.transform.localScale;
            m_Player.transform.localPosition = maze.GetComponent<Maze>().m_PlayerSpawn.transform.localPosition;
            m_Player.transform.localRotation = Quaternion.LookRotation(maze.GetComponent<Maze>().m_PlayerSpawn.transform.forward);

            CodeManager.Instance.m_NumberOfTries = 0;
            CodeManager.Instance.UpdateAttemptCounter(0);
        }

        public void SpawnPlayer()
        {
            if (m_Player != null)
                Destroy(m_Player);

            var player = GameObject.Instantiate(m_PlayerPrefab);
            player.name = m_PlayerPrefab.name;
            m_Player = player;
            CodeManager.Instance.ExchangeRoboyInMaze(m_Player);
        }
    }
}

