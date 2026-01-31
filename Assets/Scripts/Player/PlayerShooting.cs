using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public PlayerBullet prefab;
    public GameObject spawnPoint;
    public int maxSize = 50;
    public float bulletLt = 1f;
    public float spreadIntensity = 1f;

    private ShootingPool shPool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shPool = new ShootingPool();
        shPool.Init();
        shPool.size = maxSize;
        shPool.spawnPoint = spawnPoint;
        shPool.prefab = prefab.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        HandleShooting();
    }

    void HandleShooting()
    {
        if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown("shoot"))
        {
            Vector3 dir = CalculateDirectionSpread();
            Shoot(dir);
        }
    }

    Vector3 CalculateDirectionSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - spawnPoint.transform.position;
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private void Shoot(Vector3 dir)
    {
        BasicBullet bb = shPool.Get().GetComponent<BasicBullet>();
        bb.dir = dir;
        StartCoroutine(ReturnAfter(bb.gameObject, bulletLt));
    }

    private System.Collections.IEnumerator ReturnAfter(GameObject gameObject, float seconds)
    {
        if (seconds > 0)
        {
            yield return new WaitForSeconds(seconds);
            // Give it back to the pool.
            shPool.Release(gameObject);
        }
    }
}
