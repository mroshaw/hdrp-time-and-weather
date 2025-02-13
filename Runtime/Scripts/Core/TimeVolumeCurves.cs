using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using DaftAppleGames.Attributes;
#endif

namespace DaftAppleGames.TimeAndWeather
{
    internal enum UpdateMode { OnTimeChanged, OnHour, OnMinute, OnUpdate, Manual }

    [ExecuteInEditMode]
    public class TimeVolumeCurves : MonoBehaviour
    {
        #region Class Variables
        [BoxGroup("Settings")] [SerializeField] private UpdateMode updateMode = UpdateMode.OnUpdate;
        [BoxGroup("Volume Settings")] [SerializeField] private Volume volume;
        [BoxGroup("Curves")] [SerializeField] private VolumeSettingTimeCurve[] settingTimeCurves;

        private TimeOfDayController _timeOfDayController;
        private VolumeProfile _volumeProfile;

        #endregion

        #region Startup
        private void OnEnable()
        {
            _timeOfDayController = GetComponent<TimeOfDayController>();
            _volumeProfile = volume.profile;

            InitProfileOverrides();

            _timeOfDayController.hourPassedEvent.RemoveListener(ApplySettings);
            _timeOfDayController.minutePassedEvent.RemoveListener(ApplySettings);
            _timeOfDayController.timeChangedEvent.RemoveListener(ApplySettings);

            switch (updateMode)
            {
                case UpdateMode.OnTimeChanged:
                    _timeOfDayController.timeChangedEvent.AddListener(ApplySettings);
                    break;

                case UpdateMode.OnHour:
                    _timeOfDayController.hourPassedEvent.AddListener(ApplySettings);
                    break;

                case UpdateMode.OnMinute:
                    _timeOfDayController.minutePassedEvent.AddListener(ApplySettings);
                    break;
            }
        }

        private void OnDisable()
        {
            _timeOfDayController.hourPassedEvent.RemoveListener(ApplySettings);
            _timeOfDayController.minutePassedEvent.RemoveListener(ApplySettings);
            _timeOfDayController.timeChangedEvent.RemoveListener(ApplySettings);
        }

        private void Start()
        {
            ApplySettings(_timeOfDayController.TimeOfDay);
        }
        #endregion

        #region Update
        private void Update()
        {
            if (updateMode != UpdateMode.OnUpdate)
            {
                return;
            }

            ApplySettings(_timeOfDayController.TimeOfDay);
        }
        #endregion

        #region Class methods

        private void ApplySettings(int time)
        {
            ApplySettings((float)time);
        }

        private void ApplySettings(float time)
        {
            foreach (VolumeSettingTimeCurve timeCurve in settingTimeCurves)
            {
                switch (timeCurve.VolumeSetting)
                {
                    case VolumeSetting.ExposureCompensation:
                        SetExposureCompensation(timeCurve.EvaluateCurve(time));
                        break;
                }
            }
        }

        private void SetExposureCompensation(float value)
        {
            if (!_volumeProfile.TryGet<Exposure>(out Exposure exposure))
            {
                return;
            }
            exposure.compensation.value = value;
        }

        private void InitProfileOverrides()
        {
            if (_volumeProfile.TryGet<Exposure>(out Exposure exposure))
            {
                exposure.compensation.overrideState = true;
            }
        }
        #endregion
    }
}