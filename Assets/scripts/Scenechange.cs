using UnityEngine;
using UnityEngine.SceneManagement;
public class Scenechange : MonoBehaviour
{
    [SerializeField] private Vector3 bossLoseRespawnPos = new Vector3(17.5f, 27, 0); 
    [SerializeField] private Vector3 danpenLoseRespawnPos = new Vector3(-10.5f, 36, 0); 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void buttongostart()
    {
        PlayerPrefs.SetFloat("PlayerPosX", 0);
            PlayerPrefs.SetFloat("PlayerPosY", 0);
            PlayerPrefs.SetFloat("PlayerPosZ", 0);
            
            // 書き込みを確定
            PlayerPrefs.Save();
        SceneManager.LoadScene("Main");
    }
    public void buttongomain()
    {
        if (GameManager.Instance != null && GameManager.Instance.enemyNumberToBattle == 5)
        {
            // セーブデータの座標を「安全な場所」に書き換える
            // (player_save.cs で使っているキー名 "PlayerPosX" 等を使用)
            PlayerPrefs.SetFloat("PlayerPosX", bossLoseRespawnPos.x);
            PlayerPrefs.SetFloat("PlayerPosY", bossLoseRespawnPos.y);
            PlayerPrefs.SetFloat("PlayerPosZ", bossLoseRespawnPos.z);
            
            // 書き込みを確定
            PlayerPrefs.Save();

            GameManager.Instance.playerHP += 100;
            GameManager.Instance.playerAttack += 10;
            GameManager.Instance.playerHPnow = GameManager.Instance.playerHP; // HP全回復

            Debug.Log("ボス戦での敗北を確認。安全地帯から再開します。");
        }
        else if (GameManager.Instance != null && GameManager.Instance.enemyNumberToBattle == 4)
        {
            // セーブデータの座標を「安全な場所」に書き換える
            // (player_save.cs で使っているキー名 "PlayerPosX" 等を使用)
            PlayerPrefs.SetFloat("PlayerPosX", danpenLoseRespawnPos.x);
            PlayerPrefs.SetFloat("PlayerPosY", danpenLoseRespawnPos.y);
            PlayerPrefs.SetFloat("PlayerPosZ", danpenLoseRespawnPos.z);
            
            // 書き込みを確定
            PlayerPrefs.Save();

            GameManager.Instance.playerHP += 100;
            GameManager.Instance.playerAttack += 10;
            GameManager.Instance.playerHPnow = GameManager.Instance.playerHP; // HP全回復

            Debug.Log("ボス戦での敗北を確認。安全地帯から再開します。");
        }
        else
        {
            GameManager.Instance.playerHP += 100;
            GameManager.Instance.playerAttack += 10;
            GameManager.Instance.playerHPnow = GameManager.Instance.playerHP; // HP全回復

            Debug.Log("通常の再開処理を実行します。");
        }
        
        SceneManager.LoadScene("Main");
    }
    public void buttongotitle()
    {
        SceneManager.LoadScene("Title");
        GameManager.Instance.ResetData();
    }
}