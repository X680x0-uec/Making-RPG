// ファイル名: BattleManager.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 戦闘全体の流れ（状態）を管理するクラス
/// </summary>
public class BattleManager : MonoBehaviour
{
    public enum BattleState { SETUP, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    public BattleState currentState;

    [Header("キャラクター参照")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform enemySpawnPoint;
    private EnemyController enemy;

    [Header("敵データ")]
    [SerializeField] private EnemyData[] enemyDatabase; // ScriptableObjectの配列

    [Header("UI参照")]
    [SerializeField] private BattleUI battleUI;

    void Start()
    {
        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// 戦闘準備コルーチン
    /// </summary>
    private IEnumerator SetupBattle()
    {
        currentState = BattleState.SETUP;

        // GameManagerから敵IDを取得し、該当する敵を生成
        int enemyId = GameManager.Instance != null ? GameManager.Instance.enemyNumberToBattle : 0;
        EnemyData enemyToLoad = enemyDatabase[enemyId];
        GameObject enemyInstance = Instantiate(enemyToLoad.prefab, enemySpawnPoint);
        enemy = enemyInstance.GetComponent<EnemyController>();
        enemy.Setup(enemyToLoad);

        // UIの初期設定
        battleUI.SetupUI(player, enemy);

        // イベント購読
        player.OnDied += OnPlayerDied;
        enemy.OnDied += OnEnemyDied;

        yield return battleUI.ShowMessage($"{enemy.name} が現れた！");

        StartPlayerTurn();
    }

    private void OnPlayerDied()
    {
        currentState = BattleState.LOSE;
        StartCoroutine(LoseRoutine());
    }

    private void OnEnemyDied()
    {
        currentState = BattleState.WIN;
        StartCoroutine(WinRoutine());
    }

    /// <summary>
    /// プレイヤーのターンを開始
    /// </summary>
    private void StartPlayerTurn()
    {
        currentState = BattleState.PLAYERTURN;
        player.isDefending = false; // 前のターンの防御状態をリセット
        battleUI.ShowMessage("あなたのターン", 0.5f); // 待たずに次の処理へ
        battleUI.SetPlayerControls(true);
    }

    /// <summary>
    /// 攻撃ボタンが押されたときの処理
    /// </summary>
    public void OnAttackButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerAttackRoutine());
    }

    /// <summary>
    /// 防御ボタンが押されたときの処理
    /// </summary>
    public void OnDefendButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerDefendRoutine());
    }

    /// <summary>
    /// 逃げるボタンが押されたときの処理
    /// </summary>
    public void OnEscapeButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerEscapeRoutine());
    }

    private IEnumerator PlayerAttackRoutine()
    {
        battleUI.SetPlayerControls(false);
        yield return battleUI.ShowMessage("ゆうしゃ のこうげき！");
        enemy.TakeDamage(player.attackPower);
        yield return new WaitForSeconds(1.5f);

        if (!enemy.isDead) StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator PlayerDefendRoutine()
    {
        battleUI.SetPlayerControls(false);
        player.isDefending = true;
        yield return battleUI.ShowMessage("ゆうしゃ は防御の姿勢をとった。");
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator PlayerEscapeRoutine()
    {
        battleUI.SetPlayerControls(false);
        // 50%の確率で成功
        if (Random.value > 0.5f)
        {
            yield return battleUI.ShowMessage("うまく逃げきれた！");
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("Main"); // Mainシーンの名前を適宜変更してください
        }
        else
        {
            yield return battleUI.ShowMessage("しかし、回り込まれてしまった！");
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    private IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.ENEMYTURN;
        yield return battleUI.ShowMessage($"{enemy.name} のこうげき！");
        player.TakeDamage(enemy.attackPower);
        yield return new WaitForSeconds(1.5f);

        if (!player.isDead) StartPlayerTurn();
    }

    private IEnumerator WinRoutine()
    {
        player.SaveHPToGameManager(); // HPをGameManagerに保存
        yield return battleUI.ShowMessage($"{enemy.name} をやっつけた！");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Main"); // Mainシーンの名前を適宜変更してください
    }

    private IEnumerator LoseRoutine()
    {
        yield return battleUI.ShowMessage("ゆうしゃ は倒れてしまった...");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Gameover"); // Gameoverシーンの名前を適宜変更してください
    }

    // オブジェクト破棄時にイベント購読を解除
    private void OnDestroy()
    {
        if (player != null) player.OnDied -= OnPlayerDied;
        if (enemy != null) enemy.OnDied -= OnEnemyDied;
    }
}