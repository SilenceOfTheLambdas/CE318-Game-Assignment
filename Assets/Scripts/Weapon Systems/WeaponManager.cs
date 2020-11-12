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

}
