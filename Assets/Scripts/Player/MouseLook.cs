using UnityEngine;

namespace Player
{
    public class MouseLook : MonoBehaviour
    {
        [Header("Sensitivity")] public float mouseSensitivity = 100f;

        [Range(0f, 100f)] public float adsMouseSensitivity;

        public  Transform playerBody;
        private float     _xRotation;

        public float DefaultMouseSensitivity { get; private set; }

        // Start is called before the first frame update
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            DefaultMouseSensitivity = mouseSensitivity;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                _xRotation -= mouseY;
                _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

                transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * mouseX);
            }
        }
    }
}