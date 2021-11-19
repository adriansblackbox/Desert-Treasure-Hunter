using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Aim_And_Shoot : MonoBehaviour
{
    private float _defaultSensitivity;
    public float _bladeTimer = 0f;
    private float _targetSpeed = 0f;
    private float _chargeTime = 0f;
    private float _landTime = 0f;
    private float _rotationFactor = 0f;
    private Animator _animator;
    private MeshRenderer _bladeMesh;

    public float SpeedChangeRate = 10f;
    public float AimSensitivity = 50f;
    public float BladeSensitivity = 100f;
    public float BladeSpeed = 20f;
    public float BladeTime = 3f;
    public float BladeRotateSpeed = 50f;
    public bool IsBlading = false;
    public bool IsAiming = false;
    public CinemachineVirtualCamera AimCamera;
    public CinemachineVirtualCamera BladeCamera;
    public GameObject Crosshair;
    public LayerMask AimColliderLayerMask;
    public GameObject Blade;
    public GameObject PlayerGeo;
    
    private void Start() {
        _defaultSensitivity = GetComponent<Player_Controller>().MouseSensitivity;
        _animator = GetComponent<Animator>();
        _bladeMesh = Blade.GetComponent<MeshRenderer>();
    }

    private void Update(){
        if(!IsBlading && GetComponent<CharacterController>().isGrounded)
            ADS();
        if(!IsBlading){
            Blade.GetComponent<MeshCollider>().enabled = false;
            this.gameObject.layer = LayerMask.NameToLayer("Player");
        }else{
            Blade.GetComponent<MeshCollider>().enabled = true;
            this.gameObject.layer = 7;
        }
        ShootBlade();
    }
    
    private void ADS(){
        if(Input.GetKeyDown(KeyCode.Mouse1)){
            // Sensitivity, crosshair, and camera are adjusted when aiming
            AimCamera.gameObject.SetActive(true);
            Crosshair.SetActive(true);
            IsAiming = true;
            GetComponent<Player_Controller>().RotateOnMoveDirection = false;
            GetComponent<Player_Controller>().MouseSensitivity = AimSensitivity;
        }
        if(Input.GetKey(KeyCode.Mouse1) && IsAiming){
            // Player rotates to look at aim direction
            Vector3 _aimDirection = new Vector3(FindObjectOfType<Player_Controller>().CinemachineCameraTarget.transform.forward.x,0f,FindObjectOfType<Player_Controller>().CinemachineCameraTarget.transform.forward.z);
            transform.forward = Vector3.Lerp(transform.forward, _aimDirection, Time.deltaTime * 15f);

            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * SpeedChangeRate));
            //RotateBlade();
            ShootBlade();
        }else{
            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * SpeedChangeRate));
        }
        if(Input.GetKeyUp(KeyCode.Mouse1)){
            // Revert to default player camera when not aiming
            AimCamera.gameObject.SetActive(false);
            ResetToPlayerControls();
        }
    }

    private void ShootBlade(){
        if(IsAiming && Input.GetKeyDown(KeyCode.Mouse0)){
            IsAiming = false;
            IsBlading = true;
            BladeCamera.gameObject.SetActive(true);
            AimCamera.gameObject.SetActive(false);
            Crosshair.SetActive(false);
            _bladeTimer = BladeTime;
            _chargeTime = 0.5f;
        }

        //Temporary charge time. Replace with 'when animation is finished playing'
        if(_chargeTime > 0f){
            _chargeTime -= Time.deltaTime;
            _landTime = 0.25f;
        }else if(IsBlading){
            // Temporary representation of the blade form
            RotateBladeInAir();
            _bladeMesh.enabled = true;
            PlayerGeo.SetActive(false);
            BladeForm();
        }
        
    }
    private void RotateBlade(){
        _rotationFactor = Input.GetAxisRaw("Horizontal");
        Vector3 rotation = new Vector3(0, 0, _rotationFactor * -BladeRotateSpeed * Time.deltaTime);
        Blade.transform.Rotate(rotation);
        Crosshair.transform.Rotate(rotation);
    }
    private void RotateBladeInAir(){
        _rotationFactor = Mathf.Lerp(_rotationFactor, Input.GetAxis("Horizontal"), Time.deltaTime * 20f);
        Blade.transform.rotation = Quaternion.Euler (new Vector3(
            FindObjectOfType<Player_Controller>().CinemachineCameraTarget.transform.rotation.eulerAngles.x, 
            FindObjectOfType<Player_Controller>().CinemachineCameraTarget.transform.rotation.eulerAngles.y, 
            Blade.transform.rotation.eulerAngles.z));
        //Vector3 rotation = new Vector3(0, 0, _rotationFactor * -BladeRotateSpeed * 2 * Time.deltaTime);
        //Blade.transform.Rotate(rotation);
    }
    private void BladeForm(){
        if(_bladeTimer > 0){
            _targetSpeed = Mathf.Lerp(_targetSpeed, BladeSpeed, Time.deltaTime);
            this.gameObject.GetComponent<CharacterController>().Move(FindObjectOfType<Player_Controller>().CinemachineCameraTarget.transform.forward.normalized * BladeSpeed * Time.deltaTime);
            _bladeTimer -= Time.deltaTime;
            if(Input.GetKeyDown(KeyCode.Mouse0)) _bladeTimer = 0;
        }else if(IsBlading){
            _targetSpeed = 0;
            // Temporary representation of the blade form
            _bladeMesh.enabled = false;
            PlayerGeo.SetActive(true);
            AimCamera.gameObject.SetActive(false);
            if(_landTime > 0f)
                _landTime -= Time.deltaTime;
            else{
                AimCamera.gameObject.SetActive(false);
                BladeCamera.gameObject.SetActive(false);
                ResetToPlayerControls();
            }
        }
    }
    public void ResetToPlayerControls(){
        Blade.transform.rotation = Quaternion.Euler (new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f));
        Crosshair.transform.rotation = Quaternion.Euler(Vector3.zero);
        _rotationFactor = 0f;
        IsBlading = false;
        Crosshair.SetActive(false);
        IsAiming = false;
        GetComponent<Player_Controller>().RotateOnMoveDirection = true;
        GetComponent<Player_Controller>().MouseSensitivity = _defaultSensitivity;
    }
}
