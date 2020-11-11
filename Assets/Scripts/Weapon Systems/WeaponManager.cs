using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This class is responsible for managing weapon functionality.
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
    /// The fire mode of this weapon.
    /// </summary>
    public WeaponFireStates WeaponFireState;

    [Header("Weapon Stats")] 
    // How fast the weapon shoots in seconds
    public float fireRate;

    [FormerlySerializedAs("_ammunitionManager")] [Header("Ammunition")] [SerializeField]
    private AmmunitionManager ammunitionManager;
    [SerializeField]
    private Transform bulletHolder;

    private Animator _animator;
    private static readonly int IsPressingFire = Animator.StringToHash("isPressingFire");
    private float nextFire;
    public GameObject hitEffect;


    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire && fireRate > 0)
        {
            nextFire = Time.time + fireRate;
            Fire();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            _animator.SetBool(IsPressingFire, false);
        }
        _animator.SetBool("isRunning", PlayerController.MovementState == PlayerController.MovementStates.Running && PlayerController.IsGrounded);
    }

    private void Fire()
    {
        _animator.SetBool(IsPressingFire, true);
        var shot = Instantiate(ammunitionManager.gameObject, bulletHolder.position,
            ammunitionManager.gameObject.transform.localRotation);
        shot.GetComponent<Rigidbody>().AddForce(bulletHolder.transform.forward * ammunitionManager.speed);
        
        GetComponent<AudioSource>().Play();
    }
}
