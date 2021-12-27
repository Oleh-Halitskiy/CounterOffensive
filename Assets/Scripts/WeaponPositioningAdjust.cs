using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponPositioningAdjust : MonoBehaviour
{
    [SerializeField] private Transform ADS;
    [SerializeField] private Transform HIP;
    public GameObject currentWeapon;
    //private Vector3 ADSpos, HIPpos;
    private void Start()
    {
        ADS.localPosition = currentWeapon.GetComponent<WeaponController>().AdsVector;
        HIP.localPosition = currentWeapon.GetComponent<WeaponController>().HipVector;
    }
    private void ChangePos()
    {
    }
    private void Update()
    {
        ChangePos();
    }
}
