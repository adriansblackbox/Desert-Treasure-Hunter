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
    public float DistToGround = .25f;

    public bool RotateOnMoveDirection = true;

    public GameObject CinemachineCameraTarget;
    public Transform GroundChecker;
    public CinemachineVirtualCamera FollowCamera;
    public CinemachineVirtualCamera AimCamera;

    private float _speed;
    private float _gravity = 0.0f;
    private float _targetSpeed;
    private float _targetClamp;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _cinemachineTargetYaw;
	private float _cinemachineTargetPitch;
    private float _cameraNoise;
    private float _negYclamp;
    private float _posYclamp;

    private CharacterController _controller;
    private Animator _animator;
	private GameObject _mainCamera;

    private void Awake() {
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(!GetComponent<Aim_And_Shoot>().IsBlading)
            Gravity();
        
        if(!GetComponent<Aim_And_Shoot>().IsBlading && !GetComponent<Aim_And_Shoot>().IsAiming)
            Move();

        FollowCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = _cameraNoise;
    }
    void LateUpdate(){
        //if(!GetComponent<Aim_And_Shoot>().IsBlading)
            RotateCamera();
    }
    private void Gravity(){
        _gravity -= 9.18f * Time.deltaTime;
        if(_controller.isGrounded)
            _gravity = 0;
    }
    private void Move(){
        float input_x = Input.GetAxis("Horizontal");
        float input_y = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(input_x, 0.0f, input_y).normalized;

        _targetSpeed = JogSpeed;

        if(inputDirection == Vector3.zero){
            _targetSpeed = 0.0f;
            _cameraNoise = 0.3f;
        }else{
            _cameraNoise = 1.6f;
        }

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < _targetSpeed - speedOffset || currentHorizontalSpeed > _targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = _targetSpeed;
        }
        _animationBlend = Mathf.Lerp(_animationBlend, _targetSpeed, Time.deltaTime * SpeedChangeRate);

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (inputDirection != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // rotate to face input direction relative to camera position
            if(RotateOnMoveDirection)
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        targetDirection = new Vector3 (targetDirection.x * _speed, _gravity, targetDirection.z * _speed);
        _controller.Move(targetDirection * Time.deltaTime);

        _animator.SetFloat("Speed", _animationBlend);
        float inputMagnitude = inputDirection.magnitude;
        if(inputMagnitude > 0)
        _animator.SetFloat("MotionSpeed", inputDirection.magnitude);
        else
        _animator.SetFloat("MotionSpeed", 1f);
    }

    private void RotateCamera(){
        float _targetSensitivity;
        if(!FindObjectOfType<Aim_And_Shoot>().IsBlading) _targetSensitivity = MouseSensitivity;
        else _targetSensitivity = FindObjectOfType<Aim_And_Shoot>().BladeSensitivity;

        _cinemachineTargetYaw += Input.GetAxisRaw("Mouse X") * Time.deltaTime * _targetSensitivity;
		_cinemachineTargetPitch += -1 * Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _targetSensitivity;
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        if(!FindObjectOfType<Aim_And_Shoot>().IsAiming && !FindObjectOfType<Aim_And_Shoot>().IsBlading){
		    _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, -20.0f, 50.0f);
            _negYclamp = -15.0f;
            _posYclamp = 50.0f;
        }else if(!FindObjectOfType<Aim_And_Shoot>().IsBlading){
            //_cinemachineTargetPitch = Mathf.Lerp(_cinemachineTargetPitch, 0.0f, Time.deltaTime * 17f);
            //_posYclamp = Mathf.Lerp(_posYclamp, 0.0f, Time.deltaTime * 17f);
            //_negYclamp = Mathf.Lerp(_negYclamp, 0.0f, Time.deltaTime * 17f);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _negYclamp, _posYclamp);
        }
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
