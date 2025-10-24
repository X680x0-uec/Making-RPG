using UnityEngine;
using System.Collections;
using TMPro;


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

    public void TogglePanelTatakau()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);

            if (displayMessageText.text != null)
            {
                displayMessageText.text = "学生はたたかった！ABのダメージ！";
            }


            StartCoroutine(Paneloff());
        }
    }

    public void TogglePanelJyumonn()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);

            StartCoroutine(Paneloff());
        }
    }
}

