using UnityEngine;

public class ShootingPool : PoolTemplate
{
    public GameObject spawnPoint;
    public void Shoot(Vector3 dir)
    {
        pool_.Get();
    }

    override protected GameObject CreateItem()
    {
        //Spawneamos las balas en el espacio
        GameObject gameObject = Instantiate(prefab, spawnPoint.transform, true);
        
        //TODO: suscribir a las balas para el cambio de color y estadisticas

        gameObject.SetActive(false);
        return gameObject;
    }
}
