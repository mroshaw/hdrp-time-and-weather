using System.Collections;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace DaftAppleGames.TimeAndWeather
{
    public class ProbeVolumeScenarioBlender : MonoBehaviour
    {
        [BoxGroup("Time Scenarios")][SerializeField] public TimeProbeScenarios timeScenarios;
        [BoxGroup("Settings")] [SerializeField] private TimeOfDayController timeOfDayController;
        [BoxGroup("Settings")] [SerializeField] private float defaultBlendDuration = 5.0f;
        [BoxGroup("Settings")] [SerializeField] [Range(0, 1)] private float blendingFactor = 0.5f;
        [BoxGroup("Settings")] [SerializeField] [Min(1)] private int numberOfCellsBlendedPerFrame = 10;
        [BoxGroup("Debug")] [SerializeField] private string currentScenario;

        private ProbeReferenceVolume _probeRefVolume;
        private bool _isBlending;

        #region Startup
        private void Start()
        {
            _probeRefVolume = ProbeReferenceVolume.instance;
            _probeRefVolume.numberOfCellsBlendedPerFrame = numberOfCellsBlendedPerFrame;

            // Check scenario every time an hour passes
            // timeOfDayController.hourPassedEvent.AddListener(TimeChanged);

            // Set the current scenario
            ApplyScenarioForCurrentTime();
        }

        #endregion

        #region Class methods

        private void ApplyScenarioForCurrentTime()
        {
            ApplyScenario(timeScenarios.GetScenario((int)timeOfDayController.timeOfDay));
        }

        private void TimeChanged(int newHour)
        {
            if (_isBlending)
            {
                return;
            }

            string timeScenario = timeScenarios.GetScenario(newHour);
            if (timeScenario != currentScenario)
            {
                StartCoroutine(BlendScenariosAsync(timeScenario));
                currentScenario = timeScenario;
            }
        }

        private void ApplyScenario(string scenarioName)
        {
            // _probeRefVolume.SetActiveBakingSet(scenarioName);
        }

        private IEnumerator BlendScenariosAsync(string targetScenario)
        {
            _isBlending = true;
            float elapsedTime = 0;
            while (elapsedTime < defaultBlendDuration)
            {
                _probeRefVolume.BlendLightingScenario(targetScenario, blendingFactor);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _isBlending = false;
        }
        #endregion
        #region Editor methods
        #if UNITY_EDITOR
        [Button("Bake Scenarios")]
        private void BakeScenarios()
        {

        }
        #endif
        #endregion
    }
}