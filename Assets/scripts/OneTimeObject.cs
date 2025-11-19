using UnityEngine;

[RequireComponent(typeof(Collider2D))] // コライダーを必須にする
public class OneTimeObject : MonoBehaviour
{
    [Tooltip("このオブジェクト固有のID。空白の場合は自動でオブジェクト名になります。")]
    public string objectID;

    [Tooltip("触れたときに実行したい処理（HP回復など）があれば、ここにplayereventなどをアタッチして連携も可能です")]
    // 今回はシンプルに「触れたら消える」機能に集中します

    void Start()
    {
        // IDが空なら、オブジェクト名をIDとして使う
        if (string.IsNullOrEmpty(objectID))
        {
            objectID = gameObject.name;
        }

        // ゲーム開始時に、すでに取得済み（登録済み）かチェック
        if (GameManager.Instance != null && GameManager.Instance.IsItemCollected(objectID))
        {
            // すでに取得済みなら、即座に消える（復活しない）
            gameObject.SetActive(false); 
            // Destroy(gameObject); // 完全に消したい場合はこちら
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーが触れたら
        if (collision.CompareTag("Player")) // 必要に応じてタグを変更してください
        {
            // 1. GameManagerに「取得した」と記録する
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCollectedItem(objectID);
                Debug.Log($"[{objectID}] を取得済みリストに登録しました。");
            }

            // 2. 効果を発動する
            // ここでplayereventなどの処理を呼び出すか、playerevent側で検知させます。
            // 今回は「playerevent側で検知して処理し、このスクリプトは消滅管理だけする」パターンを想定します。
            
            // 3. 自分を消す
            gameObject.SetActive(false);
        }
    }
}
