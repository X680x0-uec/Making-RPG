using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battle_button : MonoBehaviour
{
    public Battle_text text;
    public bool control = false;

    [SerializeField] private GameObject firstButton_Magic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     //   EventSystem.current.SetSelectedGameObject(firstButton_Magic);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnButtonfight()
    {
        if (control)
        {
            control = false;
            text.actionnumber = 0;
            text.StartText();
        }
    }
    public void OnButtonmagic()
    {

    }
    public void OnButtonitem()
    {

    }
    public void OnButtondefence()
    {
        if (control)
        {
            control = false;
            text.actionnumber = 4;
            text.StartText();
        }
    }
    public void OnButtonescape()
    {
        if (control)
        {
            control = false;
            int randomInt = Random.Range(0, 2);
            if (randomInt == 0)
            {
                text.actionnumber = 5;
                text.StartText();
            }
            if (randomInt == 1)
            {
                text.actionnumber = 6;
                text.StartText();
            }
        }
    }
    public void OnButtonsurrender()
    {
        if (control)
        {
            control = false;
            text.actionnumber = 7;
            text.StartText();
        }
    }
}
