using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.SetBool("Button_menu_highlighted", true);
        anim.SetBool("Button_menu_normal", false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool("Button_menu_highlighted", false);
        anim.SetBool("Button_menu_normal", true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        anim.ResetTrigger("Button_menu_pressed");
        anim.SetTrigger("Button_menu_pressed");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 指を離したら通常状態に戻す
        anim.SetBool("Button_menu_highlighted", true);
        anim.SetBool("Button_menu_normal", false);
    }
}