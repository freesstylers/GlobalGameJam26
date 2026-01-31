using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PegatinaSelectable :  MonoBehaviour
{

    public Upgrade upgrade_;

    private RawImage image_;
    private Button boton_;
    private Animator animator_;
    private EventTrigger eventTrigger_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image_ = GetComponent<RawImage>();
        image_.texture = upgrade_.pegatina_;

        boton_ = GetComponent<Button>();
        boton_.onClick.AddListener(OnClick);

        animator_ = GetComponent<Animator>();
        eventTrigger_ = GetComponent<EventTrigger>();
    }

    void OnClick()
    {
        Debug.Log(upgrade_.class_);
    }

    public void OnHoverStart()
    {
        animator_.SetBool("Hovering", true);
    }

    public void OnHoverExit()
    {
        animator_.SetBool("Hovering", false);
    }
}
