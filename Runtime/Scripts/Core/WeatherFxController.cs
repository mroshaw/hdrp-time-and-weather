using DaftAppleGames.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.VFX;

namespace DaftAppleGames.TimeAndWeather
{
    [RequireComponent(typeof(BoxCollider))]
    [ExecuteInEditMode]
    public class WeatherFxController : MonoBehaviour
    {
        [BoxGroup("Prefabs")] [SerializeField] private GameObject weatherFxPrefab;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject audioFxPrefab;
        [BoxGroup("Events")] public UnityEvent<int>cellEnteredEvent;

        private BoxCollider _boxCollider;
        private VisualEffect _visualEffect;

        private bool _isCenterCell;
        private LayerMask _colliderLayerMask;
        private string _colliderTag;

        [BoxGroup("Debug")] [SerializeField] private int _gridCell;

        private void OnEnable()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _visualEffect = GetComponentInChildren<VisualEffect>();
        }

        private void Start()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _visualEffect = GetComponentInChildren<VisualEffect>();
        }

        public void RepositionCell(Transform centerTransform, float gridSquareSize)
        {
            float activeGridX = centerTransform.position.x;
            float activeGridY = centerTransform.position.y;
            float activeGridZ = centerTransform.position.z;

            DisableTrigger();

            switch (_gridCell)
            {
                case 0:
                    transform.position = centerTransform.position;
                    _isCenterCell = true;
                    break;
                case 1:
                    // Left
                    transform.position = new Vector3(activeGridX - gridSquareSize, activeGridY, activeGridZ);
                    _isCenterCell = false;
                    EnableTrigger();
                    break;
                case 2:
                    // Right
                    transform.position = new Vector3(activeGridX + gridSquareSize, activeGridY, activeGridZ);
                    _isCenterCell = false;
                    EnableTrigger();
                    break;
                case 3:
                    // Front
                    transform.position = new Vector3(activeGridX, activeGridY, activeGridZ + gridSquareSize);
                    _isCenterCell = false;
                    EnableTrigger();
                    break;
                // Behind
                case 4:
                    transform.position = new Vector3(activeGridX, activeGridY, activeGridZ - gridSquareSize);
                    _isCenterCell = false;
                    EnableTrigger();
                    break;
                // Top left
                case 5:
                    transform.position = new Vector3(activeGridX - gridSquareSize, activeGridY, activeGridZ + gridSquareSize);
                    _isCenterCell = false;
                    EnableTrigger();
                    break;
                case 6:
                    // Top right
                    transform.position = new Vector3(activeGridX + gridSquareSize, activeGridY, activeGridZ + gridSquareSize);
                    _isCenterCell = false;
                    EnableTrigger();
                    break;
                case 7:
                    // Bottom left
                    transform.position = new Vector3(activeGridX - gridSquareSize, activeGridY, activeGridZ - gridSquareSize);
                    _isCenterCell = false;
                    EnableTrigger();
                    break;
                case 8:
                    // Bottom right
                    transform.position = new Vector3(activeGridX + gridSquareSize, activeGridY, activeGridZ - gridSquareSize);
                    _isCenterCell = false;
                    EnableTrigger();
                    break;
            }
        }

        public void SetGridCell(int gridCell, Transform centerTransform, float gridSquareSize)
        {
            _gridCell = gridCell;
            RepositionCell(centerTransform, gridSquareSize);
        }

        public void Configure(int gridCell, Transform centerTransform, float boxSize, LayerMask colliderLayerMask, string colliderTag)
        {
            _boxCollider.size = new Vector3(boxSize, 500.0f, boxSize);
            _colliderLayerMask = colliderLayerMask;
            _colliderTag = colliderTag;

            SetGridCell(gridCell, centerTransform, boxSize);

            _visualEffect.SetVector2("Area", new Vector2(boxSize, boxSize));
        }

        public void StartWeather()
        {

        }

        public void StopWeather()
        {

        }

        private void SetTriggerState(bool state)
        {
            _boxCollider.enabled = state;
        }

        public void EnableTrigger()
        {
             SetTriggerState(true);
        }

        public void DisableTrigger()
        {
            SetTriggerState(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Collision entered in GridCell {_gridCell} by {other.gameObject.name}");
            if(!_isCenterCell && other.CompareTag(_colliderTag))
            {
                Debug.Log($"Camera entered cell: {_gridCell}");
                cellEnteredEvent.Invoke(_gridCell);
            }
        }
    }
}