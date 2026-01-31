using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public PlayerBullet prefab;
    public GameObject spawnPoint;
    public int maxSize = 50;
    public float bulletLt = 1f;
    public float spreadIntensity = 1f;
    public int maxAmo = 30;
    public float cadence = 0.1f;
    public float reloadTime = 1f;

    private ShootingPool shPool;
     
    private float currentCadence = 0f;
    private int currentAmo = 0;
    private float currentReloadTime = 0f;
    private bool reload = false;

    private FMOD.Studio.EventInstance shootInstance_;
    private FMOD.Studio.EventInstance reloadInstance_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FlowManager.instance.SuscribeMaskChange(OnMaskChange);

        shPool = new ShootingPool();
        shPool.Init();
        shPool.size = maxSize;
        shPool.spawnPoint = spawnPoint;
        shPool.prefab = prefab.gameObject;

        Stats maskStats = FlowManager.instance.GetCurrentMask().stats_;
        maxAmo = maskStats.realAmmo_;
        cadence = maskStats.realRate_;
        reloadTime = maskStats.realReload_;

        currentAmo = maxAmo;

        shootInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/Shoot");
        reloadInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/Reload");
    }

    // Update is called once per frame
    void Update()
    {
        HandleShooting();
    }

    void HandleShooting()
    {
        if (Rewired.ReInput.players.GetPlayer(0).GetButton("shoot") && !reload && currentAmo > 0 && currentCadence > cadence)
        {
            Vector3 dir = CalculateDirectionSpread().normalized;
            Shoot(dir);
        }

        if(currentCadence < cadence)
        {
            currentCadence += Time.deltaTime;
        }

        if (reload)
        {
            currentReloadTime += Time.deltaTime;
            if(currentReloadTime >= reloadTime)
            {
                reload = false;
                currentAmo = maxAmo;
            }
        }


    }

    void Reload()
    {
        reload = true;
        currentReloadTime = 0;
        reloadInstance_.start();
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
        float x = UnityEngine.Random.Range(-1f, 1f);
        float y = UnityEngine.Random.Range(-1f, 1f);

        return direction.normalized + new Vector3(x, y, 0)*spreadIntensity*0.1f;
    }

    private void Shoot(Vector3 dir)
    {
        BasicBullet bb = shPool.Get().GetComponent<BasicBullet>();
        bb.dir = dir;
        StartCoroutine(ReturnAfter(bb.gameObject, bulletLt));

        currentAmo--;
        if (currentAmo <= 0)
        {
            Reload();
        }
        currentCadence = 0f;

        shootInstance_.start();
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

    private void OnMaskChange(Mask m)
    {
        Reload();
        Stats maskStats = m.stats_;
        maxAmo = maskStats.realAmmo_;
        cadence = maskStats.realRate_;
        reloadTime = maskStats.realReload_;
    }
}
