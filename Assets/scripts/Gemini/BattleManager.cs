using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { SETUP, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    public BattleState currentState;

    [SerializeField] private Player player;
    [SerializeField] private Transform enemySpawnPoint;
    private EnemyController enemy;

    [SerializeField] private List<EnemyData> enemyDatabase; // ScriptableObject

    [SerializeField] private Panel battleUI;

    void Start()
    {
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        currentState = BattleState.SETUP;

        // GameManager
        int enemyId = GameManager.Instance != null ? GameManager.Instance.enemyNumberToBattle : 0;
        EnemyData enemyToLoad = enemyDatabase[enemyId];
        GameObject enemyInstance = Instantiate(enemyToLoad.prefab, enemySpawnPoint);
        enemy = enemyInstance.GetComponent<EnemyController>();
        enemy.Setup(enemyToLoad);

        // SetupUIの呼び出し
        battleUI.SetupUI(player, enemy);

        // player及びenemyがやられたときの処理
        player.OnDied += OnPlayerDied;
        enemy.OnDied += OnEnemyDied;

        yield return battleUI.ShowMessage($"{enemy.charaName}が現れた！");

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

    private void StartPlayerTurn()
    {
        currentState = BattleState.PLAYERTURN;
        player.isDefending = false; // 防御を解除
        // battleUI.SetPlayerControls(true);
    }

    public void OnAttackButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerAttackRoutine());
    }

    public void OnDefendButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerDefendRoutine());
    }

    public void OnEscapeButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerEscapeRoutine());
    }

    public void OnItemButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerItemRoutine());
    }

    private IEnumerator PlayerAttackRoutine()
    {
        // battleUI.SetPlayerControls(false);
        yield return battleUI.ShowMessage("�䂤���� �̂��������I");
        enemy.TakeDamage(player.Attack);
        yield return new WaitForSeconds(1.5f);

        if (!enemy.isDead) StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator PlayerDefendRoutine()
    {
        // battleUI.SetPlayerControls(false);
        player.isDefending = true;
        yield return battleUI.ShowMessage("�䂤���� �͖h��̎p�����Ƃ����B");
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator PlayerEscapeRoutine()
    {
        // battleUI.SetPlayerControls(false);
        // 50%の確立で逃げ切ることができる
        if (Random.value > 0.5f)
        {
            yield return battleUI.ShowMessage("���܂��������ꂽ�I");
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("Main"); // Mainを呼び出す。
        }
        else
        {
            yield return battleUI.ShowMessage("�������A��荞�܂�Ă��܂����I");
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    private IEnumerator PlayerItemRoutine()
    {
        // battleUI.SetPlayerControls(false);
        yield return battleUI.ShowMessage("アイテムを使った");
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.ENEMYTURN;
        yield return battleUI.ShowMessage($"{enemy.charaName} �̂��������I");
        Debug.Log("enemy");
        player.TakeDamage(enemy.Attack);
        yield return new WaitForSeconds(1.5f);

        if (!player.isDead) StartPlayerTurn();
    }

    private IEnumerator WinRoutine()
    {
        player.SaveHPToGameManager(); // HPをGameManagerに保存する
        yield return battleUI.ShowMessage($"{enemy.charaName} ����������I");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Main"); // Mainシーンに切り替え
    }

    private IEnumerator LoseRoutine()
    {
        yield return battleUI.ShowMessage("�䂤���� �͓|��Ă��܂���...");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Gameover"); // Gameoverシーンに切り替え
    }

    // 死んだとき
    private void OnDestroy()
    {
        if (player != null) player.OnDied -= OnPlayerDied;
        if (enemy != null) enemy.OnDied -= OnEnemyDied;
    }
}