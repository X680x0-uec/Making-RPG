using UnityEngine;

public abstract class Character : MonoBehaviour
{
    // 識別用のID (リスト管理で使用)
    public int CharacterID { get; private set; } 

    // 基本ステータス（CharacterDataからロードされる）
    public string charaName;
    public float maxHP;
    public float currentHP;
    public float Attack;
    public float Defense;
    public float Speed;
    public float maxMP;
    public float currentMP;

    // バフ/デバフ管理用プロパティ（変動するパラメータ。倍率が1未満でデバフ）
    protected float attackMultiplier = 1f;
    protected int attackBoostTurns = 0;
    protected float defenseMultiplier = 1f;
    protected int defenseBoostTurns = 0;
    protected float speedMultiplier = 1f;
    protected int speedBoostTurns = 0;

    // 初期化メソッド（BattleSystemから呼び出し、IDを割り当てる）
    public void Initialize(int id)
    {
        CharacterID = id;
    }

    // 実行攻撃力の取得（バフ/デバフを適用）
    public int EffectiveAttack => Mathf.RoundToInt(Attack * attackMultiplier);

    // 実行素早さの取得（バフ/デバフを適用）
    public int EffectiveSpeed => Mathf.RoundToInt(Speed * speedMultiplier);

    // 実行防御力のプロパティ（継承先で防御コマンドなどを考慮）
    public abstract int EffectiveDefense{ get; }

    // 攻撃力バフ/デバフの適用メソッド
    public void ApplyAttackBoost(float multiplier, int duration)
    {
        attackMultiplier = multiplier;
        attackBoostTurns = duration;
        string message = multiplier >= 1f ? "上昇" : "下降";
        Debug.Log($"{charaName}の攻撃力が一時的に{message}した！ (倍率: {multiplier}, ターン: {duration})");
    }

    // 防御力バフ/デバフの適用メソッド
    public void ApplyDefenseBoost(float multiplier, int duration)
    {
        defenseMultiplier = multiplier;
        defenseBoostTurns = duration;
        string message = multiplier >= 1f ? "上昇" : "下降";
        Debug.Log($"{charaName}の防御力が一時的に{message}した！ (倍率: {multiplier}, ターン: {duration})");
    }

    // 素早さバフ/デバフの適用メソッド
    public void ApplySpeedBoost(float multiplier, int duration)
    {
        speedMultiplier = multiplier;
        speedBoostTurns = duration;
        string message = multiplier >= 1f ? "上昇" : "下降";
        Debug.Log($"{charaName}の素早さが一時的に{message}した！ (倍率: {multiplier}, ターン: {duration})");
    }

    protected void DecrementAttackBuffTurns()
    {
        if (attackBoostTurns > 0)
        {
            attackBoostTurns--;
            if (attackBoostTurns == 0)
            {
                attackMultiplier = 1f;
                Debug.Log($"{charaName}の攻撃力が元に戻った。");
            }
        }
    }

    protected void DecrementDefenseBuffTurns()
    {
        if (defenseBoostTurns > 0)
        {
            defenseBoostTurns--;
            if (defenseBoostTurns == 0)
            {
                defenseMultiplier = 1f;
                Debug.Log($"{charaName}の防御力が元に戻った。");
            }
        }
    }

    protected void DecrementSpeedBuffTurns()
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

    public virtual void TakeDamage(float damage)
    {
        // ダメージ計算： (攻撃力 - 有効防御力) 。最低ダメージは0
        float effectiveDamage = Mathf.Max(0, damage - EffectiveDefense); 
        currentHP -= effectiveDamage;
        Debug.Log($"{charaName}は{effectiveDamage}のダメージを受けた！");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // 全バフのターン数を減らす
    public virtual void DecrementBuffTurns()
    {
        DecrementAttackBuffTurns();
        DecrementDefenseBuffTurns();
        DecrementSpeedBuffTurns();
    }

    protected abstract void Die();
    public abstract void PerformAction(Character target);
}