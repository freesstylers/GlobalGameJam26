using UnityEngine;
using UnityEngine.Pool;

public class PoolTemplate : MonoBehaviour
{
    public int defaultCapacity = 10;
    public int size = 50;
    public GameObject prefab;

    protected ObjectPool<GameObject> pool_;

    public void Init()
    {
        // Create a pool with the four core callbacks.
        pool_ = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,   // helps catch double-release mistakes
            defaultCapacity: defaultCapacity,
            maxSize: size
        );
    }

    public GameObject Get()
    {
        return pool_.Get();
    }

    
    virtual protected GameObject CreateItem()
    {
        GameObject gameObject = Instantiate(prefab);
        gameObject.SetActive(false);
        return gameObject;
    }

    virtual protected void OnGet(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }


    virtual protected void OnRelease(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    virtual protected void OnDestroyItem(GameObject gamesObject)
    {
        Destroy(gameObject);
    }
}
