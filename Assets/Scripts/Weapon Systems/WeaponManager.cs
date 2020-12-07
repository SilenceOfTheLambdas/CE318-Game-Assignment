using UnityEngine;

/// <summary>
///     This class is responsible for managing weapon functionality.
/// </summary>
public class WeaponManager : MonoBehaviour
{
    public enum WeaponFireStates
    {
        Single,
        Automatic,
        Burst
    }

    /// <summary>
    ///     The fire mode of this weapon.
    /// </summary>
    public WeaponFireStates WeaponFireState;

    [Header("Weapon Stats")]
    // How fast the weapon shoots in seconds
    public float fireRate;

    [SerializeField] private Transform         bulletSpawnPoint;
    public                   AmmunitionManager ammunitionManager;
    private                  float             _nextFire;
    private static readonly  int               IsPressingFire = Animator.StringToHash("IsPressingFire");

    public void Shoot()
    {
        switch (WeaponFireState)
        {
            case WeaponFireStates.Automatic:
                if (Time.time > _nextFire && fireRate > 0)
                {
                    _nextFire = Time.time + fireRate;

                    GetComponent<Animator>().SetBool("IsPressingFire", true);

                    var shot = Instantiate(ammunitionManager.gameObject, bulletSpawnPoint.position,
                        ammunitionManager.gameObject.transform.localRotation);
                    shot.GetComponent<Rigidbody>()
                        .AddForce(bulletSpawnPoint.transform.forward * ammunitionManager.speed);
                    GetComponent<AudioSource>().Play();
                }

                GetComponent<Animator>().SetBool("isRunning",
                    PlayerController.MovementState == PlayerController.MovementStates.Running &&
                    PlayerController.IsGrounded);
                break;
        }
    }
    
    public void DisableGunAnimation()
    {
        GetComponent<Animator>().SetBool(IsPressingFire, false);
    }
}