using System.Collections.Generic;
using UnityEngine;

public class Player : Figure
{
    //アイテムリストの定義
    public List<Item> inventory = new List<Item>();

    //バフ管理
    private float attackMultiplier = 1f;
    private int attackBoostTurns = 0;
    private float defenseMultiplier = 1f;
    private int defenseBoostTurns = 0;

    //防御中のダメ軽減
    public bool IsDefending { get; private set; } = false;

    //実行防御力(防御力・ぼうぎょコマンドを参照する)
    public override int EffectiveDefense
    {
        get
        {
            int baseDefense = Mathf.RoundToInt(Defense * defenseMultiplier);
            // ぼうぎょコマンド中の防御力増加計算
            return IsDefending ? baseDefense * 2 : baseDefense;
        }
    }

    //攻撃力バフの適用
    public void ApplyAttackBoost(float multiplier, int duration)
    {
        attackMultiplier = multiplier;
        attackBoostTurns = duration;
        Debug.Log($"{charaName}の攻撃力が一時的に上昇した！");
    }

    //防御力バフの適用
    public void ApplyDefenseBoost(float multiplier, int duration)
    {
        defenseMultiplier = multiplier;
        defenseBoostTurns = duration;
        Debug.Log($"{charaName}の防御力が一時的に上昇した！");
    }


    //全バフのターン経過と解除
    public override void DecrementBuffTurns()
    {
        IsDefending = false; //ターン終了時、防御状態解除

        //攻撃力バフの処理
        if (attackBoostTurns > 0)
        {
            attackBoostTurns--;
            if (attackBoostTurns == 0) 
            { 
                attackMultiplier = 1f; 
                Debug.Log($"{charaName}の攻撃力が元に戻った。");
            }
        }

        //防御バフの処理
        if (defenseBoostTurns > 0)
        {
            defenseBoostTurns--;
            if (defenseBoostTurns == 0) 
            {
                defenseMultiplier = 1f; 
                Debug.Log($"{charaName}の防御力が元に戻った。");
            }
        }

        //素早さバフの処理
        DecrementBuffTurns();
    }


    //Player固有の処理
    protected override void Die()
    {
        Debug.Log($"{charaName}は力尽きてしまった...");
        //ここにゲームオーバー時の処理を入れる
    }


    //ここからは戦闘時の処理の設定
    public override void PerformAction(Figure target)
    {
        float damage = Mathf.RoundToInt(Attack * attackMultiplier);
        target.TakeDamage(damage);
        Debug.Log($"{charaName}の攻撃！　{damage}のダメージ！");
    }

    //呪文の実行処理（例）
    public void Spell(Figure target)
    {
        if (currentMP >= 5) //コスト設定
        {
            currentMP -= 5;
            float damage = Attack * attackMultiplier * 1.5f;
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
        //ステータス関連はBattle-Systemで実装する
    }

    //アイテム使用時の処理
    public void UseItem(Item item)
    {
        Debug.Log($"{charaName}は{item.item_name}を使った！");
        
        // item.Use(this); //Itemクラスに格納されているUseメソッドを呼び出している

        inventory.Remove(item);
    }
}

