using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Profiling;
using System;

[Serializable]
public class DeviceStat
{
    public float CPUTime;
    public float CurrentFPS;
    public float AvgFPS;
    public float TotalAllocatedMemory;
    public float TotalReservedMemory;
    public float TotalUnusedReserverdMemory;
    public float BatteryLevel;
    public float TimeStamp;
}

[Serializable]
public class DeviceStats
{
    public List<DeviceStat> Data = new List<DeviceStat>();
}

public class DeviceStatsLogger : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private DeviceStats m_Stats = new DeviceStats();

    [SerializeField]
    private float m_UpdateInterval = 1f;

    private float m_LastStatsUpdate;

    private float m_TimeNow;

    private float m_FramesSinceLastUpdate;

    private float m_FramesCountForAverage;

    private float m_FPSSum;

    private float m_CurrentFPS;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        m_TimeNow = Time.realtimeSinceStartup;
        m_LastStatsUpdate = m_TimeNow;
    }

    void Update()
    {
        m_FramesSinceLastUpdate++;
        m_TimeNow = Time.realtimeSinceStartup;

        if (m_TimeNow > m_LastStatsUpdate + m_UpdateInterval && m_TimeNow > 10f)
        {
            m_CurrentFPS = m_FramesSinceLastUpdate / (m_TimeNow - m_LastStatsUpdate);
            m_FramesCountForAverage++;
            m_FPSSum += m_CurrentFPS;

            var deviceStat = new DeviceStat();
            deviceStat.CPUTime = 1000.0f / Mathf.Max(m_CurrentFPS, 0.00001f);
            deviceStat.CurrentFPS = m_CurrentFPS;
            deviceStat.AvgFPS = m_FPSSum / m_FramesCountForAverage;
            deviceStat.TotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / 1048576;
            deviceStat.TotalReservedMemory = Profiler.GetTotalReservedMemoryLong() / 1048576;
            deviceStat.TotalUnusedReserverdMemory = Profiler.GetTotalUnusedReservedMemoryLong() / 1048576;
            deviceStat.BatteryLevel = SystemInfo.batteryLevel;
            deviceStat.TimeStamp = m_TimeNow;
            m_Stats.Data.Add(deviceStat);
            m_LastStatsUpdate = m_TimeNow;
            m_FramesSinceLastUpdate = 0;
        }
    }

    private void SaveStats()
    {
        string statsJSON = JsonUtility.ToJson(m_Stats);
        string filePath = string.Format("{0}/DeviceStats_{1}_{2}.json", Application.persistentDataPath, Application.productName, SystemInfo.deviceModel).Replace(' ', '_');
        using (var streamWriter = File.CreateText(filePath))
        {
            streamWriter.Write(statsJSON);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            SaveStats();
    }

    private void OnDestroy()
    {
        SaveStats();
    }
}
