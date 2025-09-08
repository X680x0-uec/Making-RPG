using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_encounter : MonoBehaviour
{
    [SerializeField] private GameObject UI_Encount;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UI_Encount.SetActive(false);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "encounter")
        {
            int randomencount = Random.Range(0, 100);
            if (randomencount == 0)
            {
                UI_Encount.SetActive(true);
                animator.SetTrigger("encount");
                StartCoroutine("WAITTIME");
                int randomenemy = Random.Range(0, 3);
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
