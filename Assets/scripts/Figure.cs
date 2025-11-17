using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Figure : MonoBehaviour
{
    // HP変化時に呼び出される
    public event Action<float, float> OnHPChanged; // (currentHP, maxHP)

    // MP変化時に呼び出される
    public event Action<float, float> OnMPChanged; // (currentMP, maxMP)

    // 死んだとき
    public event Action OnDied;

    public string charaName;
    public float maxHP;
    public float currentHP;
    public float Attack;
    public float Defense;
    public float Speed;
    public float maxMP;
    public float currentMP;

    public bool isDead { get; private set; } = false;

    //バフ管理用プロパティ
    //protected float speedADD = 0(多分使わない)
    protected float speedMultiplier = 1f;
    protected int speedBoostTurns = 0;

    //HP,MPの更新イベント
    protected void RaiseHPChanged()
    {
        OnHPChanged?.Invoke(currentHP,maxHP);
    }
    protected void RaiseMPChanged()
    {
        OnMPChanged?.Invoke(currentMP,maxMP);
    }

    //実行素早さの取得
    public int EffectiveSpeed => Mathf.RoundToInt(Speed * speedMultiplier);

    //実行防御力のプロパティ
    public abstract int EffectiveAttack{ get; }

    //実行防御力のプロパティ
    public abstract int EffectiveDefense{ get; }

    protected virtual void Awake()
    {
        currentHP = maxHP;
    }

    //素早さバフの適用メソッド
    public void ApplySpeedBoost(float multiplier, int duration)
    {
        float speedMultiplier = multiplier;
        int speedBoostTurns = duration;
        Debug.Log($"{charaName}の素早さが上昇した！");
    }

    //素早さバフのターン経過と解除の処理メソッド
    public void DecrementSpeedBuffTurns()
    {
        if (speedBoostTurns > 0)
        {
            speedBoostTurns--;
            if (speedBoostTurns == 0)
            {
                speedMultiplier = 1f;
                Debug.Log($"{charaName}の素早さが元に戻った。");
            }
        }
    }

    //ダメージを受けるメソッド(PlayerとEnemyで防御力の計算が違うため実行防御力を使用した。)
    public virtual float TakeDamage(float damage)
    {
        float effectiveDamage = Mathf.Max(0, damage - EffectiveDefense);
        currentHP -= effectiveDamage;
        if ( currentHP <= 0 ) { currentHP = 0; isDead = true; }
        if (OnHPChanged != null) {
            OnHPChanged.Invoke(currentHP, maxHP);
        }
        return effectiveDamage;
    }


//死亡時の処理（抽象的に指定して、継承先で具体的に実装する）
protected abstract void Die();

//前場符のターンを減らす処理
public abstract void DecrementBuffTurns();

}
