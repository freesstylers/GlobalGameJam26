using UnityEngine;

public class ShootingPool : PoolTemplate
{
    public GameObject spawnPoint;

    override protected GameObject CreateItem()
    {
        //Spawneamos las balas en el espacio
        GameObject gameObject = Instantiate(prefab, spawnPoint.transform.position, Quaternion.identity);

        gameObject.SetActive(false);
        return gameObject;
    }

    protected override void OnGet(GameObject gameObject)
    {
        gameObject.transform.SetPositionAndRotation(spawnPoint.transform.position, Quaternion.identity);
        base.OnGet(gameObject);

    }

}
