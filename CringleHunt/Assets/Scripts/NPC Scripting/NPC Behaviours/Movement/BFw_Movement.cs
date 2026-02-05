using System.Collections;
using UnityEngine;

public class BFw_Movement : MonoBehaviour
{
    public float personalDistance = 2f;
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float fleeSpeed = 3f;
    public float patrolSpeed = 1.5f;
    public float jumpForce = 5f;
    
    private NPCEntity _npcEntity;
    private Rigidbody _rb;
    
    private void Awake()
    {
        _npcEntity = GetComponent<NPCEntity>();
        _rb = _npcEntity.rb;
    }
    

    public IEnumerator CombatCoroutine(Vector3 targetPosition)
    {
            yield return new WaitForSeconds(0.2f);

            switch (_npcEntity.Health)
            {
                case >= 60f:
                    Debug.Log("NPC is in Chase Mode");
                    Chase(targetPosition);
                    break;
                
                case < 60f and >= 30f:
                    Debug.Log("NPC is in Cautious Mode");
                    MoveTowards(targetPosition);
                    break;
                
                case < 30f:
                    Debug.Log("NPC is in Flee Mode");
                    FleeFrom(targetPosition);
                    break;
            }
    }

    public void NpcJump()
    {
        if (_rb.constraints != RigidbodyConstraints.None)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    
    public void MoveTowards(Vector3 targetPosition)
    {
        if (_rb.constraints != RigidbodyConstraints.None)
        {
            Debug.Log(targetPosition);
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float speed = moveSpeed;
            _rb.MovePosition(transform.position + direction * (speed * Time.deltaTime));
            _rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f));
        }
    }
    
    public void Chase(Vector3 targetPosition)
    {
        if (_rb.constraints != RigidbodyConstraints.None)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float speed = chaseSpeed;
            _rb.MovePosition(transform.position + direction * (speed * Time.deltaTime));
            _rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f));
        }
    }
    
    public void FleeFrom(Vector3 threatPosition)
    {
        if (_rb.constraints != RigidbodyConstraints.None)
        {
            Vector3 direction = (transform.position - threatPosition).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float speed = fleeSpeed;
            _rb.MovePosition(transform.position + direction * (speed * Time.deltaTime));
            _rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f));
        }
    }

    public void NpcRetreat(Vector3 retreatPosition)
    {
        if (_rb.constraints != RigidbodyConstraints.None)
        {
            Chase(retreatPosition);
        }
    }
    
    public void PatrolBetween(Vector3 pointA, Vector3 pointB)
    {
        if (_rb.constraints != RigidbodyConstraints.None)
        {
            Vector3 targetPoint = Vector3.Distance(transform.position, pointA) < Vector3.Distance(transform.position, pointB) ? pointB : pointA;
            Vector3 direction = (targetPoint - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float speed = patrolSpeed;
            _rb.MovePosition(transform.position + direction * (speed * Time.deltaTime));
            _rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f));
        }
    }
    
    
    
    public void StopMovement()
    {
        _rb.linearVelocity = Vector3.zero;
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
       // Gizmos.DrawWireSphere(transform.position, personalDistance);
        Gizmos.DrawRay(transform.position, transform.forward);
    }
    

}
