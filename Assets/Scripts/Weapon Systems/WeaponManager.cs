using Player;
using UnityEngine;

namespace Weapon_Systems
{
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

        private static readonly int IsPressingFire = Animator.StringToHash("IsPressingFire");
        private static readonly int AkmAds         = Animator.StringToHash("akmADS");
        private static readonly int IsRunning      = Animator.StringToHash("isRunning");

        /// <summary>
        ///     The fire mode of this weapon.
        /// </summary>
        public WeaponFireStates weaponFireState;

        [Header("Weapon Stats")]
        // How fast the weapon shoots in seconds
        public float fireRate;

        [SerializeField] private Transform         bulletSpawnPoint;
        [SerializeField] private GameObject        bulletPrefab;
        public                   AmmunitionManager ammunitionManager;
        private                  float             _nextFire;

        private void Update()
        {
            if (GameManager.Instance.playerController.primaryWeapon != null
                || GameManager.Instance.playerController.secondaryWeapon != null
                || GameManager.Instance.playerController.thirdWeapon != null)
            {
                if (Input.GetMouseButton(0))
                    Shoot();
                AimDownSights();
            }
        }

        public void Shoot()
        {
            if (weaponFireState == WeaponFireStates.Single)
            {
                if (Time.time > _nextFire && fireRate > 0)
                {
                    _nextFire = Time.time + 1f / fireRate;

                    GetComponent<Animator>().SetBool(IsPressingFire, true);

                    var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position,
                        bulletSpawnPoint.rotation);

                    var ballistics = bullet.GetComponent<Ballistics>();
                    bullet.GetComponent<Rigidbody>().velocity =
                        ballistics.muzzleVelocity * 0.3048f * bulletSpawnPoint.forward;

                    PlayShotSound();
                }

                GetComponent<Animator>().SetBool(IsRunning,
                    PlayerController.MovementState == PlayerController.MovementStates.Running &&
                    PlayerController.IsGrounded);
            }

            if (weaponFireState == WeaponFireStates.Automatic)
            {
                if (Time.time > _nextFire && fireRate > 0)
                {
                    _nextFire = Time.time + 1f / fireRate;

                    GetComponent<Animator>().SetBool(IsPressingFire, true);

                    var shot = Instantiate(ammunitionManager.gameObject, bulletSpawnPoint.position,
                        transform.rotation);
                    shot.transform.LookAt(shot.transform.position + shot.GetComponent<Rigidbody>().velocity);

                    shot.GetComponent<Rigidbody>()
                        .AddForce(bulletSpawnPoint.transform.forward * ammunitionManager.speed);
                    
                    PlayShotSound();
                }

                GetComponent<Animator>().SetBool(IsRunning,
                    PlayerController.MovementState == PlayerController.MovementStates.Running &&
                    PlayerController.IsGrounded);
            }
        }

        public void PlayShotSound()
        {
            GetComponent<AudioSource>().Play();
        }

        public void AimDownSights()
        {
            if (Input.GetMouseButton(1))
            {
                GameManager.Instance.adsCamera.gameObject.SetActive(true);
                GameManager.Instance.adsCamera.GetComponent<Animator>().SetBool(AkmAds, true);
                GameManager.Instance.playerController.isPlayerAds = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                GameManager.Instance.adsCamera.GetComponent<Animator>().SetBool(AkmAds, false);
                GameManager.Instance.playerController.isPlayerAds = false;
            }
        }

        public void DisableGunAnimation()
        {
            GetComponent<Animator>().SetBool(IsPressingFire, false);
        }

        public void StopShotSound()
        {
            GetComponent<AudioSource>().Stop();
        }
    }
}