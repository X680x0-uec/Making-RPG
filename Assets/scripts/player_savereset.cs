using UnityEngine;

public class player_savereset: MonoBehaviour
{
    // �ȑO�̃X�N���v�g�ŕۑ��Ɏg�����L�[�ƁA�܂������������O���w��
    private const string PosXKey = "PlayerPosX";
    private const string PosYKey = "PlayerPosY";
    private const string PosZKey = "PlayerPosZ";
    // �������̃f�[�^�i�X�R�A�Ȃǁj������΁A���l�ɃL�[��ǉ����܂�
    // private const string ScoreKey = "PlayerScore";

    // ���̃I�u�W�F�N�g���L���ɂȂ������i�V�[�����ǂݍ��܂ꂽ���j�ɌĂ΂��
    void Start()
    {
        // �ۑ�����Ă�����W�f�[�^���폜
        PlayerPrefs.DeleteKey(PosXKey);
        PlayerPrefs.DeleteKey(PosYKey);
        PlayerPrefs.DeleteKey(PosZKey);

        // �����X�R�A�Ȃǂ����Z�b�g��������΁A���l�ɍ폜
        // PlayerPrefs.DeleteKey(ScoreKey);

        // �����ӁF�����S�ẴZ�[�u�f�[�^�����������ꍇ�́A�ȉ���1�s���g���܂�
        // PlayerPrefs.DeleteAll();

        Debug.Log("�v���C���[�̃Z�[�u�f�[�^�����Z�b�g���܂����B");
    }
}
