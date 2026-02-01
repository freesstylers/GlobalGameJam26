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

        Cursor.lockState = CursorLockMode.None; //pilla el foco

        Tiers = new Dictionary<Upgrade.UpgradeClass, int>();

        foreach (var m in MaskButtons)
        {
            m.interactable = false;
        }

        for (int i = 0; i < MaskStatTexts.Count; i++)
        {
            MaskStatTexts[i].text = FlowManager.instance.masks_[i].stats_.GetTextDump();
        }

        RefreshSales();
    }

    public void RefreshSales()
    {
        //restar cost
        FlowManager.instance.pointsInterface -= timesRefreshed * BaseRefreshCost;


        bool comprobacion;

        timesRefreshed++;

        for (Upgrade.UpgradeClass i = Upgrade.UpgradeClass.DMG; i <= Upgrade.UpgradeClass.HP; i++)
        {
            int rand = UnityEngine.Random.Range(0, maxLevelRandom);

            Tiers[i] = rand;

            Upgrade aux = new Upgrade(i, rand);

            int cost = aux.cost_;

            CostAndLevel[i].lvl.text = "" + (rand + 1);
            CostAndLevel[i].cost.text = cost.ToString();

            comprobacion = FlowManager.instance.pointsInterface >= cost;

            CostAndLevel[i].cost.color = comprobacion ? CanBuy : CanNOTBuy;
            CostAndLevel[i].b.interactable = comprobacion;
        }

        comprobacion = FlowManager.instance.pointsInterface >= timesRefreshed * BaseRefreshCost;

        RefreshCostTxt.text = "-" + (timesRefreshed * BaseRefreshCost);
        RefreshCostTxt.color = comprobacion ? CanBuy : CanNOTBuy;
        RefreshCostButton.interactable = comprobacion;
    }

    public PegatinaSelectable pegatinaBase;
    public Transform pegatinaContainer;
    public List<PegatinaSelectable> pegatinas;
    public List<Sprite> pegatinasSprites;


    public void Buy(int upgrade)
    {
        Buy((Upgrade.UpgradeClass)upgrade + 1);
    }

    public void Buy(Upgrade.UpgradeClass upgrade)
    {
        Upgrade u = new Upgrade(upgrade, Tiers[upgrade]);
        u.pegatina_ = pegatinasSprites[(int)upgrade];
        int cost = u.cost_;

        //restar cost
        FlowManager.instance.pointsInterface -= cost;

        PegatinaSelectable p = Instantiate(pegatinaBase, pegatinaContainer);
        p.SetUpgrade(u);
        p.Init(this);
        p.gameObject.SetActive(true);
        pegatinas.Add(p);


        bool comprobacion;

        for (Upgrade.UpgradeClass i = Upgrade.UpgradeClass.DMG; i <= Upgrade.UpgradeClass.HP; i++)
        {
            int specificTier = Tiers[i];

            Upgrade aux = new Upgrade(i, specificTier);

            int costAux = aux.cost_;

            comprobacion = FlowManager.instance.pointsInterface >= costAux;

            CostAndLevel[i].cost.color = comprobacion ? CanBuy : CanNOTBuy;
            CostAndLevel[i].b.interactable = comprobacion;
        }

        comprobacion = FlowManager.instance.pointsInterface >= timesRefreshed * BaseRefreshCost;

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

        Cursor.lockState = CursorLockMode.Locked; //pilla el foco
        FlowManager.instance.UpdateMasks();
    }

    [HideInInspector]
    public PegatinaSelectable upgradeApplied;

    public List<Button> MaskButtons;
    public List<TextMeshProUGUI> MaskStatTexts;

    public void PrepareMasksForUpgrade(PegatinaSelectable u)
    {
        if (upgradeApplied != null)
            upgradeApplied.Unselected();

        upgradeApplied = u;

        foreach (var m in MaskButtons)
        {
            m.interactable = true;
        }
    }

    public void ApplyUpgrade(int mask)
    {
        Apply(upgradeApplied, (Mask.MaskColor)mask);

        for (int i = 0; i < MaskStatTexts.Count; i++)
        {
            MaskStatTexts[i].text = FlowManager.instance.masks_[i].stats_.GetTextDump();
        }

        NoMasksUpgrade();
    }

    public void NoMasksUpgrade()
    {
        upgradeApplied = null;

        foreach (var m in MaskButtons)
        {
            m.interactable = false;
        }
    }
}
