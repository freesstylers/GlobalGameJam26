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

    Vector2 mousePosStart;

    bool radialOn = false;

    void Update()
    {
        if (!radialOn && Rewired.ReInput.players.Players[0].GetButtonDown("radial"))
        {
            FlowManager.instance.SlowDown(true);

            mousePosStart = Rewired.ReInput.players.Players[0].controllers.Mouse.screenPosition;
            RadialMenuDisplay.SetActive(true);

            radialOn = true;
        }
        else if (radialOn && Rewired.ReInput.players.Players[0].GetButtonUp("radial"))
        {
            FlowManager.instance.SlowDown(false);

            RadialMenuDisplay.SetActive(false);

            radialOn = false;
        }


        if (!radialOn)
            return;

        Vector2 mov = Rewired.ReInput.players.Players[0].controllers.Mouse.screenPosition - mousePosStart;

        if (Rewired.ReInput.players.Players[0].controllers.GetLastActiveController() != Rewired.ReInput.players.Players[0].controllers.Mouse)
            mov = new Vector2(Rewired.ReInput.players.Players[0].GetAxis("xCamera"), Rewired.ReInput.players.Players[0].GetAxis("yCamera"));

        if(Mathf.Abs(mov.magnitude) >= deadzoneJoystick)
        {
            float deg = Mathf.Rad2Deg * Mathf.Atan2(mov.x, mov.y);

            MaskColor selected;

            if (deg < 300 && deg >= 180)
                selected = MaskColor.BLUE;
            else if (deg < 180 && deg >= 60)
                selected = MaskColor.GREEN;
            else
                selected = MaskColor.RED;

            if (selected != currentMask)
            {
                if(currentMask != MaskColor.NONE)
                    slots[(int)currentMask].SetBool("selected", false);
                slots[(int)selected].SetBool("selected", true);
                UpdateSelectedMask(selected);
            }
        }
        else if(currentMask != MaskColor.NONE)
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
        if(currentMask != MaskColor.NONE)
            FlowManager.instance.SetMask((int)currentMask);
    }
}
