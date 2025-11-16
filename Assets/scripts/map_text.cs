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
    void Start()
    {
        // Static.talkbox = false; // 削除
        UI_talkbox.SetActive(false);
        target.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // UI_talkbox.SetActive(Static.talkbox); // 削除 (Updateごと不要)
    }

    public void StartPlace()
    {
        // Static.talkbox = true; // 変更
        UI_talkbox.SetActive(true); // 変更
        StartCoroutine(DisplayLine(facility[placenumber], waits));
    }

    // 文字列を順に表示していく
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
        // Static.talkbox = false; // 変更
        UI_talkbox.SetActive(false); // 変更
    }
}