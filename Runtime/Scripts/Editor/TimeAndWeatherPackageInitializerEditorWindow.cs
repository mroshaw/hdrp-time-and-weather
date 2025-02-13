using DaftAppleGames.Editor;
using UnityEditor;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Editor
{
    public class TimeAndWeatherPackageInitializerEditorWindow : PackageInitializerEditorWindow
    {
        [MenuItem("Daft Apple Games/Packages/HDRP Time And Weather Initializer")]
        public static void ShowWindow()
        {
            TimeAndWeatherPackageInitializerEditorWindow packageInitWindow = GetWindow<TimeAndWeatherPackageInitializerEditorWindow>();
            packageInitWindow.titleContent = new GUIContent("HDRP Time and Weather");
        }
        protected override string GetIntroText()
        {
            return "Welcome to HDRP Time And Weather!";
        }

        protected override string GetBaseInstallLocation()
        {
            throw new System.NotImplementedException();
        }
    }
}