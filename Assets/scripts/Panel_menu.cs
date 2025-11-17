using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Panel_menu : MonoBehaviour
{
    public GameObject menuPanel;
    //public GameObject targetPanelMenu; //panel.csのtargetpanelと重複してるかも

    [SerializeField] public Player player;

    [SerializeField] public GameObject itemButtonPrefab;
    [SerializeField] public Transform buttonParentContainer;

    private BattleManager battleManager;

    //public GameObject itemButtonPrefab; //追加する用のボタンのオブジェクト(アイテムのこと)

    //public Transform buttonParentContainer;//ボタンを縦に並べるオブジェクト

    //public Animator[] commandButtonAnimators;//Button_openitemlistのアニメーション

    //public Button[] commandButtons;//Button_opemitemlist自体のボタン

    //public GameObject shopPanel;

    void Start()
    {
        battleManager = FindFirstObjectByType<BattleManager>();

        if (menuPanel != null && menuPanel.activeSelf)
        {
            menuPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (menuPanel != null && menuPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseItemPanel();
        }
    }

    private void GenerateButtons()
    {
        foreach (Transform child in buttonParentContainer)
        {
            Destroy(child.gameObject);
        }

        if (player.inventory == null)  
        {
            return;
        }

        for (int i = 0; i < player.inventory.Count; i++)
        {
            var itemData = player.inventory[i];
            if (itemData == null) continue;

            GameObject newButtonObj = Instantiate(itemButtonPrefab, buttonParentContainer);
            newButtonObj.tag = "ItemButton";

            TextMeshProUGUI buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = itemData.item_name;
            }

            Button buttonComp = newButtonObj.GetComponent<Button>();
            if (buttonComp != null)
            {
                int itemIndex = i;
                buttonComp.onClick.AddListener(() => ToggleItem(itemIndex));
            }
        }
    }

    public void OpenItemPanel()
    {
        if(player == null)
        {
            Debug.LogError("PlayerがPanel_menuに設定されていません。");
            battleManager.ReturnToPlayerTurn();
            return;
        }
        GenerateButtons();
        menuPanel.SetActive(true);

        Button firstButton = buttonParentContainer.GetChild(0).GetComponent<Button>();
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }

    public void CloseItemPanel()
    {
        menuPanel.SetActive(false);
        if (battleManager != null)
        {
            battleManager.ReturnToPlayerTurn();
        }
    }
    
    public void ToggleItem(int index)
    {
        if (player.inventory.Count > index && index >= 0)
        {
            var itemToUse = player.inventory[index]; // 使用するアイテムを特定

            // KeyItem (重要アイテム) は戦闘では使えない
            if (itemToUse.type == Item.Type.KeyItem)
            {
                Debug.Log(itemToUse.item_name + " は戦闘中には使えない。");
                return; // 処理を終了
            }

            // UsableItem (消費アイテム) の場合
            // (Start() で取得済みの 'battleManager' 変数を使う)
            if (battleManager != null && itemToUse.type == Item.Type.UsableItem)
            {
                // [修正]
                // 正しいコルーチン名 'UseItemRoutine' を 'StartCoroutine' で呼び出す
                battleManager.StartCoroutine(battleManager.UseItemRoutine(itemToUse)); 
                
                // アイテムパネルを閉じる
                if (menuPanel.activeSelf)
                {
                    menuPanel.SetActive(false);
                }
            }
        }
    }
}

    //こっから先のメソッドは競合が発生するのでコメントアウトした
    /*
    private IEnumerator Paneloff()
    {
        yield return new WaitForSeconds(3.0f);

        if (targetPanelMenu.activeSelf)
        {
            targetPanelMenu.SetActive(false);
        }
    }

    //  Buttonコンポーネントを一時的にリセットするコルーチン
    private IEnumerator ResetButtonStates()
    {
        // 全てのButtonコンポーネントを無効化
        foreach(Button button in commandButtons)
        {
            if (button != null) button.enabled = false;
        }

        yield return null;
        yield return null;
        yield return null;

        //全てのボタンを有効
        foreach(Button button in commandButtons)
        {
            if (button != null) button.enabled = true;
        }
    }

    private IEnumerator ResetAnimatorsAfterActive()
    {
        yield return new WaitForEndOfFrame(); // nullより遅い。UIが初期化されたあとに実行される

        foreach (Animator anim in commandButtonAnimators)
        {
            if (anim != null && anim != commandButtonAnimators[0])
            {
                // anim.ResetTrigger("ResetToNormal");
                // anim.SetTrigger("ResetToNormal"); // Normalステートに戻す
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()//ESCキーを押した時
    {

        bool isCurrentlyActive = menuPanel.activeSelf;
        menuPanel.SetActive(!isCurrentlyActive);

        if (!isCurrentlyActive)
        {
            Time.timeScale = 0f;
            StartCoroutine(OpenMenuSequence());
        }
        else // メニューを閉じるとき
        {
            Time.timeScale = 1f;

            if (targetPanelMenu.activeSelf)
                targetPanelMenu.SetActive(false);

            // 💡 Animatorパラメータのリセットを追加
            foreach (Animator anim in commandButtonAnimators)
            {
                if (anim != null)
                {
                    // Boolパラメータをリセット
                    anim.SetBool("Button_menu_selected", false);

                    // Trigger系も一応リセットしておく
                    anim.ResetTrigger("Button_menu_pressed");
                    anim.ResetTrigger("ResetToNormal");
                    anim.SetTrigger("ResetToNormal");
                }
            }

            StartCoroutine(ResetButtonStates());
            
            foreach (Transform child in buttonParentContainer)
            {   
                if (child.gameObject.CompareTag("ItemButton"))
                {
                    Destroy(child.gameObject);
                }
            }

        }
    }

    private IEnumerator OpenMenuSequence()
    {
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(ResetAnimatorsAfterActive());
        yield return StartCoroutine(ResetButtonStates());
        yield return StartCoroutine(SetFirstSelectedButton());
    }

    public IEnumerator SetFirstSelectedButton()
    {
        yield return new WaitForEndOfFrame();
        commandButtonAnimators[0].SetTrigger("Button_menu_highlighted");
    }

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
            if (child.gameObject.CompareTag("ItemButton"))
            {
                Destroy(child.gameObject);
            }
        }
        
        // 2. インベントリをループし、アイテムごとにボタンを生成
        for (int i = 0; i < player.inventory.Count; i++)
        {
            var itemData = player.inventory[i]; // 現在のアイテムデータ
            
            // 3. ボタンを生成し、親を設定
            // Instantiate(ひな型, 親オブジェクト)
            GameObject newButtonObj = Instantiate(itemButtonPrefab, buttonParentContainer);
            newButtonObj.tag = "ItemButton";
            
            // 4. ボタンのテキストを設定
            TextMeshProUGUI buttonText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                // 💡 フィールド名が Item_name の場合は itemData.Item_name を使用
                // 💡 フィールド名が name の場合は itemData.name を使用 (一般的なのはこちら)
                buttonText.text = itemData.item_name; 
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
        if (player.inventory.Count > index && index >= 0)
        {
            var itemToUse = player.inventory[index];//itemtouseを定義
            BattleManager bm = FindObjectOfType<BattleManager>();

            if (bm != null)
            {
                bm.UseItem(itemToUse);

                if (menuPanel.activeSelf)
                {
                    menuPanel.SetActive(false);
                }
            }
        }
    }



public void ToggleShop()
{
    if (shopPanel != null)
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }
}
*/

