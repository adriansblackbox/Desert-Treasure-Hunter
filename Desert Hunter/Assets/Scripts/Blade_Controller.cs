using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade_Controller : MonoBehaviour
{
    private float _speed;
    private float _baldeYaw;
    private float _bladePitch;
    private CharacterController _controller;
    private MeshRenderer _mesh;
    private Aim_And_Shoot ShootScript;
    public GameObject FollowRoot;
    public GameObject Player;
    public float _chargeTime = 0f;
    public float MaxTrunRadius = 1f;
    public float _landTime = 0f;
    public float _bladeTimer = 0f;
    public bool SafeToRotate = false;

    private void Start() {
        ShootScript = FindObjectOfType<Aim_And_Shoot>();
        _controller = GetComponent<CharacterController>();
        _speed = ShootScript.BladeSpeed;
        _mesh = GetComponent<MeshRenderer>();
    }
    private void Awake() {
    }
    private void Update() {
        //rotate the balde different ways depending if the player is in balde form
        if(ShootScript.IsBlading)
            RotateBlade();
        else{
            transform.rotation = FindObjectOfType<Player_Controller>().CinemachineCameraTarget.transform.rotation;
            _bladePitch = FindObjectOfType<Player_Controller>()._cinemachineTargetPitch;
            _baldeYaw = FindObjectOfType<Player_Controller>()._cinemachineTargetYaw;
            FollowRoot.transform.rotation = transform.rotation;
        }
        // Keep follow root for blade camera at the location of the balde
        FollowRoot.transform.position = transform.position;

        // Begin of Blade form
        if(_chargeTime > 0f){
           _chargeTime -= Time.deltaTime;
           _landTime = 1f;
           _mesh.enabled = true;
        }else if(_bladeTimer > 0){
            ShootScript.PlayerGeo.SetActive(false);
            _bladeTimer -= Time.deltaTime;
            if(Input.GetKeyDown(KeyCode.Mouse0)) _bladeTimer = 0;
            GetComponent<MeshCollider>().enabled = true;
            BladeMovement();
            Player.transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        }else if(ShootScript.IsBlading){
            this.gameObject.GetComponent<CharacterController>().Move(Vector3.zero);
            _mesh.enabled = false;
            GetComponent<MeshCollider>().enabled = false;
            ShootScript.PlayerGeo.SetActive(true);
            ShootScript.AimCamera.gameObject.SetActive(false);
            ShootScript.BladeCamera.gameObject.SetActive(false);
            if(_landTime > 0f){
                _landTime -= Time.deltaTime;
            }
            else{
                ShootScript.ResetToPlayerControls();
                ShootScript.IsBlading = false;
            }
        // End of Blade form
        }else{
            this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            FollowRoot.transform.rotation = transform.rotation;
        }
    }
    private void RotateBlade(){
        float input_x = Input.GetAxisRaw("Mouse X");
        float input_y = -Input.GetAxisRaw("Mouse Y");
        input_x = Mathf.Clamp(input_x, -1f, 1f);
        input_y = Mathf.Clamp(input_y, -1f, 1f);

        _bladePitch += input_y * 200f * Time.deltaTime;
        _baldeYaw += input_x * 200f * Time.deltaTime;

       _bladePitch = ClampAngle(_bladePitch, -50.0f, 50.0f);

        transform.rotation = Quaternion.Euler(_bladePitch, _baldeYaw, 0.0f);
        FollowRoot.transform.rotation = Quaternion.RotateTowards( FollowRoot.transform.rotation, transform.rotation, Time.deltaTime * 50f);
    }
    private void BladeMovement(){
        //transform.rotation = FollowRoot.transform.rotation;
        this.gameObject.GetComponent<CharacterController>().Move(transform.forward.normalized * _speed * Time.deltaTime);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == 0){
            Debug.Log("Collided");
            _bladeTimer = 0f;
        }
    }
}
