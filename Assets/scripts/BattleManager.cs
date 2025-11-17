using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // UI操作に必要

public class BattleManager : MonoBehaviour
{
    // BUSY ステートを追加
    public enum BattleState { SETUP, PLAYERTURN, ENEMYTURN, BUSY, WIN, LOSE }
    public BattleState currentState;

    [SerializeField] private Player player;
    [SerializeField] private Transform enemySpawnPoint;
    private EnemyController enemy;
    // private bool isUsingItemPanel = false; // (onGoingフラグで代替)

    [SerializeField] private List<EnemyData> enemyDatabase; // ScriptableObject

    [SerializeField] private Panel battleUI;
    
    // GameObject ではなく Panel_menu スクリプトへの参照
    [SerializeField] private Panel_menu itemPanel; 
    // private Vector2 rightTransform; // (Panel_menu 側で制御)
    // private RectTransform transform; // (同上)

    private bool onGoing = false; // プレイヤーターンの行動選択待ちフラグ

    void Start()
    {
        // (Panel_menu 側で制御)
        // rightTransform = ItemPanel.GetComponent<RectTransform>().anchoredPosition;
        
        // 参照が未設定の場合、自動で検索
        if (itemPanel == null)
        {
            itemPanel = FindFirstObjectByType<Panel_menu>();
        }
        if (battleUI == null)
        {
            battleUI = FindFirstObjectByType<Panel>();
        }
        if (player == null)
        {
             player = FindFirstObjectByType<Player>();
        }
        
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        currentState = BattleState.SETUP;

        // -----------------------------------------------------
        // バグ修正: 敵がフラスコに固定されるのを解消
        // -----------------------------------------------------
        int enemyId = 0;
        if (GameManager.Instance != null && GameManager.Instance.enemyNumberToBattle != 0)
        {
            // GameManagerでIDが指定されている場合 (ボス戦など)
            enemyId = GameManager.Instance.enemyNumberToBattle;
        }
        else if (enemyDatabase != null && enemyDatabase.Count > 0)
        {
            // 応急処置: GameManagerで指定がない場合、ランダムに選択
            enemyId = Random.Range(0, enemyDatabase.Count);
        }
        else
        {
            Debug.LogError("敵データベースが空か、設定されていません！");
            yield break;
        }
        // -----------------------------------------------------

        if (enemyDatabase.Count <= enemyId)
        {
            Debug.LogError($"敵ID {enemyId} はデータベースの範囲外です。ID 0 をロードします。");
            enemyId = 0;
            if (enemyDatabase.Count == 0) yield break;
        }

        EnemyData enemyToLoad = enemyDatabase[enemyId];
        GameObject enemyInstance = Instantiate(enemyToLoad.prefab, enemySpawnPoint);
        enemy = enemyInstance.GetComponent<EnemyController>();
        
        // SetupにbattleUIを渡す
        enemy.Setup(enemyToLoad, battleUI); 

        // SetupUIの呼び出し
        battleUI.SetupUI(player, enemy);

        // player及びenemyがやられたときの処理
        player.OnDied += OnPlayerDied;
        enemy.OnDied += OnEnemyDied;

        yield return battleUI.ShowMessage($"{enemy.charaName}が現れた！");

        StartPlayerTurn();
    }

    // プレイヤーターン開始
    private void StartPlayerTurn()
    {
        currentState = BattleState.PLAYERTURN;
        player.ResetFlags(); // 防御フラグなどをリセット
        
        // プレイヤーのバフターンを減らす
        player.DecrementSpeedBuffTurns(); 
        
        StartCoroutine(PlayerTurnRoutine());
    }

    // プレイヤーの行動選択待ち
    private IEnumerator PlayerTurnRoutine()
    {
        battleUI.ClearMessage();
        yield return battleUI.ShowMessage("プレイヤーのターン", duration: 0.5f);
        
        battleUI.ShowActionPanel(true); // 攻撃・防御などのボタンを表示
        onGoing = true; // 行動選択待ち

        // onGoingがfalseになるまで（＝ボタンが押されるまで）待機
        while (onGoing)
        {
            yield return null;
        }
        
        battleUI.ShowActionPanel(false); // ボタンを非表示
    }

    // メッセージだけ表示する汎用コルーチン
    public IEnumerator ShowMessageRoutine(string message)
    {
        currentState = BattleState.BUSY;
        yield return battleUI.ShowMessage(message);
        currentState = BattleState.PLAYERTURN;
        ReturnToPlayerTurn(); // 再度選択に戻る
    }

    #region UI Button Calls
    //UIの「攻撃」ボタンから呼び出す
    public void OnAttackButton()
    {
        if (currentState != BattleState.PLAYERTURN || !onGoing) return;
        onGoing = false;
        StartCoroutine(AttackRoutine());
    }

    //UIの「防御」ボタンから呼び出す
    public void OnDefenseButton()
    {
        if (currentState != BattleState.PLAYERTURN || !onGoing) return;
        onGoing = false;
        StartCoroutine(DefenseRoutine());
    }

    //UIの「呪文」ボタンから呼び出す
    public void OnMagicButton()
    {
        if (currentState != BattleState.PLAYERTURN || !onGoing) return;
        onGoing = false;
        StartCoroutine(MagicRoutine());
    }

    //UIの「アイテム」ボタンから呼び出す
    public void OnItemButton()
    {
        if (currentState != BattleState.PLAYERTURN || !onGoing) return;
        onGoing = false; // ターン進行を止める
        currentState = BattleState.BUSY; // アイテム選択中

        if (itemPanel != null)
        {
            itemPanel.gameObject.SetActive(true);
            itemPanel.OpenItemPanel();
        }
        else
        {
            Debug.LogError("Item_PanelがBattleManagerに設定されていません。");
            ReturnToPlayerTurn();
        }
    }
    
