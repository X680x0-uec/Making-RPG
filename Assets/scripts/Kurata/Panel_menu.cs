using UnityEngine;

public class Panel_menu : MonoBehaviour
{
    public GameObject menuPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

    }
    
   void ToggleMenu()
    {   
    bool isCurrentlyActive = menuPanel.activeSelf;
    
    menuPanel.SetActive(!isCurrentlyActive);

    if (!isCurrentlyActive)
    { 
        Time.timeScale = 0f;
    }
    else
    {
        Time.timeScale = 1f;
    }
    }

}
