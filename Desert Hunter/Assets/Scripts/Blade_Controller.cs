using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade_Controller : MonoBehaviour
{
    private float _baldeYaw;
    private float _bladePitch;
    private float _baldeYawLerped;
    private float _bladePitchLerped;
    private CharacterController _controller;
    private MeshRenderer _mesh;
    private Aim_And_Shoot _shootScript;
    private Player_Controller _playerController;

    public float _chargeTime = 0f;
    public float _landTime = 0f;
    public float _bladeTimer = 0f;
    public float BladeSpeed = 20f;
    public float BladeRotationSmoothing = 5f;
    public GameObject FollowRoot;
    public GameObject Player;

    private void Start() {
        _controller = GetComponent<CharacterController>();
        _mesh = GetComponent<MeshRenderer>();
        _shootScript = Player.GetComponent<Aim_And_Shoot>();
        _playerController = Player.GetComponent<Player_Controller>();
    }
    private void Update() {
        // Keep follow root for blade camera at the location of the balde
        FollowRoot.transform.position = transform.position;
        //rotate the balde different ways depending if the player is in balde form
        if(_shootScript.IsBlading){
            RotateBlade();
            // while the player is invisible, adjust the character's rotation and the
            // follow camera root of the player to match what the blade is facing
            _playerController.CinemachineCameraTarget.transform.rotation = transform.rotation;
            _playerController._cinemachineTargetPitch = _bladePitch;
            _playerController._cinemachineTargetYaw = _baldeYaw;
            Vector3 _lookDirection = new Vector3(_playerController.CinemachineCameraTarget.transform.forward.x,0f,_playerController.CinemachineCameraTarget.transform.forward.z);
            Player.transform.forward = _lookDirection;
        }else{
            // makes the balde and blade root reset to the players camera rotation
            transform.rotation = _playerController.CinemachineCameraTarget.transform.rotation;
            _bladePitch = _playerController._cinemachineTargetPitch;
            _baldeYaw = _playerController._cinemachineTargetYaw;
            _bladePitchLerped = _bladePitch;
            _baldeYawLerped = _baldeYaw;
            FollowRoot.transform.rotation = transform.rotation;
        }

        // enter blade form only if given the signal from the aim and shoot script
        if(_shootScript.IsBlading) BladeForm();
        else{
            // make sure to reset our balde components to be still and exact
            this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            FollowRoot.transform.rotation = transform.rotation;
        }
    }
    public void BladeForm(){
        //=======================================================================
        // 1st phase: Charge up time. No movement unitl charge timer has depleted
        //=======================================================================
        if(_chargeTime > 0f){
           _chargeTime -= Time.deltaTime;
           _landTime = 1f;
           _mesh.enabled = true;
        //=======================================================================
        // 2nd phase: Blade time. This is where the player can move as a blade
        //=======================================================================
        }else if(_bladeTimer > 0){
         _shootScript.PlayerGeo.SetActive(false);
            _bladeTimer -= Time.deltaTime;
            if(Input.GetKeyDown(KeyCode.Mouse0)) _bladeTimer = 0;
            GetComponent<MeshCollider>().enabled = true;
            GetComponent<CharacterController>().Move(transform.forward.normalized * BladeSpeed * Time.deltaTime);
            Player.transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        //=======================================================================
        // 3rd phase: once the player has tan out of blade time, we reset the
        // player visibilty and dissapear the balde.
         //=======================================================================
        }else{
            GetComponent<MeshCollider>().enabled = false;
            GetComponent<CharacterController>().Move(Vector3.zero);
            _mesh.enabled = false;
            _shootScript.PlayerGeo.SetActive(true);
            _shootScript.AimCamera.gameObject.SetActive(false);
            _shootScript.BladeCamera.gameObject.SetActive(false);

            // locks tplayer movement for a fixed amount of time before they can
            // move again out of blade state
            if(_landTime > 0f) _landTime -= Time.deltaTime;
            else{
                _playerController.RotateOnMoveDirection = true;
                _shootScript.IsBlading = false;
            }
        }
    }
    private void RotateBlade(){
        // grabs the vertical and horizontal input of the player; however,
        // the amount the blade rotates by is clamped by -1 - 1
        float input_x = Input.GetAxisRaw("Mouse X");
        float input_y = -Input.GetAxisRaw("Mouse Y");
        input_x = Mathf.Clamp(input_x, -1f, 1f);
        input_y = Mathf.Clamp(input_y, -1f, 1f);
        // adjust how much we need to rotate the blade
        _bladePitch += input_y * 200f * Time.deltaTime;
        _baldeYaw += input_x * 200f * Time.deltaTime;
        // hold another two floats that lerp to how much we need to rotate by
        _bladePitchLerped = Mathf.Lerp(_bladePitchLerped, _bladePitch, Time.deltaTime * BladeRotationSmoothing);
        _baldeYawLerped = Mathf.Lerp(_baldeYawLerped, _baldeYaw, Time.deltaTime * BladeRotationSmoothing);
        // clamp vertivle movvemnt so the player can't do any flips
       _bladePitch = ClampAngle(_bladePitch, -50.0f, 50.0f);
        // rotate the balde and camera accordingly
        transform.rotation = Quaternion.Euler(_bladePitchLerped, _baldeYawLerped, 0.0f);
        // smooth out rotation of the camera
        FollowRoot.transform.rotation = Quaternion.Lerp(FollowRoot.transform.rotation, transform.rotation, Time.deltaTime * 10.0f);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == 0){
            _bladeTimer = 0f;
        }
    }
}
