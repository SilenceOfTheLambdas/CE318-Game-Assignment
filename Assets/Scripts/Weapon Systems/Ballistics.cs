using UnityEngine;

namespace Weapon_Systems
{
    /// <summary>
    /// Original author: https://matthew-isidore.ovh/unity-tutorial-full-body-fps-controller-part-3-realistic-ballistic-system/
    /// This class will calculate the various ballistic calculations needed to provide a
    /// very accurate depiction off real-world bullets.
    /// </summary>
    public class Ballistics : MonoBehaviour
    {
        #region Variables
        // Public or Serialized Variables
        [Header("Ballistic Settings")] 
        public float ballisticCoefficient;

        public enum GModel
        {
            G1,
            G2,
            G5,
            G6,
            G7,
            G8
        }

        public GModel bulletGModel;
    
        [Tooltip("Feet per second")]      public float   muzzleVelocity;
        [Tooltip("Degrees from equator")] public float   currentLatitude;
        [Tooltip("Grains")]               public float   bulletMass;
        [Tooltip("Inches")]               public float   bulletDiameter;
        [Tooltip("Inches")]               public float   bulletLength;
        [Tooltip("Inches per twist")]     public float   barrelTwist;
        [Tooltip("Fahrenheit")]           public float   temperature;
        [Tooltip("Percent")]              public float   relativeHumidity;
        [Tooltip("In Hg")]                public float   airPressure;
        [Tooltip("m/s")]                  public Vector3 windVector;
        [Tooltip("g/cm3")]                public float   bulletDensity;

        [SerializeField] private GameObject bulletHolePrefab = default;
    

        // Private Variables
        private Rigidbody _rb;
        private Vector3   _gravity;
        private Vector3   _vectorDrag;
        private Vector3   _trueVelocity;
        private Vector3   _startPosition;
        private Vector3   _vectorCoriolis;
        private Vector3   _previousCoriolisDeflection;
        private Vector3   _vectorCentripetal;
        private Vector3   _previousDrift;
        private Vector3   _vectorSpin;
        private Vector3   _lastPosition;
        private Vector3   _entryHole;
        private Vector3   _exitHole;

        private float _stabilityFactor;
        private float _retardation;
        private float _drag;
        private float _startTime;
        private float _timeOffFlight;
        private float _bulletDirection;
        private float _distance;
        private float _area;
        private float _massGram;
        private float _areaMeters;
        private float _massKg;

        private const float msToFps     = 3.2808399f;
        private const float fpsToms     = 0.3048f;
        private const float omega       = 0.000072921159f;
        private const float gravity     = 9.80665f;
        private const float dryAir      = 287.058f;
        private const float waterVapor  = 461.495f;
        private const float kgm3Togrin3 = 0.252891f;
        private const float HgToPa      = 3386.3886666718315f;
        private const float grTog       = 0.0647989f;
        private const float inTomm      = 25.4f;

        #endregion

        #region BuiltIn Methods

        private void OnEnable()
        {
            _rb = GetComponent<Rigidbody>();
            ConvertUnits();
            SetInitialParameters();
            CalculateStabilityFactor();
        }

        private void FixedUpdate()
        {
            GetSpeed();
            GetTimeOffFlight();
            GetPosition();
            CalculateRetardation();
            CalculateDrag();
            CalculateSpinDrift();
            CalculateCoriolis();
            CalculateCentripetal();
            UpdateBulletVelocity();
        }

        private void OnCollisionEnter(Collision other)
        {
            HandleHit(other);
            Debug.Log(other.gameObject.name);
        }

        #endregion

        #region Custom Methods

        private void ConvertUnits()
        {
            currentLatitude = Mathf.PI / 180 * currentLatitude;
            muzzleVelocity *= fpsToms;
            temperature = (temperature - 32) * 5f / 9f;
            airPressure *= HgToPa;

        }
    
        /// <summary>
        /// Set the initial params
        /// </summary>
        private void SetInitialParameters()
        {
            _startTime = Time.time;
            _startPosition = transform.position;
        
            _gravity = new Vector3(0, Physics.gravity.y * Time.fixedDeltaTime, 0);
            _area = Mathf.Pow((bulletDiameter * inTomm) / 2, 2) * Mathf.PI;
            _area /= 100f;

            _massGram = bulletMass * grTog;

            _areaMeters = _area * 0.0001f;
            _massKg = _massGram * 0.001f;
        }

