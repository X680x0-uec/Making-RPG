using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;


public class ScrollMenuController : MonoBehaviour
{
    [Header("UI参照")]
    public List<GameObject> itemArrows;            
    public List<TMPro.TextMeshProUGUI> itemTexts;
    public BattleManager manager;
    public Player player;
    private bool IsActioning = false;

    [Header("状態")]
    private int currentIndex = 0;                   // 現在選択中のアイテムのインデックス
    private int topIndex = 0;      // 3つ表示のうち、真ん中のインデックス (0, 1, 2)
    private bool isInputEnabled = false;            // 入力を受け付けているか

    void Start()
    {
        // First Selectedの強制設定
        // StartCoroutine(SetInitialSelectionDelayed(itemButtons[0].gameObject));
        // Start時にアイテムを生成
        Show();
        
    }

    void Update()
    {
        if (!isInputEnabled) return;

        int direction = 0;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = -1; // 上キーでインデックスを減らす
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = 1; // 下キーでインデックスを増やす
        }
        else if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !IsActioning)
        {
            // Enterキーで現在選択中のボタンのOnClickを実行
            // itemButtons[currentIndex].GetComponent<Button>()?.onClick.Invoke();
            StartCoroutine(Action());
            Debug.Log("通っちゃいました");
        }

        if (direction != 0)
        {
            MoveSelection(direction);
        }
    }

    IEnumerator Action(){
        IsActioning = true;
        yield return StartCoroutine(manager.PlayerUseItemRoutine(player.inventory[currentIndex]));
        Debug.Log("finished");
        IsActioning = false;
    }

    private void Show()
    {
        isInputEnabled = true;

        for (int i = topIndex; i < topIndex + 3; i++)
        {
            // First Selectedの強制設定
            if (i == currentIndex) {
                itemArrows[i - topIndex].SetActive(true);
            } else {
                itemArrows[i - topIndex].SetActive(false);
            }

            if (player.inventory.Count >= i + 1) { 
                itemTexts[i - topIndex].text = player.inventory[i].item_name; 
            } else {
                itemTexts[i - topIndex].text = "";
            }
        }
    }

    private void MoveSelection(int direction)
    {
        currentIndex = (currentIndex + direction) % player.inventory.Count;
        if (currentIndex < 0) { currentIndex = player.inventory.Count + currentIndex; }

        // relativeIndexを決定する
        if (topIndex > currentIndex)
        {
            topIndex = currentIndex;
        }
        else if (currentIndex - topIndex >= 3) 
        {
            topIndex = currentIndex - 2;
        }

        Show();

    }

}