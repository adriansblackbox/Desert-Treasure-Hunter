using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Aim_And_Shoot : MonoBehaviour
{
    private float _defaultSensitivity;
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
    public GameObject BladeStart;
    public GameObject PlayerGeo;
    
    private void Start() {
        _defaultSensitivity = GetComponent<Player_Controller>().MouseSensitivity;
        _animator = GetComponent<Animator>();
    }

    private void Update(){
        if(!IsBlading && GetComponent<CharacterController>().isGrounded)
            ADS();
        if(!IsBlading){
            this.gameObject.layer = LayerMask.NameToLayer("Player");
            Blade.transform.position = BladeStart.transform.position;
            Blade.transform.rotation = BladeStart.transform.rotation;
        }else{
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
        }else{
            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * SpeedChangeRate));
        }
        if(Input.GetKeyUp(KeyCode.Mouse1)){
            // Revert to default player camera when not aiming
            GetComponent<Player_Controller>().RotateOnMoveDirection = true;
            AimCamera.gameObject.SetActive(false);
            IsAiming = false;
            Crosshair.SetActive(false);
        }
    }

    private void ShootBlade(){
        if(IsAiming && Input.GetKeyDown(KeyCode.Mouse0) && !IsBlading){
            IsAiming = false;
            IsBlading = true;
            BladeCamera.gameObject.SetActive(true);
            AimCamera.gameObject.SetActive(false);
            Crosshair.SetActive(false);
            FindObjectOfType<Blade_Controller>()._bladeTimer = BladeTime;
            FindObjectOfType<Blade_Controller>()._chargeTime = 0.5f;
        }
        
    }
    public void ResetToPlayerControls(){
        _rotationFactor = 0f;
        GetComponent<Player_Controller>().RotateOnMoveDirection = true;
        GetComponent<Player_Controller>().MouseSensitivity = _defaultSensitivity;
    }
}
