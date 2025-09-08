// ファイル名: EnemyData.cs
using UnityEngine;

/// <summary>
/// 敵の基本パラメータを保持するScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "New EnemyData", menuName = "RPG/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("基本情報")]
    public string enemyName = "敵の名前";
    public GameObject prefab; // 敵の見た目となるプレハブ

    [Header("戦闘パラメータ")]
    public int maxHP = 50;
    public int attackPower = 10;
    public int defensePower = 0;
}
