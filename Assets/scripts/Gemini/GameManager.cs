// �t�@�C����: GameManager.cs
using UnityEngine;

/// <summary>
/// �Q�[���S�̂̃f�[�^���Ǘ�����V���O���g���N���X
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("�v���C���[�̃X�e�[�^�X")]
    public int playerHP = 100;
    public int playerHPnow = 100;
    public int playerMP = 100;
    public int playerMPnow = 100;
    public int playerDefence = 5;
    public int playerAttack = 20;

    [Header("�퓬�֘A�f�[�^")]
    public int enemyNumberToBattle; // �퓬�V�[���ɓn���G�̔ԍ�

    private void Awake()
    {
        // �V���O���g���p�^�[���̎���
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
