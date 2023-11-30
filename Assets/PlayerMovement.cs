using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1;
    public float gravity = -9.81f;
    // inputSource is the customized slot for devices
    public XRNode inputSource;
    public LayerMask groundLayer;


    private float fallingSpeed;
    private float flapSpeed;
    private XROrigin rig;
    private Vector2 inputAxis;
    private CharacterController character;
    private bool isPressed;
    //private Rigidbody body;
    // Start is called before the first frame update
    void Start()
    {
        //get character
        character = GetComponent<CharacterController>();
        //get rig
        rig = GetComponent<XROrigin>();
        //body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //get input device controller from self defined-XRNode
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
        device.TryGetFeatureValue(CommonUsages.primaryButton, out isPressed);
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();
        Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        character.Move(direction * Time.fixedDeltaTime * speed);

        OnFlap();

        
        //gravity
        bool isGround = IfGround();
        if(isGround) 
        { 
            fallingSpeed = 0; 
        }
        if(isPressed)
        {
            flapSpeed = 15f;
            fallingSpeed = 0;
        }
        //if(flapSpeed > 0)
        //{
            //flapSpeed -= gravity * Time.fixedDeltaTime;
        //}
        else 
        {
            fallingSpeed += gravity * Time.fixedDeltaTime; 
        }
        character.Move(Vector3.up * (fallingSpeed + flapSpeed) * Time.fixedDeltaTime);
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
