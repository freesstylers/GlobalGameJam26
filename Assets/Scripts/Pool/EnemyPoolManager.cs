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
        for (int i = 0; i < Enum.GetNames(typeof(enemyType)).Length; i++)
        {
            enemyPools[i] = new PoolTemplate();
            enemyPools[i].prefab = enemyTypes[i].prefab;
        }

        spawners = GetComponentsInChildren<EnemySpawner>().ToList();        
    }

    public void onRoundChange(int r)
    {
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

                spawnersInMind.RemoveAt(aux); //Para que no spawnee 2 en el mismo por ronda

            }
        }
    }
}
