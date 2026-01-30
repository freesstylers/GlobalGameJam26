using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName = "Scriptable Objects/Stats")]
public class Stats : ScriptableObject
{
    public int maxPlayerHP_ = 10;
    public int playerHP_ = 10;
    public int baseDmg_ = 1;
    public float baseAmmo_ = 100.0f;
    public float baseSpeed_ = 10.0f;
    public float baseReload_ = 1.0f;

    private float multDmg_ = 1.0f;
    private float multSpeed_ = 1.0f;
    private float multReload = 1.0f;

    private int realDmg_ = 1;
    private float realAmmo_ = 100.0f;
    private float realSpeed_ = 10.0f;
    private float realReload_ = 1.0f;

    private void Awake()
    {
        ResetHP();
    }

    public void ResetHP()
    {
        playerHP_ = maxPlayerHP_;
    }

    public void UpdateStats()
    {

    }
}
