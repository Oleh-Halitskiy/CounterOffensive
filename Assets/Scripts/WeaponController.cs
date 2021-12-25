using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] float swayMultiplier = 0f;
    [SerializeField] float smoothMultuplier = 0f;
    [SerializeField] float damage = 10f;
    [SerializeField] float range = 10f;
    [SerializeField] float fireRate = 15f;
    public GameObject camera, bullethole, recoilCamera;
    public Transform WeaponPosition, ADSPos, Muzzle;
    public ParticleSystem MuzzleFlash;
    private FirstPersonCamera firstPersonCamera;
    private float xInput, yInput, nextTimeToFire = 0f;
    private Quaternion rotationX, rotationY, swayRotation;
    private Vector3 vel;
    private RaycastHit hit;
    private AudioSource audioSource;

    void Init()
    {
        firstPersonCamera = camera.GetComponent<FirstPersonCamera>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Sway()
    {
        xInput = firstPersonCamera.Xrotation * swayMultiplier;
        yInput = firstPersonCamera.Yrotation * swayMultiplier;
        rotationX = Quaternion.AngleAxis(-yInput, Vector3.right);
        rotationY = Quaternion.AngleAxis(xInput, Vector3.up);
        swayRotation = rotationX * rotationY;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, swayRotation, smoothMultuplier * Time.deltaTime);
    }
    private void AimDownSights()
    {
        if (Input.GetMouseButton(1))
        {
         transform.localPosition = Vector3.SmoothDamp(transform.localPosition, ADSPos.localPosition, ref vel, 3.5f * Time.deltaTime);
            camera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(camera.GetComponent<Camera>().fieldOfView, 30, Time.deltaTime * 10f);
            
          
        }
        else if (!Input.GetMouseButton(1))
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, WeaponPosition.localPosition, ref vel, 3.5f * Time.deltaTime);
            camera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(camera.GetComponent<Camera>().fieldOfView, 75, Time.deltaTime * 10f);
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
