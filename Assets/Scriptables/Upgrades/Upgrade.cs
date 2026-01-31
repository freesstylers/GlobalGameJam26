using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]


public class Upgrade : ScriptableObject
{
    public enum UpgradeClass
    {
        NONE, DMG, RATE, SPEED, AMMO, RELOAD, HP
    }

    private float[] DMGPerLevel = { 1.2f, 1.4f, 1.6f, 1.8f, 2.0f };
    private float[] RatePerLevel = { 1.2f, 1.4f, 1.6f, 1.8f, 2.0f };
    private float[] SpeedPerLevel = { 1.2f, 1.4f, 1.6f, 1.8f, 2.0f };
    private int[] AmmoPerLevel = { 10, 20, 30, 40, 50 };
    private float[] ReloadPerLevel = { 1.2f, 1.4f, 1.6f, 1.8f, 2.0f };
    private int[] HPPerLevel = { 10, 20, 30, 40, 50 };

    public UpgradeClass class_ = UpgradeClass.NONE;
    private string name_ = "";
    public int level_ = 1;
    public float mult_ = 2.0f;
    public int value_ = 10;
    public int cost_ = 100;

    public Texture2D pegatina_;

    private void Awake()
    {
        switch(class_) {
            case UpgradeClass.DMG:
                mult_ = DMGPerLevel[level_];
                cost_ = (int)(cost_ * mult_);
                name_ = "Pegata poderosa";
                break;
            case UpgradeClass.RATE:
                mult_ = RatePerLevel[level_];
                cost_ = (int)(cost_ * mult_);
                name_ = "Papela cañera";
                break;
            case UpgradeClass.SPEED:
                mult_ = SpeedPerLevel[level_];
                cost_ = (int)(cost_ * mult_);
                name_ = "Pegatinilla rapidilla";
                break;
            case UpgradeClass.RELOAD:
                mult_ = ReloadPerLevel[level_];
                cost_ = (int)(cost_ * mult_);
                name_ = "Adhesivo preparado";
                break;
            case UpgradeClass.AMMO:
                value_ = AmmoPerLevel[level_];
                cost_ = (int)(cost_ + value_);
                name_ = "Etiqueta cargada";
                break;
            case UpgradeClass.HP:
                value_ = HPPerLevel[level_];
                cost_ = (int)(cost_ + value_);
                name_ = "Sticker grueso";
                break;
        }
        cost_ += (int)Random.Range(10.0f, 30.0f);

        name += " level " + level_.ToString();
    }

}
