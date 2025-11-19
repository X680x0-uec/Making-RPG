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
    public GameObject BGM_Battle;
    public GameObject BGM_BossBattle;
    public GameObject BGM_Victory;

    public AudioSource SE_Click;
    public AudioSource SE_Attack;

    private Vector2 rightTransform;
    private RectTransform transform;
    private bool onGoing = false;

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

        // Boss BGM 呼び出し
        if (enemy.type == EnemyData.Types.Boss)
        {
            BGM_BossBattle.SetActive(true);
        }
        else if (enemy.type != EnemyData.Types.Danpen)
        {
            BGM_Battle.SetActive(true);
        }

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
        onGoing = false;
        currentState = BattleState.PLAYERTURN;
        player.isDefending = false; // 防御を解除
        // battleUI.SetPlayerControls(true);
    }

    public void OnAttackButton()
    {
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel || onGoing) return;
        onGoing = true;
        SE_Click.Play();
        StartCoroutine(PlayerAttackRoutine());
    }

    public void OnMagicButton()
    {
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel || onGoing) return;
        onGoing = true;
        SE_Click.Play();
        StartCoroutine(PlayerMagicRoutine());
    }

    public void OnDefendButton()
    {
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel || onGoing) return;
        onGoing = true;
        SE_Click.Play();
        StartCoroutine(PlayerDefendRoutine());
    }

    public void OnEscapeButton()
    {
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel || onGoing) return;
        onGoing = true;
        SE_Click.Play();
        StartCoroutine(PlayerEscapeRoutine());
    }

    public void OnItemButton()
    {
        if (currentState != BattleState.PLAYERTURN || isUsingItemPanel || onGoing) return;
        onGoing = true;
        SE_Click.Play();
        StartCoroutine(PlayerItemRoutine());
    }

    private IEnumerator PlayerAttackRoutine()
    {
        // battleUI.SetPlayerControls(false);
        yield return battleUI.ShowMessage($"{ player.charaName }の攻撃！");
        SE_Attack.Play();
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
        int randomnumber = UnityEngine.Random.Range(0, Mathf.FloorToInt(enemy.maxHP));
        if (enemy.type == EnemyData.Types.Boss)
        {
            yield return battleUI.ShowMessage("絶対に負けられない戦いがここにある！");
            StartPlayerTurn();
        }
        else
        {
            yield return battleUI.ShowMessage($"{ player.charaName }は逃げようとした...");
            if (randomnumber >= (enemy.maxHP - player.maxHP / 1.6f))
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
        
    }

    private IEnumerator PlayerItemRoutine()
    {
        yield return new WaitForEndOfFrame();
        yield return battleUI.ShowMessage($"{ player.charaName }は天に向かって祈りをささげた...！");
        if (Random.value >= 0.99f)
        {
            yield return battleUI.ShowMessage($"{ player.charaName }の願いは天に届いた！");
            player.heal(player.maxHP);
            yield return battleUI.ShowMessage($"なんと{ player.charaName }のHPが全回復した！");
        }
        else
        {
            yield return battleUI.ShowMessage("しかし何も起こらなかった...");
        }
        StartCoroutine(EnemyTurnRoutine());
    }

    public IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.ENEMYTURN;
        yield return battleUI.ShowMessage($"{ enemy.charaName }の攻撃！");
        SE_Attack.Play();
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

        // BGM
        BGM_Battle.SetActive(false);
        BGM_BossBattle.SetActive(false);
        BGM_Victory.SetActive(true);

        yield return battleUI.ShowMessage($"{ enemy.charaName }を倒した！", deactivate: false);
        yield return new WaitForSeconds(1.5f);
        
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