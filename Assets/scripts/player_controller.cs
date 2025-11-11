using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

public class player_controller : MonoBehaviour
{
    [SerializeField] private GameObject UI_Encount;
    private Animator animator;
    [SerializeField] Animator cameraAnimator;  // カメラワーク

    [SerializeField] public float speed_const = 2.5f;
    public float speed;
    public bool stop = false;
    private Rigidbody2D rb;
    private Vector2 move;
    private int idX = Animator.StringToHash("x"), idY = Animator.StringToHash("y");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UI_Encount.SetActive(false);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        x = Math.Abs(x) >= Math.Abs(y) ? x : 0;
        y = Math.Abs(y) > Math.Abs(x) ? y : 0;

        if ((Mathf.FloorToInt(x) != 0 || Mathf.FloorToInt(y) != 0) && Time.timeScale != 0f && !this.stop)
        {   
            this.speed = this.speed_const;
            this.animator.speed = 1;
            this.animator.SetFloat (idX, x);
			this.animator.SetFloat (idY, y);
        }
        else
        {   
            this.speed = 0;
            this.animator.speed = 0;
        }
        this.move = new Vector2(x, y);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {   
        if (collision.tag == "encounter")
        {
            int randomencount = UnityEngine.Random.Range(0, 100);
            if (randomencount == 0)
            {
                UI_Encount.SetActive(true);
                stop = true;
                cameraAnimator.SetTrigger("Encounter");
                StartCoroutine("WAITTIME");
                int randomenemy = UnityEngine.Random.Range(0, 3);
                if (randomenemy == 0)
                {
                    Static.enemynumber = 0;
                }
                if (randomenemy == 1)
                {
                    Static.enemynumber = 1;
                }
                if (randomenemy == 2)
                {
                    Static.enemynumber = 2;
                }
            }
        }
        if(collision.tag == "danpen")
        {
            UI_Encount.SetActive(true);
            animator.SetTrigger("encount");
            StartCoroutine("WAITTIME");
            Static.enemynumber = 10;
        }
    }
    IEnumerator WAITTIME()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Battle");
    }
}
