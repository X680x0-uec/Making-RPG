using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "CreateItem")]
public class Item : ScriptableObject
{
    public enum Type // 実装するItemの種類
    {
        UsableItem,  // 使用可能なアイテム
        KeyItem  // 使用不可能なアイテム
    }

    public enum Effects // 効果一覧
    {
        Damage,  // 敵に一定のダメージを与える
        AttackUp,  // 攻撃力上昇
        DefenseUp, // 防御力上昇
        MagicUP, // 魔法の威力上昇
        CriticalRateUP, // 会心発生率アップ
        Revive,  // 一度だけ復活
        None // 何も起きない
        // SpeedUp,  // 素早さ上昇
    }

    public Type type; // 種類
    public String infomation; // 情報
    public String text; // アイテム使用時のテキスト
    public int id;  // アイテムのID
    public Effects effect;  // アイテム使用時の効果

    public Item(Item item)
    {
        this.type = item.type;
        this.infomation = item.infomation;
        this.text = item.text;
        this.id = item.id;
        this.effect = item.effect;
    }
}