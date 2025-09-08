// ファイル名: PlayerController.cs
using UnityEngine;

/// <summary>
/// 戦闘中のプレイヤーを管理するクラス
/// </summary>
public class PlayerController : CharacterStats
{
    public bool isDefending { get; set; } = false;

    protected override void Awake()
    {
        // GameManagerからステータスを読み込む
        if (GameManager.Instance != null)
        {
            maxHP = GameManager.Instance.playerHP;
            currentHP = GameManager.Instance.playerHPnow;
            attackPower = GameManager.Instance.playerAttack;
            defensePower = GameManager.Instance.playerDefence;
        }
        else
        {
            // GameManagerがない場合（テスト用）
            base.Awake();
        }
    }

    public override void TakeDamage(int damage)
    {
        // 防御中は防御力を倍にする（例）
        int originalDefense = defensePower;
        if (isDefending)
        {
            defensePower *= 2;
        }

        base.TakeDamage(damage);

        // ステータスを元に戻す
        defensePower = originalDefense;
        isDefending = false;
    }

    // 戦闘終了時にGameManagerにHPを保存する
    public void SaveHPToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerHPnow = this.currentHP;
        }
    }
}