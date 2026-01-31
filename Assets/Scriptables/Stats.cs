using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Stats", menuName = "Scriptable Objects/Stats")]
public class Stats : ScriptableObject
{
    public int maxPlayerHP_ = 10;
    public int playerHP_ = 10;
    public float baseDmg_ = 1;
    public float baseRate_ = 1.0f;
    public int baseAmmo_ = 100;
    public float baseSpeed_ = 10.0f;
    public float baseReload_ = 1.0f;

    public int basePlayerHP_ = 10;
    public float realDmg_ = 1;
    public float realRate_ = 1.0f;
    public int realAmmo_ = 100;
    public float realSpeed_ = 10.0f;
    public float realReload_ = 1.0f;

    private List<Upgrade> upgrades_;
    private float radiusToTogleSpawners = 5.0f;

    public CapsuleCollider spawnerCollider;

    public void resetRadiusForSpawners()
    {
        spawnerCollider.radius = 0.0f;
    }

    public IEnumerator lerpRadiusForSpawners()
    {
        float aux = 0.0f;
        while (aux < radiusToTogleSpawners)
        {
            aux += Time.deltaTime;
            yield return null;
        }
    }

    public void ResetHP()
    {
        playerHP_ = maxPlayerHP_;
    }

    public void UpdateStats()
    {
        foreach (Upgrade u in upgrades_)
        {
            switch(u.class_)
            {
                case Upgrade.UpgradeClass.DMG:
                    realDmg_ *= u.mult_;
                    break;
                case Upgrade.UpgradeClass.RATE:
                    realRate_ *= u.mult_;
                    break;
                case Upgrade.UpgradeClass.SPEED:
                    realSpeed_ *= u.mult_;
                    break;
                case Upgrade.UpgradeClass.AMMO:
                    realAmmo_ += u.value_;
                    break;
                case Upgrade.UpgradeClass.RELOAD:
                    realReload_ *= u.mult_;
                    break;
                case Upgrade.UpgradeClass.HP:
                    maxPlayerHP_ += u.value_;
                    playerHP_ += u.value_;
                    if (playerHP_ > maxPlayerHP_) playerHP_ = maxPlayerHP_;
                    break;
                default:
                    break;
            }
        }
    }

    public void AddUpgrade(Upgrade u)
    {
        upgrades_.Add(u);
    }

    public void ResetStats()
    {
        upgrades_.Clear();
        ResetHP();
        realDmg_ = baseDmg_;
        realRate_ = baseRate_;
        realAmmo_ = baseAmmo_;
        realReload_ = baseReload_;
        realSpeed_ = baseSpeed_;
    }
}
