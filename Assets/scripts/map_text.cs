using TMPro;
using System.Collections;
using System.Text;
using UnityEngine;

public class map_text : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI target;
    [SerializeField] GameObject UI_talkbox;

    public string[] facility = new string[5];
    public string[] item = new string[5];
    public float waits = 0.1f;
    public int placenumber = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Start()
    {
        Static.talkbox = false;
        UI_talkbox.SetActive(false);
        target.text = "";
    }
    void Update()
    {
        UI_talkbox.SetActive(Static.talkbox);
    }
    public void StartPlace()
    {
        Static.talkbox = true;
        StartCoroutine(DisplayLine(facility[placenumber], waits));
    }
    public IEnumerator DisplayLine(string text, float seconds)
    {
        target.text = "";
        var length = text.Length;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(text[i]);
            target.text = sb.ToString();
            yield return new WaitForSeconds(seconds);
        }
        yield return new WaitForSeconds(1.5f);
        target.text = "";
        Static.talkbox = false;
    }
}
