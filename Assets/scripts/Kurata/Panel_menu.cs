using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;



public class Panel_menu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject targetPanelMenu;

    [SerializeField] public Player player;

    public GameObject itemButtonPrefab; //追加する用のボタンのオブジェクト(アイテムのこと)

    public Transform buttonParentContainer;//ボタンを縦に並べるオブジェクト

    private IEnumerator Paneloff()
    {
        yield return new WaitForSeconds(3.0f);

        if (targetPanelMenu.activeSelf)
        {
            targetPanelMenu.SetActive(false);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();

        }

    }

    void ToggleMenu() //escキーを押した時
    {
        bool isCurrentlyActive = menuPanel.activeSelf;

        menuPanel.SetActive(!isCurrentlyActive);

        if (!isCurrentlyActive)
        {
            Time.timeScale = 0f;
        }
        else // メニューを閉じるとき
        {
            Time.timeScale = 1f;

            if (targetPanelMenu.activeSelf)
            {
                targetPanelMenu.SetActive(false);
            }
        }

    }




    // 💡 TogglePanelMenu() は完全に以下のコードに置き換えてください
public void TogglePanelMenu()
{
    targetPanelMenu.SetActive(true);

    // 必要な参照が設定されているかチェック
    if (itemButtonPrefab == null || buttonParentContainer == null || player == null) 
    {
        Debug.LogError("ボタン生成に必要な参照がインスペクターで設定されていません！");
        return; 
    }

    // 1. 【重要】古いボタンを全て削除する
    // メニューを開くたびにボタンが重複して増えないようにするため
    foreach (Transform child in buttonParentContainer)
    {
        Destroy(child.gameObject);
    }
    
    // 2. インベントリをループし、アイテムごとにボタンを生成
    for (int i = 0; i < player.inventory.Length; i++)
    {
        var itemData = player.inventory[i]; // 現在のアイテムデータ
        
        // 3. ボタンを生成し、親を設定
        // Instantiate(ひな型, 親オブジェクト)
        GameObject newButtonObj = Instantiate(itemButtonPrefab, buttonParentContainer);
        
        // 4. ボタンのテキストを設定
        // ボタンの子オブジェクトから TextMeshProUGUI を探して設定
        TextMeshProUGUI buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            // 💡 フィールド名が Item_name の場合は itemData.Item_name を使用
            // 💡 フィールド名が name の場合は itemData.name を使用 (一般的なのはこちら)
            buttonText.text = itemData.name; 
        }

        // 5. ボタンのOnClickイベントに機能（使う動作）を割り当てる
        Button buttonComp = newButtonObj.GetComponent<Button>();
        if (buttonComp != null)
        {
            int itemIndex = i; // ループ変数 i (インデックス) をキャプチャ
            
            // ボタンが押されたら ToggleItem(itemIndex) を実行するように設定
            buttonComp.onClick.AddListener(() => ToggleItem(itemIndex));
        }
    }
}



    public void ToggleItem(int index)
    {
        if (player.inventory.Length > index && index >= 0)
        {
            var itemToUse = player.inventory[index];//itemtouseを定義
        }
    }


}

