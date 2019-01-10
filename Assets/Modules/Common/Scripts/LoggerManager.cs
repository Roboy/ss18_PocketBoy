using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Pocketboy.Common;
using System.Net.Http;

namespace Pocketboy.Logging
{
    public class LoggerManager : Singleton<LoggerManager>
    {
        private Stopwatch m_Stopwatch = new Stopwatch();

        private SceneLogger m_SceneLogger;

        private int m_ID = 1;

        private static string m_IDPlayerPrefsName = "StudyID";

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            LoadID();
            SceneManager.sceneUnloaded += (scene) => SaveSceneStats(scene); m_SceneLogger = null;
            SceneManager.sceneLoaded += (scene, loadMode) => ResetSceneStats();
        }

        private void OnDestroy()
        {
            SaveSceneStats(SceneManager.GetActiveScene());
        }

        public void RegisterLogger(SceneLogger logger)
        {
            m_SceneLogger = logger;
        }

        private void ResetSceneStats()
        {
            m_Stopwatch.Start();
        }

        private void SaveSceneStats(Scene scene)
        {
            m_Stopwatch.Stop();
            var time = m_Stopwatch.ElapsedMilliseconds;
            string stats = "No stats available for this scene";
            if(m_SceneLogger != null)
                stats = m_SceneLogger.GetStats();

            string output = string.Format("{{ID : {0}, Scene : {1},  Time : {2} seconds, Data : {3}}}", m_ID, scene.name, time / 1000f, stats);
            SaveSceneStatsToFile(scene.name, output);
        }

        private void SaveSceneStatsToFile(string scene, string stats)
        {
            string filePath = string.Format("{0}/{1}_{2}.json", Application.persistentDataPath, m_ID, scene);
            StreamWriter streamWriter = new StreamWriter(filePath, true);
            streamWriter.WriteLine(stats);
            streamWriter.Close();
        }

        private void LoadID()
        {
            if (PlayerPrefs.HasKey(m_IDPlayerPrefsName))
            {
                m_ID = PlayerPrefs.GetInt(m_IDPlayerPrefsName) + 1;
            }
            
            PlayerPrefs.SetInt(m_IDPlayerPrefsName, m_ID);
        } 
    }
}


