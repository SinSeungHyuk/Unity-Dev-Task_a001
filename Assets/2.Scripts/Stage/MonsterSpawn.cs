using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
using Random = UnityEngine.Random;


public class MonsterSpawn : MonoBehaviour
{
    private MonsterSpawnEvent monsterSpawnEvent;
    private List<MonsterDetailsSO> spawnParameter;

    private CancellationTokenSource cts = new CancellationTokenSource();



    private void Awake()
    {
        monsterSpawnEvent = GetComponent<MonsterSpawnEvent>();
    }
    private void OnEnable()
    {
        monsterSpawnEvent.OnWaveStart += MonsterSpawnEvent_OnWaveStart;
    }
    private void OnDisable()
    {
        monsterSpawnEvent.OnWaveStart -= MonsterSpawnEvent_OnWaveStart;

        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }


    #region STAGE EVENT
    private void MonsterSpawnEvent_OnWaveStart(MonsterSpawnEvent @event, MonsterSpawnEventArgs spawnerArgs)
    {
        spawnParameter = spawnerArgs.spawnParameter;

        WaveMonsterSpawn().Forget(); // UniTask 호출
    }
    #endregion


    #region SPAWN FUNCTION
    private async UniTask WaveMonsterSpawn()
    {
        try
        {
            // 랜덤 시간마다 스폰 (임의코드)
            while (true)
            {
                int delayTime = Random.Range(2000, 5000);
                await UniTask.Delay(delayTime, cancellationToken: cts.Token);
             
                RandomSpawn(spawnParameter);
            }
        }
        catch (OperationCanceledException)
        {
            //Debug.Log("WaveMonsterSpawn - Spawn Canceled!!!");
        }
    }

    private void RandomSpawn(List<MonsterDetailsSO> spawnParameter)
    {
        var monsterData = spawnParameter[Random.Range(0, spawnParameter.Count)];

        var monster = ObjectPoolManager.Instance.Get(EPool.Monster, transform).GetComponent<Monster>();
        monster.InitializeMonster(monsterData, gameObject.layer);
    }
    #endregion
}