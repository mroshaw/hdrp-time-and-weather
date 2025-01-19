#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using DaftAppleGames.TimeAndWeather;

namespace DaftAppleGames.Samples
{
    public class LightSwitch : MonoBehaviour
    {
        [BoxGroup("References")] [SerializeField] private TimeOfDayController timeOfDayController;
        [BoxGroup("Settings")] [SerializeField] private float onHour;
        [BoxGroup("Settings")] [SerializeField] private float offHour;
        [BoxGroup("Settings")] [SerializeField] private int pollFrameFrequency = 5;

        private bool _lightIsOn;
        private Light _light;

        private void Start()
        {
            _light = GetComponent<Light>();
        }

        private void Update()
        {
            if (Time.frameCount % pollFrameFrequency != 0)
            {
                return;
            }

            bool targetLightState = GetLightStateForCurrentTime();

            if (targetLightState && !_lightIsOn)
            {
                Debug.Log("Switching lights on...");
                SwitchOn();
            }

            if (!targetLightState && _lightIsOn)
            {
                Debug.Log("Switching lights off...");
                SwitchOff();
            }
        }

        private bool GetLightStateForCurrentTime()
        {
            if (onHour < offHour)
            {
                return timeOfDayController.TimeOfDay >= onHour && timeOfDayController.TimeOfDay < offHour;
            }

            // Handles cases where timeOn > timeOff (e.g., on in the evening, off in the morning)
            return timeOfDayController.TimeOfDay >= onHour || timeOfDayController.TimeOfDay < offHour;
        }

        [Button("Toggle Switch")]
        private void ToggleSwitch()
        {
            SetLightState(!_lightIsOn);
        }

        [Button("Switch On")]
        private void SwitchOn()
        {
            SetLightState(true);
        }

        [Button("Switch Off")]
        private void SwitchOff()
        {
           SetLightState(false);
        }

        private void SetLightState(bool state)
        {
            _light.enabled = state;
            _lightIsOn = state;
        }
    }
}