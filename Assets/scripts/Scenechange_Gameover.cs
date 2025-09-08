using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenechange_Gameover : MonoBehaviour
{
    public int b;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButton_Gameover()
    {
        StartCoroutine("GAMEOVERWAIT");
    }
    public void OnButton_Gameover2()
    {
        StartCoroutine("GAMEOVERWAIT2");
    }
    public void OnButton_Continue()
    {
        SceneManager.LoadScene("Main");
    }
    public void OnButton_Start()
    {
        SceneManager.LoadScene("Title");
    }
    IEnumerator GAMEOVERWAIT()
    {
        yield return new WaitForSeconds(b);
        SceneManager.LoadScene("Gameover");
    }
    IEnumerator GAMEOVERWAIT2()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("Gameover");
    }
}
