#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace DaftAppleGames.TimeAndWeather
{
    public class TimeLightingController : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField] private TimeOfDayController timeOfDayController;
        [BoxGroup("References")] [SerializeField] private Volume physicallyBasedSkyVolume;
        [BoxGroup("Settings")] [SerializeField] private int updateFrameFrequency = 1;
        [BoxGroup("Sky Settings")] [SerializeField] private TimeCurve skyExposureCompensationCurve;
        private VolumeProfile _skyVolumeProfile;
        private PhysicallyBasedSky _skySettings;

        private void Awake()
        {
            _skyVolumeProfile = physicallyBasedSkyVolume.profile;
            if(!_skyVolumeProfile.TryGet<PhysicallyBasedSky>(out _skySettings))
            {
                Debug.Log("TimeLightingController: Physically base sky settings not found.");
            }
        }

        private void Update()
        {
            if (Time.frameCount % updateFrameFrequency != 0)
            {
                return;
            }
            float normalisedCurrentTime = timeOfDayController.TimeOfDay / 24;

            // Update the curves
            _skySettings.exposure.value = skyExposureCompensationCurve.Evaluate(normalisedCurrentTime);;
        }
    }
}