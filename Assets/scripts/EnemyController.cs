using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Figure
{
    [HideInInspector]
    public EnemyData enemyData;
    public EnemyData.Types type;
    public float report_point;

    /// <summary>
    /// EnemyData�Ɋ�Â��ēG������������
    /// </summary>
    public void Setup(EnemyData data)
    {   
        type = data.type;
        charaName = data.charaName;
        maxHP = data.maxHP;
        currentHP = maxHP;
        Attack = data.Attack;
        Defense = data.Defense;
        Speed = data.Speed;
        maxMP = data.maxMP;
        currentMP = maxMP;
        report_point = data.report_point;
    }
    
    private bool Charging = false; //溜め状態のフラグ
    private int turnCount = 0; //行動回数のカウント

    public override int EffectiveAttack => Mathf.RoundToInt(Attack);
    public override int EffectiveDefense => Mathf.RoundToInt(Defense); //敵は防御バフがないので、実行防御力を基本防御と素早さバフのみ

    protected  override void Die()
    {
        Debug.Log($"{charaName}を倒した！");
        //ここに経験値・金の処理を入れる
    }

    // 敵のバフターンの処理（素早さバフのみ）
    public override void DecrementBuffTurns()
    {
        // ★ 修正点：自分自身ではなく、親クラス(Figure)のメソッドを呼ぶ
        base.DecrementSpeedBuffTurns();
        // DecrementBuffTurns(); // ← これが無限再帰の原因
    }

    //敵の行動AI
    /*
    public override void PerformAction(Figure target)
    {
        //行動回数カウンタを1増加
        turnCount++;

        //溜め状態のチェック（強攻撃をするか否かの確認）
        if (Charging)
        {
            StrongAttack(target);
            Charging = false ; //攻撃後の溜め状態解除
            return;
        }

        //3ターンに1回の「溜め行動」の判定
        if (turnCount % 3 == 0)
        {
            Charge();
        }
        //それ以外の場合の処理
        else
        {
            int actionChoice = UnityEngine.Random.Range(0,100);

            if (actionChoice < 65)
            {
                WeakAttack(target);
            }
            else
            {
                MiddleAttack(target);
            }
        }
    }
    */

    //各種行動の具体的な定義
    private void WeakAttack(Figure target)
    {
        float damage = Attack * 1f;
        target.TakeDamage(damage);
        Debug.Log($"{charaName}はフォーマットの違いを指摘した！");
    }

    private void MiddleAttack(Figure target)
    {
        float damage = Attack * 1.3f;
        target.TakeDamage(damage);
        Debug.Log($"{charaName}は内容不備について指摘した！");
    }

    private void Charge()
    {
        Charging = true;
        Debug.Log($"{charaName}はレポートを熟読している...何か嫌な予感がする。");

        ApplySpeedBoost(0f, 2);
    }

    private void StrongAttack(Figure target)
    {
        float damage = Attack * 1.8f;
        target.TakeDamage(damage);
        Debug.Log($"{charaName}は実験意義について鋭い質問をした！");
    }
}

/*
public class EnemyController_sub : Figure
{
    [HideInInspector]
    public EnemyData enemyData;

    /// <summary>
    /// EnemyData�Ɋ�Â��ēG������������
    /// </summary>
    public void Setup(EnemyData data)
    {
        enemyData = data;
        gameObject.name = data.charaName;
        maxHP = data.maxHP;
        attackPower = data.Attack;
        defensePower = data.Defense;

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
*/