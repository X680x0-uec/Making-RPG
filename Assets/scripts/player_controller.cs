using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

public class player_controller : MonoBehaviour
{
    public static player_controller instance;
    [SerializeField] private GameObject UI_Encount;
    private Animator animator;
    [SerializeField] Animator cameraAnimator;  // カメラワーク

    [SerializeField] public float speed_const = 5.0f;
    public float speed;
    public bool stop = false;
    private float encount_range = 100;
    private Rigidbody2D rb;
    private Vector2 move;
    private int idX = Animator.StringToHash("x"), idY = Animator.StringToHash("y");

    private UIManager uiManager;

    private Panel_menu panelMenu;

     private bool inStrongZone = false; // ★ 追加：強敵エリアにいるかどうか

     int enemyId = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UI_Encount.SetActive(false);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        uiManager = FindFirstObjectByType<UIManager>();

        panelMenu = FindFirstObjectByType<Panel_menu>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        x = Math.Abs(x) >= Math.Abs(y) ? x : 0;
        y = Math.Abs(y) > Math.Abs(x) ? y : 0;

        if ((Mathf.FloorToInt(x) != 0 || Mathf.FloorToInt(y) != 0) && Time.timeScale != 0f && !this.stop)
        {   
            this.speed = this.speed_const;
            this.animator.speed = 1;
            this.animator.SetFloat (idX, x);
			this.animator.SetFloat (idY, y);
        }
        else
        {   
            this.speed = 0;
            this.animator.speed = 0;
        }
        this.move = new Vector2(x, y);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ★ 追加：strongzoneに入った時の処理
        if (collision.tag == "strongzone")
        {
            inStrongZone = true;
            Debug.Log("危険地帯に入った！");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.tag == "encounter")&&(this.speed > 0)) //止まってたらエンカしない
        {
            encount_range -= Time.fixedDeltaTime; // 経過時間でエンカウント率が上昇
            int randomencount = UnityEngine.Random.Range(0, Mathf.FloorToInt(encount_range));
            Debug.Log(collision.gameObject);
            if (encount_range < 0) { encount_range = 0; }

            if (randomencount == 0)
            {
                encount_range = 300;
                UI_Encount.SetActive(true);
                stop = true;
                cameraAnimator.SetTrigger("Encounter");
                StartCoroutine("WAITTIME");
                // ★ 修正：strongzoneにいるならID 3、そうでなければランダム
                if (inStrongZone)
                {
                    enemyId = 3; // キングフラスコ
                }
                else
                {
                    enemyId = UnityEngine.Random.Range(0, 3); // 0, 1, 2 のいずれか
                }

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.enemyNumberToBattle = enemyId;
                }
            }
        }
        if (collision.tag == "danpen")
        {
            UI_Encount.SetActive(true);
            stop = true;
            animator.SetTrigger("encount");
            StartCoroutine("WAITTIME");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.enemyNumberToBattle = 4;
            }
        }
        if (collision.tag == "boss") // 1. で作成したタグ名
        {
            // エンカウント演出を開始
            UI_Encount.SetActive(true);

            // アニメーターが設定されていればトリガーを引く
            if (animator != null)
            {
                animator.SetTrigger("encount");
            }
            encount_range = 1000000;  // 初期化
            stop = true; // プレイヤーを止める
            StartCoroutine("WAITTIME"); // 戦闘シーンへ
            GameManager.Instance.enemyNumberToBattle = 5; // ★ 敵IDを 5 に設定

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ★ 追加：強敵エリアから出た
        if (collision.tag == "strongzone")
        {
            inStrongZone = false;
            Debug.Log("危険地帯から出た");
        }
    }
    IEnumerator WAITTIME()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("New Battle");
    }
}

/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

public class player_controller : MonoBehaviour
{
    public static player_controller instance;
    [SerializeField] private GameObject UI_Encount;
    private Animator animator;
    [SerializeField] Animator cameraAnimator;  // カメラワーク

    [SerializeField] public float speed_const = 5.0f;
    public float speed;
    public bool stop = false;
    private float encount_range = 100;
    private Rigidbody2D rb;
    private Vector2 move;
    private int idX = Animator.StringToHash("x"), idY = Animator.StringToHash("y");

    private UIManager uiManager;

    private Panel_menu panelMenu;

     private bool hasOpenedShop = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UI_Encount.SetActive(false);
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        uiManager = FindFirstObjectByType<UIManager>();

        panelMenu = FindFirstObjectByType<Panel_menu>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        x = Math.Abs(x) >= Math.Abs(y) ? x : 0;
        y = Math.Abs(y) > Math.Abs(x) ? y : 0;

        if ((Mathf.FloorToInt(x) != 0 || Mathf.FloorToInt(y) != 0) && Time.timeScale != 0f && !this.stop)
        {   
            this.speed = this.speed_const;
            this.animator.speed = 1;
            this.animator.SetFloat (idX, x);
			this.animator.SetFloat (idY, y);
        }
        else
        {   
            this.speed = 0;
            this.animator.speed = 0;
        }
        this.move = new Vector2(x, y);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "enemy")
        {
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "Library_door")
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("Agora");//移動先のシーンの名前を必ずshopにしてください！
        }
        else if (collision.CompareTag("Shop") && !hasOpenedShop)
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log("Shopに入りました");
            hasOpenedShop = true;

            if (panelMenu != null)
            {
                // panelMenu.CloseItemPanel(); // ESCキーと同じ動作
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "encounter")
        {
            int randomencount = UnityEngine.Random.Range(0, Mathf.FloorToInt(encount_range));
            encount_range -= Time.fixedDeltaTime;
            Debug.Log(collision.gameObject);
            if (encount_range < 0) { encount_range = 0; }

            if (randomencount == 0)
            {
                encount_range = 100;  // 初期化
                UI_Encount.SetActive(true);
                stop = true;
                cameraAnimator.SetTrigger("Encounter");
                StartCoroutine("WAITTIME");
                int randomenemy = UnityEngine.Random.Range(0, 3);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.enemyNumberToBattle = randomenemy;
                }
            }
        }
        if (collision.tag == "danpen")
        {
            UI_Encount.SetActive(true);
            animator.SetTrigger("encount");
            StartCoroutine("WAITTIME");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.enemyNumberToBattle = 4; //（または 10 のまま ※要確認）
            }
        }
        if (collision.tag == "boss") // 1. で作成したタグ名
        {
            // エンカウント演出を開始
            UI_Encount.SetActive(true);

            // アニメーターが設定されていればトリガーを引く
            if (animator != null)
            {
                animator.SetTrigger("encount");
            }
            encount_range = 1000000;  // 初期化
            stop = true; // プレイヤーを止める
            StartCoroutine("WAITTIME"); // 戦闘シーンへ

            
                GameManager.Instance.enemyNumberToBattle = 5; // ★ 敵IDを 5 に設定
        }
        if (collision.tag == "Item")
        {
            if (uiManager != null)
            {
                uiManager.ShowItemGetPanel();
            }
            collision.gameObject.SetActive(false);
        }

    }
    IEnumerator WAITTIME()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("New Battle");
    }
}
*/