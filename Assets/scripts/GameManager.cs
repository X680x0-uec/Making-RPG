using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string playerName = "";
    public float playerHP = 30f;
    public float playerHPnow = 30f;
    public float playerMP = 0f;
    public float playerMPnow = 0f;
    public float playerDefence = 10f;
    public float playerAttack = 7f;

    public int enemyNumberToBattle;

    // ★ 追加：取得済みアイテムのIDリスト
    // HashSetは「重複なしリスト」で、検索が高速です
    public HashSet<string> collectedItems = new HashSet<string>();

    private void Awake()
    {
        // このオブジェクトは破壊されない
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
    // リストの中身をすべてリセット
    public void ResetCollectedItem()
    {
        collectedItems.Clear();
    }

    // ★ 追加：アイテム取得を記録するメソッド
    public void AddCollectedItem(string itemId)
    {
        if (!collectedItems.Contains(itemId))
        {
            collectedItems.Add(itemId);
        }
    }

    // ★ 追加：アイテムが取得済みか確認するメソッド
    public bool IsItemCollected(string itemId)
    {
        return collectedItems.Contains(itemId);
    }
    public void ResetData()
    {
        playerName = "勇者";
        playerHP = 30f;
        playerHPnow = 30f;
        playerMP = 0f;
        playerMPnow = 0f;
        playerDefence = 10f;
        playerAttack = 7f;
        enemyNumberToBattle = 0;
        ResetCollectedItem();
        Debug.Log("GameManagerのデータをリセットしました。");
    }

}