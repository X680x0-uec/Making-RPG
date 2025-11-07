using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class Panel : MonoBehaviour
{
    private System.Random randomGenerator = new System.Random(); 
    public GameObject targetPanel;
    public TMPro.TextMeshProUGUI displayMessageText;
    
    void Start()
    {
        if (displayMessageText.text != null)
            {
                displayMessageText.text = "敵が現れた";
            }

            StartCoroutine(Paneloff());
    }
    
    private IEnumerator Paneloff()
    {
        yield return new WaitForSeconds(3.0f);

        if (targetPanel.activeSelf)
        {
            targetPanel.SetActive(false);
        }
    }

    private IEnumerator EscapeProcess()
    {
        if (displayMessageText.text != null)
        {
            displayMessageText.text = "学生は逃げようとした！";
        }
    
        yield return new WaitForSeconds(3.0f);

        int randomNumber = randomGenerator.Next(0, 2);
    
        if(randomNumber == 1)
        {
            displayMessageText.text = "逃げ切れた！";
        }
        else
        {
            displayMessageText.text = "逃げ切れなかった、、。";
        }

        StartCoroutine(Paneloff());
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

            if (displayMessageText.text != null)
            {
                displayMessageText.text = "学生はまほうを使った！BCのダメージ！";
            }

            StartCoroutine(Paneloff());
            Deselect();
        }
    }

    public void TogglePanelDefense()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);

            if (displayMessageText.text != null)
            {
                displayMessageText.text = "学生はぼうぎょした！ダメージ軽減率がCD%上昇！";
            }

            StartCoroutine(Paneloff());
            Deselect();
        }
    }

    public void TogglePanelEscape()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            StartCoroutine(EscapeProcess());
            Deselect();
        }
    }
    
    public void TogglePanelItem()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);

            if (displayMessageText.text != null)
            {
                displayMessageText.text = "アイテムを使用した";
            }

            StartCoroutine(Paneloff());
            Deselect();
        }
    }
}
