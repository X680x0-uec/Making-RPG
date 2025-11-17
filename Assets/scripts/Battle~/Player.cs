using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : Character
{
    //アイテムリストの定義
    public List<Item> inventory = new List<Item>();

    //防御中のダメ軽減
    public bool IsDefending { get; private set; } = false;

    //実行防御力(防御力・ぼうぎょコマンドを参照する)
    public override int EffectiveDefense
    {
        get
        {
            int baseDefense = Mathf.RoundToInt(Defense * defenseMultiplier); 
            // 防御中は防御力が2倍
            return IsDefending ? baseDefense * 2 : baseDefense;
        }
    }

    //全バフのターン経過と解除
    public override void DecrementBuffTurns()
    {
        IsDefending = false; //ターン終了時、防御状態解除
        base.DecrementBuffTurns(); // 基底クラスのバフ減少処理を呼び出す
    }

    protected override void Die()
    {
        Debug.Log($"{charaName}は力尽きてしまった...");
        //ここにゲームオーバー時の処理を入れる
    }

    public override void PerformAction(Character target)
    {
        // PerformActionは基本の「たたかう」を想定
        float damage = EffectiveAttack; 
        target.TakeDamage(damage);
        Debug.Log($"{charaName}の攻撃！　{damage}のダメージ！");
    }

    //呪文の実行処理（例）
    public void Spell(Character target)
    {
        if (currentMP >= 5) // 呪文の消費MPを5と仮定
        {
            currentMP -= 5;
            float damage = EffectiveAttack * 1.5f; 
            target.TakeDamage(damage);
            Debug.Log($"{charaName}は呪文を唱えた”");
        }
        else
        {
            Debug.Log($"MPが足りない！");
        }
    }

    //防御時の処理
    public void Defend()
    {
        IsDefending = true;
        Debug.Log($"{charaName}は身を守っている。");
    }

    //アイテム使用時の処理
    public void UseItem(Item item, Character target)
    {
        Debug.Log($"{charaName}は{item.item_name}を使った！"); 
        
        // ItemのUseメソッドはList<Character>を要求するので、ターゲットをリストにして渡す
        item.Use(new List<Character> { target }); 

        if (item.type == Item.Type.UsableItem)
        {
            // 消費アイテムであればインベントリから削除
            inventory.Remove(item);
        }
    }
}