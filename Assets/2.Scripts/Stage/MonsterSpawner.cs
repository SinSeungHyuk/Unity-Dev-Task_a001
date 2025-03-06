using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private List<MonsterDetailsSO> spawnParameter;

    public MonsterSpawnEvent MonsterSpawnEvent { get; private set; }


    private void Awake()
    {
        MonsterSpawnEvent = GetComponent<MonsterSpawnEvent>();
    }

    private void Start()
    {
        MonsterSpawnEvent.CallWaveStart(spawnParameter);
    }
}
