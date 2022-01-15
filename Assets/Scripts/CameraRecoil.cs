
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    private Vector3 currentRotation, targetRotation;
    [SerializeField] private float recoilX, recoilY, recoilZ, snapiness, returnSpeed;
    void FixedUpdate()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.fixedDeltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snapiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}
