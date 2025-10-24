
using UnityEngine;

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
        if (collision.tag == "library")
        {
            Static.playerAttack += 10;
            collision.gameObject.SetActive(false);
            text.placenumber = 1;
            text.StartPlace();
        }
    }
}
