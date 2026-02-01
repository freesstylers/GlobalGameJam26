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

    public TextMeshProUGUI RefreshCostTxt;
    public Button RefreshCostButton;

    public int maxLevelRandom = 5;

    public Color CanBuy = Color.white;
    public Color CanNOTBuy = Color.red;

    int timesRefreshed = 0;

    public int BaseRefreshCost = 100;

    private void OnEnable()
    {
        timesRefreshed = 0;
        RefreshSales();
    }

    public void RefreshSales()
    {
        //restar cost
        FlowManager.instance.pointsInterface -= timesRefreshed * BaseRefreshCost;


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

        RefreshCostTxt.text = "-" + (timesRefreshed * BaseRefreshCost);
        RefreshCostTxt.color = comprobacion ? CanBuy : CanNOTBuy;
        RefreshCostButton.interactable = comprobacion;

        timesRefreshed++;
    }

    public PegatinaSelectable pegatinaBase;
    public Transform pegatinaContainer;
    public List<PegatinaSelectable> pegatinas;


    public void Buy(int upgrade)
    {
        Buy((Upgrade.UpgradeClass)upgrade);
    }

    public void Buy(Upgrade.UpgradeClass upgrade)
    {
        Upgrade u = new Upgrade(upgrade, Tiers[upgrade]);
        int cost = u.cost_;

        //restar cost
        FlowManager.instance.pointsInterface -= cost;

        Instantiate(pegatinaBase, pegatinaContainer).upgrade_ = u;


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

        RefreshCostTxt.color = comprobacion ? CanBuy : CanNOTBuy;
        RefreshCostButton.interactable = comprobacion;
    }

    public void Apply(PegatinaSelectable p, Mask.MaskColor col)
    {
        if (pegatinas.Contains(p))
        {


            //temp?
            FlowManager.instance.masks_[(int)col].AddUpgrade(p.upgrade_);


            pegatinas.Remove(p);

            DestroyImmediate(p.gameObject);
        }
    }

    public void CloseMenu()
    {
        FlowManager.instance.UpdateMasks();
    }
}
