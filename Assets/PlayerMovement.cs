using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

/// <summary>
/// Class <c>PlayerMovement</c> moves the player with XR controller.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    /// <value>
    /// Property <c>speed</c> represents the speed at which the player moves by walking.
    /// </value>
    public float speed = 5;
    /// <value>
    /// Property <c>gravity</c> represents the customized gravity: how fast player drops.
    /// </value>
    public float gravity = -2.81f;
    /// <value>
    /// Property <c>flapPower</c> represents when player flap, how fast the player moves.
    /// </value>
    public float flapPower = 3.5f;
    /// <value>
    /// Property <c>controllerSensitivity</c> represents the sensitivity of the controller related to flap.
    /// </value>
    public float controllerSensitivity = 0.3f;
    /// <param>
    /// The input sources for the left hand, right hand, and headset XRNodes.
    /// </param>
    public XRNode inputSourceLeft;
    public XRNode inputSourceRight;
    public XRNode inputSourceHeadset;
    /// <param>
    /// Property <c>groundLayer</c> represents the layer of the ground.
    /// </param>
    public LayerMask groundLayer;

    /// <value>
    /// Property <c>fallingSpeed</c> represents the falling speed of the player. computed by gravity*time.
    /// </value>
    private float fallingSpeed;
    /// <value>
    /// Property <c>flapSpeed</c> represents the flap speed of the player, how fast player moves by fly forward. 
    /// </value>
    private float flapSpeed;
    /// <value>
    /// Property <c>glidingSpeed</c> represents the flying speed of the player,
    /// computed by flapSpeed. 
    /// </value>
    private float glidingSpeed;
    //private float flyBackSpeed;
    /// <XROrigin>
    /// Property <c>rig</c> represents XR origin of the player.
    /// </XROrigin>
    private XROrigin rig;
    /// <Vector2>
    ///  Property <c>rinputAxis/c> represents the input axis of the player.
    /// </Vector2>
    private Vector2 inputAxis;
    /// <CharacterController>
    ///  Property <c>character/c> represents the character controller of the player.
    /// </CharacterController>
    private CharacterController character;
    /// <bool>
    ///  Property <c>isPressed/c> to check if the primary button on the left controlleris pressed.
    /// </bool>
    private bool isPressed;
    /// <Vector3>
    ///  Property <c>LeftControllerVelocity</c> and <c>RightControllerVelocity</c> 
    ///  represents the velocity of the left and right controllers.
    /// </Vector3>
    private Vector3 LeftControllerVelocity;
    private Vector3 RightControllerVelocity;
    /// <Vector3>
    ///  Property <c>devicePosition</c> represents the position of the device. used to 
    ///  detect the position for headset and controller, currently not used.
    /// </Vector3>
    private Vector3 devicePosition;
    /// <InputDevice>
    ///  Property <c>deviceLeft</c> and <c>deviceLeft</c> represent the left and right input devices.
    /// </InputDevice>
    private InputDevice deviceLeft;
    private InputDevice deviceRight;
    //private float timer;
    /// <Vector3>
    ///  Property <c>direction</c> represent the direction of the player when flying.
    /// </Vector3>
    private Vector3 direction;
    /// <Vector3>
    ///  Property <c>onGroundDirection</c> represent the direction of the player when walking.
    /// </Vector3>
    private Vector3 onGroundDirection;
    //private Rigidbody m_Rigidbody;
    // Start is called before the first frame update

    void Start()
    {
        ///get character
        character = GetComponent<CharacterController>();
        ///get rig
        rig = GetComponent<XROrigin>();
        //body = GetComponent<Rigidbody>();
        //glidingSpeed = 0f;
        //m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ///get input device controller from self defined-XRNode
        deviceLeft = InputDevices.GetDeviceAtXRNode(inputSourceLeft);
        deviceRight = InputDevices.GetDeviceAtXRNode(inputSourceRight);
        ///InputDevice deviceHead = InputDevices.GetDeviceAtXRNode(inputSourceHeadset);
        deviceLeft.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
        deviceLeft.TryGetFeatureValue(CommonUsages.primaryButton, out isPressed);
        ///get input device controller Velocity in vector 3
        deviceLeft.TryGetFeatureValue(CommonUsages.deviceVelocity, out LeftControllerVelocity);
        deviceRight.TryGetFeatureValue(CommonUsages.deviceVelocity, out RightControllerVelocity);
        deviceLeft.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition);
        
        //deviceHead.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition);
        //XRBaseInteractable interactable = deviceLeft.GetComponent<XRBaseInteractable>();
        //interactable.activated.AddListener();
    }

    private void FixedUpdate()
    {
        ///call CapsuleFollowHeadset, so the the Capsule within the player would follows player in FixedUpdate.
        CapsuleFollowHeadset();
        ///get the y rotation of the player’s VR headset in specific number of degrees around y axis
        Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);

        //Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        //character.Move(direction * Time.fixedDeltaTime * speed);
        //Vector3 direction = headYaw * new Vector3(0, 0, -RightControllerVelocity.z);
        //character.Move(direction * Time.fixedDeltaTime * speed);

        //timer += Time.deltaTime;

        if (isPressed)
        {
            SceneManager.LoadScene("NatureTest2_UV");
        }
        /*
         * fly backward, currently not in use.
        //glidingSpeed = Mathf.Max(glidingSpeed, 0);
        
        
        //character.Move(direction * Time.fixedDeltaTime * speed * glidingSpeed);

        //float angle = Vector3.Angle(-rig.Camera.transform.forward, (devicePosition - rig.Camera.transform.position).normalized);
        //float distance = Vector3.Distance(object1.transform.position, object2.transform.position);
        //if (Mathf.Abs(angle) < 60f)

        //Vector3 cameraRelative = rig.Camera.transform.position - devicePosition;
        //float dotProduct = Vector3.Dot(devicePosition.forward, cameraRelative.normalized);
        //Vector3 direction = headYaw * new Vector3(0, 0, cameraRelative.z);
        //Debug.Log("Text: " + angle.ToString());
        */


        ///walking direction with the left controller joystick
        onGroundDirection = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        character.Move(onGroundDirection * Time.fixedDeltaTime * speed);

        /// to check if player is standing on ground
        bool isGround = IfGround();
        /// <summary>
        /// if the player is on ground, player stop falling and flying forward.
        /// </summary>
        if (isGround) 
        { 
            fallingSpeed = 0f;
            glidingSpeed = 0f;
            //flyBackSpeed = 0f;
        }
        /// <summary>
        /// if the player is not on ground, player can turn with controller velocity and flying forward.
        /// </summary>
        if (!isGround)
        {

            FlyForward(headYaw);
            Turning();
            /*character.Move(direction * Time.fixedDeltaTime * (glidingSpeed + flyBackSpeed));
            if (flyBackSpeed < 0)
            {
                flyBackSpeed = flyBackSpeed + 0.1f;
            }

            if (direction != Vector3.zero)
            {
                transform.forward = direction;
            }*/

            //transform.rotation = Quaternion.LookRotation(new )
            character.Move(direction * Time.fixedDeltaTime * glidingSpeed);

        }
        /// <summary>
        /// call OnFlap function in FixedUpdate
        /// </summary>
        OnFlap();
        /// <summary>
        /// call ControllerVibration function in FixedUpdate
        /// </summary>
        ControllerVibration();

        /// <summary>
        /// flap speed decrease overtime.
        /// </summary>
        if (flapSpeed > 0)
        {
            //flapSpeed -= gravity * Time.fixedDeltaTime;
            flapSpeed--;
        }
        else 
        {
            fallingSpeed += gravity * Time.fixedDeltaTime; 
        }
        /// <summary>
        /// player fly up
        /// </summary>
        character.Move(Vector3.up * (fallingSpeed + flapSpeed) * Time.fixedDeltaTime);
        
        //Debug.Log("Text: " + RightControllerVelocity.ToString());
    }
    /// <summary>
    /// The CapsuleFollowHeadset method adjusts the character's height and center to follow the headset.
    /// </summary>
    void CapsuleFollowHeadset()
    {
        /// Set the height of the character to the height of the camera in the XR rig.
        character.height = rig.CameraInOriginSpaceHeight;
        /// Convert the position of the camera from world space to local space.
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.Camera.transform.position);
        /// Set the center of the character to the x and z coordinates of the capsule center,
        /// and the y coordinate to half of the character's height.
        character.center = new Vector3(capsuleCenter.x, character.height / 2, capsuleCenter.z);
    }

    /// <summary>
    /// The IfGround method checks if the character is on the ground.
    /// </summary>
    /// <returns>
    /// Returns a boolean indicating whether the character is on the ground.
    /// </returns>
    bool IfGround()
    {
        /// The start point of the ray is the character's center in world space.
        Vector3 rayStart = transform.TransformPoint(character.center);
        /// The length of the ray is slightly more than the y-coordinate of the character's center.
        //float rayLength = character.center.y;
        float rayLength = character.center.y + 0.01f;
        /// <summary>
        ///  Cast a sphere from the ray start downwards. The sphere's radius is the same as the character's.
        ///  If the sphere hits something in the ground layer, hasHit will be true and hitInfo will contain more information about the hit.
        ///  groundLayer includes everthing in the layer of Ground(need config outside of the script)
        /// </summary>
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
    /// <summary>
    /// The OnFlap method is called in FixedUpdate every now and then.
    /// </summary>
    void OnFlap()
    {
        //body.AddForce(Vector3.up * inputAxis.x);
        /// If the y velocity of both the right and left controllers is less than -0.8 (indicating a downward motion),
        /// then the player is considered to be flapping their wings.
        if (RightControllerVelocity.y < -0.8 && LeftControllerVelocity.y < -0.8)
        {
            flapSpeed = -flapPower * (RightControllerVelocity.y + LeftControllerVelocity.y);
            fallingSpeed = 0;
        }
    }
    /// <summary>
    /// The ControllerVibration method is used to provide haptic feedback to the user through the controllers.
    /// </summary>
    void ControllerVibration()
    {
        /// If the y velocity of the right controller is less than -0.8 (indicating a downward motion),
        /// then a haptic impulse is sent to the right controller.
        if (RightControllerVelocity.y < -0.8)
        {
            /// The SendHapticImpulse method sends a haptic impulse to the controller.
            ///device.SendHapticImpulse(channel, amplitude, duration);
            deviceRight.SendHapticImpulse(0, 0.8f, 0.1f);
            //Debug.Log("Right Text: " + RightControllerVelocity.y.ToString());
        }
        /// If the y velocity of the left controller is less than -0.8 (indicating a downward motion),
        /// then a haptic impulse is sent to the left controller.
        if (LeftControllerVelocity.y < -0.8)
        {
            deviceLeft.SendHapticImpulse(0, 0.8f, 0.1f);
            //Debug.Log("Left Text: " + LeftControllerVelocity.y.ToString());
        }
    }
    /// <summary>
    /// The FlyForward method is used to control the player's forward movement in the VR environment.
    /// </summary>
    /// <param name="headYaw">A Quaternion representing the rotation of the player's head.</param>
    void FlyForward(Quaternion headYaw)
    {
        /// If the z velocity of both the right and left controllers is less than -controllerSensitivity (indicating a forward motion),
        /// and the y velocity of both controllers is less than -0.8 (indicating a downward motion),
        /// then the player is considered to be moving forward.
        if (RightControllerVelocity.z < -controllerSensitivity && LeftControllerVelocity.z < -controllerSensitivity && RightControllerVelocity.y < -0.8 && LeftControllerVelocity.y < -0.8)
        {
            glidingSpeed = 10f;
            //m_Rigidbody.AddForce(transform.forward * 20f);
            /// <summary>
            /// The direction of the player's movement is set to the forward direction of the player's head,
            /// multiplied by the sum of the z velocities of the right and left controllers.
            /// The direction is then normalized to ensure it has a magnitude of 1.
            /// </summary>
            direction = headYaw * new Vector3(0, 0, -(RightControllerVelocity.z + LeftControllerVelocity.z));
            direction.Normalize();
        }
        ///fly back code, does not in use
        /*if (RightControllerVelocity.z > controllerSensitivity && LeftControllerVelocity.z > controllerSensitivity && RightControllerVelocity.y < -0.8 && LeftControllerVelocity.y < -0.8)
        {
            glidingSpeed = 0f;
            flyBackSpeed = -5f;
            if (glidingSpeed <= 0)
            {
                direction = headYaw * new Vector3(0, 0, -(RightControllerVelocity.z + LeftControllerVelocity.z));
            }

            //m_Rigidbody.AddForce(transform.forward * 20f);
            //direction = headYaw * new Vector3(0, 0, -(RightControllerVelocity.z + LeftControllerVelocity.z));
        }*/

        /// If the gliding speed is greater than 0, it is gradually decreased by 0.1.
        if (glidingSpeed > 0)
        {
            glidingSpeed = glidingSpeed - 0.1f;
            //glidingSpeed = Mathf.Lerp(glidingSpeed, 0, timer / 2f);
        }

    }
    /// <summary>
    /// The Turning method is used to control the player's turning.
    /// </summary>
    void Turning()
    {
        /// If the y velocity of the right controller is between -controllerSensitivity and controllerSensitivity (indicating no vertical motion),
        /// and the y velocity of the left controller is less than -0.5 (indicating a downward motion),
        /// then the player is considered to be turning right.
        if (RightControllerVelocity.y > -controllerSensitivity && RightControllerVelocity.y < controllerSensitivity && LeftControllerVelocity.y < -0.5)
        {
            /// The player's transform is rotated 150 degrees per second to the right.
            transform.Rotate(0f, 150f * Time.fixedDeltaTime, 0f, Space.Self);
        }
        if (LeftControllerVelocity.y > -controllerSensitivity && LeftControllerVelocity.y < controllerSensitivity && RightControllerVelocity.y < -0.5)
        {
            transform.Rotate(0f, -150f * Time.fixedDeltaTime, 0f, Space.Self);
        }
    }
}
