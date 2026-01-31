using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptable", menuName = "Scriptable Objects/EnemyScriptable")]
public class EnemyScriptable : ScriptableObject
{
    public new string name;

    public GameObject prefab;

    public int maxHP;
    public float speed;
    public float damage;
    public int reward;
}
