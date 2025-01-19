using System;
using System.Globalization;
using DaftAppleGames.Attributes;
using DaftAppleGames.TimeAndWeather;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaftAppleGames.Samples
{
    public class DebugUiController : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private TimeOfDayController timeOfDayController;
        [BoxGroup("Settings")] [SerializeField] private WeatherController weatherController;
        [BoxGroup("UI Settings")] [SerializeField] private TMP_Text timeText;
        [BoxGroup("UI Settings")] [SerializeField] private Slider timeSlider;
        [BoxGroup("UI Settings")] [SerializeField] private Slider speedSlider;
        [BoxGroup("UI Settings")] [SerializeField] private Toggle timeProgressionToggle;
        private void Start()
        {
            // Add listeners for changes in time and weather
            timeOfDayController.timeChangedEvent.AddListener(UpdateTime);

            // Add UI listeners to update time or weather
            timeSlider.onValueChanged.RemoveAllListeners();
            timeSlider.onValueChanged.AddListener(SetTime);

            speedSlider.onValueChanged.RemoveAllListeners();
            speedSlider.onValueChanged.AddListener(SetSpeed);

            timeProgressionToggle.onValueChanged.RemoveAllListeners();
            timeProgressionToggle.onValueChanged.AddListener(ToggleAuto);

            // Set UI defaults
            timeText.text = timeOfDayController.TimeOfDay.ToString(CultureInfo.InvariantCulture);
            timeSlider.value = timeOfDayController.TimeOfDay;
            speedSlider.value = timeOfDayController.TimeSpeed;
            timeProgressionToggle.isOn = timeOfDayController.TimeProgression;
        }

        private void UpdateTime(float time)
        {
            timeText.text = $"{Convert.ToInt32(Math.Floor(time)):D2}:{(int)((time - (int)time) * 60):D2}";
            timeSlider.SetValueWithoutNotify(time);
        }

        private void SetSpeed(float speed)
        {
            timeOfDayController.TimeSpeed = speed;
        }

        private void SetTime(float time)
        {
            timeOfDayController.SetTimeOfDay(time);
        }

        private void ToggleAuto(bool toggleState)
        {
            timeOfDayController.TimeProgression = toggleState;
        }

        public void GoToTimeOfDay(int timeOfDay)
        {
            timeOfDayController.GoToTimeOfDay(timeOfDay);
        }
    }
}