        private void CalculateStabilityFactor()
        {
            var dewPoint          = temperature - ((100f - relativeHumidity) / 5f);
            var exponent          = (7.5f * dewPoint) / (dewPoint + 237.8f);
            var pSat              = 6.102f * Mathf.Pow(10, exponent);
            var pv                = (relativeHumidity / 100f) * pSat;
            var pd                = airPressure - pv;
            var temperatureKelvin = temperature;
            var pAir = kgm3Togrin3 * (pd / (dryAir * temperatureKelvin))
                       + (pv / (waterVapor * temperatureKelvin));
            var l = bulletLength / bulletDiameter;
            _stabilityFactor =
                ((8 * Mathf.PI) / (pAir * Mathf.Pow(barrelTwist, 2) * Mathf.Pow(bulletDiameter, 5) * 0.57f * l))
                * ((bulletMass * Mathf.Pow(bulletDiameter, 2) / (4.83f * (1 + Mathf.Pow(l, 2)))));
        }

        /// <summary>
        /// Similar to the Coriolis, except we calculate the vertical drift.
        /// </summary>
        private void CalculateCentripetal()
        {
            var centripetalAcceleration = 2 * omega * (muzzleVelocity /
                                                       gravity) * Mathf.Cos(currentLatitude) * Mathf.Sin(_bulletDirection);
            centripetalAcceleration *= Time.fixedDeltaTime;

            _vectorCentripetal = new Vector3(0, -centripetalAcceleration, 0);
        }

        /// <summary>
        /// Updates the bullet's velocity using _gravity.
        /// </summary>
        private void UpdateBulletVelocity()
        {
            _lastPosition = _rb.velocity;
            if (_rb.velocity != Vector3.zero)
                _rb.rotation = Quaternion.LookRotation(_rb.velocity);
            _rb.velocity += _gravity;
            _rb.velocity -= _vectorDrag;
            _rb.velocity += _vectorCentripetal;
        
            if (!float.IsNaN(_vectorCoriolis.x) &&
                !float.IsNaN(_vectorCoriolis.y) &&
                !float.IsNaN(_vectorCoriolis.z))
            {
                _rb.position += _vectorCoriolis;
            }

            if (!float.IsNaN(_vectorSpin.x) &&
                !float.IsNaN(_vectorSpin.y) &&
                !float.IsNaN(_vectorSpin.z))
            {
                _rb.position += _vectorSpin;
            }

            _rb.velocity += windVector * Time.fixedDeltaTime;
        }

        /// <summary>
        /// Update the trueVelocity scaled to the rigid body's velocity according to Fixed delta time.
        /// </summary>
        private void GetSpeed() => _trueVelocity = _rb.velocity + windVector * Time.fixedDeltaTime;
    
        private void GetTimeOffFlight() => _timeOffFlight = Time.time - _startTime;

        /// <summary>
        /// Updates the direction the bullet is going, along with the distance it has travelled.
        /// </summary>
        private void GetPosition()
        {
            _bulletDirection = Mathf.Atan2(_rb.velocity.z, _rb.velocity.x);
            _distance = Vector3.Distance(transform.position, _startPosition);
        }

