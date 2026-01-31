using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Pool;
using static FlowManager;

public class EnemyPoolManager : MonoBehaviour
{
    public EnemyScriptable[] enemyTypes;
    public PoolTemplate[] enemyPools;
    public RoundScriptable[] rounds;

    public List<EnemySpawner> spawners;
    public List<EnemySpawner> spawnersInMind;

    public void Awake()
    {
        enemyPools = new PoolTemplate[Enum.GetNames(typeof(enemyType)).Length];
        for (int i = 0; i < enemyPools.Length; i++)
        {
            enemyPools[i] = new PoolTemplate();
            enemyPools[i].Init();
            enemyPools[i].prefab = enemyTypes[i].prefab;
        }

        spawners = GetComponentsInChildren<EnemySpawner>().ToList();        
    }

    public void onRoundChange(int r)
    {
        Debug.LogError("Pool Manager - Round change");

        spawnersInMind.Clear();

        for (int i = 0; i < spawners.Count; i++) //Para que no spawneen en tu puta cara
        {
            if (spawners[i].enabled)
            {
                spawnersInMind.Add(spawners[i]);
            }
        }

        foreach (KeyValuePair<enemyType, int> enemy in rounds[r].roundEnemies) //Lee información de cada ronda
        {
            for (int i = 0; i < enemy.Value; i++)
            {
                GameObject g = enemyPools[(int)enemy.Key].Get();

                int aux = UnityEngine.Random.Range(0, spawnersInMind.Count);

                g.transform.SetPositionAndRotation(spawnersInMind[aux].transform.position, new Quaternion());
                g.GetComponent<EnemyBase>().reference = enemyPools[(int)enemy.Key];

                FlowManager.instance.currentAliveEnemies += 1;

                spawnersInMind.RemoveAt(aux); //Para que no spawnee 2 en el mismo por ronda

            }
        }

        FlowManager.instance.advanceState();
    }
}
