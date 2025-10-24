using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class player_parameter : MonoBehaviour
{
    public TMP_Text PHP_Text;
    public TMP_Text PMP_Text;
    public int PHPdecrease = 0;
    public int PMPdecrease = 0;
    public bool defence = false;

    public Battle_text battle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PHP_Text.text = Static.playerHPnow + "/" + Static.playerHP.ToString();
        PMP_Text.text = Static.playerMPnow + "/" + Static.playerMP.ToString();
    }

    public void StartAttack()
    {
        StartCoroutine("MAINPROCESS");
    }
    IEnumerator MAINPROCESS()
    {
        if (defence)
        {
            PHPdecrease -= Static.playerDefence;
        }
        Static.playerHPnow -= PHPdecrease;
        yield return new WaitForSeconds(0.5f);
        PHP_Text.text = Static.playerHPnow + "/" + Static.playerHP.ToString();
        if (Static.playerHP <= 0)
        {
            Static.playerHPnow = 0;
            PHP_Text.text = Static.playerHPnow + "/" + Static.playerHP.ToString();
            yield return new WaitForSeconds(1.0f);
            SceneManager.LoadScene("Gameover");
        }
        else
        {
            StartCoroutine("LASTPROCESS");
        }
    }
    IEnumerator LASTPROCESS()
    {
        yield return new WaitForSeconds(1.0f);

    }


    // Update is called once per frame
    void Update()
    {

    }
}

