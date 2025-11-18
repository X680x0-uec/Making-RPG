using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class Panel : MonoBehaviour
{
    private System.Random randomGenerator = new System.Random(); 
    public GameObject targetPanel;
    public TMPro.TextMeshProUGUI displayMessageText;
    [SerializeField] private TextMeshProUGUI playerHPText;
    [SerializeField] private TextMeshProUGUI playerMPText;
    
    private Player player;
    private EnemyController enemy;

    void Start()
    {
        StartCoroutine(Paneloff());
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
        playerHPText.text = $"HP: {current} / {max}";
    }

    private void UpdatePlayerMP(float current, float max)
    {
        playerMPText.text = $"MP: {current} / {max}";
    }

    public Coroutine ShowMessage(string message, float waitTime = 1.5f, bool deactivate = true)
    {
        targetPanel.SetActive(true);
        return StartCoroutine(ShowMessageRoutine(message, waitTime, deactivate));
    }

    private IEnumerator ShowMessageRoutine(string message, float waitTime, bool deactivate)
    {
        displayMessageText.text = "";
        foreach (char c in message)
        {
            displayMessageText.text += c;
            yield return new WaitForSeconds(0.05f); // 少し待つ
        }

        yield return new WaitForSeconds(waitTime);
        if (deactivate) { targetPanel.SetActive(false); }
    }
    
    public IEnumerator Paneloff()
    {
        yield return new WaitForSeconds(3.0f);

        if (targetPanel.activeSelf)
        {
            targetPanel.SetActive(false);
        }
    }

    private IEnumerator EscapeProcess()
    {
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(Paneloff());
    }

    private void Deselect()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void TogglePanelAttack()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            StartCoroutine(Paneloff());
            Deselect();
        }
    }

    public void TogglePanelMagic()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            StartCoroutine(Paneloff());
            Deselect();
        }
    }

    public void TogglePanelDefense()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            StartCoroutine(Paneloff());
            Deselect();
        }
    }

    public void TogglePanelEscape()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            StartCoroutine(EscapeProcess());
            Deselect();
        }
    }
    
    public void TogglePanelItem()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            StartCoroutine(Paneloff());
            Deselect();
        }
    }
}
