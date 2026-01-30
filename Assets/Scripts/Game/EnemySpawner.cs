using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;

    public IEnumerator Spawn()
    {
        float f = Random.Range(0.25f, 3.5f);

        yield return new WaitForSeconds(f);

        Instantiate(enemyToSpawn, this.transform);
    }
}
