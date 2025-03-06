using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class MonsterSpawnEvent : MonoBehaviour
{
    public event Action<MonsterSpawnEvent, MonsterSpawnEventArgs> OnWaveStart;
    public event Action<MonsterSpawnEvent> OnWaveFinish;


    public void CallWaveStart(List<MonsterDetailsSO> spawnParameter)
    {
        OnWaveStart?.Invoke(this, new MonsterSpawnEventArgs
        {
            spawnParameter = spawnParameter,
        });
    }

    public void CallWaveFinish()
    {
        OnWaveFinish?.Invoke(this);
    }
}


public class MonsterSpawnEventArgs : EventArgs
{
    public List<MonsterDetailsSO> spawnParameter;
}