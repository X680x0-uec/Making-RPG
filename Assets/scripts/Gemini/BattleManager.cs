// �t�@�C����: BattleManager.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �퓬�S�̗̂���i��ԁj���Ǘ�����N���X
/// </summary>
public class BattleManager : MonoBehaviour
{
    public enum BattleState { SETUP, PLAYERTURN, ENEMYTURN, WIN, LOSE }
    public BattleState currentState;

    [Header("�L�����N�^�[�Q��")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform enemySpawnPoint;
    private EnemyController_sub enemy;

    [Header("�G�f�[�^")]
    [SerializeField] private EnemyData[] enemyDatabase; // ScriptableObject�̔z��

    [Header("UI�Q��")]
    [SerializeField] private BattleUI battleUI;

    void Start()
    {
        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// �퓬�����R���[�`��
    /// </summary>
    private IEnumerator SetupBattle()
    {
        currentState = BattleState.SETUP;

        // GameManager����GID���擾���A�Y������G�𐶐�
        int enemyId = GameManager.Instance != null ? GameManager.Instance.enemyNumberToBattle : 0;
        EnemyData enemyToLoad = enemyDatabase[enemyId];
        GameObject enemyInstance = Instantiate(enemyToLoad.prefab, enemySpawnPoint);
        enemy = enemyInstance.GetComponent<EnemyController_sub>();
        enemy.Setup(enemyToLoad);

        // UI�̏����ݒ�
        battleUI.SetupUI(player, enemy);

        // �C�x���g�w��
        player.OnDied += OnPlayerDied;
        enemy.OnDied += OnEnemyDied;

        yield return battleUI.ShowMessage($"{enemy.name} �����ꂽ�I");

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
    /// �v���C���[�̃^�[�����J�n
    /// </summary>
    private void StartPlayerTurn()
    {
        currentState = BattleState.PLAYERTURN;
        player.isDefending = false; // �O�̃^�[���̖h���Ԃ����Z�b�g
        battleUI.ShowMessage("���Ȃ��̃^�[��", 0.5f); // �҂����Ɏ��̏�����
        battleUI.SetPlayerControls(true);
    }

    /// <summary>
    /// �U���{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    public void OnAttackButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerAttackRoutine());
    }

    /// <summary>
    /// �h��{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    public void OnDefendButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerDefendRoutine());
    }

    /// <summary>
    /// ������{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    public void OnEscapeButton()
    {
        if (currentState != BattleState.PLAYERTURN) return;
        StartCoroutine(PlayerEscapeRoutine());
    }

    private IEnumerator PlayerAttackRoutine()
    {
        battleUI.SetPlayerControls(false);
        yield return battleUI.ShowMessage("�䂤���� �̂��������I");
        enemy.TakeDamage(player.attackPower);
        yield return new WaitForSeconds(1.5f);

        if (!enemy.isDead) StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator PlayerDefendRoutine()
    {
        battleUI.SetPlayerControls(false);
        player.isDefending = true;
        yield return battleUI.ShowMessage("�䂤���� �͖h��̎p�����Ƃ����B");
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator PlayerEscapeRoutine()
    {
        battleUI.SetPlayerControls(false);
        // 50%�̊m���Ő���
        if (Random.value > 0.5f)
        {
            yield return battleUI.ShowMessage("���܂��������ꂽ�I");
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("Main"); // Main�V�[���̖��O��K�X�ύX���Ă�������
        }
        else
        {
            yield return battleUI.ShowMessage("�������A��荞�܂�Ă��܂����I");
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    private IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.ENEMYTURN;
        yield return battleUI.ShowMessage($"{enemy.name} �̂��������I");
        player.TakeDamage(enemy.attackPower);
        yield return new WaitForSeconds(1.5f);

        if (!player.isDead) StartPlayerTurn();
    }

    private IEnumerator WinRoutine()
    {
        player.SaveHPToGameManager(); // HP��GameManager�ɕۑ�
        yield return battleUI.ShowMessage($"{enemy.name} ����������I");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Main"); // Main�V�[���̖��O��K�X�ύX���Ă�������
    }

    private IEnumerator LoseRoutine()
    {
        yield return battleUI.ShowMessage("�䂤���� �͓|��Ă��܂���...");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Gameover"); // Gameover�V�[���̖��O��K�X�ύX���Ă�������
    }

    // �I�u�W�F�N�g�j�����ɃC�x���g�w�ǂ�����
    private void OnDestroy()
    {
        if (player != null) player.OnDied -= OnPlayerDied;
        if (enemy != null) enemy.OnDied -= OnEnemyDied;
    }
}