using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]


public class Upgrade : ScriptableObject
{
    public enum UpgradeClass
    {
        NONE, DMG, RATE, SPEED, AMMO, RELOAD, HP
    }

    public UpgradeClass class_ = UpgradeClass.NONE;
    public float mult_ = 2.0f;
    public int value_ = 10;

    public Texture2D pegatina_;
    
}
