using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;
using System;

public class Battle_text : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI target;
    [SerializeField] GameObject UI_talkbox;

    public string[] enemy = new string[11];
    public string[] damagedenemy = new string[11];
    public string[] enemyaction = new string[11];
    public string[] killedenemy = new string[11];
    public string[] action = new string[10];
    public float waits = 0.2f;
    public Battle_button buttoncontrol;
    private int playerturn = 0;
    public player_parameter playerparameter;
    private int enemypower = 0;

    public enemy_parameter enemyparameter;
    public int actionnumber = 999;

    public GameObject karienemy1;
    public GameObject karienemy2;
    public GameObject karienemy3;
    public GameObject karienemydanpen;
    private string enemyname;

    private void Start()
    {
        if (Static.enemynumber == 0)
        {
            karienemy1.SetActive(true);
            karienemy2.SetActive(false);
            karienemy3.SetActive(false);
            karienemydanpen.SetActive(false);
            enemyname = "�c�C�p";
            enemypower = 10;
        }
        if (Static.enemynumber == 1)
        {
            karienemy1.SetActive(false);
            karienemy2.SetActive(true);
            karienemy3.SetActive(false);
            karienemydanpen.SetActive(false);
            enemyname = "GPA0.68";
            enemypower = 10;
        }
        if (Static.enemynumber == 2)
        {
            karienemy1.SetActive(false);
            karienemy2.SetActive(false);
            karienemy3.SetActive(true);
            karienemydanpen.SetActive(false);
            enemyname = "����";
            enemypower = 10;
        }
        if (Static.enemynumber == 10)
        {
            karienemy1.SetActive(false);
            karienemy2.SetActive(false);
            karienemy3.SetActive(false);
            karienemydanpen.SetActive(true);
            enemyname = "�֊s�̒f��";
            enemypower = 9999;
        }
        Static.talkbox = true;
        enemy[Static.enemynumber] = enemyname + "�����ꂽ�I";
        enemyaction[Static.enemynumber] = enemyname + "�̂��������I";
        StartCoroutine(DisplayLine(enemy[Static.enemynumber], waits));
        target.text = "";
    }
    private void Update()
    {
        UI_talkbox.SetActive(Static.talkbox);
    }

    public void StartText()
    {
        Static.talkbox = true;
        // 1�������\������R���[�`�����Ăяo��
        StartCoroutine(DisplayLine(action[actionnumber], waits));
    }

    public void StartEnemy()
    {
        Static.talkbox = true;
        // 1�������\������R���[�`�����Ăяo��
        StartCoroutine(DisplayLine(enemyaction[Static.enemynumber], waits));
    }

    public void KillEnemy()
    {
        playerturn = 1;
        actionnumber = 100;
        Static.talkbox = true;
        // 1�������\������R���[�`�����Ăяo��
        StartCoroutine(DisplayLine(killedenemy[Static.enemynumber], waits));
    }

    // 1����1�������\������@�\�i����͂��̂܂܎g���܂��j
    public IEnumerator DisplayLine(string text, float seconds)
    {
        target.text = "";
        var length = text.Length;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(text[i]);
            target.text = sb.ToString();
            yield return new WaitForSeconds(seconds);
        }
        if(playerturn == 0)
        {
            yield return new WaitForSeconds(1.5f);
            target.text = "";
            Static.talkbox = false;
            playerturn = 1;
        }
        else if (playerturn == 1)
        {
            playerturn ++;
            if (actionnumber == 0)
            {
                //��������
                yield return new WaitForSeconds(1.0f);
                target.text = "";
                damagedenemy[Static.enemynumber] = enemyname + "��" + Static.playerAttack + "�_���[�W��^�����I";
                StartCoroutine(DisplayLine(damagedenemy[Static.enemynumber], waits));
                yield return new WaitForSeconds(0.5f);
                enemyparameter.HPdecrease = Static.playerAttack;
                enemyparameter.StartAttack();
            }
            else if (actionnumber == 4)
            {
                yield return new WaitForSeconds(1.5f);
                playerparameter.defence = true;
                target.text = "";
                enemyaction[Static.enemynumber] = enemyname + "�̂��������I";
                playerturn++;
                StartEnemy();
            }
            else if (actionnumber == 5)
            {
                //�ɂ��鐬��
                yield return new WaitForSeconds(1.0f);
                SceneManager.LoadScene("Main");
            }
            else if(actionnumber == 6)
            {
                yield return new WaitForSeconds(1.5f);
                target.text = "";
                enemyaction[Static.enemynumber] = enemyname + "�̂��������I";
                playerturn++;
                StartEnemy();
            }
            else if (actionnumber == 7)
            {
                //��������
                yield return new WaitForSeconds(1.0f);
                SceneManager.LoadScene("Gameover");
            }
            else if (actionnumber == 100)
            {
                //�|�����I
                yield return new WaitForSeconds(2.0f);
                SceneManager.LoadScene("Main");
                yield return new WaitForSeconds(2.0f);
            }
        }
        else if (playerturn == 2)
        {
            //�_���[�W��^����
            enemyaction[Static.enemynumber] = enemyname + "�̂��������I";
            playerturn ++;
            yield return new WaitForSeconds(1.5f);
            target.text = "";
            if (actionnumber == 100)
            { yield break; }
                StartEnemy();
        }
        else if(playerturn == 3)
        {
            //�G�̂�������
            if (playerparameter.defence)
            {
                enemyaction[Static.enemynumber] = "�䂤�����" + (enemypower-Static.playerDefence) + "�_���[�W���󂯂�";
            }
            else
            {
                enemyaction[Static.enemynumber] = "�䂤�����" + enemypower + "�_���[�W���󂯂�";
            }
            playerturn ++;
            yield return new WaitForSeconds(1.5f);
            target.text = "";
            StartEnemy();
        }
        else if(playerturn == 4)
        {
            //�_���[�W���󂯂�
            playerparameter.PHPdecrease = enemypower;
            playerparameter.StartAttack();
            yield return new WaitForSeconds(1.5f);
            target.text = "";
            Static.talkbox = false;
            buttoncontrol.control = true;
            playerturn = 1;
        }
    }
}