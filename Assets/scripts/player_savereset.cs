using UnityEngine;

public class player_savereset: MonoBehaviour
{
    // 以前のスクリプトで保存に使ったキーと、まったく同じ名前を指定
    private const string PosXKey = "PlayerPosX";
    private const string PosYKey = "PlayerPosY";
    private const string PosZKey = "PlayerPosZ";
    // もし他のデータ（スコアなど）もあれば、同様にキーを追加します
    // private const string ScoreKey = "PlayerScore";

    // このオブジェクトが有効になった時（シーンが読み込まれた時）に呼ばれる
    void Start()
    {
        // 保存されている座標データを削除
        PlayerPrefs.DeleteKey(PosXKey);
        PlayerPrefs.DeleteKey(PosYKey);
        PlayerPrefs.DeleteKey(PosZKey);

        // もしスコアなどもリセットしたければ、同様に削除
        // PlayerPrefs.DeleteKey(ScoreKey);

        // ★注意：もし全てのセーブデータを消したい場合は、以下の1行を使います
        // PlayerPrefs.DeleteAll();

        Debug.Log("プレイヤーのセーブデータをリセットしました。");
    }
}
