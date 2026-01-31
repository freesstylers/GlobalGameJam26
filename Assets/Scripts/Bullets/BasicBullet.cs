using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public Vector3 dir { get; set; }
    public float speed { get; set; }

    //Asignamos el color a las particulas de las balas
    public Color color; 
    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }
}
