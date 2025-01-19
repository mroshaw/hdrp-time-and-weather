#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

namespace DaftAppleGames.Samples
{
    public class FollowPath : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private Transform[] pathPoints;
        [BoxGroup("Settings")] [SerializeField] private float moveSpeed = 0.5f;
        [BoxGroup("Settings")] [SerializeField] private float waitTime = 0.5f;
        [BoxGroup("Settings")] [SerializeField] private bool moveOnStart = true;

        private bool _move;
        private int _currentPoint = 0;

        private void Start()
        {
            _move = moveOnStart;
        }

        private void Update()
        {
            if (!_move)
            {
                return;
            }

            MoveTowardsPoint();
        }

        private void MoveTowardsPoint()
        {

            // Calculate the direction towards the target
            Vector3 direction = (pathPoints[_currentPoint].position - transform.position).normalized;

            // Move the transform towards the target
            transform.position += direction * moveSpeed * Time.deltaTime;

            transform.LookAt(pathPoints[_currentPoint].position);

            // Move to next point when arrived
            if (!(Vector3.Distance(transform.position, pathPoints[_currentPoint].position) < 0.05f)) return;
            if (_currentPoint == pathPoints.Length - 1)
            {
                _currentPoint = 0;
            }
            else
            {
                _currentPoint++;
            }
        }

        [Button("Pause/Unpause")]
        private void PauseUnpause()
        {
            _move = !_move;
        }
    }
}