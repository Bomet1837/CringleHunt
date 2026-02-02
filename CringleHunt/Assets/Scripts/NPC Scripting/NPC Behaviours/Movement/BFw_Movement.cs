using UnityEngine;

public class BFw_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float chaseSpeed = 8f;
    
    private Rigidbody _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    public void MoveTowards(Vector3 targetPosition)
    {
        Debug.Log(targetPosition);
        Vector3 direction = (targetPosition).normalized;
        Quaternion targetRotation = new Quaternion(0f, direction.y, 0f, 1f);
        float speed = moveSpeed;
        //_rb.position += direction * (speed * Time.deltaTime);
        /*_rb.MovePosition(transform.position - targetPosition * (speed * Time.deltaTime));
        _rb.MoveRotation(targetRotation);*/
        _rb.Move(targetPosition, targetRotation);
    }

}
