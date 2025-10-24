// ファイル名: BattleUI.cs
using System.Collections;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// 戦闘シーンのUI表示をすべて管理するクラス
/// </summary>
public class BattleUI : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject talkBoxPanel;
    [SerializeField] private GameObject playerControlsPanel;

    [Header("プレイヤーUI")]
    [SerializeField] private TextMeshProUGUI playerHPText;

    [Header("敵UI")]
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyHPText;

    private PlayerController player;
    private EnemyController enemy;

    /// <summary>
    /// UIの初期設定
    /// </summary>
    public void SetupUI(PlayerController p, EnemyController e)
    {
        player = p;
        enemy = e;

        // イベント購読
        player.OnHPChanged += UpdatePlayerHP;
        enemy.OnHPChanged += UpdateEnemyHP;

        // 初期表示
        enemyNameText.text = enemy.name;
        UpdatePlayerHP(player.currentHP, player.maxHP);
        UpdateEnemyHP(enemy.currentHP, enemy.maxHP);

        enemyInfoPanel.SetActive(true);
        playerControlsPanel.SetActive(false);
        talkBoxPanel.SetActive(false);
    }

    private void UpdatePlayerHP(int current, int max)
    {
        playerHPText.text = $"HP: {current} / {max}";
    }

    private void UpdateEnemyHP(int current, int max)
    {
        enemyHPText.text = $"HP: {current} / {max}";
    }

    /// <summary>
    /// メッセージをタイプライター形式で表示する
    /// </summary>
    public Coroutine ShowMessage(string message, float waitTime = 1.5f)
    {
        return StartCoroutine(ShowMessageRoutine(message, waitTime));
    }

    private IEnumerator ShowMessageRoutine(string message, float waitTime)
    {
        talkBoxPanel.SetActive(true);
        messageText.text = "";
        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(0.05f); // 1文字あたりの表示速度
        }

        yield return new WaitForSeconds(waitTime);
        talkBoxPanel.SetActive(false);
    }

    /// <summary>
    /// プレイヤーの操作パネルの表示/非表示を切り替える
    /// </summary>
    public void SetPlayerControls(bool isActive)
    {
        playerControlsPanel.SetActive(isActive);
    }

    // オブジェクト破棄時にイベント購読を解除
    private void OnDestroy()
    {
        if (player != null) player.OnHPChanged -= UpdatePlayerHP;
        if (enemy != null) enemy.OnHPChanged -= UpdateEnemyHP;
    }
}