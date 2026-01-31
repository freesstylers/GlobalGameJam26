using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public Vector3 dir { get; set; }
    public float speed = 1f;
    public string collideWith = "";

    public PoolTemplate pool;


    //Asignamos el color a las particulas de las balas
    public Color color; 
    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null && other.gameObject.tag == collideWith)
        {
            //Guarrada historica
            float dmg = FlowManager.instance.GetCurrentMask().stats_.baseDmg_;
            other.gameObject.GetComponentInParent<Transform>().gameObject.GetComponentInParent<EnemyBase>().ReceiveDamage((int)dmg);
            pool.Release(this.gameObject);
        }
    }

}
