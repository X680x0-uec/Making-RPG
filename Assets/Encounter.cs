
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    [SerializeField] private GameObject UI_Encount;
    [SerializeField] private Animation anime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UI_Encount.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

     private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Encount")
        {
            int randomInt = Random.Range(0, 100);
            if (randomInt < 100)
            { 
                UI_Encount.SetActive(true);
                StartCoroutine("WAITTIME");
            }

        }
    }
    IEnumerator WAITTIME()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Battle");
    }
}
