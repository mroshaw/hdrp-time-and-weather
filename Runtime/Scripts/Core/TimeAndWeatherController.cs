using NaughtyAttributes;
using UnityEngine;


namespace DaftAppleGames.Core.TimeAndWeather
{
    [ExecuteInEditMode]
    public class TimeAndWeatherController : MonoBehaviour
    {
        #region Class Variables
        [BoxGroup("Time")] [SerializeField] private TimeOfDay timeOfDay;
        [BoxGroup("Time")] [SerializeField] private float startingHour = 20.9f;
        [BoxGroup("Weather")] [SerializeField] private VolumeFader fogVolumeFader;
        [BoxGroup("Weather")] [SerializeField] private VolumeFader cloudVolumeFader;
        [BoxGroup("Weather")] [SerializeField] private WeatherPreset startingWeatherPreset;
        [BoxGroup("Weather")] [SerializeField] private WeatherPreset[] allWeatherPresets;
        [BoxGroup("Debug")] [SerializeField] private float sceneHour = 6.0f;
        #endregion

        #region Startup
        /// <summary>
        /// Configure the component on awake
        /// </summary>   
        private void Awake()
        {
            if (Application.isPlaying)
            {
                timeOfDay.timeOfDay = startingHour;
            }

            if (startingWeatherPreset)
            {
                TransitionToWeatherPresetImmediate(startingWeatherPreset);
            }
        }

        #endregion

        #region Class methods

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


        [Button("Set Scene Time")]
        private void SetSceneToDebug()
        {
            SetSceneTime(sceneHour);
        }

        [Button("Dawn")]
        private void SetDawn()
        {
            SetSceneTime(5.5f);
        }


        [Button("Morning")]
        private void SetMorning()
        {
            SetSceneTime(8.0f);

        }

        [Button("Afternoon")]
        private void SetAfternoon()
        {
            SetSceneTime(14.0f);

        }
        [Button("Evening")]
        private void SetEvening()
        {
            SetSceneTime(17.0f);

        }
        [Button("Dusk")]
        private void SetDusk()
        {
            SetSceneTime(19.50f);

        }

        [Button("Night")]
        private void SetNight()
        {
            SetSceneTime(21.0f);

        }

        private void SetSceneTime(float hour)
        {
            timeOfDay.timeOfDay = hour;
        }
        #endregion
    }
}