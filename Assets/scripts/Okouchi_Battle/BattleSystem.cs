using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public enum BattleState { Start, Player_turn, Enemy_turn, Perform_action, Won, Lost, Run}
public enum BattleCommand { ATTACK, SPELL, DEFEND, ITEM, RUN} 

public class BattleSystem : MonoBehaviour
{
    public Player player;
    public EnemyController enemy;

    public BattleState state;
    private BattleCommand selectedCommand;

    public GameObject commandPanel; //コマンドUIパネルへの参照
    public Text statusText;
    public GameObject itemSelectionPanel;

    void Start()
    {
        state = BattleState.Start;
        if (commandPanel != null) commandPanel.SetActive(false);
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);

        StartCoroutine(SetupBattle());
    }

    //戦闘開始フェーズ
    IEnumerator SetupBattle()
    {
        Debug.Log($"**{enemy.charaName}が現れた！**");
        yield return new WaitForSeconds(1f);

        // 素早さによるターン順決定
        DetermineTurnOrder();
    }

    //ターン順決定
    void DetermineTurnOrder()
    {
        int playerSpeed = player.EffectiveSpeed;
        int enemySpeed = enemy.EffectiveSpeed;

        if (playerSpeed > enemySpeed)
        {
            Debug.Log("プレイヤーの先行！");
            state = BattleState.Player_turn;
            PlayerTurn();
        }
        else if (enemySpeed > playerSpeed)
        {
            Debug.Log("敵の先行！");
            state = BattleState.Enemy_turn;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            //素早さ同値のときは1/2で判定
            if (Random.value < 0.5f)
            {
                state = BattleState.Player_turn;
                PlayerTurn();
            }
            else
            {
                state = BattleState.Enemy_turn;
                StartCoroutine(EnemyTurn());
            }
        }
    }

    //プレイヤーのターン
    void PlayerTurn()
    {
        //UI連動(表示)
        if (statusText != null)
        {
            statusText.text = $"**{player.charaName}のターン：コマンドを選んでください**";
            statusText.text += $"\nHP:{player.currentHP} MP:{player.currentMP}";
        }

        if (commandPanel != null)
        {
            commandPanel.SetActive(true);
        }
    }

    //UIボタンからの呼び出しメソッド
    public void OnCommandSelected(BattleCommand command)
    {
        if (state != BattleState.Player_turn) return;

        selectedCommand = command;
        StartCoroutine(PerformPlayerAction());
    }

    //
    public void OnItemSelected(int itemID)
    {
        //アイテム選択パネル非表示
        if (itemSelectionPanel != null) itemSelectionPanel.SetActive(false);

        //アイテム仕様ロジックのコルーチン再開
        StartCoroutine(UseItemAction(itemID));
    }

    //プレイヤーの行動
    IEnumerator PerformPlayerAction()
    {
        state = BattleState.Perform_action;

        //UI連動（非表示）
        if (commandPanel != null) commandPanel.SetActive(false);

        if (statusText != null) statusText.text = $"{player.charaName}が行動中...";

        switch (selectedCommand)
        {
            case BattleCommand.ATTACK:
                // player.PerformAction(enemy);
                goto EndAction;
            case BattleCommand.SPELL:
                player.Spell(enemy);
                goto EndAction;
            case BattleCommand.DEFEND:
                player.Defend();
                goto EndAction;
            case BattleCommand.ITEM:
                //アイテム選択時のUI処理
                if (statusText != null) statusText.text = "どのアイテムを使いますか？";
                if (itemSelectionPanel != null) itemSelectionPanel.SetActive(true);

                //実際の仕様ロジック
                yield break;

            case BattleCommand.RUN:
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    state = BattleState.Run;
                    if (statusText != null) statusText.text = "逃げ切った！";
                    yield break;
                }
                else
                {
                    if (statusText != null) statusText.text = "しかし、回り込まれた！";
                }
                break;
        }

        EndAction:
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(CheckEndTurn());
    }

    //アイテム使用後のターン処理
    IEnumerator UseItemAction(int itemID)
    {
        state = BattleState.Perform_action;

        Item itemToUse = player.inventory.FirstOrDefault(item => item.id == itemID);

        if(itemToUse != null)
        {
            //アイテム使用
            player.UseItem(itemToUse);

            //メッセージボックスへの表示
            if (statusText != null)
            {
                statusText.text = $"{player.charaName}は **{itemToUse.item_name}を使った！**";
            }
        }
        else
        {
            if (statusText != null) statusText.text = "そのアイテムは見つかりません。";
        }

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(CheckEndTurn());
    }

    public IEnumerator CheckEndTurn()
    {
        //敵のHPチェック（勝利判定）
        if (enemy.currentHP <= 0)
        {
            state = BattleState.Won;
            EndBattle();
            yield break;
        }

        //バフ処理
        player.DecrementBuffTurns();

        //ターン移行
        state = BattleState.Enemy_turn;
        StartCoroutine(EnemyTurn());

        yield return new WaitForSeconds(1.0f);
    }

    //敵のターン
    IEnumerator EnemyTurn()
    {
        if (statusText != null) statusText.text = $"**{enemy.charaName}のターン**";
        yield return new WaitForSeconds(1f);

        //敵の行動
        // enemy.PerformAction(player);

        yield return new WaitForSeconds(1.5f);

        //プレイヤーのHPチェック（敗北判定）
        if (player.currentHP <= 0)
        {
            state = BattleState.Lost;
            EndBattle();
            yield break;
        }

        //バフ処理
        enemy.DecrementBuffTurns();

        //プレイヤーのターンへ移行
        state = BattleState.Player_turn;
        PlayerTurn();
    }

    void EndBattle()
    {
        if (state == BattleState.Won)
        {
            Debug.Log("戦闘に勝利した！");
            if (statusText != null) statusText.text = "戦闘に勝利した！";
        }
        else if (state == BattleState.Lost)
        {
            Debug.Log("全滅してしまった...");
            if (statusText != null) statusText.text = "全滅してしまった...！";
        }
        else if (state == BattleState.Run)
        {
            Debug.Log("逃げ切った！");
            if (statusText != null) statusText.text = "逃げ切った！";
        }
    }
}
