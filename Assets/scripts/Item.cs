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

    public String item_name; // 名前
    public Type type; // 種類
    public String infomation; // 情報
    public String text; // アイテム使用時のテキスト
    public String place; // アイテムを拾う場所
    public int id;  // アイテムのID
    public Effects effect;  // アイテム使用時の効果
    public Targets target;
    public float param;  // アイテム使用時効果の具体的なパラメータを設定する 
    // 例: Damageなら、ダメージ量 
    //     HPRecover、MPRecoverなら回復量
    //     ~Up系はステータス上昇倍率
    //     JammingやRevive その回数を指定する
    public int duration;  // アイテムの継続時間(ターン数)
    // フィールドで使用する場合はターン数に一定の定数をかけて秒に直すという仕様にする(仮)

    public Item(Item item)
    {
        this.item_name = item.item_name;
        this.type = item.type;
        this.infomation = item.infomation;
        this.text = item.text;
        this.id = item.id;
        this.effect = item.effect;
        this.target = item.target;
        this.param = item.param;
        this.duration = item.duration;
    }

    //アイテム使用時のロジック
    public void Use(Figure[] targets)
    {
        // Player userではなく、List<Character> targetsとかの方がいいかもしれない(アイテムは自分だけでなく、敵(しかも複数)に対しても利用できるため)
        // この辺の仕様をどうするかは要話し合い

        // これはUseの前の方がいいかもしれない
        // Debug.Log($"{targets[0].charaName}は{this.item_name}を使った！");
        Debug.Log(this.text); // 使用時テキスト

        // 配列の中身を順に参照
        for (int i = 0; i < targets.Length; i++)
        {
            switch (this.effect)
            {
                case Effects.Damage:
                    // ここではthis.paramは与えるダメージ量
                    targets[i].TakeDamage(this.param);
                    break;

                case Effects.HPRecover:
                    // ここではthis.paramはHP回復量
                    targets[i].currentHP = Mathf.Min(targets[i].maxHP, targets[i].currentHP + this.param);
                    Debug.Log($"{targets[i].charaName}のHPが{this.param}回復した！");
                    break;

                case Effects.MPRecover:
                    // ここではthis.paramはMP回復量
                    targets[i].currentMP = Mathf.Min(targets[i].maxMP, targets[i].currentMP + this.param);
                    Debug.Log($"{targets[i].charaName}のMPが{this.param}回復した！");
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
                    targets[i].ApplySpeedBoost(this.param, this.duration);
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

            }
        }
        

        //このタイミングでUseItemメソッドによりアイテムが消費される？
    }
}