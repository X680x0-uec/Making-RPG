using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class playerevent : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private TextMeshProUGUI target;      // メッセージを表示するテキストエリア
    [SerializeField] private GameObject UI_talkbox;       // メッセージウィンドウ
    [SerializeField] private float waits = 0.05f;         // 文字送りの速度

    [Header("メッセージ設定")]
    [SerializeField] private string[] itemtext;
    [SerializeField] private string[] statustext;
    public int itemnumber = 0;
    private bool collision_entered = false;

    // プレイヤーのステータス操作用
    private Player player;

    void Start()
    {
        itemtext = new string[11];
        statustext = new string[11];

        itemtext[0] = "図書館「Agora」で勉強した。第二の家のように落ち着く。";
        itemtext[1] = "噴水でリフレッシュした。水は学費から賄われている。";
        itemtext[2] = "生協で買い物をした。消費税が少し多い気がする。";
        itemtext[3] = "トレーニングルームで鍛えた。筋肉は裏切らない";
        itemtext[4] = "レポートの結果と考察以外書けた。気分がいい";
        itemtext[5] = "レポートがついに完成した。ケツイがみなぎった。";
        itemtext[6] = "ザンギ丼を食べた。最高だ。";
        itemtext[7] = "こんなところにスイッチ２を見つけた。うれしくて泣きそう。";
        itemtext[8] = "眼鏡を新調した。ちょっとカッコいいよね。";
        itemtext[9] = "翼を授ける飲み物を拾った！早速飲んでみよう。";
        itemtext[10] = "モバイルバッテリーを手に入れた。大学生の生活必需品だ。";

        statustext[0] = "HPが全回復した！";
        statustext[1] = "HPが全回復した！";
        statustext[2] = "最大HPが100上がった！";
        statustext[3] = "攻撃力が50上がった！";
        statustext[4] = "最大HPが20上がった！さらに攻撃力が5、防御力が3上がった！";
        statustext[5] = "最大HPが500上がった！HPが全回復した！";
        statustext[6] = "HPが全回復した！";
        statustext[7] = "攻撃力が30上がった！";
        statustext[8] = "防御力が5上がった！HPが全回復した！";
        statustext[9] = "攻撃力が30上がった！HPが全回復した！";
        statustext[10] = "攻撃力が30上がった！HPが全回復した！";

        // プレイヤーコンポーネントを取得
        player = GetComponent<Player>();

        if (UI_talkbox != null)
        {
            UI_talkbox.SetActive(false);
        }
        if (target != null)
        {
            target.text = "";
        }
    }

    public IEnumerator DisplayLine(string text, bool deactivate = true)
    {
        if (UI_talkbox != null) UI_talkbox.SetActive(true);
        
        if (target != null)
        {
            target.text = "";
            var length = text.Length;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(text[i]);
                target.text = sb.ToString();
                yield return new WaitForSeconds(waits);
            }
        }
        
        yield return new WaitForSeconds(1.5f);
        
        // ★ 修正：deactivateがtrueの時だけパネルを閉じる
        if (deactivate)
        {
            if (target != null) target.text = "";
            if (UI_talkbox != null) UI_talkbox.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player == null || collision_entered) return;

        if (collision.CompareTag("library"))
        {
            itemnumber = 0;
            

            player.heal(player.maxHP);
            //HPを全回復
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("hunsui"))
        {
            itemnumber = 1;
            

            player.heal(player.maxHP);
            //HPを全回復
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("shop"))
        {
            itemnumber = 2;
            

            player.maxHP += 100;
            player.heal(player.maxHP);
            //HP最大値+100
            //HPを全回復
            
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("trainingroom"))
        {
            itemnumber = 3;
            player.Attack += 50;
            //攻撃力+50
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("report1"))
        {
            itemnumber = 4;
            

            player.maxHP += 20;
            player.Attack += 5;
            player.Defense += 3;
            player.heal(player.maxHP);
            //HP最大値+20
            //攻撃力+5
            //防御力+3
            
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("report2"))
        {
            itemnumber = 5;
            

            player.maxHP += 500;
            player.heal(player.maxHP);
            player.SaveHPToGameManager();
            //HP最大値+550
            //HPを全回復
            
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("zangi"))
        {
            itemnumber = 6;
            

            player.heal(player.maxHP);
            //HPを全回復
            
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("switch2"))
        {
            itemnumber = 7;
            

            player.Attack += 30;
            //攻撃力+30
            
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("megane"))
        {
            itemnumber = 8;
            

            player.Defense += 5;
            player.heal(player.maxHP);
            //防御力+5
            //HPを全回復
            
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("redbull"))
        {
            itemnumber = 9;
            

            player.Attack += 20;
            player.heal(player.maxHP);
            //攻撃力+20
            //HPを全回復
            
            StartCoroutine(TEXTEVENT());
        }
        if (collision.CompareTag("battery"))
        {
            itemnumber = 10;
            

            player.Attack += 20;
            player.heal(player.maxHP);
            //攻撃力+20
            //HPを全回復
            
            StartCoroutine(TEXTEVENT());
        }
    }
    IEnumerator TEXTEVENT()
    {
        player.SaveHPToGameManager();
        // 配列の範囲外アクセスを防ぐ安全装置
        if (itemnumber < 0 || itemnumber >= itemtext.Length)
        {
            Debug.LogError("itemnumberが配列の範囲外です: " + itemnumber);
            yield break;
        }

        collision_entered = true;
        yield return StartCoroutine(DisplayLine(itemtext[itemnumber], false));
        yield return StartCoroutine(DisplayLine(statustext[itemnumber], true));
        collision_entered = false;
    }
}