        /// <summary>
        /// This *large* empirical method uses the Pejsa model to calculate the retardation of the bullet.
        /// It uses 2 parameters; a and n; a is the retardation rate, and n is the slope constant factor.
        /// Both of these values are used to get the retardation that we use to get our drag vector.
        /// </summary>
        private void CalculateRetardation()
        {
            var   velFps = _rb.velocity.magnitude * msToFps;
            float a      = -1;
            float n      = -1;
            switch (bulletGModel)
            {
                case GModel.G1 when velFps > 4230:
                    a = 1.477404177730177e-04f; n = 1.9565f;
                    break;
                case GModel.G1 when velFps > 3680:
                    a = 1.920339268755614e-04f; n = 1.925f;
                    break;
                case GModel.G1 when velFps > 3450:
                    a = 2.894751026819746e-04f; n = 1.875f;
                    break;
                case GModel.G1 when velFps > 3295:
                    a = 4.349905111115636e-04f; n = 1.825f;
                    break;
                case GModel.G1 when velFps > 3130:
                    a = 6.520421871892662e-04f; n = 1.775f;
                    break;
                case GModel.G1 when velFps > 2960:
                    a = 9.748073694078696e-04f; n = 1.725f;
                    break;
                case GModel.G1 when velFps > 2830:
                    a = 1.453721560187286e-03f; n = 1.675f;
                    break;
                case GModel.G1 when velFps > 2680:
                    a = 2.162887202930376e-03f; n = 1.625f;
                    break;
                case GModel.G1 when velFps > 2460:
                    a = 3.209559783129881e-03f; n = 1.575f;
                    break;
                case GModel.G1 when velFps > 2225:
                    a = 3.904368218691249e-03f; n = 1.55f;
                    break;
                case GModel.G1 when velFps > 2015:
                    a = 3.222942271262336e-03f; n = 1.575f;
                    break;
                case GModel.G1 when velFps > 1890:
                    a = 2.203329542297809e-03f; n = 1.625f;
                    break;
                case GModel.G1 when velFps > 1810:
                    a = 1.511001028891904e-03f; n = 1.675f;
                    break;
                case GModel.G1 when velFps > 1730:
                    a = 8.609957592468259e-04f; n = 1.75f;
                    break;
                case GModel.G1 when velFps > 1595:
                    a = 4.086146797305117e-04f; n = 1.85f;
                    break;
                case GModel.G1 when velFps > 1520:
                    a = 1.954473210037398e-04f; n = 1.95f;
                    break;
                case GModel.G1 when velFps > 1420:
                    a = 5.431896266462351e-05f; n = 2.125f;
                    break;
                case GModel.G1 when velFps > 1360:
                    a = 8.847742581674416e-06f; n = 2.375f;
                    break;
                case GModel.G1 when velFps > 1315:
                    a = 1.456922328720298e-06f; n = 2.625f;
                    break;
                case GModel.G1 when velFps > 1280:
                    a = 2.419485191895565e-07f; n = 2.875f;
                    break;
                case GModel.G1 when velFps > 1220:
                    a = 1.657956321067612e-08f; n = 3.25f;
                    break;
                case GModel.G1 when velFps > 1185:
                    a = 4.745469537157371e-10f; n = 3.75f;
                    break;
                case GModel.G1 when velFps > 1150:
                    a = 1.379746590025088e-11f; n = 4.25f;
                    break;
                case GModel.G1 when velFps > 1100:
                    a = 4.070157961147882e-13f; n = 4.75f;
                    break;
                case GModel.G1 when velFps > 1060:
                    a = 2.938236954847331e-14f; n = 5.125f;
                    break;
                case GModel.G1 when velFps > 1025:
                    a = 1.228597370774746e-14f; n = 5.25f;
                    break;
                case GModel.G1 when velFps > 980:
                    a = 2.916938264100495e-14f; n = 5.125f;
                    break;
                case GModel.G1 when velFps > 945:
                    a = 3.855099424807451e-13f; n = 4.75f;
                    break;
                case GModel.G1 when velFps > 905:
                    a = 1.185097045689854e-11f; n = 4.25f;
                    break;
                case GModel.G1 when velFps > 860:
                    a = 3.566129470974951e-10f; n = 3.75f;
                    break;
                case GModel.G1 when velFps > 810:
                    a = 1.045513263966272e-08f; n = 3.25f;
                    break;
                case GModel.G1 when velFps > 780:
                    a = 1.291159200846216e-07f; n = 2.875f;
                    break;
                case GModel.G1 when velFps > 750:
                    a = 6.824429329105383e-07f; n = 2.625f;
                    break;
                case GModel.G1 when velFps > 700:
                    a = 3.569169672385163e-06f; n = 2.375f;
                    break;
                case GModel.G1 when velFps > 640:
                    a = 1.839015095899579e-05f; n = 2.125f;
                    break;
                case GModel.G1 when velFps > 600:
                    a = 5.71117468873424e-05f; n = 1.950f;
                    break;
                case GModel.G1 when velFps > 550:
                    a = 9.226557091973427e-05f; n = 1.875f;
                    break;
                case GModel.G1 when velFps > 250:
                    a = 9.337991957131389e-05f; n = 1.875f;
                    break;
                case GModel.G1 when velFps > 100:
                    a = 7.225247327590413e-05f; n = 1.925f;
                    break;
                case GModel.G1 when velFps > 65:
                    a = 5.792684957074546e-05f; n = 1.975f;
                    break;
                case GModel.G1:
                {
                    if (velFps > 0) { a = 5.206214107320588e-05f; n = 2.000f; }

                    break;
                }
                case GModel.G2 when velFps > 1674:
                    a = .0079470052136733f; n = 1.36999902851493f;
                    break;
                case GModel.G2 when velFps > 1172:
                    a = 1.00419763721974e-03f; n = 1.65392237010294f;
                    break;
                case GModel.G2 when velFps > 1060:
                    a = 7.15571228255369e-23f; n = 7.91913562392361f;
                    break;
                case GModel.G2 when velFps > 949:
                    a = 1.39589807205091e-10f; n = 3.81439537623717f;
                    break;
                case GModel.G2 when velFps > 670:
                    a = 2.34364342818625e-04f; n = 1.71869536324748f;
                    break;
                case GModel.G2 when velFps > 335:
                    a = 1.77962438921838e-04f; n = 1.76877550388679f;
                    break;
                case GModel.G2:
                {
                    if (velFps > 0) { a = 5.18033561289704e-05f; n = 1.98160270524632f; }

                    break;
                }
                case GModel.G5 when velFps > 1730:
                    a = 7.24854775171929e-03f; n = 1.41538574492812f;
                    break;
                case GModel.G5 when velFps > 1228:
                    a = 3.50563361516117e-05f; n = 2.13077307854948f;
                    break;
                case GModel.G5 when velFps > 1116:
                    a = 1.84029481181151e-13f; n = 4.81927320350395f;
                    break;
                case GModel.G5 when velFps > 1004:
                    a = 1.34713064017409e-22f; n = 7.8100555281422f;
                    break;
                case GModel.G5 when velFps > 837:
                    a = 1.03965974081168e-07f; n = 2.84204791809926f;
                    break;
                case GModel.G5 when velFps > 335:
                    a = 1.09301593869823e-04f; n = 1.81096361579504f;
                    break;
                case GModel.G5:
                {
                    if (velFps > 0) { a = 3.51963178524273e-05f; n = 2.00477856801111f; }

                    break;
                }
                case GModel.G6 when velFps > 3236:
                    a = 0.0455384883480781f; n = 1.15997674041274f;
                    break;
                case GModel.G6 when velFps > 2065:
                    a = 7.167261849653769e-02f; n = 1.10704436538885f;
                    break;
                case GModel.G6 when velFps > 1311:
                    a = 1.66676386084348e-03f; n = 1.60085100195952f;
                    break;
                case GModel.G6 when velFps > 1144:
                    a = 1.01482730119215e-07f; n = 2.9569674731838f;
                    break;
                case GModel.G6 when velFps > 1004:
                    a = 4.31542773103552e-18f; n = 6.34106317069757f;
                    break;
                case GModel.G6 when velFps > 670:
                    a = 2.04835650496866e-05f; n = 2.11688446325998f;
                    break;
                case GModel.G6:
                {
                    if (velFps > 0) { a = 7.50912466084823e-05f; n = 1.92031057847052f; }

                    break;
                }
                case GModel.G7 when velFps > 4200:
                    a = 1.29081656775919e-09f; n = 3.24121295355962f;
                    break;
                case GModel.G7 when velFps > 3000:
                    a = 0.0171422231434847f; n = 1.27907168025204f;
                    break;
                case GModel.G7 when velFps > 1470:
                    a = 2.33355948302505e-03f; n = 1.52693913274526f;
                    break;
                case GModel.G7 when velFps > 1260:
                    a = 7.97592111627665e-04f; n = 1.67688974440324f;
                    break;
                case GModel.G7 when velFps > 1110:
                    a = 5.71086414289273e-12f; n = 4.3212826264889f;
                    break;
                case GModel.G7 when velFps > 960:
                    a = 3.02865108244904e-17f; n = 5.99074203776707f;
                    break;
                case GModel.G7 when velFps > 670:
                    a = 7.52285155782535e-06f; n = 2.1738019851075f;
                    break;
                case GModel.G7 when velFps > 540:
                    a = 1.31766281225189e-05f; n = 2.08774690257991f;
                    break;
                case GModel.G7:
                {
                    if (velFps > 0) { a = 1.34504843776525e-05f; n = 2.08702306738884f; }

                    break;
                }
                case GModel.G8 when velFps > 3571:
                    a = .0112263766252305f; n = 1.33207346655961f;
                    break;
                case GModel.G8 when velFps > 1841:
                    a = .0167252613732636f; n = 1.28662041261785f;
                    break;
                case GModel.G8 when velFps > 1120:
                    a = 2.20172456619625e-03f; n = 1.55636358091189f;
                    break;
                case GModel.G8 when velFps > 1088:
                    a = 2.0538037167098e-16f; n = 5.80410776994789f;
                    break;
                case GModel.G8 when velFps > 976:
                    a = 5.92182174254121e-12f; n = 4.29275576134191f;
                    break;
                case GModel.G8:
                {
                    if (velFps > 0) { a = 4.3917343795117e-05f; n = 1.99978116283334f; }

                    break;
                }
            }

            if (a != -1 && n != -1 && velFps > 0 && velFps < 100000)
            {
                _retardation = a * Mathf.Pow(velFps, n) / ballisticCoefficient;
                _retardation /= msToFps;
            }
        }

