using System;
using System.Collections;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;

namespace DaftAppleGames.TimeAndWeather
{
    [ExecuteInEditMode]
    public class TimeOfDayController : MonoBehaviour
    {
        [Tooltip("Select the game object with the celestial body manager component.")]
        [BoxGroup("Settings")] [SerializeField] private CelestialBodiesManager celestialBodiesManager;
        [Tooltip("Time of day normalized between 0 and 24h. For example 6.5 amounts to 6:30am.")]
        [BoxGroup("Settings")] [Range(0, 23.99f)] public float timeOfDay = 12f;
        [BoxGroup("Settings")] [SerializeField] private float defaultTransitionDuration = 10.0f;
        [Tooltip("Determines whether time automatically passes when in run mode.")]
        [BoxGroup("Settings")] [SerializeField] private bool timeProgression = true;
        [Tooltip("Sets the speed at which the time of day passes.")]
        [BoxGroup("Settings")] [SerializeField] private float timeSpeed = 0.5f;
        [BoxGroup("Settings")] [SerializeField] private bool applyInEditor = false;
        // South East England
        [BoxGroup("Settings")] [SerializeField] private float latitude = 51.183139f;
        [BoxGroup("Settings")] [SerializeField] private float longitude = -0.707395f;

        // Paris
        // [BoxGroup("Settings")] [SerializeField] private float latitude = 48.83402f;
        // [BoxGroup("Settings")] [SerializeField] private float longitude = 2.367259f;

        [FoldoutGroup("Events")] public UnityEvent<float> timeChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<int> hourPassedEvent;
        [FoldoutGroup("Events")] public UnityEvent<int> minutePassedEvent;

        public float TimeSpeed
        {
            get => timeSpeed;
            set => timeSpeed = value;
        }

        public bool TimeProgression
        {
            get => timeProgression;
            set => timeProgression = value;
        }

        public float TimeOfDay => timeOfDay;

        // Arbitrary date to have the sunset framed in the camera frustum.
        private DateTime _date = new DateTime(2024, 4, 21).Date;
        private DateTime _time;

        private int _currentHour;
        private int _currentMinute;

        private void Awake()
        {
            GetHoursMinutesSecondsFromTimeOfDay(out int hours, out int minutes, out int seconds);
            _time = _date + new TimeSpan(hours, minutes, 0);

            _currentHour = Convert.ToInt32(Math.Floor(timeOfDay));
            _currentMinute = (int)((timeOfDay - (int)timeOfDay) * 60);
        }

        private void OnValidate()
        {
            GetHoursMinutesSecondsFromTimeOfDay(out int hours, out int minutes, out int seconds);
            _time = _date + new TimeSpan(hours, minutes, seconds);
            SetSunPosition();
        }

        private void Update()
        {
            if (timeProgression && (applyInEditor || Application.isPlaying))
            {
                AdvanceTimeOfDay();
                SetSunPosition();
                return;
            }

            if (applyInEditor && !Application.isPlaying)
            {
                SetSunPosition();
            }
        }


        #region Class Methods

        public void SetTimeOfDay(float timeToSet)
        {
            timeOfDay = timeToSet;
            AdvanceTimeOfDay();
        }

        public void GoToTimeOfDay(float timeToSet)
        {
            bool currTimeProgress = timeProgression;
            timeProgression = false;
            StartCoroutine(GoToTimeAsync(timeToSet, currTimeProgress));
        }

        private IEnumerator GoToTimeAsync(float timeToSet, bool restoreTimeProgression)
        {
            float timeElapsed = 0;
            float startTime = timeOfDay;

            if (timeToSet < startTime)
            {
                timeToSet += 24;
            }

            while (timeElapsed < defaultTransitionDuration)
            {
                timeElapsed += Time.deltaTime;
                float newTime = Mathf.Lerp(startTime, timeToSet, timeElapsed / defaultTransitionDuration);
                if (newTime >= 24.0f)
                {
                    newTime -= 24.0f;
                }
                SetTimeOfDay(newTime);
                yield return null;
            }
            SetTimeOfDay(timeToSet);
            timeProgression = restoreTimeProgression;
        }

