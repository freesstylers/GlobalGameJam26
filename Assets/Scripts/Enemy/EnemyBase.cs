using System;
using System.Collections.Generic;
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
    public List<Collider> collider_to_deactivate;
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

    private void OnDestroy()
    {
        hitInstance_.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        hitInstance_.release();
    }

    public void EnemyInit(Mask.MaskColor c)
    {
        int layer;
        FlowManager.instance.SuscribeMaskChange(OnMaskChange);
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

        OnMaskChange(FlowManager.instance.currentPlayer.currentMask_);
    }

    private void SetGameLayerRecursive(GameObject _go, int _layer)
    {
        _go.layer = _layer;
        foreach (Transform child in _go.transform)
        {
            if (child.gameObject.name == "Aura" || child.gameObject.name == "Aurilla") continue;
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
        FlowManager.instance.addEnemyKilled();
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
            transform_.localScale -= new Vector3(scaleChangeRate_, scaleChangeRate_, scaleChangeRate_) * 5.0f * Time.deltaTime;
        }
        else
        {
            transform_.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            dying_ = false;
            int addedPoints = 150 + UnityEngine.Random.Range(50, 150);
            FlowManager.instance.pointsInterface += addedPoints;
            FlowManager.instance.AddScore(addedPoints);
            FlowManager.instance.currentAliveEnemies -= 1;
            if(FlowManager.instance.currentAliveEnemies == 0)
            {
                FlowManager.instance.advanceState();
            }
            FlowManager.instance.UnsuscribeMaskChange(OnMaskChange);
            reference.Release(gameObject);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject != null && other.gameObject.tag == "Bullet" && (enemyColor == FlowManager.instance.GetCurrentMask().color_ || enemyColor == Mask.MaskColor.NONE) && !dying_)
        {
            //Guarrada historica
            float dmg = FlowManager.instance.GetCurrentMask().stats_.baseDmg_;
            ReceiveDamage((int)dmg);
        }*/
        if (other.gameObject != null && other.gameObject.tag == "Player")
        {
            if(other.gameObject.GetComponent<PlayerMovement>().currentMask_.color_ == enemyColor)
            {
                other.gameObject.GetComponent<PlayerMovement>().GetHurt(transform.position);
            }

        }
    }

    void OnMaskChange(Mask m)
    {
        if(m.color_ == enemyColor)
        {
            foreach(Collider c in collider_to_deactivate)
            {
                c.enabled = true;
            }
        }
        else
        {
            foreach (Collider c in collider_to_deactivate)
            {
                c.enabled = false;
            }
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
