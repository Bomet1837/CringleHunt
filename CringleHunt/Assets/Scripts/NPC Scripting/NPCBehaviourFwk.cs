using UnityEngine;
using UnityEngine.Events;

///<summary>
/// Serves as a foundational framework for NPC behaviours. Extend this class to implement specific AI functionalities such as patrolling, chasing, or attacking.
///</summary>
public class NPCBehaviourFwk : MonoBehaviour
{
    protected NPCEntity npcEntity;
    
    protected virtual void Awake()
    {
        npcEntity = GetComponent<NPCEntity>();
        if (npcEntity == null)
        {
            Debug.LogError("NPCEntity component not found on " + gameObject.name);
        }
    }
}