        private void AdvanceTimeOfDay()
        {
            timeOfDay += timeSpeed * Time.deltaTime;

            // This is for the variable to loop for easier use.
            if (timeOfDay > 24f)
            {
                timeOfDay = 0f;
            }

            if (timeOfDay < 0f)
            {
                timeOfDay = 24f;
            }

            timeChangedEvent?.Invoke(timeOfDay);
            // Determine if Events need to trigger
            int newHour = Convert.ToInt32(Math.Floor(timeOfDay));
            int newMinute = (int)((timeOfDay - (int)timeOfDay) * 60);

            if (newHour != _currentHour)
            {
                _currentHour = newHour;
                hourPassedEvent?.Invoke(_currentHour);
            }

            if (newMinute != _currentMinute)
            {
                _currentMinute = newMinute;
                minutePassedEvent?.Invoke(_currentMinute);
            }
        }

        public void SetTimeSpeed(float speed)
        {
            timeSpeed = speed;
        }

        public void GoToTime(float hour)
        {
            StartCoroutine(GoToTimeAsync(hour, defaultTransitionDuration));
        }

        public void GoToTime(float hour, float transitionDuration)
        {
            StartCoroutine(GoToTimeAsync(hour, transitionDuration));
        }

        private IEnumerator GoToTimeAsync(float targetHour, float transitionDuration)
        {
            float elapsedTime = 0;
            float startTime = timeOfDay;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                timeOfDay = Mathf.Lerp(startTime, targetHour, elapsedTime / transitionDuration);
                yield return null;
            }

            timeOfDay = targetHour;
        }

        #endregion

        private void SetSunPosition()
        {
            if (!celestialBodiesManager)
            {
                return;
            }

            CalculateSunPosition(_time, latitude, longitude, timeOfDay, out double azi, out double alt);

            if (double.IsNaN(azi))
                azi = celestialBodiesManager.transform.localRotation.y;

            Vector3 angles = new Vector3((float)alt, (float)azi, 0);
            celestialBodiesManager.transform.localRotation = Quaternion.Euler(angles);
        }

        private void CalculateSunPosition(DateTime dateTime, double sunLatitude, double sunLongitude, float currentTimeOfDay,
            out double outAzimuth, out double outAltitude)
        {
            float declination = -23.45f * Mathf.Cos(Mathf.PI * 2f * (dateTime.DayOfYear + 10) / 365f);

            float localSolarTime = currentTimeOfDay;
            float localHourAngle = 15f * (localSolarTime - 12f);
            localHourAngle *= Mathf.Deg2Rad;

            declination *= Mathf.Deg2Rad;
            float latRad = (float)sunLatitude * Mathf.Deg2Rad;

            float latSin = Mathf.Sin(latRad);
            float latCos = Mathf.Cos(latRad);

            float hourCos = Mathf.Cos(localHourAngle);

            float declinationSin = Mathf.Sin(declination);
            float declinationCos = Mathf.Cos(declination);

            float elevation = Mathf.Asin(declinationSin * latSin + declinationCos * latCos * hourCos);
            float elevationCos = Mathf.Cos(elevation);
            float azimuth =
                Mathf.Acos((declinationSin * latCos - declinationCos * latSin * hourCos) / elevationCos);

            elevation *= Mathf.Rad2Deg;
            azimuth *= Mathf.Rad2Deg;

            if (localHourAngle >= 0f)
                azimuth = 360 - azimuth;

            outAltitude = elevation;
            outAzimuth = azimuth;
        }

        private void GetHoursMinutesSecondsFromTimeOfDay(out int hours, out int minutes, out int seconds)
        {
            hours = Mathf.FloorToInt(timeOfDay);
            minutes = Mathf.FloorToInt((timeOfDay - hours) * 60f);
            seconds = Mathf.FloorToInt((timeOfDay - hours - (minutes / 60f)) * 60f * 60f);
        }

        /*
        #region Editor methods
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
            timeOfDay = hour;
            SetSunPosition();
        }
        #endregion
        */
    }
}