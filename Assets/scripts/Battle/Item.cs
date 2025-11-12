using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "CreateItem")]
public class Item : ScriptableObject
{
    public enum Type { UsableItem, KeyItem }
    public enum Effects { Damage, HPRecover, MPRecover, AttackUp, DefenseUp, MagicUP, CriticalRateUP, SpeedUp, StatusUp, Jamming, Revive, None }
    public enum Targets { Self, OneOpponent, AllOpponents, RandomOpponent, None }

    public string item_name;
    public Type type;
    public string infomation;
    public string text;
    public string place;
    public int id;
    public Effects effect;
    public Targets target;
    public float param;  // バフ/デバフの倍率 (デバフの場合は1未満)
    public int duration;

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
    public void Use(List<Character> targets)
    {
        if (targets == null || targets.Count == 0) return;

        foreach (Character character in targets)
        {
            switch (this.effect)
            {
                case Effects.Damage:
                    character.TakeDamage(this.param);
                    break;
                case Effects.HPRecover:
                    character.currentHP = Mathf.Min(character.maxHP, character.currentHP + this.param);
                    Debug.Log($"{character.charaName}のHPが{this.param}回復した！");
                    break;
                case Effects.MPRecover: // ★追加済み
                    character.currentMP = Mathf.Min(character.maxMP, character.currentMP + this.param);
                    Debug.Log($"{character.charaName}のMPが{this.param}回復した！");
                    break;
                case Effects.AttackUp:
                    // 倍率(param)をそのまま渡すことで、デバフも可能
                    character.ApplyAttackBoost(this.param, this.duration);
                    break;
                case Effects.DefenseUp:
                    character.ApplyDefenseBoost(this.param, this.duration);
                    break;
                case Effects.MagicUP:
                    // MagicUPのロジックを実装
                    break;
                case Effects.CriticalRateUP:
                    // CriticalRateUPのロジックを実装
                    break;
                case Effects.SpeedUp:
                    character.ApplySpeedBoost(this.param, this.duration);
                    break;
                case Effects.StatusUp:
                    // StatusUpのロジックを実装
                    break;
                case Effects.Jamming:
                    // Jammingのロジックを実装
                    break;
                case Effects.Revive:
                    // Reviveのロジックを実装
                    break;
                case Effects.None:
                    Debug.Log($"しかし、何も起こらなかった...");
                    break;
            }
        }
    }
}