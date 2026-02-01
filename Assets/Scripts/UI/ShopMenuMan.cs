using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86;

public class ShopMenuMan : MonoBehaviour
{
    [Serializable]
    public struct ShopSlot
    {
        public TextMeshProUGUI lvl;
        public TextMeshProUGUI cost;
        public Button b;
    }

    public SerializedDictionary<Upgrade.UpgradeClass, ShopSlot> CostAndLevel;
    public Dictionary<Upgrade.UpgradeClass, int> Tiers;

    public TextMeshProUGUI RefreshCost;
    public Button RefreshCostButton;

    public int maxLevelRandom = 5;

    public Color CanBuy = Color.white;
    public Color CanNOTBuy = Color.red;

    int timesRefreshed = 0;

    private void OnEnable()
    {
        timesRefreshed = 0;
        RefreshSales();
    }

    public void RefreshSales(int refreshCost = 0)
    {
        if (refreshCost > 0)
            timesRefreshed++;
        //restar cost


        bool comprobacion;

        for (Upgrade.UpgradeClass i = Upgrade.UpgradeClass.DMG; i <= Upgrade.UpgradeClass.HP; i++)
        {
            int rand = UnityEngine.Random.Range(1, maxLevelRandom + 1);

            Upgrade aux = new Upgrade(i, rand);

            int cost = aux.cost_;

            CostAndLevel[i].lvl.text = rand.ToString();
            CostAndLevel[i].cost.text = cost.ToString();


            comprobacion = true;

            CostAndLevel[i].cost.color = comprobacion ? CanBuy : CanNOTBuy;
            CostAndLevel[i].b.interactable = comprobacion;

            Tiers[i] = rand;
        }


        comprobacion = true;

        RefreshCost.text = (timesRefreshed * 100).ToString();
        RefreshCost.color = comprobacion ? CanBuy : CanNOTBuy;
        RefreshCostButton.interactable = comprobacion;
    }

    public void Buy(Upgrade.UpgradeClass upgrade)
    {
        int cost = new Upgrade(upgrade, Tiers[upgrade]).cost_;

        //restar cost



        bool comprobacion;

        for (Upgrade.UpgradeClass i = Upgrade.UpgradeClass.DMG; i <= Upgrade.UpgradeClass.HP; i++)
        {
            int specificTier = Tiers[i];

            Upgrade aux = new Upgrade(i, specificTier);

            int costAux = aux.cost_;

            comprobacion = true;

            CostAndLevel[i].cost.color = comprobacion ? CanBuy : CanNOTBuy;
            CostAndLevel[i].b.interactable = comprobacion;
        }

        comprobacion = true;

        RefreshCost.color = comprobacion ? CanBuy : CanNOTBuy;
        RefreshCostButton.interactable = comprobacion;
    }

    public void Apply(Upgrade.UpgradeClass upgrade, Mask.MaskColor col)
    {
        int tier = Tiers[upgrade];


        //temp?
        FlowManager.instance.masks_[(int)col].AddUpgrade(new Upgrade(upgrade, tier));
    }
}
