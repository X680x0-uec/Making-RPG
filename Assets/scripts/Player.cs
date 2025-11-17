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
    public float baseAttackMultipier = 1f;
    public float baseDefenseMultipier = 1f;

    //防御中のダメ軽減
    public bool isDefending { get; set; } = false;

    protected override void Awake()
    {
        // GameManagerからステータスを読み込む
        if (GameManager.Instance != null)
        {
            charaName = GameManager.Instance.playerName;
            maxHP = GameManager.Instance.playerHP;
            currentHP = GameManager.Instance.playerHPnow; //バトル後もHPを引き継ぐようにした
            maxMP = GameManager.Instance.playerMP;
            currentMP = GameManager.Instance.playerMPnow; //MPも同様
            Attack = GameManager.Instance.playerAttack;
            Defense = GameManager.Instance.playerDefence;

            //GameManagerにSpeedがなかったからデフォルトで値を設定する
            Speed = Speed == 0 ? 10f : Speed;
        }
        else
        {
            // GameManagerがない場合
            base.Awake();
            Speed = 10f;
            charaName = "Player";
        }
    }

    //実行攻撃力
    public override int EffectiveAttack
    {
        get 
        {
            return Mathf.RoundToInt(Attack * attackMultiplier); 
        }
    }

    //実行防御力(防御力・ぼうぎょコマンドを参照する)
    public override int EffectiveDefense
    {
        get
        {
            int finalDefence = (int)(Defense * defenseMultiplier);
            // ぼうぎょコマンド中の防御力増加計算
            if (isDefending)
            {
                finalDefence *= 2; //防御中は2倍に
            }
            return Mathf.RoundToInt(finalDefence);
        }
    }

    //HPをGameManagerに保存
    public void SaveHPToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerHPnow = this.currentHP;
            GameManager.Instance.playerMPnow = this.currentMP;
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


    //全バフのターン経過と解除(BattleManagerから呼び出す)
    public override void DecrementSpeedBuffTurns()
    {
        isDefending = false; //ターン終了時、防御状態解除

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
        base.DecrementSpeedBuffTurns();
    }


    //Player固有の処理
    protected override void Die()
    {
        Debug.Log($"{charaName}は力尽きてしまった...");
        //イベントはTakeDamageにて行う
    }

    //呪文の実行処理（例）
    public bool Spell(Figure target, out float damage)
    {
        damage = 0;
        float cost = 5f;

        if (ConsumeMP(cost))
        {
            damage = Attack * attackMultiplier * 1.5f;
            target.TakeDamage(damage);
            Debug.Log($"{charaName}は呪文を唱えた”");
            return true;
        }
        else
        {
            Debug.Log($"MPが足りない！");
            return false;
        }
    }

    //防御時の処理
    public void Defend()
    {
        isDefending = true;
        Debug.Log($"{charaName}は身を守っている。");
    }

    //ターン開始時に防御状態をリセット
    public void ResetFlags()
    {
        isDefending = false;
    }

    //アイテム使用時の処理(Item.csのApplyEffectを呼び出し)
    public void UseItem(Item item, Figure target)
    {
        //ターゲットを配列で渡す
        Figure[] targets;

        switch(item.target)
        {
            case Item.Targets.Self:
                targets = new Figure[] { this };
                break;
            case Item.Targets.OneOpponent:
                targets = new Figure[] { target }; //一応敵を想定
                break;
            case Item.Targets.AllOpponents:
                targets = new Figure[] { target }; //今のところ複数体呼び出すような処理がなさそうなのでこのままで...
                break;
            default:
                targets = new Figure[] { this }; //不明な時は自分を指定
                break;
        }
        //Debug.Log($"{charaName}は{item.item_name}を使った！");
        
        // item.Use(this); //Itemクラスに格納されているUseメソッドを呼び出している
        item.ApplyEffect(targets);
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
        }
    }

}

