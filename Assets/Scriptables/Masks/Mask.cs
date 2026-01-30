using UnityEngine;

[CreateAssetMenu(fileName = "Mask", menuName = "Scriptable Objects/Mask")]
public class Mask : ScriptableObject
{
    public enum MaskColor
    {
        NONE, RED, GREEN, BLUE
    }

    public MaskColor color_ = MaskColor.NONE;
    public Stats stats_;

    public void AddUpgrade(Upgrade u)
    {
        stats_.AddUpgrade(u);
    }
}
