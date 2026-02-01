using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerBullet : BasicBullet
{
    public Color[] particleColors = { Color.red, Color.green, Color.blue };

    private VisualEffect vfx;
    
    private void Awake()
    {
        FlowManager.instance.SuscribeMaskChange(OnMaskChange);
    }

    private void Start()
    {
        Mask currentMask = FlowManager.instance.GetCurrentMask();
        vfx = GetComponent<VisualEffect>();
        vfx.SetVector4("MaskColor", particleColors[(int)currentMask.color_]);
        vfx.SetVector3("Direction", -dir);
        //Setting stats
        //speed = currentMask.stats_.baseBulletSpeed_;

        //Le asignamos a lo que necesite el color
        //color = particleColors[((int)currentMask.color_)];
    }

    public void OnMaskChange(Mask newMask)
    {
        vfx.SetVector4("MaskColor", particleColors[(int)newMask.color_]);
        //Le asignamos a lo que necesite el color
        color = particleColors[((int)newMask.color_)];
    }
}
