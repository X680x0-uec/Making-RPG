using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    //メッセージパネル
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private TextMeshProUGUI displayMessageText;

    //private System.Random randomGenerator = new System.Random(); 
    //public GameObject targetPanel;
    //public TMPro.TextMeshProUGUI displayMessageText;

    //ステータス表示
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private TextMeshProUGUI playerMPText;

    [SerializeField] private GameObject actionPanel;
    
    private Player player;
    private EnemyController enemy;

    private Coroutine messageCoroutine;

    void Start()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(false);
        }
        if (actionPanel != null)
        {
            actionPanel.SetActive(false);
        }
    }

    public void SetupUI(Player p, EnemyController e)
    {
        player = p;
        enemy = e;

        // プレイヤーのHP変更時に対応する
        player.OnHPChanged += UpdatePlayerHP;
        player.OnMPChanged += UpdatePlayerMP;

        // HP・MPの表示
        // enemyNameText.text = enemy.name;
        UpdatePlayerHP(player.currentHP, player.maxHP);
        UpdatePlayerMP(player.currentMP, player.maxMP);
    }

    private void UpdatePlayerHP(float current, float max)
    {
        if (playerHPText != null)
        {
            playerHPText.text = $"HP: {current:F0} / {max:F0}";
        }
    }

    private void UpdatePlayerMP(float current, float max)
    {
        if (playerMPText != null)
        {
            playerMPText.text = $"MP: {current:F0} / {max:F0}";
        }
    }
    
    // 追加: メッセージテキストとパネルを即座にクリアするメソッド
    public void ClearMessage()
    {
       if (messageCoroutine != null)
       {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
       }
       if (targetPanel != null)
       {
            targetPanel.SetActive(false);
       }
       if (displayMessageText != null)
       {
            displayMessageText.text = "";
       }
    }

    //メッセージ表示のコルーチン
    // deactivate: true のとき、メッセージを自動で閉じる
    public IEnumerator ShowMessage(string message, bool deactivate = true, float duration = 1.5f)
    {
        if (targetPanel == null || displayMessageText == null)
        {
            Debug.LogError("targetPanelまたはdisplayMessageTextが設定されていません。");
            yield break;
        }

        ClearMessage();

        // 新しいメッセージを表示する前に、念のためテキストをクリアする（パネルはClearMessage()で非アクティブ化されているはず）
        displayMessageText.text = message; 
        targetPanel.SetActive(true);

        if (deactivate)
        {
            //指定時間待機する
            yield return new WaitForSeconds(duration);
            //待機後に非表示
            targetPanel.SetActive(false);
        }
        else
        {
            //メッセージを表示したままにする
            messageCoroutine = null;
        }
    }

    public void ShowActionPanel(bool show)
    {
        if (actionPanel != null)
        {
            actionPanel.SetActive(show);
            if(show)
            {
                //最初のボタンを選択状態にする
                Button firstButton = actionPanel.GetComponentInChildren<Button>();
                if (firstButton != null)
                {
                    EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
                }
            }
            else
            {
                Deselect();
            }
        }
    }

    private void Deselect()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // 各TogglePanelメソッドの末尾のPaneloff()は、メッセージを表示し、3秒後に消すという元の動作を維持します。
    // BattleManager側でメッセージを出す前にClearMessage()を呼ぶことで対応します。

    // TogglePanelAttackなどはUIのOnclickからBattleManagerを直接呼ぶようにした
}