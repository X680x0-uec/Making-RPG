// ファイル名: CharacterStats.cs
using UnityEngine;
using System;

/// <summary>
/// 戦闘キャラクターの基本ステータスを管理するクラス
/// </summary>
public class CharacterStats : MonoBehaviour
{
    // 現在のHPが変更されたときに通知するイベント
    public event Action<int, int> OnHPChanged; // (現在のHP, 最大HP)
    // キャラクターが倒れたときに通知するイベント
    public event Action OnDied;

    [Header("ステータス")]
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
    /// ダメージを受ける処理
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;

        // 防御力を考慮したダメージ計算
        int actualDamage = Mathf.Max(damage - defensePower, 1);
        currentHP = Mathf.Max(currentHP - actualDamage, 0);

        // HP変更イベントを通知
        OnHPChanged?.Invoke(currentHP, maxHP);

        Debug.Log($"{gameObject.name} は {actualDamage} のダメージを受けた！ 残りHP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    protected virtual void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} は倒れた。");
        OnDied?.Invoke();
    }
}