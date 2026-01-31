using System;
using UnityEngine;
using static FlowManager;

public class EnemyBase : MonoBehaviour
{
    public Mask.MaskColor enemyColor;
    public int maxHp_;
    private int hp_;
    public bool spawning_;
    public bool dying_;

    FlowManager.enemyType type;

    //private BasicEnemyMovement movement_;
    private Transform transform_;

    public float scaleChangeRate_ = 0.5f;

    public Mask.MaskColor color_;
    public GameObject filter_;

    public Animator hurtAnimation_;

    void Awake()
    {
        hp_ = maxHp_;

        enemyColor = (Mask.MaskColor)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Mask.MaskColor)).Length);
    }

    private void Start()
    {
        //movement_ = GetComponent<BasicEnemyMovement>();
        transform_ = GetComponent<Transform>();
    }

    public void EnemyInit(Mask.MaskColor c)
    {
        switch (c)
        {
            case Mask.MaskColor.RED:
                filter_.GetComponent<MeshRenderer>().materials[1] = FlowManager.instance.enemyFilters_[0];
                break;
            case Mask.MaskColor.BLUE:
                filter_.GetComponent<MeshRenderer>().materials[1] = FlowManager.instance.enemyFilters_[1];
                break;
            case Mask.MaskColor.YELLOW:
                filter_.GetComponent<MeshRenderer>().materials[1] = FlowManager.instance.enemyFilters_[2];
                break;
            case Mask.MaskColor.NONE:
                break;
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
    }

    public void ReceiveDamage(int dmg)
    {
        hp_ -= dmg;
        if (hp_ <= 0) Die();
        hurtAnimation_.Play("OnHit");
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
            FlowManager.instance.currentPoints += 15 + UnityEngine.Random.Range(0, 10);
            Destroy(this.gameObject);
        }
    }
}
