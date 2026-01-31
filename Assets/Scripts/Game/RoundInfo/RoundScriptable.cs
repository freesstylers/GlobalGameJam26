using UnityEngine;
using UnityEngine.Rendering;
using static FlowManager;

[CreateAssetMenu(fileName = "RoundScriptable", menuName = "Scriptable Objects/RoundScriptable")]
public class RoundScriptable : ScriptableObject
{
    public int roundNumber;

    public AYellowpaper.SerializedCollections.SerializedDictionary<enemyType, int> roundEnemies;
}
