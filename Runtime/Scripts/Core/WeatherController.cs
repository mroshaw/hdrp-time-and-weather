using DaftAppleGames.Attributes;
using UnityEngine;
using UnityEngine.Events;


namespace DaftAppleGames.TimeAndWeather
{
    [ExecuteInEditMode]
    public class WeatherController : MonoBehaviour
    {
        #region Class Variables
        [BoxGroup("Weather")] [SerializeField] private VolumeFader fogVolumeFader;
        [BoxGroup("Weather")] [SerializeField] private VolumeFader cloudVolumeFader;
        [BoxGroup("Weather")] [SerializeField] private WeatherPreset startingWeatherPreset;
        [BoxGroup("Weather")] [SerializeField] private WeatherPreset[] allWeatherPresets;
        [BoxGroup("Debug")] [SerializeField] private float sceneHour = 6.0f;

        [BoxGroup("Events")] public UnityEvent weatherChangeStartedEvent;
        [BoxGroup("Events")] public UnityEvent weatherChangeFinishedEvent;

        #endregion

        #region Startup
        /// <summary>
        /// Configure the component on awake
        /// </summary>   
        private void Awake()
        {
            if (startingWeatherPreset)
            {
                TransitionToWeatherPresetImmediate(startingWeatherPreset);
            }
        }

        #endregion

        #region Class methods

        [Button("Apply Starting")]
        private void ApplyStartingWeatherPreset()
        {
            
        }

        public void TransitionToWeatherPreset(string weatherPresetName)
        {
            Debug.Log($"TimeAndWeatherController: Attempting transition to: {weatherPresetName}");
            foreach (WeatherPreset preset in allWeatherPresets)
            {
                if (preset.weatherPresetName == weatherPresetName)
                {
                    TransitionToWeatherPreset(preset);
                    return;
                }
            }
            Debug.Log($"TimeAndWeatherController: Weather preset: {weatherPresetName} was not found!");

        }

        private void TransitionToWeatherPresetImmediate(WeatherPreset weatherPreset)
        {
            fogVolumeFader.TransitionToProfile(weatherPreset.fogVolumeProfile, 0.0f);
            // cloudVolumeFader.TransitionToProfile(weatherPreset.cloudVolumeProfile, 0.0f);
        }

        private void TransitionToWeatherPreset(WeatherPreset weatherPreset)
        {
            fogVolumeFader.TransitionToProfile(weatherPreset.fogVolumeProfile);
            // cloudVolumeFader.TransitionToProfile(weatherPreset.cloudVolumeProfile);
        }

        #endregion

        #region Editor Methods

        [Button("Foggy")]
        private void SetFoggyWeather()
        {
            TransitionToWeatherPreset("FoggyDry");
        }

        [Button("Light Cloud")]
        private void SetClearWeather()
        {
            TransitionToWeatherPreset("LightCloudDry");
        }
        #endregion
    }
}