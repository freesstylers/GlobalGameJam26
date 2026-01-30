using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]


public class Upgrade : ScriptableObject
{
    public enum UpgradeClass
    {
        NONE, DMG, SPEED, AMMO, RELOAD
    }

    public UpgradeClass class_ = UpgradeClass.NONE;
    public float mult_ = 2.0f;
    
}
