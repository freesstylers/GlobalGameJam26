using UnityEngine;

public class ToggleSpawners : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Spawner")
        {
            other.GetComponent<EnemySpawner>().enabled = false;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Spawner")
        {
            other.GetComponent<EnemySpawner>().enabled = true;
        }
    }

}