        /// <summary>
        /// Using our retardation to calculate the bullet's drag.
        /// </summary>
        private void CalculateDrag()
        {
            _drag = Time.fixedDeltaTime * _retardation;
            _vectorDrag = Vector3.Normalize(_trueVelocity) * _drag;
        }

        private void CalculateSpinDrift()
        {
            var spinDrift = 1.25f * (_stabilityFactor + 1.2f) * Mathf.Pow(_timeOffFlight, 1.83f);

            _vectorSpin = new Vector3(spinDrift, 0, 0);
            _vectorSpin -= _previousDrift;
            _previousDrift = new Vector3(spinDrift, 0, 0);
        }

        private void CalculateCoriolis()
        {
            var speed = _distance / _timeOffFlight;
            var deflectionX = (omega * Mathf.Pow(_distance, 2) *
                               Mathf.Sin(currentLatitude)) / speed;
        
            var deflectionY = (1 - 2 * (omega * muzzleVelocity / 
                                        gravity) * Mathf.Cos(currentLatitude) * Mathf.Sin(_bulletDirection));

            var drop = _startPosition.y - transform.position.y;
            deflectionY = deflectionY * drop - drop;

            _vectorCoriolis = new Vector3(deflectionX, deflectionY, 0);
            _vectorCoriolis -= _previousCoriolisDeflection;
            _previousCoriolisDeflection = new Vector3(deflectionX, deflectionY, 0);
        }

