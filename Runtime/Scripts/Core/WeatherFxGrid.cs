#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather
{
    public class WeatherFxGrid : MonoBehaviour
    {
        [BoxGroup("Prefabs")] [SerializeField] private WeatherFxController[] weatherFxGrid = new WeatherFxController[9];
        [BoxGroup("Prefabs")] [SerializeField] private GameObject snowFxPrefab;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject rainFxPrefab;
        [BoxGroup("References")] [SerializeField] private Camera mainCamera;
        [BoxGroup("Settings")] [SerializeField] private float gridSquareSize = 10.0f;
        [BoxGroup("Setting")] [SerializeField] private LayerMask colliderLayerMask;
        [BoxGroup("Setting")] [SerializeField] private string colliderTag;

        private WeatherFxController _tempCenter;
        private int _currentCenter = 0;
        private void Awake()
        {
            ConfigureGrid();
            ResetGrid(0);

            foreach (WeatherFxController weatherFxController in weatherFxGrid)
            {
                weatherFxController.cellEnteredEvent.RemoveAllListeners();
                weatherFxController.cellEnteredEvent.AddListener(ResetGrid);
            }
        }

        private void Start()
        {

        }

        private void SetMainCamera(Camera newCamera)
        {
            mainCamera = newCamera;
            ResetGrid(0);
        }

        [Button("Configure Grid")]
        private void ConfigureGrid()
        {
            if (!mainCamera)
            {
                return;
            }

            weatherFxGrid[0].transform.position = mainCamera.transform.position;

            for (int currGridIndex = 0; currGridIndex <= 8; currGridIndex++)
            {
                weatherFxGrid[currGridIndex].Configure(currGridIndex, weatherFxGrid[0].transform, gridSquareSize, colliderLayerMask, colliderTag);
            }
        }

        [Button("Reset Grid")]
        private void ResetGridToZero()
        {
            ResetGrid(0);
        }

        private void ResetGrid(int activeCellIndex)
        {
            if (!mainCamera)
            {
                return;
            }

            SwitchCenter(activeCellIndex);
        }

        private void SwitchCenter(int newCenter)
        {
            if (_currentCenter == newCenter)
            {
                return;
            }

            Debug.Log($"Switching to new center, from {_currentCenter} to {newCenter}");
            _currentCenter = newCenter;

            // Swap the cells
            _tempCenter = weatherFxGrid[0];
            weatherFxGrid[0] = weatherFxGrid[newCenter];
            weatherFxGrid[newCenter] = _tempCenter;

            weatherFxGrid[0].SetGridCell(0, weatherFxGrid[0].transform, gridSquareSize);
            weatherFxGrid[newCenter].SetGridCell(newCenter, weatherFxGrid[0].transform, gridSquareSize);

            // Reposition all other cells
            for (int currGridIndex = 0; currGridIndex < 9; currGridIndex++)
            {
                if (currGridIndex != newCenter && currGridIndex != 0)
                {
                    weatherFxGrid[currGridIndex].RepositionCell(weatherFxGrid[0].transform, gridSquareSize);
                }
            }
        }
    }
}