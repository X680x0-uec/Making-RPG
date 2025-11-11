// �t�@�C����: BattleUI.cs
using System.Collections;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// �퓬�V�[����UI�\�������ׂĊǗ�����N���X
/// </summary>
public class BattleUI : MonoBehaviour
{
    [Header("UI�v�f")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject talkBoxPanel;
    [SerializeField] private GameObject playerControlsPanel;

    [Header("�v���C���[UI")]
    [SerializeField] private TextMeshProUGUI playerHPText;

    [Header("�GUI")]
    [SerializeField] private GameObject enemyInfoPanel;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI enemyHPText;

    private PlayerController player;
    private EnemyController_sub enemy;

    /// <summary>
    /// UI�̏����ݒ�
    /// </summary>
    public void SetupUI(PlayerController p, EnemyController_sub e)
    {
        player = p;
        enemy = e;

        // �C�x���g�w��
        player.OnHPChanged += UpdatePlayerHP;
        enemy.OnHPChanged += UpdateEnemyHP;

        // �����\��
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
    /// ���b�Z�[�W���^�C�v���C�^�[�`���ŕ\������
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
            yield return new WaitForSeconds(0.05f); // 1����������̕\�����x
        }

        yield return new WaitForSeconds(waitTime);
        talkBoxPanel.SetActive(false);
    }

    /// <summary>
    /// �v���C���[�̑���p�l���̕\��/��\����؂�ւ���
    /// </summary>
    public void SetPlayerControls(bool isActive)
    {
        playerControlsPanel.SetActive(isActive);
    }

    // �I�u�W�F�N�g�j�����ɃC�x���g�w�ǂ�����
    private void OnDestroy()
    {
        if (player != null) player.OnHPChanged -= UpdatePlayerHP;
        if (enemy != null) enemy.OnHPChanged -= UpdateEnemyHP;
    }
}