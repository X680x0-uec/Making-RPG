using UnityEngine;

public class player_save : MonoBehaviour
{
    // �Z�[�u�f�[�^�Ɏg�����O�i�L�[�j���Œ�
    private const string PosXKey = "PlayerPosX";
    private const string PosYKey = "PlayerPosY";
    private const string PosZKey = "PlayerPosZ";

    // �V�[���J�n���Ɏ����ŌĂ΂��
    void Awake()
    {
        // �����ۑ����ꂽ���W�f�[�^�������
        if (PlayerPrefs.HasKey(PosXKey))
        {
            // ���W��ǂݍ���ŕ�������
            float x = PlayerPrefs.GetFloat(PosXKey);
            float y = PlayerPrefs.GetFloat(PosYKey);
            float z = PlayerPrefs.GetFloat(PosZKey);
            transform.position = new Vector3(x, y, z);
        }
    }

    // �V�[���I�����i��I�u�W�F�N�g�������鎞�j�Ɏ����ŌĂ΂��
    void OnDestroy()
    {
        // ���݂̍��W��ۑ�����
        PlayerPrefs.SetFloat(PosXKey, transform.position.x);
        PlayerPrefs.SetFloat(PosYKey, transform.position.y);
        PlayerPrefs.SetFloat(PosZKey, transform.position.z);
    }
}