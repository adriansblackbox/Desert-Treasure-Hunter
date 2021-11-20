using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade_Controller : MonoBehaviour
{
    private float _speed;
    private CharacterController _controller;
    private MeshRenderer _mesh;
    private Aim_And_Shoot ShootScript;
    public GameObject FollowRoot;
    public GameObject Player;
    public float _chargeTime = 0f;
    public float MaxTrunRadius = 10f;
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
        RotateBlade();
        FollowRoot.transform.position = transform.position;

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
        }else{
            this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            FollowRoot.transform.rotation = transform.rotation;
        }
    }
    private void RotateBlade(){
        transform.rotation = Quaternion.RotateTowards(transform.rotation, FindObjectOfType<Player_Controller>().CinemachineCameraTarget.transform.rotation, Time.deltaTime * 120f);
        FollowRoot.transform.rotation = Quaternion.RotateTowards( FollowRoot.transform.rotation, FindObjectOfType<Player_Controller>().CinemachineCameraTarget.transform.rotation, Time.deltaTime * 100f);
    }
    private void BladeMovement(){
        //transform.rotation = FollowRoot.transform.rotation;
        this.gameObject.GetComponent<CharacterController>().Move(transform.forward.normalized * _speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == 0){
            Debug.Log("Collided");
            _bladeTimer = 0f;
        }
    }
}
