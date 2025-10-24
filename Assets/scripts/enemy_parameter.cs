using UnityEngine;
using TMPro;
using System.Collections;

public class enemy_parameter : MonoBehaviour
{
    private int enemy_HP;
    public TMP_Text enemyHP_Text;
    public TMP_Text enemyHP_decrease;
    public int HPdecrease = 0;
    [SerializeField] GameObject enemybox;

    public Battle_text battle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (Static.enemynumber == 0)
        {
            enemy_HP = 50;
        }
        if (Static.enemynumber == 1)
        {
            enemy_HP = 68;
        }
        if (Static.enemynumber == 2)
        {
            enemy_HP = 100;
        }
        if (Static.enemynumber == 10)
        {
            enemy_HP = 9999;
        }
        enemyHP_Text.text = "HP " + enemy_HP.ToString();
        enemyHP_decrease.text = "".ToString(); 
    }

    public void StartAttack()
    {
        StartCoroutine("MAINPROCESS");
    }
    IEnumerator MAINPROCESS()
    {
        enemy_HP -= HPdecrease;
        yield return null;
        enemyHP_Text.text = "HP " + enemy_HP.ToString();
        enemyHP_decrease.text = "-" + HPdecrease.ToString();
        if (enemy_HP <= 0)
        {
            enemy_HP = 0;
            enemyHP_Text.text = "HP " + enemy_HP.ToString();
            yield return new WaitForSeconds(0.2f);
            enemyHP_decrease.text = "".ToString();
            enemybox.SetActive(false);
            battle.KillEnemy();
        }
        else
        {
            StartCoroutine("LASTPROCESS");
        }
    }
    IEnumerator LASTPROCESS()
    {
        yield return new WaitForSeconds(1.0f);
        enemyHP_decrease.text = "".ToString();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
