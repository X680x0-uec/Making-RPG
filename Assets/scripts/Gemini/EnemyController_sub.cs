// �t�@�C����: EnemyController.cs
using UnityEngine;

/// <summary>
/// �퓬���̓G���Ǘ�����N���X
/// </summary>
public class EnemyController_sub : CharacterStats
{
    [HideInInspector]
    public EnemyData enemyData;

    /// <summary>
    /// EnemyData�Ɋ�Â��ēG������������
    /// </summary>
    public void Setup(EnemyData data)
    {
        enemyData = data;
        gameObject.name = data.enemyName;
        maxHP = data.maxHP;
        attackPower = data.attackPower;
        defensePower = data.defensePower;

        // base.Awake()�̏������蓮�ŌĂяo��
        currentHP = maxHP;
    }

    protected override void Die()
    {
        base.Die();
        // �����ɓG�����ꂽ���̃A�j���[�V������G�t�F�N�g�����Ȃǂ�ǉ��ł���
        gameObject.SetActive(false);
    }
}
