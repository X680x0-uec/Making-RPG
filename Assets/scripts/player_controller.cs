using UnityEngine;
using System;

public class player_controller : MonoBehaviour
{
    public float speed = 2.5f;
    Rigidbody2D rb;
    Vector2 move;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        move = new Vector2(Math.Abs(x) >= Math.Abs(y) ? x : 0, Math.Abs(y) > Math.Abs(x) ? y : 0);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }
}
