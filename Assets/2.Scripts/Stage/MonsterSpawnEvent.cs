using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MonsterSpawnEvent : MonoBehaviour
{
    public event Action<MonsterSpawnEvent> OnStageStart;
    public event Action<MonsterSpawnEvent> OnWaveStart;
    public event Action<MonsterSpawnEvent> OnWaveFinish;
    public event Action<MonsterSpawnEvent> OnStageFinish;


    public void CallStageStart()
    {
        OnStageStart?.Invoke(this);
    }

    public void CallWaveStart()
    {
        OnWaveStart?.Invoke(this);
    }

    public void CallWaveFinish()
    {
        OnWaveFinish?.Invoke(this);
    }

    public void CallStageFinish()
    {
        OnStageFinish?.Invoke(this);
    }
}