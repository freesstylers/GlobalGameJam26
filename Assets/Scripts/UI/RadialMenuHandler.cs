using TMPro;
using UnityEngine;
using Rewired;
using static Mask;
using UnityEngine.EventSystems;

public class RadialMenuHandler : MonoBehaviour
{
    public float deadzoneJoystick = 0.1f;

    public Animator[] slots;
    public GameObject RadialMenuDisplay;
    public TextMeshProUGUI statDataTxt;
    public MaskColor currentMask;

    bool radialOn = false;

    Vector2 MouseAccumulaMov;

    private FMOD.Studio.EventInstance changeInstance_;
    private FMOD.Studio.EventInstance chooseInstance_;

    private void Start()
    {
        changeInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/ChangeMask");
        chooseInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/Choose");
    }

    void Update()
    {
        if (!radialOn && Rewired.ReInput.players.Players[0].GetButtonDown("radial"))
        {
            FlowManager.instance.SlowDown(true);

            MouseAccumulaMov = Vector2.zero;
            RadialMenuDisplay.SetActive(true);

            radialOn = true;
            chooseInstance_.start();

            FlowManager.instance.currentPlayer.SetPlayerLook(false);
        }
        else if (radialOn && Rewired.ReInput.players.Players[0].GetButtonUp("radial"))
        {
            if (currentMask != MaskColor.NONE)
                SelectMask();


            FlowManager.instance.SlowDown(false);

            RadialMenuDisplay.SetActive(false);

            radialOn = false;

            FlowManager.instance.currentPlayer.SetPlayerLook(true);
        }


        if (!radialOn)
            return;

        //Debug.Log(mov);

        if (Rewired.ReInput.players.Players[0].controllers.GetLastActiveController() == Rewired.ReInput.players.Players[0].controllers.Mouse || Rewired.ReInput.players.Players[0].controllers.GetLastActiveController() == Rewired.ReInput.players.Players[0].controllers.Keyboard)
            MouseAccumulaMov += new Vector2(Rewired.ReInput.players.Players[0].GetAxis("xCamera"), Rewired.ReInput.players.Players[0].GetAxis("yCamera"));
        else
            MouseAccumulaMov = new Vector2(Rewired.ReInput.players.Players[0].GetAxis("xCamera"), Rewired.ReInput.players.Players[0].GetAxis("yCamera"));


        if (Mathf.Abs(MouseAccumulaMov.magnitude) >= deadzoneJoystick)
        {
            float deg = Mathf.Rad2Deg * Mathf.Atan2(MouseAccumulaMov.x, MouseAccumulaMov.y);

            if (deg < 0)
                deg += 360;

            MaskColor selected;

            if (deg < 300 && deg >= 180)
                selected = MaskColor.BLUE;
            else if (deg < 180 && deg >= 60)
                selected = MaskColor.YELLOW;
            else
                selected = MaskColor.RED;

            if (selected != currentMask)
            {
                if (currentMask != MaskColor.NONE)
                    slots[(int)currentMask].SetBool("selected", false);
                slots[(int)selected].SetBool("selected", true);
                UpdateSelectedMask(selected);
            }
        }
        else if (currentMask != MaskColor.NONE)
        {
            slots[(int)currentMask].SetBool("selected", false);
            UpdateSelectedMask(MaskColor.NONE);
            statDataTxt.gameObject.SetActive(false);
        }
    }

    public void UpdateSelectedMask(MaskColor mask)
    {
        currentMask = mask;

        if (mask == MaskColor.NONE)
        {
            statDataTxt.gameObject.SetActive(false);
        }
        else
        {
            Stats s = FlowManager.instance.masks_[(int)currentMask].stats_;

            string statTxt =
                "<sprite=0>Daño: " + s.realDmg_ + "\n" +
                "<sprite=1>RoF: " + s.realRate_ + "\n" +
                "<sprite=2>Max Ammo: " + s.realAmmo_ + "\n" +
                "<sprite=3>Player Speed: " + s.realSpeed_ + "\n" +
                "<sprite=4>Reload Speed: " + s.realReload_;


            statDataTxt.gameObject.SetActive(true);
        }
    }

    public void SelectMask()
    {
        if (currentMask != MaskColor.NONE)
        {
            FlowManager.instance.SetMask((int)currentMask);
            changeInstance_.setParameterByName("Color", (int)currentMask);
            changeInstance_.start();
        }
    }
}
