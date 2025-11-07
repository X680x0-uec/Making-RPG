using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class player_controller : MonoBehaviour
{
    public float speed = 2.5f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 move;
    private int idX = Animator.StringToHash("x"), idY = Animator.StringToHash("y");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        x = Math.Abs(x) >= Math.Abs(y) ? x : 0;
        y = Math.Abs(y) > Math.Abs(x) ? y : 0;

        if (Mathf.FloorToInt(x) != 0 || Mathf.FloorToInt(y) != 0) 
        {   
            animator.speed = 1;
            animator.SetFloat (idX, x);
			animator.SetFloat (idY, y);
        }
        else
        {
            animator.speed = 0;
        }
        move = new Vector2(x, y);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }
}
