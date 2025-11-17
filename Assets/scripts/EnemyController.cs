using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Figure
{
    [HideInInspector]
    public EnemyData enemyData;
    public EnemyData.Types type;
    public float report_point;

    private Panel battleUI; //メッセージ表示用
    private bool Charging = false; //溜め状態のフラグ
    private int turnCount = 0; //行動回数のカウント

    /// <summary>
    /// EnemyDataに戻づいて敵をセットアップ
    /// </summary>
    public void Setup(EnemyData data, Panel ui)
    {   
        enemyData = data; //データ保持
        battleUI = ui; //UIの参照を保持

        type = data.type;
        charaName = data.charaName;
        maxHP = data.maxHP;
        currentHP = maxHP;
        Attack = data.Attack;
        Defense = data.Defense;
        Speed = data.Speed;
        maxMP = data.maxMP;
        currentMP = maxMP;
        report_point = data.report_point;
    }

    public override int EffectiveAttack => Mathf.RoundToInt(Attack);
    public override int EffectiveDefense => Mathf.RoundToInt(Defense); //敵は防御バフがないので、実行防御力を基本防御と素早さバフのみ

    protected  override void Die()
    {
        Debug.Log($"{charaName}を倒した！");
        //ここに経験値・金の処理を入れる
    }

    // 敵のバフターンの処理（素早さバフのみ）
    /*
    public override void DecrementBuffTurns()
    {
        base.DecrementSpeedBuffTurns();
        // DecrementBuffTurns(); // ← これが無限再帰の原因
    }
    */

    //敵の行動AI（battleManagerからコルーチンとして呼び出す）
    public IEnumerator Act(Figure target)
    {
        //行動回数カウンタを1増加
        turnCount++;
        yield return battleUI.ShowMessage($"{charaName}の行動！");
        yield return new WaitForSeconds(0.5f); //行動のタメ時間

        //溜め状態のチェック（強攻撃をするか否かの確認）
        if (Charging)
        {
            yield return StartCoroutine(StrongAttack(target));
            Charging = false ; //攻撃後の溜め状態解除
        }
        else
        {
            //敵タイプに応じたAI分岐
            switch (type)
            {
                case EnemyData.Types.Normal:
                    //3ターンに1回の「溜め行動」の判定
                    if (turnCount % 3 == 0)
                    {
                        yield return StartCoroutine(Charge());
                    }  
                    else
                    {
                        yield return StartCoroutine(WeakAttack(target));
                    }
                    break;

                case EnemyData.Types.Strong:
                    //50%で溜め、溜め後は強攻撃
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        yield return StartCoroutine(Charge());
                    } 
                    else
                    {
                        yield return StartCoroutine(MiddleAttack(target));
                    }
                    break;

                case EnemyData.Types.Boss:
                    //ボスのAI
                    if (turnCount % 4 == 0)
                    {
                        yield return StartCoroutine(Charge());
                    }
                    else if (turnCount % 2 == 0)
                    {
                        yield return StartCoroutine(MiddleAttack(target));
                    }
                    else
                    {
                        yield return StartCoroutine(WeakAttack(target));
                    }
                    break;

                default:
                    yield return StartCoroutine(WeakAttack(target));
                    break;
            }
        }
            //敵のバフターン処理（素早さのみ）
            DecrementSpeedBuffTurns();
    }

    //各種行動のコルーチン化
    private IEnumerator WeakAttack(Figure target)
    {
        float damage = Attack * 1f;
        float actualDamage = target.TakeDamage(damage);
        yield return battleUI.ShowMessage($"{charaName}は軽いジャブ（精神的な意味で）を繰り出した！ {target.charaName}に{actualDamage:F0}のダメージ！");
    }

    private IEnumerator MiddleAttack(Figure target)
    {
        float damage = Attack * 1.3f;
        float actualDamage = target.TakeDamage(damage);
        yield return battleUI.ShowMessage($"{charaName}は言葉で精神をえぐってきた！ {target.charaName}に{actualDamage:F0}のダメージ！");
    }

    private IEnumerator StrongAttack(Figure target)
    {
        float damage = Attack * 1.8f;
        float actualDamage = target.TakeDamage(damage);
        yield return battleUI.ShowMessage($"{charaName}はあまりにも鋭い質問で勇者の心を打ち砕いた！ {target.charaName}に{actualDamage:F0}のダメージ！");
    }

    private IEnumerator Charge()
    {
        Charging = true;
        //溜め中は素早さを1にする
        ApplySpeedBoost(1f, 2);
        yield return battleUI.ShowMessage($"{charaName}は熟考している...");
    }
}