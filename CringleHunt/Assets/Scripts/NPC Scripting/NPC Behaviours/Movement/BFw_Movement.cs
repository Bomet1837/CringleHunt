using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BFw_Movement : MonoBehaviour
{
    public float personalDistance = 2f;
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public float fleeSpeed = 3f;
    public float patrolSpeed = 1.5f;
    public float jumpForce = 5f;

    public LayerMask obstacleLayer;
    public bool debugsEnabled = false;

    private NPCEntity _npcEntity;
    private Rigidbody _rb;
    private NavMeshAgent _navAgent;
    private Transform _closestObst;
    private GunModule _gunModule;

    private void Awake()
    {
        _npcEntity = GetComponent<NPCEntity>();
        if (!_npcEntity)
        {
            Debug.LogError(debugsEnabled ? "NPCEntity component not found on " + gameObject.name : null);
            return;
        }

        _rb = _npcEntity.rb;
        _navAgent = GetComponent<NavMeshAgent>();

        // If we have a NavMeshAgent, let it control position but keep rotation control disabled so we can look-at smoothly
        if (_navAgent != null)
        {
            _navAgent.updateRotation = false;
            _navAgent.updateUpAxis = true;
        }
    }

    public void FixedUpdate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, personalDistance, obstacleLayer);
        Transform closest = null;
        float closestDist = Mathf.Infinity;

        for (int i = 0; i < hits.Length; i++)
        {
            Transform t = hits[i].transform;
            if (t == transform) continue;
            
            float distance = Vector3.SqrMagnitude(t.position - transform.position);
            if (distance < closestDist)
            {
                closestDist = distance;
                closest = t;
                _closestObst = closest;
            }
        }
        
        if (_closestObst != null)
        {
            MaintainPersonalSpace(_closestObst);
        }
    }
    
    public void MaintainPersonalSpace(Transform target)
    {
        if (_navAgent == null || target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < personalDistance)
        {
            Vector3 directionAway = (transform.position - target.position).normalized;
            Vector3 newDestination = transform.position + directionAway * (personalDistance - distance);
            _navAgent.SetDestination(newDestination);
            //Debug.Log("Maintaining personal space from " + target.name);
        }
    }

    public IEnumerator CombatCoroutine(Transform targetPosition)
    {
        while (targetPosition != null && _npcEntity != null && _npcEntity.Health > 0 && _navAgent != null)
        {
            Vector3 target = targetPosition.position;
            LookAt(target);

            Vector3 targetBound = new Vector3(target.x - personalDistance, transform.position.y, target.z - personalDistance);
            
            if (_navAgent != null)
            {
                if (_npcEntity.Health >= 60f)
                {
                    _navAgent.speed = chaseSpeed;
                    _navAgent.SetDestination(targetBound);
                }
                else if (_npcEntity.Health >= 30f)
                {
                    _navAgent.speed = moveSpeed;
                    _navAgent.SetDestination(targetBound);
                }
                else
                {
                    _navAgent.speed = fleeSpeed;

                    // For flee, compute a point away from the threat by moving opposite direction
                    Vector3 fleeDirection = (transform.position - targetBound).normalized;
                    Vector3 fleeTarget = transform.position + fleeDirection * (2f * personalDistance);
                    _navAgent.SetDestination(fleeTarget);
                }
            }
            yield return new WaitForFixedUpdate();
        }

        // When coroutine ends, ensure NavMeshAgent stops if present
        if (_navAgent != null)
        {
            _navAgent.isStopped = true;
        }
    }

    public void NpcJump()
    {
        if (_navAgent != null)
        {
            // If using NavMeshAgent, temporarily disable position updates so physics can act; simple approach: disable agent
            _navAgent.enabled = false;
            if (_rb != null) _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // Re-enable agent shortly after (caller could handle timing); keep simple here
            _navAgent.enabled = true;
        }
    }

    public void LookAt(Vector3 targetPosition)
    {
        if (_rb.constraints != RigidbodyConstraints.None)
        {
            Vector3 lookPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            Vector3 direction = (lookPos - transform.position).normalized;
            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
            }
        }
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        if (_rb.constraints != RigidbodyConstraints.None)
        {
            _navAgent.speed = moveSpeed;
            _navAgent.SetDestination(targetPosition);
            _navAgent.isStopped = false;
            return;
        }
    }

    public void Chase(Vector3 targetPosition)
    {
        if (_navAgent != null)
        {
            _navAgent.speed = chaseSpeed;
            _navAgent.SetDestination(targetPosition);
            _navAgent.isStopped = false;
            return;
        }
    }

    public void FleeFrom(Vector3 threatPosition)
    {
        if (_navAgent != null)
        {
            _navAgent.speed = fleeSpeed;
            Vector3 direction = (transform.position - threatPosition).normalized;
            Vector3 fleeTarget = transform.position + direction * (2f * personalDistance);
            _navAgent.SetDestination(fleeTarget);
            _navAgent.isStopped = false;
            return;
        }
        
    }

    public void NpcRetreat(Vector3 retreatPosition)
    {
            Chase(retreatPosition);
    }

    public void PatrolBetween(Vector3 pointA, Vector3 pointB)
    {
        if (_navAgent != null)
        {
            Vector3 targetPoint = Vector3.Distance(transform.position, pointA) < Vector3.Distance(transform.position, pointB) ? pointB : pointA;
            _navAgent.speed = patrolSpeed;
            _navAgent.SetDestination(targetPoint);
            _navAgent.isStopped = false;
            return;
        }
    }

    public void StopMovement()
    {
        if (_navAgent != null)
        {
            _navAgent.isStopped = true;
            _navAgent.ResetPath();
        }
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, personalDistance);
        Gizmos.DrawRay(transform.position, transform.forward);
    }


}
