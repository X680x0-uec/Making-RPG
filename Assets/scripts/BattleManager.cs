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
    private bool try_Escape = false;
    private int boss_pattern = 0;
    private int special_order = 0;
    private int turncount = 0;

    [SerializeField] private List<EnemyData> enemyDatabase; // ScriptableObject

    [SerializeField] private Panel battleUI;
    public GameObject BGM_Battle;
    public GameObject BGM_BossBattle;
    public GameObject BGM_Victory;

    public AudioSource SE_Click;
    public AudioSource SE_Attack;

    private SpriteRenderer spriteRederer;
    public Animator Slash;

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
        spriteRederer = enemy.GetComponent<SpriteRenderer>();
        enemy.Setup(enemyToLoad);

        // Boss BGM 呼び出し
        if (enemy.type == EnemyData.Types.Boss)
        {
            BGM_BossBattle.SetActive(true);
            enemy.maxHP = enemy.maxHP + 200 < player.Attack * 11 ? player.Attack * 11 : enemy.maxHP;
            enemy.currentHP = enemy.maxHP;
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
        Slash.SetTrigger("slash");
        yield return battleUI.ShowMessage($"{ player.charaName }の攻撃！");
        SE_Attack.Play();
        StartCoroutine(Damage_Animation());
        yield return battleUI.ShowMessage($"{ enemy.charaName }に{ enemy.TakeDamage(player.EffectiveAttack) }のダメージを与えた！");

        if (!enemy.isDead) {
            StartCoroutine(EnemyTurnRoutine());
        } else {
            StartCoroutine(WinRoutine());  // 勝ち
        }
    }

    private IEnumerator Damage_Animation()
    {
        Debug.Log("通ったぜ");

        // 色
        for (int i = 0; i < 8; i++)
        {
            Color color = spriteRederer.color;
            color.a = i % 2;
            spriteRederer.color = color;
            yield return new WaitForSeconds(0.10f);
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
            if (randomnumber >= (enemy.maxHP - player.maxHP / 1.6f) || try_Escape)
            {
                yield return battleUI.ShowMessage("逃げ切れた！", deactivate: false);
                player.SaveHPToGameManager(); // HPをGameManagerに保存する
                SceneManager.LoadScene("Main"); // Mainを呼び出す。
            }
            else
            {
                yield return battleUI.ShowMessage("逃げ切れなかった！");
                try_Escape = true;
                StartCoroutine(EnemyTurnRoutine());
            }
        }
        
    }

    private IEnumerator PlayerItemRoutine()
    {
        yield return new WaitForEndOfFrame();
        yield return battleUI.ShowMessage($"{ player.charaName }は天に向かって祈りをささげた...！");
        if (player.currentHP < 10f)
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
        turncount++;
        if (enemy.type == EnemyData.Types.Boss)
        {
            yield return battleUI.ShowMessage(Professer_kogoto());
            switch (boss_pattern)
            {
                // 通常攻撃
                case 0:
                    SE_Attack.Play();
                    yield return battleUI.ShowMessage($"{ player.charaName }は{ player.TakeDamage(enemy.Attack) }のダメージを受けた！");
                    break;
                
                // スペシャル攻撃
                case 1:
                    yield return battleUI.ShowMessage("何も言い返せない自分が悔しい...");
                    SE_Attack.Play();
                    yield return battleUI.ShowMessage($"クリティカルヒット！{ player.charaName }は{ player.TakeDamage(enemy.Attack * 2.0f) }のダメージを受けた！");
                    break;
            }
        }
        else
        {
            yield return battleUI.ShowMessage($"{ enemy.charaName }の攻撃！");
            SE_Attack.Play();
            yield return battleUI.ShowMessage($"{ player.charaName }は{ player.TakeDamage(enemy.Attack) }のダメージを受けた！");
        }
        
        if (!player.isDead) {
            StartPlayerTurn();
        } else {
            StartCoroutine(LoseRoutine());
        }
    }

    public string Professer_kogoto()
    {
        string[] kogotos = new string[4] {
            "実験手順は過去形じゃなきゃダメですよ",
            "有効数字を適当につけてませんか？",
            "グラフの線はまっすぐ書きましょう",
            "勝手に実験をやり直さないでください"
        };

        string[] specials = new string[3] {
            "君実験の意味理解してないよね？",
            "君、ほんとに予習やった？",
            "この文章とかAIに書かせてるでしょ？"
        };

        int pattern = UnityEngine.Random.Range(0, 99);


        if (turncount == 1)
        {   
            // 1ターン目
            boss_pattern = 2;
            if (player.Attack < 30)
            {
                return "君の考察は...やり直したほうがいいね";
            }
            else if (player.Attack >= 70)
            {
                return "君のレポートは...なかなか読みごたえがありますね";
            }
            else
            {
                return "君の考察は...まあ、良い線行ってるんじゃないかな";
            }
        }
        else
        {   
            // スペシャル攻撃
            if ((enemy.currentHP > enemy.maxHP / 5 && pattern < 20) || (enemy.currentHP <= enemy.maxHP / 5))
            {
                boss_pattern = 1; special_order = (special_order + 1) % specials.Length;
                return specials[special_order];
            }
            else if (enemy.currentHP > enemy.maxHP / 5 && pattern >= 80)
            {
                boss_pattern = 2;
                return "本日は出張のためお休みです";
            }
            else
            {
                boss_pattern = 0;
                return kogotos[UnityEngine.Random.Range(0, kogotos.Length - 1)];
            }
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
        yield return new WaitForSeconds(1.0f);
        
        if (enemy.type == EnemyData.Types.Boss)
        {
            SceneManager.LoadScene("Clear");
        }
        else
        {
            SceneManager.LoadScene("Main"); // Mainシーンに切り替え
        }
        
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