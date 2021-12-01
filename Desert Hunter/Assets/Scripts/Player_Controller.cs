using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class Player_Controller : MonoBehaviour
{
    public float JogSpeed = 2.0f;
    public float RotationSmoothTime = 1f;
    public float SpeedChangeRate = 10.0f;
    public float MouseSensitivity = 300f;

    public bool RotateOnMoveDirection = true;

    public GameObject CinemachineCameraTarget;
    public CinemachineVirtualCamera FollowCamera;
    public CinemachineVirtualCamera AimCamera;
    public GameObject lastCheckpoint;

    private float _speed;
    public float _gravity = 0.0f;
    private float _targetSpeed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    public float _cinemachineTargetYaw;
	public float _cinemachineTargetPitch;
    private float _cameraNoise;

    private CharacterController _controller;
    private Animator _animator;
	private GameObject _mainCamera;

    // Initialize camera
    private void Awake() {_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");}
    void Start()
    {
        // Keep the cursor locked and invisible in the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
        // Get the controller and animator of the player character
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        // So long as the player isn't blading, gravity will effect the player
        if(!GetComponent<Aim_And_Shoot>().IsBlading)
            Gravity();
        // If the player is not aiming or blading, they can move around freely
        if(!GetComponent<Aim_And_Shoot>().IsBlading && !GetComponent<Aim_And_Shoot>().IsAiming)
            Move();
        // Getting noise of the follow camera to adust dependant on movement state (run or idle)
        FollowCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = _cameraNoise;

        // Kills the player
        //if (transform.position.y < 3) {
        //    Reset();
        //    StartCoroutine(_mainCamera.GetComponentInChildren<Fader>().Fade(this));
        //    this.enabled = false;
        //}
    }
    // Rotate camera logic on late update since the camera follow is on late update as well
    void LateUpdate(){RotateCamera();}
    private void Gravity(){
        // Defalt gravity to be scaled by 9.18
        _gravity -= 9.18f * Time.deltaTime;
        // If the character is grounded, still apply constant downward force for isGrounded detection
        if(_controller.isGrounded) _gravity = -9.18f * Time.deltaTime;
    }
    private void Move(){
        // Gathering the horizontal and vertical input of mouse, and normalizing it in order to calculate
        // a vector 3 direction of movement
        float input_x = Input.GetAxis("Horizontal");
        float input_y = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(input_x, 0.0f, input_y).normalized;
        // initialize target speed as the editor JogSpeed
        _targetSpeed = JogSpeed;
        // adjust camera noise and target speed depending on player input
        if(inputDirection == Vector3.zero){
            _targetSpeed = 0.0f;
            _cameraNoise = 0.3f;
        }
        else _cameraNoise = 1.6f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        if (currentHorizontalSpeed < _targetSpeed - speedOffset || currentHorizontalSpeed > _targetSpeed + speedOffset){
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed, Time.deltaTime * SpeedChangeRate);
            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else _speed = _targetSpeed;

        _animationBlend = Mathf.Lerp(_animationBlend, _targetSpeed, Time.deltaTime * SpeedChangeRate);
        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (inputDirection != Vector3.zero){
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
            // rotate to face input direction relative to camera position
            if(RotateOnMoveDirection) transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        // move the player
        targetDirection = new Vector3 (targetDirection.x * _speed, _gravity, targetDirection.z * _speed);
        _controller.Move(targetDirection * Time.deltaTime);
        // adjust animation with lerped animationBlend
        _animator.SetFloat("Speed", _animationBlend);
        float inputMagnitude = inputDirection.magnitude;
        if(inputMagnitude > 0) _animator.SetFloat("MotionSpeed", inputDirection.magnitude);
        else _animator.SetFloat("MotionSpeed", 1f);
    }

    private void RotateCamera(){
        // So long as the player is not in blade form, we rotate simply by rotating our look target depending on uder input
        if(!GetComponent<Aim_And_Shoot>().IsBlading){
            _cinemachineTargetYaw += Input.GetAxisRaw("Mouse X") * Time.deltaTime *  MouseSensitivity;
            _cinemachineTargetPitch += -1 * Input.GetAxisRaw("Mouse Y") * Time.deltaTime *  MouseSensitivity;
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -50.0f, 50.0f);
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
        }
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void Reset() {
        transform.position = lastCheckpoint.transform.position;
        lastCheckpoint.GetComponent<CheckpointScript>().Reset();
    }
}
