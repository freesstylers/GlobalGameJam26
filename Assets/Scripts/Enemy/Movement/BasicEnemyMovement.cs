using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    //Posicion a la que nos movemoss
    public float speed;
    public float minDist;

    protected Transform player;

    private bool follow_ = true;

    void Start()
    {
        //Asignamos la pos del jugador
        player = GameObject.FindWithTag("Player").transform;
    }

    
    void Update()
    {
        Vector3 dist = player.position - transform.position;
        if (follow_ && dist.magnitude > minDist)
        {
            FollowPlayer(player.position, speed);
        }
    }

    //Movimiento recto hacia el jugador
    virtual protected void FollowPlayer(Vector3 MoveTo_, float speed_)
    {
        Vector3 uMov = MoveTo_ - transform.position;
        transform.Translate(uMov.normalized * speed_ * Time.deltaTime);
    } 

    public void Stop() { follow_ = false; }
    public void Play() { follow_ = true; }
}
