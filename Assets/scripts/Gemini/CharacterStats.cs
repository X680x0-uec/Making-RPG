// �t�@�C����: CharacterStats.cs
using UnityEngine;
using System;

/// <summary>
/// �퓬�L�����N�^�[�̊�{�X�e�[�^�X���Ǘ�����N���X
/// </summary>
public class CharacterStats : MonoBehaviour
{
    // HP変更時
    public event Action<int, int> OnHPChanged; // (currentHP, maxHP)
    
    // MP変更時
    public event Action OnDied;

    [Header("�X�e�[�^�X")]
    public int maxHP = 100;
    public int currentHP { get; protected set; }
    public int attackPower = 10;
    public int defensePower = 5;

    public bool isDead { get; private set; } = false;

    protected virtual void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// �_���[�W���󂯂鏈��
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        // �h��͂��l�������_���[�W�v�Z
        int actualDamage = Mathf.Max(damage - defensePower, 1);
        currentHP = Mathf.Max(currentHP - actualDamage, 0);

        // HP�ύX�C�x���g��ʒm
        OnHPChanged?.Invoke(currentHP, maxHP);

        Debug.Log($"{gameObject.name} �� {actualDamage} �̃_���[�W���󂯂��I �c��HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// ���S����
    /// </summary>
    protected virtual void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} �͓|�ꂽ�B");
        OnDied?.Invoke();
    }
}