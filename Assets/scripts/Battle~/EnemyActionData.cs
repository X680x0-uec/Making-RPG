using UnityEngine;

// Unityエディターのメニューから行動データを作成できるようにする
[CreateAssetMenu(fileName = "NewEnemyAction", menuName = "Character/Enemy Action Data")]
public class EnemyActionData : ScriptableObject
{
    // ★追加：バフ/デバフの対象を定義するEnum
    public enum BuffTarget { None, Attack, Defense, Speed }

    [Header("行動情報")]
    public string actionName;       // 行動の名前 (例: "強烈な一撃")
    public float damageMultiplier;  // 攻撃力にかかる倍率 (例: 1.8f)
    [TextArea(3, 5)]
    public string messageText;      // メッセージボックスに表示する専用テキスト (例: "〇〇は鋭い爪を振り下ろした！")
    
    [Header("特殊効果")]
    public BuffTarget targetBuff = BuffTarget.None; // バフ/デバフの対象
    public float buffMultiplier = 1f; // 攻撃以外のバフ/デバフ効果の倍率 (例: 0.5fで素早さデバフ)
    public int buffDuration = 0;      // バフ/デバフの継続ターン数
    public bool isCharge = false;     // 溜め行動であるか (trueの場合、次ターンに何かを発動するなど)
}