using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public Vector3 dir { get; set; }
    public float speed = 1f;
    public string collideWith = "";


    //Asignamos el color a las particulas de las balas
    public Color color; 
    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag(collideWith))
    //    {

    //    }
    //}

}
