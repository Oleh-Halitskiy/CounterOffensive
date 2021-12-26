using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] float swayMultiplier = 0f;
    [SerializeField] float swaySmoothMultuplier = 0f;
    [SerializeField] float swayReset = 0f;
    [SerializeField] float damage = 10f;
    [SerializeField] float range = 10f;
    [SerializeField] float fireRate = 15f;

    // cameras
    public GameObject recoilCamera, mainCamera;
    //
    public GameObject bullethole, player;
    public Transform WeaponPosition, ADSPos, Muzzle;
    public ParticleSystem MuzzleFlash;
    private FirstPersonCamera firstPersonCamera;
    private FirstPersonMovement firstPersonMovement;
    private float xInput, yInput, nextTimeToFire = 0f;
    private float mXinput, mYinput;
    private Vector3 scopingVelocity;
    // vectors for sway support
    private Vector3 newWeaponRotation, newWeaponRotationVelocity, targetWeaponRotation, targetWeaponVelocity;
    private Vector3 newWeaponMovementRotation, newWeaponMovementRotationVelocity, targetWeaponMovementRotation, targetWeaponMovementVelocity;
    //
    private RaycastHit hit;
    private AudioSource audioSource;

    void Init()
    {
        firstPersonMovement = player.GetComponent<FirstPersonMovement>();
        firstPersonCamera = mainCamera.GetComponent<FirstPersonCamera>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Sway()
    {
        xInput = firstPersonCamera.Xrotation;
        yInput = firstPersonCamera.Yrotation;
        mXinput = firstPersonMovement.Xinpt;
        mYinput = firstPersonMovement.Xinpt;
        targetWeaponRotation.y += swayMultiplier * (false ? -xInput : xInput) * Time.deltaTime;
        targetWeaponRotation.x += swayMultiplier * (false ?  yInput : -yInput) * Time.deltaTime;
        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -10f, 10f);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -10f, 10f);
        targetWeaponRotation.z = targetWeaponRotation.y * 0.9f;
        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponVelocity, swayReset);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, swaySmoothMultuplier);
        transform.localRotation = Quaternion.Euler(newWeaponRotation);
    }
    private void AimDownSights()
    {
        if (Input.GetMouseButton(1))
        {
         transform.localPosition = Vector3.SmoothDamp(transform.localPosition, ADSPos.localPosition, ref scopingVelocity, 3.5f * Time.deltaTime);
            mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, 30, Time.deltaTime * 10f);
            
          
        }
        else if (!Input.GetMouseButton(1))
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, WeaponPosition.localPosition, ref scopingVelocity, 3.5f * Time.deltaTime);
            mainCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mainCamera.GetComponent<Camera>().fieldOfView, 75, Time.deltaTime * 10f);
        }


    }
    void Shoot()
    {
        if (Input.GetButton("Fire1"))
        {
            if (nextTimeToFire <= 0)
            {
                nextTimeToFire = 1f;
                MuzzleFlash.Play();
                recoilCamera.GetComponent<CameraRecoil>().RecoilFire();
                audioSource.PlayOneShot(audioSource.clip);
                if (Physics.Raycast(Muzzle.position, Muzzle.forward, out hit, range))
                {

                    GameObject hithole = Instantiate(bullethole, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(hithole, 2f);
                }
            }
        }
        nextTimeToFire -= Time.deltaTime * fireRate;
    }
    void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        Sway();

    }
    private void FixedUpdate()
    {
        AimDownSights();
    }

}
