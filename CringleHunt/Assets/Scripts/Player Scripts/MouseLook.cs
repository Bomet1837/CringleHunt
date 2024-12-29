using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class MouseLook : MonoBehaviour
{
    [Header("Camera Control Params")] 
    public float rotSpeedY;
    public float rotSpeedX;
    public float lookMult = 10;
    
    private float camRotation;
   //  private float mvar = 3000f;

    private GameObject player;
    private Rigidbody cameraSmooth;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        LockCursor(true);
        // GetComponent<Rigidbody>().AddForce(transform.forward * mvar);
    }

    // Update is called once per frame
    void Update()
    {
        float h = rotSpeedX * Input.GetAxisRaw("Mouse X") * Time.deltaTime;
        float v = rotSpeedY * Input.GetAxisRaw("Mouse Y") * Time.deltaTime;

        float lookH = h * lookMult;
        float lookV = v * lookMult;
        

        camRotation -= lookV;
        camRotation = Mathf.Clamp(camRotation, -75f, 75f);
        transform.localRotation = Quaternion.Euler(camRotation, 0, 0);

        player.transform.Rotate((Vector3.up * lookH));
    }

    void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
