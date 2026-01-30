using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    //Posicion a la que nos movemoss
    public float speed;

    protected Vector3 MoveTo;

    void Start()
    {
        //Asignamos la pos del jugador
        MoveTo = GameObject.FindWithTag("Player").transform.position;
    }

    
    void Update()
    {
        FollowPlayer(MoveTo, speed);
    }

    //Movimiento recto hacia el jugador
    protected void FollowPlayer(Vector3 MoveTo_, float speed_)
    {
        Vector3 uMov = MoveTo_ - transform.position;
        transform.Translate(uMov.normalized * speed_ * Time.deltaTime);
    }
}
