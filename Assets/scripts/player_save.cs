using UnityEngine;

public class player_save : MonoBehaviour
{
    // セーブデータに使う名前（キー）を固定
    private const string PosXKey = "PlayerPosX";
    private const string PosYKey = "PlayerPosY";
    private const string PosZKey = "PlayerPosZ";

    // シーン開始時に自動で呼ばれる
    void Awake()
    {
        // もし保存された座標データがあれば
        if (PlayerPrefs.HasKey(PosXKey))
        {
            // 座標を読み込んで復元する
            float x = PlayerPrefs.GetFloat(PosXKey);
            float y = PlayerPrefs.GetFloat(PosYKey);
            float z = PlayerPrefs.GetFloat(PosZKey);
            transform.position = new Vector3(x, y, z);
        }
    }

    // シーン終了時（やオブジェクトが消える時）に自動で呼ばれる
    void OnDestroy()
    {
        // 現在の座標を保存する
        PlayerPrefs.SetFloat(PosXKey, transform.position.x);
        PlayerPrefs.SetFloat(PosYKey, transform.position.y);
        PlayerPrefs.SetFloat(PosZKey, transform.position.z);
    }
}