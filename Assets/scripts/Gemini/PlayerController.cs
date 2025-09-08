// �t�@�C����: PlayerController.cs
using UnityEngine;

/// <summary>
/// �퓬���̃v���C���[���Ǘ�����N���X
/// </summary>
public class PlayerController : CharacterStats
{
    public bool isDefending { get; set; } = false;

    protected override void Awake()
    {
        // GameManager����X�e�[�^�X��ǂݍ���
        if (GameManager.Instance != null)
        {
            maxHP = GameManager.Instance.playerHP;
            currentHP = GameManager.Instance.playerHPnow;
            attackPower = GameManager.Instance.playerAttack;
            defensePower = GameManager.Instance.playerDefence;
        }
        else
        {
            // GameManager���Ȃ��ꍇ�i�e�X�g�p�j
            base.Awake();
        }
    }

    public override void TakeDamage(int damage)
    {
        // �h�䒆�͖h��͂�{�ɂ���i��j
        int originalDefense = defensePower;
        if (isDefending)
        {
            defensePower *= 2;
        }

        base.TakeDamage(damage);

        // �X�e�[�^�X�����ɖ߂�
        defensePower = originalDefense;
        isDefending = false;
    }

    // �퓬�I������GameManager��HP��ۑ�����
    public void SaveHPToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerHPnow = this.currentHP;
        }
    }
}