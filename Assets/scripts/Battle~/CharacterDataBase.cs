using UnityEngine;
using System.Collections.Generic;

// Unityエディターのメニューからデータベースを作成できるようにする
[CreateAssetMenu(fileName = "CharacterDataBase", menuName = "Character/Character Database")]
public class CharacterDataBase : ScriptableObject
{
    // すべてのキャラクターデータ (ScriptableObject) を保持するリスト
    public List<CharacterData> characters = new List<CharacterData>();
}