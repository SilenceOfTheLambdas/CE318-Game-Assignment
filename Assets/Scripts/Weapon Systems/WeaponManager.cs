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
        public WeaponFireStates WeaponFireState;

        [Header("Weapon Stats")]
        // How fast the weapon shoots in seconds
        public float fireRate;

        [SerializeField] private Transform         bulletSpawnPoint;
        public                   AmmunitionManager ammunitionManager;
        private                  float             _nextFire;

        private void Update()
        {
            if (GameManager.Instance.PlayerController.primaryWeapon != null
                || GameManager.Instance.PlayerController.secondaryWeapon != null
                || GameManager.Instance.PlayerController.thirdWeapon != null)
            {
                Shoot();
                AimDownSights();   
            }
        }

        private void Shoot()
        {
            if (WeaponFireState == WeaponFireStates.Single)
            {
                if (Input.GetButtonDown("Fire1"))
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

                        GetComponent<AudioSource>().Play();
                    }

                    GetComponent<Animator>().SetBool(IsRunning,
                        PlayerController.MovementState == PlayerController.MovementStates.Running &&
                        PlayerController.IsGrounded);
                }   
            }

            if (WeaponFireState == WeaponFireStates.Automatic)
            {
                if (Input.GetMouseButton(0))
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

                        GetComponent<AudioSource>().Play();
                    }

                    GetComponent<Animator>().SetBool(IsRunning,
                        PlayerController.MovementState == PlayerController.MovementStates.Running &&
                        PlayerController.IsGrounded);
                }
            }
        }

        private void AimDownSights()
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
}