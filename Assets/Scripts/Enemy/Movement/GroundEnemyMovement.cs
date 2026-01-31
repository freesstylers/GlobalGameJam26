using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GroundEnemyMovement : BasicEnemyMovement
{
    private void Start()
    {
        GetComponent<NavMeshAgent>().speed = speed;
        player = GameObject.FindWithTag("Player").transform;
    }

    protected override void FollowPlayer(Vector3 MoveTo_, float speed_)
    {
        GetComponent<NavMeshAgent>().destination = player.position;
    }
}
