using System;
using Player;
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
    private static readonly  int               AkmAds         = Animator.StringToHash("akmADS");

    public void Shoot()
    {
        switch (WeaponFireState)
        {
            case WeaponFireStates.Automatic:
                if (Time.time > _nextFire && fireRate > 0)
                {
                    _nextFire = Time.time + 1f / fireRate;

                    GetComponent<Animator>().SetBool(IsPressingFire, true);

                    var shot = Instantiate(ammunitionManager.gameObject, bulletSpawnPoint.position,
                        transform.rotation);
                    shot.transform.LookAt(shot.transform.position + shot.GetComponent<Rigidbody>().velocity);
                    
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

    private void Update()
    {
        AimDownSights();
    }

    public void AimDownSights()
    {
        if (Input.GetMouseButton(1))
        {
            GameManager.Instance.ADSCamera.gameObject.SetActive(true);
            GameManager.Instance.ADSCamera.GetComponent<Animator>().SetBool(AkmAds, true);
            GameManager.Instance.PlayerController.isPlayerAds = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            GameManager.Instance.ADSCamera.GetComponent<Animator>().SetBool(AkmAds, false);
            GameManager.Instance.PlayerController.isPlayerAds = false;
        }
    }
    
    public void DisableGunAnimation()
    {
        GetComponent<Animator>().SetBool(IsPressingFire, false);
    }
}