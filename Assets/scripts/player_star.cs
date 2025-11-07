using UnityEngine;
using UnityEngine.SceneManagement;

public class player_star : MonoBehaviour
{
    public map_text text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "enemy")
        {
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "door")
        {
            SceneManager.LoadScene("Facility");//移動先のシーンの名前を必ずshopにしてください！
        }
    }
}
