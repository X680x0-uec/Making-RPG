using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Figure
{
    private bool Charging = false; //溜め状態のフラグ
    private int turnCount = 0; //行動回数のカウント

    public override int EffectiveDefense => Mathf.RoundToInt(Defense); //敵は防御バフがないので、実行防御力を基本防御と素早さバフのみ

    protected  override void Die()
    {
        Debug.Log($"{charaName}を倒した！");
        //ここに経験値・金の処理を入れる
    }

    // 敵のバフターンの処理（素早さバフのみ）
    public override void DecrementBuffTurns()
    {
        DecrementBuffTurns();
    }

    //敵の行動AI
    public override void PerformAction(Figure target)
    {
        //行動回数カウンタを1増加
        turnCount++;

        //溜め状態のチェック（強攻撃をするか否かの確認）
        if (Charging)
        {
            StrongAttack(target);
            Charging = false ; //攻撃後の溜め状態解除
            return;
        }

        //3ターンに1回の「溜め行動」の判定
        if (turnCount % 3 == 0)
        {
            Charge();
        }
        //それ以外の場合の処理
        else
        {
            int actionChoice = UnityEngine.Random.Range(0,100);

            if (actionChoice < 65)
            {
                WeakAttack(target);
            }
            else
            {
                MiddleAttack(target);
            }
        }
    }

    //各種行動の具体的な定義
    private void WeakAttack(Figure target)
    {
        float damage = Attack * 1f;
        target.TakeDamage(damage);
        Debug.Log($"{charaName}はフォーマットの違いを指摘した！");
    }

    private void MiddleAttack(Figure target)
    {
        float damage = Attack * 1.3f;
        target.TakeDamage(damage);
        Debug.Log($"{charaName}は内容不備について指摘した！");
    }

    private void Charge()
    {
        Charging = true;
        Debug.Log($"{charaName}はレポートを熟読している...何か嫌な予感がする。");

        ApplySpeedBoost(0f, 2);
    }

    private void StrongAttack(Figure target)
    {
        float damage = Attack * 1.8f;
        target.TakeDamage(damage);
        Debug.Log($"{charaName}は実験意義について鋭い質問をした！");
    }
}