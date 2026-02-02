using UnityEngine;
using UnityEngine.Events;

///<summary>
/// Serves as a foundational framework for NPC behaviours. Extend this class to implement specific AI functionalities such as patrolling, chasing, or attacking.
///</summary>


public interface NPCBehaviourInterface_Spawning
{
    void OnNPCSpawn();
    void OnNPCDespawn();
}

public interface NPCBehaviourInterface_Detection
{
    void OnNPCAlerted();
}

public interface NPCBehaviourInterface_Combat
{
    void OnNPCAttack();
    void OnNPCDeath();
}

public class NPCBehaviourFwk : MonoBehaviour
{
    protected NPCEntity npcEntity;

    /*[Header("NPC Behaviour Framework - Event Hooks")]
    public UnityEvent
        onNPCSpawn,
        onNPCDespawn,
        onNPCDeath,
        onNPCAlerted,
        onNPCAttack;
    public UnityEvent[] npcEventHooks = new UnityEvent[] { };*/
    
    protected virtual void Awake()
    {
       /* npcEventHooks = new UnityEvent[] May need in future? Possibly unnecessary implementation
        {
            onNPCSpawn,
            onNPCDespawn,
            onNPCDeath,
            onNPCAlerted,
            onNPCAttack
        };*/
        
        npcEntity = GetComponent<NPCEntity>();
        if (npcEntity == null)
        {
            Debug.LogError("NPCEntity component not found on " + gameObject.name);
        }
    }

    protected virtual void Update()
    {
       /* foreach (UnityEvent e in npcEventHooks)
        {
            switch (e)
            {
                case var _ when e == null:
                    Debug.LogWarning("One of the NPC event hooks is not assigned in " + gameObject.name + ". May be intentional.");
                    break;

                // spawn + despawn stuff here
                
                case var _ when e == onNPCAlerted:
                    onNPCAlerted.Invoke();
                    break;
                
                case var _ when e == onNPCAttack:
                    onNPCAttack.Invoke();
                    break;
                
                

                default:
                    break;
            }
        }*/
    }
    
    
    
    
    
    
}
