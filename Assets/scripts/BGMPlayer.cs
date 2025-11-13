using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    // --- ★ シングルトン化のためのインスタンス ---
    public static BGMPlayer instance;

    void Awake()
    {
        // --- シングルトン処理 ---
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 既にインスタンスが存在する場合は、このオブジェクトを破棄
            Destroy(gameObject);
        }
        // ------------------------
    }
}
