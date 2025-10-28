using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class Panel : MonoBehaviour
{

    public GameObject targetPanel;
    public TMPro.TextMeshProUGUI displayMessageText;

    private IEnumerator Paneloff()
    {
        yield return new WaitForSeconds(3.0f);

        if (targetPanel.activeSelf)
        {
            targetPanel.SetActive(false);
        }
    }

    private void Deselect()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void TogglePanelAttack()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);

            if (displayMessageText.text != null)
            {
                displayMessageText.text = "学生はたたかった！ABのダメージ！";
            }

            StartCoroutine(Paneloff());
            Deselect();
        }
    }

    public void TogglePanelMagic()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);

            StartCoroutine(Paneloff());
            Deselect();
        }
    }

}


