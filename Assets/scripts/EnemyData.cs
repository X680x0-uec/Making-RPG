using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Enemy", menuName = "CreateEnemy")]
public class EnemyData : ScriptableObject
{   
    public enum Types
    {
        Normal,  // 通常の敵
        Strong,  // 強めの敵(仮)
        Boss,  // ラスボス
        Danpen  // 輪郭の断片
    }
    public Types type;  // 敵の種類 
    public string charaName;  // キャラの名前
    public float maxHP;  // HPの最大値
    public float Attack;  // 攻撃力
    public float Defense;  // 防御力
    public float Speed;  // 素早さ
    public float maxMP;  // MPの最大値
    public GameObject prefab;  // 敵の見た目
    public float report_point;  // この敵が落とすレポートポイント(仮)
    
    public EnemyData(EnemyData data)
    {
        this.type = data.type;
        this.charaName = data.charaName;
        this.maxHP = data.maxHP;
        this.Attack = data.Attack;
        this.Defense = data.Defense;
        this.Speed = data.Speed;
        this.maxMP = data.maxMP;
        this.prefab = data.prefab;
        this.report_point = data.report_point;
    }
}