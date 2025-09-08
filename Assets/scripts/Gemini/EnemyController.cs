// ファイル名: EnemyController.cs
using UnityEngine;

/// <summary>
/// 戦闘中の敵を管理するクラス
/// </summary>
public class EnemyController : CharacterStats
{
    [HideInInspector]
    public EnemyData enemyData;

    /// <summary>
    /// EnemyDataに基づいて敵を初期化する
    /// </summary>
    public void Setup(EnemyData data)
    {
        enemyData = data;
        gameObject.name = data.enemyName;
        maxHP = data.maxHP;
        attackPower = data.attackPower;
        defensePower = data.defensePower;

        // base.Awake()の処理を手動で呼び出す
        currentHP = maxHP;
    }

    protected override void Die()
    {
        base.Die();
        // ここに敵がやられた時のアニメーションやエフェクト処理などを追加できる
        gameObject.SetActive(false);
    }
}