        private float GetPenDistance(BallisticMaterial bMaterial, float bulletVelocity, float dragCoefficient)
        {
            var vthr = Mathf.Sqrt(2 * bMaterial.yieldStrength /
                                  (dragCoefficient * (bMaterial.density * 1000)));
            var xc = (_massGram / _area) * (1 / (dragCoefficient * bMaterial.density));

            return xc * Mathf.Log(1 + Mathf.Pow(bulletVelocity / vthr, 2));
        }
    
        private void HandleHit(Collision collision)
        {
            var _collider = collision.collider;
            Instantiate(bulletHolePrefab, collision.contacts[0].point, Quaternion.identity);

            var               bmh = _collider.GetComponent<BallisticMaterialHolder>();
            BallisticMaterial bm;
            bm = BallisticMaterialManager.Instance.GetMaterialFromName(bmh != null ? bmh.name : "Default");

            var penDistance = GetPenDistance(bm, _trueVelocity.magnitude, 1f) / 100.0f;
            var aoi         = 90f - Vector3.Angle(collision.contacts[0].normal * -1, transform.forward);

            var newVelocity = GetNewVelocity(bm, penDistance);
            var newDir      = transform.forward.normalized;

            if (CheckForRicochet(bm, penDistance, aoi))
            {
                var outputAngle = aoi - Mathf.Asin(Mathf.Clamp01(bm.density / bulletDensity)
                                                   * Mathf.Sin((90f + aoi) * Mathf.Deg2Rad)) * Mathf.Rad2Deg;
                outputAngle = Random.Range(outputAngle * (1 - (newVelocity / _trueVelocity.magnitude)), outputAngle);
                newDir = Quaternion.AngleAxis(180f, collision.contacts[0].normal * -1) * -transform.forward.normalized;
                newDir = Quaternion.AngleAxis(outputAngle, transform.right) * newDir;
            }
            else
            {
                var bulletDirection = (_lastPosition - _rb.position);
                var ray             = new Ray(_rb.position, bulletDirection.normalized * penDistance);
                _rb.transform.position = CheckIfGoThrough(_rb.position, ray.GetPoint(penDistance));
                
                if (bm.rndSpread > 0)
                {
                    newDir = (Quaternion.AngleAxis(Random.Range(0f, 360f), 
                        bulletDirection) * Quaternion.AngleAxis(Random.Range(0f, 
                        bm.rndSpread), Vector3.Cross(Vector3.up, bulletDirection)) * bulletDirection);
                }

                if (float.IsNaN(newVelocity))
                {
                    Destroy(gameObject);
                    return;
                }   
            }
            _rb.transform.LookAt(newDir);
            _rb.velocity = newVelocity * transform.forward;

        }

