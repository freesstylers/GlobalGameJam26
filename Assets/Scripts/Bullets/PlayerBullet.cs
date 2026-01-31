using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : BasicBullet
{
    public Color[] particleColors = { Color.red, Color.green, Color.blue };
    
    private void Awake()
    {
        FlowManager.instance.SuscribeMaskChange(OnMaskChange);
    }

    private void Start()
    {
        Mask currentMask = FlowManager.instance.GetCurrentMask();

        //Setting stats
        //speed = currentMask.stats_.baseBulletSpeed_;

        //Le asignamos a lo que necesite el color
        //color = particleColors[((int)currentMask.color_)];
    }

    public void OnMaskChange(Mask newMask)
    {

        //Le asignamos a lo que necesite el color
        color = particleColors[((int)newMask.color_)];
    }
}
