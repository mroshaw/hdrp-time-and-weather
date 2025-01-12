using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace DaftAppleGames.Core.TimeAndWeather
{
    public class VolumeFader : MonoBehaviour
    {
        #region Class Variables

        [BoxGroup("Settings")] [SerializeField] private Volume sourceVolume;
        [BoxGroup("Settings")] [SerializeField] private Volume tempTargetVolume;
        [BoxGroup("Settings")] [SerializeField] private float onTargetWeight = 1.0f;
        [BoxGroup("Settings")] [SerializeField] private float offTargetWeight = 0.0f;
        [BoxGroup("Settings")] [SerializeField] private float defaultTransitionDuration;
        [BoxGroup("Events")] public UnityEvent transitionStartedEvent;
        [BoxGroup("Events")] public UnityEvent transitionCompleteEvent;

        [BoxGroup("Debug")] [SerializeField] private bool isInTransition;

        private VolumeProfile _currentProfile;

        #endregion

        #region Startup

        /// <summary>
        /// Configure the component on awake
        /// </summary>   
        private void Awake()
        {
            isInTransition = false;
            _currentProfile = sourceVolume.profile;
        }

        #endregion

        #region Class Methods

        [Button("Fade In Source")]
        public void FadeInSource()
        {
            FadeToTarget(sourceVolume, onTargetWeight);
        }

        [Button("Fade Out Source")]
        public void FadeOutSource()
        {
            FadeToTarget(sourceVolume, offTargetWeight);
        }

        private void FadeToTarget(Volume volume, float targetWeight)
        {
            if (isInTransition)
            {
                return;
            }
            StartCoroutine(FadeSingleVolumeAsync(volume, targetWeight));
        }

        public void TransitionToProfile(VolumeProfile targetProfile, float transitionDuration)
        {
            StartCoroutine(FadeToNewProfileAsync(targetProfile, transitionDuration));
        }


        public void TransitionToProfile(VolumeProfile targetProfile)
        {
            Debug.Log($"VolumeFader: Transitioning to: {targetProfile.name} on {sourceVolume.name}");
            StartCoroutine(FadeToNewProfileAsync(targetProfile, defaultTransitionDuration));
        }

        private IEnumerator FadeSingleVolumeAsync(Volume volume, float targetWeight)
        {
            StartTransition();
            float elapsedTime = 0;
            float startWeight = volume.weight;

            while (elapsedTime < defaultTransitionDuration)
            {
                volume.weight = Mathf.Lerp(startWeight, targetWeight, elapsedTime / defaultTransitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            volume.weight = targetWeight;
            CompleteTransition();
        }

        private IEnumerator FadeToNewProfileAsync(VolumeProfile targetProfile, float duration)
        {
            StartTransition();
            float elapsedTime = 0;
            sourceVolume.weight = 1;
            tempTargetVolume.weight = 0;
            tempTargetVolume.profile = targetProfile;

            while (elapsedTime < duration)
            {
                sourceVolume.weight = Mathf.Lerp(1, 0, elapsedTime / duration);
                tempTargetVolume.weight = Mathf.Lerp(0, 1, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            sourceVolume.weight = 1;
            tempTargetVolume.weight = 0;

            // Swap the volumes
            sourceVolume.profile = targetProfile;
            sourceVolume.sharedProfile = targetProfile;
            sourceVolume.weight = 1;
            tempTargetVolume.weight = 0;
            tempTargetVolume.profile = null;

            CompleteTransition();
        }

        private void StartTransition()
        {
            isInTransition = true;
            transitionStartedEvent.Invoke();
        }

        private void CompleteTransition()
        {
            isInTransition = false;
            transitionCompleteEvent.Invoke();
        }

        #endregion
    }
}