        private bool CheckForRicochet(BallisticMaterial ballisticMaterial, float penDistance, float angle)
        {
            var sd            = bulletMass / (7000f * bulletDiameter * bulletDiameter);
            var penCoef       = Mathf.Clamp01(1 - (penDistance * Mathf.Pow(sd, 0.25f)));
            var criticalAngle = Mathf.Asin(ballisticMaterial.density / bulletDensity) * Mathf.Rad2Deg;
            criticalAngle = criticalAngle + (criticalAngle * penCoef);
            var currentAngleCoef = Mathf.Log(1 + Mathf.Pow(angle / criticalAngle, 2));
            var maxAngleCoef     = Mathf.Log(1 + Mathf.Pow(90f / criticalAngle, 2));
            var angleCoef        = currentAngleCoef / maxAngleCoef;
            
            return Random.Range(sd, 1f) <= 1 - angleCoef;
        }

        private float GetNewVelocity(BallisticMaterial bm, float penDistance)
        {
            var kineticEnergy = (_massKg * (_trueVelocity.magnitude * 
                                            _trueVelocity.magnitude)) / 2;
            var dx = penDistance;
            if (_rb.transform.position != Vector3.zero)
                dx = Vector3.Distance(_entryHole, _exitHole);
            var dE = ((_areaMeters * (kineticEnergy / _massKg) * 
                       (bm.density * 1000f)) + (bm.yieldStrength / 3 * _areaMeters)) * dx;
            kineticEnergy -= (dE - kineticEnergy);
            
            return Mathf.Sqrt((kineticEnergy) / (_massKg * 0.5f));
        }

        private Vector3 CheckIfGoThrough(Vector3 start, Vector3 goal)
        {
            var direction = goal - start;
            direction.Normalize();
            var     iterations = 0;
            var point      = start;
            _exitHole = Vector3.zero;
            _entryHole = Vector3.zero;
            var entry = false;
            while (point != goal)
            {
                if (Physics.Linecast(point, goal, out var hit))
                {
                    if (hit.collider.name != name && entry == false)
                    {
                        _entryHole = hit.point;
                        entry = true;
                    }
                    iterations++;
                    point = hit.point + (direction / 100.0f);
                }
                else
                {
                    point = goal;
                }
            }
            while (point != start)
            {
                if (Physics.Linecast(point, start, out var hit))
                {
                    if (hit.collider.name != name)
                    {
                        iterations++;
                        _exitHole = hit.point;
                    }
                    point = hit.point + -direction / 100.0f;
                }
                else
                {
                    point = start;
                }
            }
            if (iterations % 2 == 1)
            {
                Destroy(gameObject);
            }
            if (_exitHole == Vector3.zero)
            {
                Destroy(gameObject);
            }
            return _exitHole + direction / 100.0f;
        }

        #endregion
    }
}
