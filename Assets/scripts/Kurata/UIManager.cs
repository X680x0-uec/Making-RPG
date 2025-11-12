using UnityEngine;


public class UIManager : MonoBehaviour
{
    [Header("アイテム取得パネル")]
    
    [SerializeField] private GameObject itemGetPanel; 
    
    // プレイヤーコントローラーへの参照 (動きを止めるため)
    private player_controller playerController;

    void Start()
    {
        
        if (itemGetPanel != null)
        {
            itemGetPanel.SetActive(false);
        }


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<player_controller>();
        }
    }

 
    public void ShowItemGetPanel()
    {
        if (itemGetPanel != null)
        {
            itemGetPanel.SetActive(true);
        }

        if (playerController != null)
        {

            playerController.stop = true; 
        }
    }


    public void ToggleHideItemGetPanel()
    {
        if (itemGetPanel != null)
        {
            itemGetPanel.SetActive(false);
        }

        if (playerController != null)
        {

            playerController.stop = false; 
        }
    }
}
