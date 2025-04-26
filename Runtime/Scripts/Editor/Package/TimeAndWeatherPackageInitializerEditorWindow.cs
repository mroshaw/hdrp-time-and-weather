using DaftAppleGames.Editor;
using UnityEditor;
using UnityEngine;

namespace DaftAppleGames.TimeAndWeather.Editor
{
    public class TimeAndWeatherPackageInitializerEditorWindow : PackageInitializerEditorWindow
    {
        protected override string ToolTitle => "HDRP Time And Weather Installer";

        protected override string WelcomeLogText =>
            "Welcome to the HDRP Time and Weather installer!";

        protected override void CreateCustomGUI()
        {
        }

        [MenuItem("Daft Apple Games/Packages/HDRP Time And Weather")]
        public static void ShowWindow()
        {
            TimeAndWeatherPackageInitializerEditorWindow packageInitWindow = GetWindow<TimeAndWeatherPackageInitializerEditorWindow>();
            packageInitWindow.titleContent = new GUIContent("HDRP Time and Weather Installer");
        }
    }
}