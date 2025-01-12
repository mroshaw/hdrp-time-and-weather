using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace DaftAppleGames.Core.TimeAndWeather
{
    [RequireComponent(typeof(Light))]
    [ExecuteInEditMode]
    public class TimeOfDay : MonoBehaviour
    {
        [Tooltip("Time of day normalized between 0 and 24h. For example 6.5 amount to 6:30am.")]
        [BoxGroup("Settings")] public float timeOfDay = 12f;

        [BoxGroup("Settings")] [SerializeField] private float defaultTransitionDuration = 10.0f;
        [Tooltip("Sets the speed at which the time of day passes.")]
        [BoxGroup("Settings")] [SerializeField] private float timeSpeed = 1f;

        [BoxGroup("Settings")] [SerializeField] private float latitude = 48.83402f;
        [BoxGroup("Settings")] [SerializeField] private float longitude = 2.367259f;

        // Arbitrary date to have the sunset framed in the camera frustum.
        private DateTime _date = new DateTime(2024, 4, 21).Date;
        private DateTime _time;
        internal static TimeOfDay Instance;

        private void OnEnable()
        {
            Instance = this;
        }

        private void Awake()
        {
            GetHoursMinutesSecondsFromTimeOfDay(out var hours, out var minutes, out var seconds);
            _time = _date + new TimeSpan(hours, minutes, 0);
        }

        private void OnValidate()
        {
            GetHoursMinutesSecondsFromTimeOfDay(out var hours, out var minutes, out var seconds);
            _time = _date + new TimeSpan(hours, minutes, seconds);
            SetSunPosition();
        }

        void Update()
        {
            timeOfDay += timeSpeed * Time.deltaTime;

            //This is for the variable to loop for easier use.
            if (timeOfDay > 24f)
                timeOfDay = 0f;

            if (timeOfDay < 0f)
                timeOfDay = 24f;

            SetSunPosition();
        }


        #region Class Methods

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

        void SetSunPosition()
        {
            CalculateSunPosition(_time, latitude, longitude, timeOfDay, out var azi, out var alt);

            if (double.IsNaN(azi))
                azi = transform.localRotation.y;

            Vector3 angles = new Vector3((float)alt, (float)azi, 0);
            transform.localRotation = Quaternion.Euler(angles);
        }

        public void CalculateSunPosition(DateTime dateTime, double latitude, double longitude, float timeOfDay,
            out double outAzimuth, out double outAltitude)
        {
            float declination = -23.45f * Mathf.Cos(Mathf.PI * 2f * (dateTime.DayOfYear + 10) / 365f);

            float localSolarTime = timeOfDay;
            float localHourAngle = 15f * (localSolarTime - 12f);
            localHourAngle *= Mathf.Deg2Rad;

            declination *= Mathf.Deg2Rad;
            float latRad = (float)latitude * Mathf.Deg2Rad;

            float lat_sin = Mathf.Sin(latRad);
            float lat_cos = Mathf.Cos(latRad);

            float hour_cos = Mathf.Cos(localHourAngle);

            float declination_sin = Mathf.Sin(declination);
            float declination_cos = Mathf.Cos(declination);

            float elevation = Mathf.Asin(declination_sin * lat_sin + declination_cos * lat_cos * hour_cos);
            float elevation_cos = Mathf.Cos(elevation);
            float azimuth =
                Mathf.Acos((declination_sin * lat_cos - declination_cos * lat_sin * hour_cos) / elevation_cos);

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
    }
}