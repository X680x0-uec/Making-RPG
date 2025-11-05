using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "CreateItem")]
public class Item : ScriptableObject
{
    public enum Type // 実装するItemの種類
    {
        UsableItem,  // 使用可能なアイテム
        KeyItem  // 使用不可能なアイテム(恒常ステータスアップアイテムもこれに含む)
    }

    public enum Effects // 効果一覧
    {
        Damage,  // 敵に一定のダメージを与える
        HPRecover, // HP回復
        MPRecover, // MP回復
        AttackUp,  // 攻撃力上昇
        DefenseUp, // 防御力上昇
        MagicUP, // 魔法の威力上昇
        CriticalRateUP, // 会心発生率アップ
        SpeedUp,  // 素早さ上昇
        StatusUp, // 全てのステータスを上昇(会心発生率アップを除く)
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

    public Type type; // 種類
    public String infomation; // 情報
    public String text; // アイテム使用時のテキスト
    public String place; // アイテムを拾う場所
    public int id;  // アイテムのID
    public Effects effect;  // アイテム使用時の効果
    public Targets target;
    public int param;  // アイテム使用時効果の具体的なパラメータを設定する 
    // 例: Damageなら、ダメージ量 
    //     HPRecover、MPRecoverなら回復量
    //     ~Up系はステータス上昇倍率
    //     JammingやRevive その回数を指定する

    public Item(Item item)
    {
        this.type = item.type;
        this.infomation = item.infomation;
        this.text = item.text;
        this.id = item.id;
        this.effect = item.effect;
        this.target = item.target;
        this.param = item.param;
    }
}