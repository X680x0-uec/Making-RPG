// �t�@�C����: GameManager.cs
using UnityEngine;

/// <summary>
/// �Q�[���S�̂̃f�[�^���Ǘ�����V���O���g���N���X
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string playerName = "";
    public float playerHP = 100f;
    public float playerHPnow = 100f;
    public float playerMP = 100f;
    public float playerMPnow = 100f;
    public float playerDefence = 5f;
    public float playerAttack = 20f;

    public int enemyNumberToBattle; // 

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
