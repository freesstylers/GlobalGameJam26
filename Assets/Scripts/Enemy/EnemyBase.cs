using System;
using UnityEngine;
using static FlowManager;

public class EnemyBase : MonoBehaviour
{
    public Mask.MaskColor enemyColor = Mask.MaskColor.NONE;
    public int maxHp_;
    [SerializeField]
    private int hp_;
    public bool spawning_;
    public bool dying_;

    FlowManager.enemyType type;

    //private BasicEnemyMovement movement_;
    private Transform transform_;

    public float scaleChangeRate_ = 0.5f;

    public GameObject filter_;

    public Animator hurtAnimation_;

    public PoolTemplate reference;

    private FMOD.Studio.EventInstance hitInstance_;

    //void Awake()
    //{
    //    hp_ = maxHp_;
    //    enemyColor = (Mask.MaskColor)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Mask.MaskColor)).Length);
    //}

    private void Start()
    {
        hitInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/EnemyHit");
        transform_ = GetComponent<Transform>();
    }

    private void OnEnable()
    {
        transform_ = GetComponent<Transform>();
        hp_ = maxHp_;
        enemyColor = (Mask.MaskColor)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Mask.MaskColor)).Length - 1);
        EnemyInit(enemyColor);
        transform_.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void EnemyInit(Mask.MaskColor c)
    {
        int layer;

        switch (c)
        {
            case Mask.MaskColor.RED:
                //filter_.GetComponent<MeshRenderer>().materials[1] = FlowManager.instance.enemyFilters_[0];

                layer = LayerMask.NameToLayer("RedC");
                SetGameLayerRecursive(gameObject, layer);
                FlowManager.instance.redEnemies += 1;
                break;
            case Mask.MaskColor.BLUE:
                //filter_.GetComponent<MeshRenderer>().materials[1] = FlowManager.instance.enemyFilters_[1];

                layer = LayerMask.NameToLayer("BlueC");
                SetGameLayerRecursive(gameObject, layer);
                FlowManager.instance.blueEnemies += 1;
                break;
            case Mask.MaskColor.YELLOW:
                //filter_.GetComponent<MeshRenderer>().materials[1] = FlowManager.instance.enemyFilters_[2];

                layer = LayerMask.NameToLayer("YellowC");
                SetGameLayerRecursive(gameObject, layer);
                FlowManager.instance.greenEnemies += 1;
                break;
            case Mask.MaskColor.NONE:
                break;
        }

        FlowManager.instance.UpdateEnemyCount();
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            if (child.gameObject.name == "Aura") continue;
            child.gameObject.layer = _layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
                SetGameLayerRecursive(child.gameObject, _layer);

        }
    }

    private void Update()
    {
        if (spawning_) SpawnUpdate();
        if (dying_) DieUpdate();
    }

    public void Spawn()
    {
        spawning_ = true;
    }

    public void Die()
    {
        //movement_.Stop();
        dying_ = true;

        switch (enemyColor)
        {
            case Mask.MaskColor.RED:
                FlowManager.instance.redEnemies -= 1;
                break;
            case Mask.MaskColor.BLUE:
                FlowManager.instance.blueEnemies -= 1;
                break;
            case Mask.MaskColor.YELLOW:
                FlowManager.instance.greenEnemies -= 1;
                break;
            case Mask.MaskColor.NONE:
                break;
        }

        FlowManager.instance.UpdateEnemyCount();
    }

    public void ReceiveDamage(int dmg)
    {
        hp_ -= dmg;
        if (hp_ <= 0) Die();
        hurtAnimation_.Play("OnHit");
        hitInstance_.start();
    }

    private void SpawnUpdate()
    {
        if(transform_.localScale.x <= 1.0)
        {
            transform_.localScale += new Vector3(scaleChangeRate_, scaleChangeRate_, scaleChangeRate_) * Time.deltaTime;
        }
        else
        {
            transform_.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            spawning_ = false;
        }
    }

    private void DieUpdate()
    {
        if (transform_.localScale.x >= 0.0)
        {
            transform_.localScale -= new Vector3(scaleChangeRate_, scaleChangeRate_, scaleChangeRate_) * Time.deltaTime;
        }
        else
        {
            transform_.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            dying_ = false;
            FlowManager.instance.pointsInterface += 15 + UnityEngine.Random.Range(0, 10);
            FlowManager.instance.currentAliveEnemies -= 1;
            if(FlowManager.instance.currentAliveEnemies == 0)
            {
                FlowManager.instance.advanceState();
            }
            reference.Release(gameObject);
        }
    }



    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != null && other.gameObject.tag == "Bullet" && (enemyColor == FlowManager.instance.GetCurrentMask().color_ || enemyColor == Mask.MaskColor.NONE))
        {
            //Guarrada historica
            float dmg = FlowManager.instance.GetCurrentMask().stats_.baseDmg_;
            ReceiveDamage((int)dmg);
        }
        else if (other.gameObject != null && other.gameObject.tag == "Player")
        {
            other.collider.gameObject.GetComponent<PlayerMovement>().GetHurt(transform.position);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject != null && other.gameObject.tag == "Player")
    //    {
    //        other.GetComponent<PlayerMovement>().GetHurt(transform.position);
    //    }
    //}

}
