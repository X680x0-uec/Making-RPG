using UnityEngine;
using System.Collections.Generic;
using System.Linq; // ★追加：Linq (LastOrDefault, Where, ToList) を使用可能にする

public class Enemy : Character
{
    // 【追加】この敵が持つ行動のリスト (Unity Editorから設定)
    public List<EnemyActionData> actionList = new List<EnemyActionData>();

    private bool Charging = false; 
    private int turnCount = 0; 

    public override int EffectiveDefense => Mathf.RoundToInt(Defense * defenseMultiplier); 

    protected  override void Die()
    {
        Debug.Log($"{charaName}を倒した！");
    }

    public override void DecrementBuffTurns()
    {
        base.DecrementBuffTurns(); 
    }

    // 敵の行動AI
    public override void PerformAction(Character target)
    {
        turnCount++;
        
        // 溜め状態のチェック
        if (Charging)
        {
            // 溜め解除後の行動を特定 (例: actionListの最後の行動を強攻撃と仮定)
            ExecuteAction(actionList.LastOrDefault(), target);
            Charging = false;
            return;
        }

        EnemyActionData chosenAction = null;

        // 3ターンに1回の「溜め行動」の判定 (ここでは actionListの最初の行動を溜めと仮定)
        if (turnCount % 3 == 0 && actionList.Count > 0 && actionList[0].isCharge)
        {
            chosenAction = actionList[0];
            Charging = true; // 溜め状態に入る
        }
        else
        {
            // それ以外の場合、溜め行動(index 0)を除いたリストからランダムに行動を選択
            var attackActions = actionList.Where(a => !a.isCharge).ToList();
            if (attackActions.Count > 0)
            {
                chosenAction = attackActions[Random.Range(0, attackActions.Count)];
            }
        }
        
        // 選択された行動を実行
        ExecuteAction(chosenAction, target);
    }
    
    // 【修正済】EnemyActionDataに基づいて行動を実行するメソッド
    private void ExecuteAction(EnemyActionData actionData, Character target)
    {
        if (actionData == null) return;
        
        Debug.Log($"[メッセージボックスへ表示] {actionData.messageText}");
        
        // バフ/デバフ効果の適用ロジック
        if (actionData.buffDuration > 0 && actionData.targetBuff != EnemyActionData.BuffTarget.None)
        {
            switch (actionData.targetBuff)
            {
                case EnemyActionData.BuffTarget.Attack:
                    ApplyAttackBoost(actionData.buffMultiplier, actionData.buffDuration);
                    break;
                case EnemyActionData.BuffTarget.Defense:
                    ApplyDefenseBoost(actionData.buffMultiplier, actionData.buffDuration);
                    break;
                case EnemyActionData.BuffTarget.Speed:
                    ApplySpeedBoost(actionData.buffMultiplier, actionData.buffDuration);
                    break;
            }
        }

        if (!actionData.isCharge) // 攻撃行動の処理
        {
            // EffectiveAttackを使用し、倍率を適用
            float damage = EffectiveAttack * actionData.damageMultiplier; 
            target.TakeDamage(damage);
            Debug.Log($"{charaName}の攻撃 ({actionData.actionName})！ {damage}のダメージ！");
        }
        else // 溜め行動の処理
        {
            Debug.Log($"{charaName}は{actionData.actionName}の準備を始めた！");
        }
    }
}