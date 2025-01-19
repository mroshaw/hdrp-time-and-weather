#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

namespace DaftAppleGames.Samples
{
    public class Rotate : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [BoxGroup("Settings")] [SerializeField] private float rotateSpeed = 0.5f;
        [BoxGroup("Settings")] [SerializeField] private bool rotateOnStart = true;

        private bool _rotate;

        private void Start()
        {
            _rotate = rotateOnStart;
        }

        private void Update()
        {
            if (!_rotate)
            {
                return;
            }

            RotateObject();
        }

        private void RotateObject()
        {
            transform.Rotate(rotationAxis, rotateSpeed * Time.deltaTime, Space.Self);
        }

        [Button("Pause/Unpause")]
        private void PauseUnpause()
        {
            _rotate = !_rotate;
        }
    }
}