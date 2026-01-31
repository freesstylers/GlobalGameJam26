using UnityEngine;
using static FlowManager;

[CreateAssetMenu(fileName = "EnemyScriptable", menuName = "Scriptable Objects/EnemyScriptable")]
public class EnemyScriptable : ScriptableObject
{
    public new string name;

    public enemyType type;

    public GameObject prefab;

    public int maxHP;
    public float speed;
    public float damage;
    public int reward;
}
