using UnityEngine;
using System.Collections.Generic;

// Unityエディターのメニューからデータを作成できるようにする
[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    public enum Side { Player, Enemy }

    [Header("基本情報")]
    public string charaName;
    public Side side;
    public int characterID; // 識別用ID

    [Header("基本ステータス")]
    public float maxHP;
    public float maxMP;
    public float Attack;
    public float Defense;
    public float Speed;
}