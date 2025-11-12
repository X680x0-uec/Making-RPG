using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public enum BattleState { Start, Player_turn, Enemy_turn, Perform_action, Won, Lost, Run}
public enum BattleCommand { ATTACK, SPELL, DEFEND, ITEM, RUN} 

public class BattleSystem : MonoBehaviour
{
    // 【データ駆動型リスト管理のための参照】
    public CharacterDataBase characterDatabase;

    // 【キャラクターリストの定義】
    public List<Character> activeCharacters = new List<Character>();
    public List<Player> playerCharacters = new List<Player>();
    public List<Enemy> enemyCharacters = new List<Enemy>();
    
    // 戦闘中のカレントプレイヤー（簡易的なターン管理用）
    private Player currentActivePlayer => playerCharacters.FirstOrDefault(p => p.currentHP > 0);
    private Enemy currentTargetEnemy => enemyCharacters.FirstOrDefault(e => e.currentHP > 0);


    public BattleState state;
    private BattleCommand selectedCommand;

    public GameObject commandPanel;
    public Text statusText;
    public GameObject itemSelectionPanel;
    private int selectedItemID = -1; 

    void Start()
    {
        state = BattleState.Start;
        if (commandPanel != null) commandPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);

        StartCoroutine(SetupBattle());
    }

    //戦闘開始フェーズ：キャラクターデータをロードし、インスタンス化する
    IEnumerator SetupBattle()
    {
        if (characterDatabase == null)
        {
            Debug.LogError("CharacterDataBaseが設定されていません！");
            yield break;
        }

        // データベースからキャラクターを生成し、リストに格納
        foreach (var data in characterDatabase.characters)
        {
            Character newChara = null;
            
            // 敵/味方の識別とコンポーネントの付与
            if (data.side == CharacterData.Side.Player)
            {
                newChara = gameObject.AddComponent<Player>(); 
                playerCharacters.Add((Player)newChara);
            }
            else if (data.side == CharacterData.Side.Enemy)
            {
                newChara = gameObject.AddComponent<Enemy>();
                enemyCharacters.Add((Enemy)newChara);
            }

            if (newChara != null)
            {
                // ScriptableObjectのデータに基づいてステータスを設定
                newChara.charaName = data.charaName;
                newChara.maxHP = newChara.currentHP = data.maxHP;
                newChara.maxMP = newChara.currentMP = data.maxMP;
                newChara.Attack = data.Attack;
                newChara.Defense = data.Defense;
                newChara.Speed = data.Speed;
                newChara.Initialize(data.characterID);
                activeCharacters.Add(newChara);
            }
        }
        
        Debug.Log($"戦闘開始！データベースから {activeCharacters.Count}体のキャラクターをロードしました。");
        
        // 敵リストの最初の敵の名前を表示
        if (enemyCharacters.Count > 0)
        {
            Debug.Log($"**{enemyCharacters[0].charaName}が現れた！**");
        }
        yield return new WaitForSeconds(1f);

        DetermineTurnOrder();
    }

    //ターン順決定 (簡略化し、プレイヤーから開始)
    void DetermineTurnOrder()
    {
        // 本来はactiveCharactersリストをEffectiveSpeedでソートするが、ここではプレイヤー先行で固定
        state = BattleState.Player_turn;
        PlayerTurn(currentActivePlayer);
    }

    //プレイヤーのターン
    void PlayerTurn(Player playerTurn)
    {
        if (playerTurn == null) return;

        if (statusText != null)
        {
            statusText.text = $"**{playerTurn.charaName}のターン：コマンドを選んでください**";
            statusText.text += $"\nHP:{playerTurn.currentHP} MP:{playerTurn.currentMP}";
        }

        if (commandPanel != null)
        {
            commandPanel.SetActive(true);
        }
    }

    // UIボタンからの呼び出しメソッド
    public void OnCommandSelected(BattleCommand command, GameObject buttonObject)
    {
        if (state != BattleState.Player_turn || currentActivePlayer == null) return;

        // 【アニメーション再生ロジック】: ボタンに設定されたアニメーターのトリガーを起動
        Animator animator = buttonObject.GetComponent<Animator>();
        if (animator != null)
        {
            // Unity EditorのAnimatorで設定したTrigger名（例: "OnPress"）を呼び出す
            animator.SetTrigger("OnPress"); 
        }

        selectedCommand = command;
        StartCoroutine(PerformPlayerAction(currentActivePlayer));
    }

    // アイテムUIボタンからの呼び出しメソッド
    public void OnItemSelected(int itemID)
    {
        // ここにアイテムIDに基づいて、実際にアイテムを使用するロジックを実装します
        selectedItemID = itemID; 
        // StartCoroutine(UseItemAction(selectedItemID));
    }

    //プレイヤーの行動
    IEnumerator PerformPlayerAction(Player playerActing)
    {
        state = BattleState.Perform_action;
        Enemy targetEnemy = currentTargetEnemy;

        if (commandPanel != null) commandPanel.SetActive(false);
        if (targetEnemy == null) goto WinCheck; // ターゲットがいなければ勝利判定へ

        switch (selectedCommand)
        {
            case BattleCommand.ATTACK:
                playerActing.PerformAction(targetEnemy);
                break;
            case BattleCommand.SPELL:
                playerActing.Spell(targetEnemy);
                break;
            case BattleCommand.DEFEND:
                playerActing.Defend();
                break;
            case BattleCommand.ITEM:
                // アイテム選択UI表示ロジックへ
                // if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);
                yield break;
            case BattleCommand.RUN:
                // 逃走ロジックへ
                break;
        }

        WinCheck:
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(CheckEndTurn());
    }

    //アイテム使用後のターン処理（未実装）
    /*
    IEnumerator UseItemAction(int itemID) 
    {
        // ItemDataBaseからアイテムを検索し、使用ロジックを呼び出す
        // currentActivePlayer.UseItem(foundItem, targetEnemy);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(CheckEndTurn());
    }
    */

    public IEnumerator CheckEndTurn()
    {
        // 【全滅判定のリスト対応】
        if (enemyCharacters.All(e => e.currentHP <= 0))
        {
            state = BattleState.Won;
            EndBattle();
            yield break;
        }
        
        // カレントプレイヤーのバフ処理
        currentActivePlayer?.DecrementBuffTurns();

        // ターン移行
        state = BattleState.Enemy_turn;
        StartCoroutine(EnemyTurn(currentTargetEnemy));

        yield return new WaitForSeconds(1.0f);
    }

    //敵のターン
    IEnumerator EnemyTurn(Enemy enemyActing)
    {
        Player targetPlayer = currentActivePlayer;
        if (enemyActing == null || targetPlayer == null) goto LossCheck;

        if (statusText != null) statusText.text = $"**{enemyActing.charaName}のターン**";
        yield return new WaitForSeconds(1f);

        //敵の行動
        enemyActing.PerformAction(targetPlayer);

        yield return new WaitForSeconds(1.5f);

        LossCheck:
        // 【全滅判定のリスト対応】
        if (playerCharacters.All(p => p.currentHP <= 0))
        {
            state = BattleState.Lost;
            EndBattle();
            yield break;
        }

        //敵のバフ処理
        enemyActing?.DecrementBuffTurns();

        //プレイヤーのターンへ移行
        state = BattleState.Player_turn;
        PlayerTurn(currentActivePlayer);
    }

    void EndBattle()
    {
        if (state == BattleState.Won)
        {
            Debug.Log("勝利！");
            if (statusText != null) statusText.text = "勝利！";
        }
        else if (state == BattleState.Lost)
        {
            Debug.Log("敗北...");
            if (statusText != null) statusText.text = "敗北...";
        }
    }
}