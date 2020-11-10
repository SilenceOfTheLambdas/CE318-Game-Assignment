using UnityEngine;
using UnityEngine.Serialization;

public class HeadBob : MonoBehaviour
{
    [FormerlySerializedAs("_controller")] public PlayerController controller;
    public float bobbingAmount = 0.02f;
    
    private Vector3 _weaponHolderOrigin;

    private void Start()
    {
        _weaponHolderOrigin = transform.localPosition;
    }

    // Update is called once per frame
    private void Update()
    {
        if (PlayerController.MovementState == PlayerController.MovementStates.Walking || PlayerController.MovementState == PlayerController.MovementStates.Idle)
        {
            GunMotion(controller.movementCounter, bobbingAmount, bobbingAmount);
            controller.movementCounter += Time.deltaTime;
        }
        if (PlayerController.MovementState == PlayerController.MovementStates.Running)
        {
            GunMotion(controller.movementCounter * controller.runSpeed, bobbingAmount, bobbingAmount * 2);
            controller.movementCounter += Time.deltaTime;
        }
    }

    private void GunMotion(float p_z, float p_x_intensity, float p_y_intensity)
    {
        transform.localPosition = _weaponHolderOrigin + new Vector3(Mathf.Cos(p_z) * p_x_intensity, 
            Mathf.Sin(p_z * 2) * p_y_intensity, 0);
    }
}
