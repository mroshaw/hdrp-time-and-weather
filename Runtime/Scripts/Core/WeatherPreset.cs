using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace DaftAppleGames.Core.TimeAndWeather
{
    /// <summary>
    /// Scriptable Object: TODO Purpose and Summary
    /// </summary>
    [CreateAssetMenu(fileName = "WeatherPreset", menuName = "Daft Apple Games/Time And Weather/Weather Preset", order = 1)]
    public class WeatherPreset : ScriptableObject
    {
        [BoxGroup("Settings")] public string weatherPresetName;
        [BoxGroup("Volumes")] public VolumeProfile fogVolumeProfile;
        [BoxGroup("Volumes")] public VolumeProfile cloudVolumeProfile;
        [BoxGroup("Weather FX")] public bool playAmbientAudio = true;
        [ShowIf("playAmbientAudio")]
        [BoxGroup("Weather FX")] public AudioClip[] ambientAudioClips;
        [BoxGroup("Weather FX")] public bool spawnWeatherFX = true;
        [ShowIf("spawnWeatherFX")] [BoxGroup("Weather FX")] public GameObject weatherFXPrefab;
    }
}