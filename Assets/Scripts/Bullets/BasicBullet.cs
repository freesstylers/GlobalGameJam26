using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public Vector3 dir { get; set; }
    public float speed = 1f;
    public string collideWith = "";

    public PoolTemplate pool;

    public GameObject Particles;
    //Asignamos el color a las particulas de las balas
    public Color color; 
    void Update()
    {
        //transform.Translate(dir * speed * Time.deltaTime);
    }


    private void OnEnable()
    {
        //GetComponent<Rigidbody>().AddForce(dir * speed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null && other.gameObject.tag == collideWith)
        {
            //Guarrada historica
            GameObject.Instantiate(Particles, this.transform.position, Quaternion.identity);
            float dmg = FlowManager.instance.GetCurrentMask().stats_.baseDmg_;
            other.gameObject.GetComponentInParent<Transform>().gameObject.GetComponentInParent<EnemyBase>().ReceiveDamage((int)dmg);
            pool.Release(this.gameObject);

        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != null && other.gameObject.tag == collideWith)
        {
            //Guarrada historica
            GameObject.Instantiate(Particles, this.transform.position, Quaternion.identity);
            float dmg = FlowManager.instance.GetCurrentMask().stats_.baseDmg_;
            other.gameObject.GetComponentInParent<Transform>().gameObject.GetComponentInParent<EnemyBase>().ReceiveDamage((int)dmg);
        }
        pool.Release(this.gameObject);
    }

    virtual public void SetDir(Vector3 newDir)
    {
        dir = newDir;
    }

}
