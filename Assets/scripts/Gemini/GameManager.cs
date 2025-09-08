// ファイル名: GameManager.cs
using UnityEngine;

/// <summary>
/// ゲーム全体のデータを管理するシングルトンクラス
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("プレイヤーのステータス")]
    public int playerHP = 100;
    public int playerHPnow = 100;
    public int playerMP = 100;
    public int playerMPnow = 100;
    public int playerDefence = 5;
    public int playerAttack = 20;

    [Header("戦闘関連データ")]
    public int enemyNumberToBattle; // 戦闘シーンに渡す敵の番号

    private void Awake()
    {
        // シングルトンパターンの実装
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
