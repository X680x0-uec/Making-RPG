// �t�@�C����: EnemyData.cs
using UnityEngine;

/// <summary>
/// �G�̊�{�p�����[�^��ێ�����ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "New EnemyData", menuName = "RPG/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("��{���")]
    public string enemyName = "�G�̖��O";
    public GameObject prefab; // �G�̌����ڂƂȂ�v���n�u

    [Header("�퓬�p�����[�^")]
    public int maxHP = 50;
    public int attackPower = 10;
    public int defensePower = 0;
}