    //アイテムパネルをキャンセルした時に Panel_menu から呼ばれる
    public void ReturnToPlayerTurn()
    {
        // プレイヤーのターン中、またはアイテム選択中(BUSY)だった場合
        if (currentState == BattleState.PLAYERTURN || currentState == BattleState.BUSY) 
        {
            StartCoroutine(PlayerTurnRoutine()); // プレイヤーの行動選択に戻る
        }
    }
    #endregion

    #region Player Action Routines (補完・修正)
    //攻撃処理
    private IEnumerator AttackRoutine()
    {
        currentState = BattleState.BUSY;
        
        yield return battleUI.ShowMessage($"{player.charaName}の攻撃！");
        
        // TODO: 攻撃アニメーションなど
        // yield return new WaitForSeconds(0.5f);
        
        float damageDealt = enemy.TakeDamage(player.EffectiveAttack);
        
        yield return battleUI.ShowMessage($"{enemy.charaName}に{damageDealt:F0}のダメージを与えた！");

        // 敵が死んでいなければエネミーターンへ
        if (!enemy.isDead)
        {
            yield return StartCoroutine(EnemyTurnRoutine());
        }
        // (敵が死んだ場合の処理は OnEnemyDied が行う)
    }

    //防御処理
    private IEnumerator DefenseRoutine()
    {
        currentState = BattleState.BUSY;
        player.Defend(); // 防御フラグを立てる
        
        yield return battleUI.ShowMessage($"{player.charaName}は身を守っている。");
        
        // 防御は即時エネミーターンへ
        yield return StartCoroutine(EnemyTurnRoutine());
    }

    // 呪文処理
    private IEnumerator MagicRoutine()
    {
        currentState = BattleState.BUSY;

        float damageDealt;
        bool success = player.Spell(enemy, out damageDealt); // 呪文実行

        if (success)
        {
            yield return battleUI.ShowMessage($"{player.charaName}は呪文を唱えた！");
            yield return battleUI.ShowMessage($"{enemy.charaName}に{damageDealt:F0}のダメージ！");

            if (!enemy.isDead)
            {
                yield return StartCoroutine(EnemyTurnRoutine());
            }
        }
        else
        {
            // MP不足
            yield return battleUI.ShowMessage($"MPが足りない！");
            // ターンを消費せず、行動選択に戻る
            StartCoroutine(PlayerTurnRoutine());
        }
    }
    
    // バグ修正: アイテム使用（行動不能・効果なし）
    // (元の UseItem(Item item) [void型] は削除)
    // Panel_menuから呼ばれるコルーチン
    public IEnumerator UseItemRoutine(Item item)
    {
        currentState = BattleState.BUSY;
        
        // (Player.csに処理を移譲)
        player.UseItem(item, enemy); // アイテム使用とインベントリ削除

        yield return battleUI.ShowMessage($"{player.charaName}は {item.name} を使った！");

        // アイテムの効果テキスト表示
        if (!string.IsNullOrEmpty(item.item_text))
        {
            yield return battleUI.ShowMessage(item.item_text);
        }
        
        // 敵が死んでおらず、プレイヤーも死んでいなければエネミーターンへ
        if (!enemy.isDead && !player.isDead)
        {
            yield return StartCoroutine(EnemyTurnRoutine());
        }
        // (死亡判定はイベントハンドラが処理する)
    }
    #endregion

    // 敵のAIの呼び出し
    public IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.ENEMYTURN;
        
        // 敵のAI行動コルーチンを呼び出す
        yield return StartCoroutine(enemy.Act(player));

        // プレイヤーが死んでいなければプレイヤーターンへ
        if (!player.isDead)
        {
            StartPlayerTurn();
        }
        // (プレイヤーが死んだ場合の処理は OnPlayerDied が行う)
    }

    #region Win/Lose Handlers (修正・補完)
    // 敵が死んだら呼ばれる
    private void OnEnemyDied()
    {
        if (currentState == BattleState.WIN) return; // 既に勝利処理中
        currentState = BattleState.WIN;
        StopAllCoroutines(); // 全ての行動を中断
        StartCoroutine(WinRoutine());
    }

    // プレイヤーが死んだら呼ばれる
    private void OnPlayerDied()
    {
        if (currentState == BattleState.LOSE) return; // 既に敗北処理中
        currentState = BattleState.LOSE;
        StopAllCoroutines(); // 全ての行動を中断
        StartCoroutine(LoseRoutine());
    }

    private IEnumerator WinRoutine()
    {
        player.SaveHPToGameManager(); // HPをGameManagerに保存する
        
        // (敵の消滅アニメーションなど)
        // Destroy(enemy.gameObject); // メッセージ表示後にする
        
        battleUI.ClearMessage(); 
        yield return battleUI.ShowMessage($"{enemy.charaName}を倒した！", deactivate: false); // メッセージを消さない
        
        yield return new WaitForSeconds(2.0f); // 勝利の余韻
        
        SceneManager.LoadScene("Main"); // Mainシーンに切り替え
    }

    private IEnumerator LoseRoutine()
    {
        battleUI.ClearMessage(); 
        yield return battleUI.ShowMessage($"{player.charaName}はやられてしまった...", deactivate: false); // メッセージを消さない
        
        yield return new WaitForSeconds(3.0f);
        
        SceneManager.LoadScene("Gameover"); // Gameoverシーンに切り替え
    }
    #endregion

    // シーン破棄時にイベント購読を解除
    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnDied -= OnPlayerDied;
        }
        if (enemy != null)
        {
            enemy.OnDied -= OnEnemyDied;
        }
    }
}