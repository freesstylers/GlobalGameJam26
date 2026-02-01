using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    public float timeToDestroy;
    float cd_destroy = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cd_destroy = timeToDestroy;
    }

    // Update is called once per frame
    void Update()
    {
        cd_destroy -= Time.deltaTime;
        if(cd_destroy < 0.0f)
        {
            Destroy(this.gameObject);
        }
    }
}
