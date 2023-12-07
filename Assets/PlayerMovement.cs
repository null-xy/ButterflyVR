using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMovement : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI debugText;
    public float speed = 5;
    //public float gravity = -9.81f;
    public float gravity = -2.81f;
    public float flapPower = 3.5f;
    // inputSource is the customized slot for devices
    public XRNode inputSourceLeft;
    public XRNode inputSourceRight;
    public XRNode inputSourceHeadset;
    public LayerMask groundLayer;


    private float fallingSpeed;
    private float flapSpeed;
    private float glidingSpeed;
    private float flyBackSpeed;
    private XROrigin rig;
    private Vector2 inputAxis;
    private CharacterController character;
    private bool isPressed;
    private Vector3 LeftControllerVelocity;
    private Vector3 RightControllerVelocity;
    private Vector3 devicePosition;
    private InputDevice deviceLeft;
    private InputDevice deviceRight;
    //private float timer;
    private Vector3 direction;
    //private Rigidbody m_Rigidbody;

    //private Rigidbody body;
    // Start is called before the first frame update

    void Start()
    {
        //get character
        character = GetComponent<CharacterController>();
        //get rig
        rig = GetComponent<XROrigin>();
        //body = GetComponent<Rigidbody>();
        glidingSpeed = 0f;
        //m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //get input device controller from self defined-XRNode
        deviceLeft = InputDevices.GetDeviceAtXRNode(inputSourceLeft);
        deviceRight = InputDevices.GetDeviceAtXRNode(inputSourceRight);
        //InputDevice deviceHead = InputDevices.GetDeviceAtXRNode(inputSourceHeadset);
        deviceLeft.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
        deviceLeft.TryGetFeatureValue(CommonUsages.primaryButton, out isPressed);
        //get input device controller Velocity in vector 3
        deviceLeft.TryGetFeatureValue(CommonUsages.deviceVelocity, out LeftControllerVelocity);
        deviceRight.TryGetFeatureValue(CommonUsages.deviceVelocity, out RightControllerVelocity);
        deviceLeft.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition);
        
        //deviceHead.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition);
        //XRBaseInteractable interactable = deviceLeft.GetComponent<XRBaseInteractable>();
        //interactable.activated.AddListener();
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();
        Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);
        //Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        //character.Move(direction * Time.fixedDeltaTime * speed);
        //Vector3 direction = headYaw * new Vector3(0, 0, -RightControllerVelocity.z);
        //character.Move(direction * Time.fixedDeltaTime * speed);
        
        //timer += Time.deltaTime;


        /*
        //glidingSpeed = Mathf.Max(glidingSpeed, 0);
        
        */
        //character.Move(direction * Time.fixedDeltaTime * speed * glidingSpeed);

        //float angle = Vector3.Angle(-rig.Camera.transform.forward, (devicePosition - rig.Camera.transform.position).normalized);
        //float distance = Vector3.Distance(object1.transform.position, object2.transform.position);
        //if (Mathf.Abs(angle) < 60f)

        //Vector3 cameraRelative = rig.Camera.transform.position - devicePosition;
        //float dotProduct = Vector3.Dot(devicePosition.forward, cameraRelative.normalized);
        //Vector3 direction = headYaw * new Vector3(0, 0, cameraRelative.z);
        //Debug.Log("Text: " + angle.ToString());

        OnFlap();

        //fly with direction 

       
        //gravity
        bool isGround = IfGround();
        
        if (isGround) 
        { 
            fallingSpeed = 0f;
            glidingSpeed = 0f;
            flyBackSpeed = 0f;
        }
        if(!isGround)
        {
            if (RightControllerVelocity.z < -0.3 && LeftControllerVelocity.z < -0.3 && RightControllerVelocity.y < -0.8 && LeftControllerVelocity.y < -0.8)
            {
                glidingSpeed = 10f;
                //m_Rigidbody.AddForce(transform.forward * 20f);
                direction = headYaw * new Vector3(0, 0, -(RightControllerVelocity.z + LeftControllerVelocity.z));
            }
            //fly back code, does not work properly
            /*if (RightControllerVelocity.z > 0.3 && LeftControllerVelocity.z > 0.3 && RightControllerVelocity.y < -0.8 && LeftControllerVelocity.y < -0.8)
            {
                flyBackSpeed = -5f;
                if (glidingSpeed <= 0)
                {
                    direction = headYaw * new Vector3(0, 0, -(RightControllerVelocity.z + LeftControllerVelocity.z));
                }
                //m_Rigidbody.AddForce(transform.forward * 20f);
                //direction = headYaw * new Vector3(0, 0, -(RightControllerVelocity.z + LeftControllerVelocity.z));
            }
            character.Move(direction * Time.fixedDeltaTime * (glidingSpeed + flyBackSpeed));
                        if (flyBackSpeed < 0)
            {
                flyBackSpeed = flyBackSpeed + 0.1f;
            }
            */
            character.Move(direction * Time.fixedDeltaTime * glidingSpeed);

            if (glidingSpeed > 0)
            {
                glidingSpeed = glidingSpeed - 0.1f;
                //glidingSpeed = Mathf.Lerp(glidingSpeed, 0, timer / 2f);
            }



        }
        //isPressed ||
        if (RightControllerVelocity.y < -0.8)
        {
            //device.SendHapticImpulse(channel, amplitude, duration);
            deviceRight.SendHapticImpulse(0, 0.8f, 0.1f);
            //Debug.Log("Right Text: " + RightControllerVelocity.y.ToString());
        }
        if (LeftControllerVelocity.y < -0.8)
        {
            deviceLeft.SendHapticImpulse(0, 0.8f, 0.1f);
            //Debug.Log("Left Text: " + LeftControllerVelocity.y.ToString());
        }
        if (RightControllerVelocity.y<-0.8 && LeftControllerVelocity.y<-0.8)
        {
            flapSpeed = - flapPower * (RightControllerVelocity.y + LeftControllerVelocity.y);
            fallingSpeed = 0;
            
        }
        if(flapSpeed > 0)
        {
            //flapSpeed -= gravity * Time.fixedDeltaTime;
            flapSpeed--;
        }
        else 
        {
            fallingSpeed += gravity * Time.fixedDeltaTime; 
        }
        character.Move(Vector3.up * (fallingSpeed + flapSpeed) * Time.fixedDeltaTime);
        
        //Debug.Log("Text: " + RightControllerVelocity.ToString());
    }

    void CapsuleFollowHeadset()
    {
        character.height = rig.CameraInOriginSpaceHeight;
        Vector3 capsuleCenter= transform.InverseTransformPoint(rig.Camera.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2, capsuleCenter.z);
    }

    //check if the player is grounded
    bool IfGround()
    {
        //cast a ray from character center to floor
        //groundLayer includes everthing in the layer of Ground(need config)
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }

    void OnFlap()
    {
        //body.AddForce(Vector3.up * inputAxis.x);
    }
}
