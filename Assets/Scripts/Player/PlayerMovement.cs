using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        float speed =  moveSpeed;
        transform.position += moveDirection * speed * Time.deltaTime;
    }
}
