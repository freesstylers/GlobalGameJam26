using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Rendering.Universal;

public class PegatinaSelectable :  MonoBehaviour
{

    public Upgrade upgrade_;

    private Image image_;
    private Button boton_;
    private Animator animator_;
    private EventTrigger eventTrigger_;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image_ = GetComponent<Image>();
        image_.sprite = upgrade_.pegatina_;

        boton_ = GetComponent<Button>();
        boton_.onClick.AddListener(OnClick);

        animator_ = GetComponent<Animator>();
    }

    ShopMenuMan menuInstance;

    public void Init(ShopMenuMan me)
    {
        menuInstance = me;
    }

    void OnClick()
    {
        if (menuInstance.upgradeApplied == this)
            menuInstance.NoMasksUpgrade();
        else
            menuInstance.PrepareMasksForUpgrade(this);
    }

    public void Unselected()
    {

    }

    public void OnHoverStart()
    {
        animator_.SetBool("Hovering", true);
    }

    public void OnHoverExit()
    {
        animator_.SetBool("Hovering", false);
    }

    public void SetUpgrade(Upgrade u)
    {
        upgrade_ = u;
    }
}
