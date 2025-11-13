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
    private bool isUsingItemPanel = false;

    [SerializeField] private List<EnemyData> enemyDatabase; // ScriptableObject

    [SerializeField] private Panel battleUI;
    public GameObject ItemPanel;

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
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel) return;
        StartCoroutine(PlayerAttackRoutine());
    }

    public void OnMagicButton()
    {
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel) return;
        StartCoroutine(PlayerMagicRoutine());
    }

    public void OnDefendButton()
    {
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel) return;
        StartCoroutine(PlayerDefendRoutine());
    }

    public void OnEscapeButton()
    {
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel) return;
        StartCoroutine(PlayerEscapeRoutine());
    }

    public void OnItemButton()
    {
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel) return;
        StartCoroutine(PlayerItemRoutine());
    }

    private IEnumerator PlayerAttackRoutine()
    {
        // battleUI.SetPlayerControls(false);
        yield return battleUI.ShowMessage($"{ player.charaName }の攻撃！");
        yield return battleUI.ShowMessage($"{ enemy.charaName }に{ enemy.TakeDamage(player.EffectiveAttack) }のダメージを与えた！");

        if (!enemy.isDead) {
            StartCoroutine(EnemyTurnRoutine());
        } else {
            StartCoroutine(WinRoutine());  // 勝ち
        }
    }

    private IEnumerator PlayerDefendRoutine()
    {
        // battleUI.SetPlayerControls(false);
        player.isDefending = true;
        yield return battleUI.ShowMessage($"{ player.charaName }は敵の攻撃から身を守る態勢に入った");
        StartCoroutine(EnemyTurnRoutine());
    }

    // 仮置き
    private IEnumerator PlayerMagicRoutine()
    {
        // battleUI.SetPlayerControls(false);
        yield return battleUI.ShowMessage("そんなものが使えたら人はレポートに苦しまないのさ");
        StartPlayerTurn();
    }

    private IEnumerator PlayerEscapeRoutine()
    {
        // battleUI.SetPlayerControls(false);
        // 50%の確立で逃げ切ることができる
        yield return battleUI.ShowMessage($"{ player.charaName }は逃げようとした...");
        if (Random.value > 0.5f)
        {
            yield return battleUI.ShowMessage("逃げ切れた！", deactivate: false);
            player.SaveHPToGameManager(); // HPをGameManagerに保存する
            SceneManager.LoadScene("Main"); // Mainを呼び出す。
        }
        else
        {
            yield return battleUI.ShowMessage("逃げ切れなかった！");
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    private IEnumerator PlayerItemRoutine()
    {
        yield return new WaitForEndOfFrame();
        // battleUI.SetPlayerControls(false);
        if (player.inventory.Count > 0)
        {
            ItemPanel.SetActive(true);
            isUsingItemPanel = true;
            battleUI.ShowMessage("", deactivate: false);
        }
        else
        {
            yield return battleUI.ShowMessage("所持しているアイテムがありません");
        }
        
        // StartCoroutine(EnemyTurnRoutine());
    }

    public IEnumerator PlayerUseItemRoutine(Item item)
    {
        ItemPanel.SetActive(false);
        battleUI.ShowMessage($"{ item.item_name }を使った！");
        EnemyTurnRoutine();
        yield return new WaitForEndOfFrame();
        isUsingItemPanel = false;
        Debug.Log("エネミー");
    }

    public IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.ENEMYTURN;
        yield return battleUI.ShowMessage($"{ enemy.charaName }の攻撃！");
        yield return battleUI.ShowMessage($"{ player.charaName }は{ player.TakeDamage(enemy.Attack) }のダメージを受けた！");

        if (!player.isDead) {
            StartPlayerTurn();
        } else {
            StartCoroutine(LoseRoutine());
        }
    }

    private IEnumerator WinRoutine()
    {
        player.SaveHPToGameManager(); // HPをGameManagerに保存する
        Destroy(enemy.gameObject);
        yield return battleUI.ShowMessage($"{ enemy.charaName }を倒した！", deactivate: false);
        SceneManager.LoadScene("Main"); // Mainシーンに切り替え
    }

    private IEnumerator LoseRoutine()
    {
        yield return battleUI.ShowMessage($"{ player.charaName }はやられてしまった...", deactivate: false);
        SceneManager.LoadScene("Gameover"); // Gameoverシーンに切り替え
    }

    // 死んだとき
    private void OnDestroy()
    {
        if (player != null) player.OnDied -= OnPlayerDied;
        if (enemy != null) enemy.OnDied -= OnEnemyDied;
    }
}