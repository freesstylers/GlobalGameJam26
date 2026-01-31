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

    [Header("GUN RECOIL")]
    public GameObject gunObject;
    public Vector3 recoilOffset = new Vector3(0, 0, -0.2f);
    public float recoilHorizontalVariation = 0.1f;
    public Vector3 recoilRotation = new Vector3(5f, 0, 0);
    public float recoilDuration = 0.1f;
    public float recoilReturnSpeed = 10f;

    private ShootingPool shPool;
     
    private float currentCadence = 0f;
    private int currentAmo = 0;
    private float currentReloadTime = 0f;
    private bool reload = false;
    private Vector3 gunBasePosition;
    private Quaternion gunBaseRotation;
    private float recoilTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shPool = new ShootingPool();
        shPool.Init();
        shPool.size = maxSize;
        shPool.spawnPoint = spawnPoint;
        shPool.prefab = prefab.gameObject;

        currentAmo = maxAmo;
        gunBasePosition = gunObject.transform.localPosition;
        gunBaseRotation = gunObject.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        HandleShooting();
        HandleRecoil();
    }

    private void HandleRecoil()
    {
        if (recoilTimer > 0f)
        {
            recoilTimer -= Time.deltaTime;
        }

        // lerp a la posicion inicial
        float t = Mathf.Clamp01(1f - (recoilTimer / recoilDuration));

        // Retrocede 
        Vector3 horizontalVariation = new Vector3(
            Random.Range(-recoilHorizontalVariation, recoilHorizontalVariation),
            0,
            0
        );
        Vector3 targetPosition = Vector3.Lerp(gunBasePosition + recoilOffset + horizontalVariation, gunBasePosition, t);
        gunObject.transform.localPosition = targetPosition;

        // Rota un poco para dar variedad
        Vector3 rotationOffset = recoilRotation * (1f - t);
        Quaternion targetRotation = gunBaseRotation * Quaternion.Euler(rotationOffset);
        gunObject.transform.localRotation = Quaternion.Lerp(gunObject.transform.localRotation, targetRotation, Time.deltaTime * 10f);
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
    }

    Vector3 CalculateDirectionSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
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
        recoilTimer = recoilDuration;
        bb.pool = shPool;
        StartCoroutine(ReturnAfter(bb.gameObject, bulletLt));

        currentAmo--;
        if (currentAmo <= 0)
        {
            Reload();
        }
        currentCadence = 0f;
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