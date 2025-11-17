using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "CreateItem")]
public class Item : ScriptableObject
{
    public enum Type // 実装するItemの種類
    {
        UsableItem,  // 使用可能なアイテム id: 256~
        KeyItem  // 使用不可能なアイテム(恒常ステータスアップアイテムもこれに含む) id: 0~255
    }

    public enum Effects // 効果一覧
    {
        Damage,  // 敵に一定のダメージを与える
        HPRecover, // HP回復
        MPRecover, // MP回復
        AttackUp,  // 攻撃力上昇(下降)
        DefenseUp, // 防御力上昇(下降)
        MagicUP, // 魔法の威力上昇(下降)
        CriticalRateUP, // 会心発生率アップ(ダウン)
        SpeedUp,  // 素早さ上昇(下降)
        StatusUp, // 全てのステータスを上昇(下降)(会心発生率アップを除く)
        Jamming, // 敵の動きを止める
        Revive,  // 復活
        None // 何も起きない
    }

    public enum Targets // 対象
    {
        Self, // 自分を対象とする
        OneOpponent, // 敵一人をターゲットとする
        AllOpponents, // 敵全体をターゲットとする
        RandomOpponent, // 敵一人をランダムに選びターゲットとする
        None // ターゲットは存在しない
    }

    [Header("Item info")]
    public String item_name; // 名前
    public Type type; // 種類
    public String infomation; // 情報
    public String item_text; // アイテム使用時のテキスト
    public String place; // アイテムを拾う場所
    public int id;  // アイテムのID

    [Header("Item Effect")]
    public Effects effect;  // アイテム使用時の効果
    public Targets target;
    public float param;  // アイテム使用時効果の具体的なパラメータを設定する 
    // 例: Damageなら、ダメージ量 
    //     HPRecover、MPRecoverなら回復量
    //     ~Up系はステータス上昇倍率
    //     JammingやRevive その回数を指定する
    public int duration;  // アイテムの継続時間(ターン数)
    // フィールドで使用する場合はターン数に一定の定数をかけて秒に直すという仕様にする(仮)

    public float mp_cost;

    public void ApplyEffect(Figure[] targets)
    {
        foreach(Figure t in targets)
        {
            if (t == null ||( t.isDead && effect != Effects.Revive)) 
            {
                continue;
            }

            switch (effect)
            {
                case Effects.Damage:
                    // ここではthis.paramは与えるダメージ量
                    float actualDamage = t.TakeDamage(this.param);
                    Debug.Log($"{t.charaName}に{actualDamage}ダメージ与えた！");
                    break;

                case Effects.HPRecover:
                    // ここではthis.paramはHP回復量
                    float healHP = t.HealHP(this.param);
                    Debug.Log($"{t.charaName}のHPが{healHP}回復した！");
                    break;

                case Effects.MPRecover:
                    // ここではthis.paramはMP回復量
                    float healMP = t.HealMP(this.param);
                    Debug.Log($"{t.charaName}のMPが{healMP}回復した！");
                    break;

                case Effects.AttackUp:
                    // ここではthis.paramは攻撃力上昇(下降)倍率
                    // targets[i].ApplyAttackBoost(this.param, this.duration);
                    break;
                
                case Effects.DefenseUp:
                    // ここではthis.paramは防御力上昇(下降)倍率
                    // targets[i].ApplyDefenseBoost(this.param, this.duration);
                    break;

                case Effects.MagicUP:
                    // ここではthis.paramは魔法攻撃力上昇(下降)倍率
                    // これは現状未定
                    // targets[i].ApplyMagicBoost(this.param, this.duration)
                    break;

                case Effects.CriticalRateUP:
                    // ここではthis.paramは会心発生率上昇(下降)倍率
                    /// これは現状未定
                    // targets[i].ApplyCriticalRateBoost(this.param, this.duration)
                    break;

                case Effects.SpeedUp:
                    // ここではthis.paramは素早さ上昇(下降)倍率
                    t.ApplySpeedBoost(this.param, this.duration);
                    break;

                case Effects.StatusUp:
                    // ここではthis.paramは全ステータス上昇(下降)倍率
                    // これは現状未定
                    // targets[i].ApplyStatusBoost(this.param, this.duration);
                    break;

                case Effects.Jamming:
                    // ここではthis.paramはターン数
                    // targets[i].turnStuckは動けなくなっているターン数(名称は仮)
                    // targets[i].turnStuck = Mathf.RoundToInt(this.param);
                    break;

                case Effects.Revive:
                    // ここではthis.paramは何も意味をなさない
                    // targets[i].canRevive = true;
                    break;

                default:
                    Debug.Log($"効果{effect}は未実装です。ごめんね");
                    break;
            }
        }
    }
}