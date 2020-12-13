using System.IO;
using UnityEditor;
using UnityEngine;

namespace ILranch
{
    [InitializeOnLoad]
    public class SettingsPrompt : EditorWindow
    {
        private static bool           showdialogwindow = true;
        private static SettingsPrompt dialogwindow;
        private static int            defaultqualitylevel;
        private static string         prefkey;
        private static bool           pressed1;
        private static bool           pressed2;
        private static bool           pressed3;
        private        string         valuename;


        static SettingsPrompt()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            var datapath = Application.dataPath;
            var strval   = datapath.Split("/"[0]);
            prefkey = strval[strval.Length - 2];

            showdialogwindow = !EditorPrefs.HasKey(prefkey);
            if (showdialogwindow)
            {
                dialogwindow = GetWindow<SettingsPrompt>(true);
                dialogwindow.minSize = new Vector2(350, 380);
                defaultqualitylevel = QualitySettings.GetQualityLevel();
            }

            EditorApplication.update -= Update;
        }

        public void OnGUI()
        {
            var rect = GUILayoutUtility.GetRect(position.width - 10, 100, GUI.skin.box);

            var ilranchlogo = AssetDatabase.LoadAssetAtPath<Texture2D>(
                Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this))) +
                "/LogoDialog.png");
            if (ilranchlogo != null) GUI.DrawTexture(rect, ilranchlogo, ScaleMode.ScaleToFit);

            GUI.backgroundColor = Color.white;
            EditorGUILayout.HelpBox("Prompt for Beginners:", MessageType.Info, true);
            GUI.backgroundColor = Color.clear;
            EditorGUILayout.HelpBox(
                "IL.ranch package DemoScene uses Post Processing Stack (Version 1). Recommended project settings for it:",
                MessageType.None);
            EditorGUILayout.HelpBox("1. Linear ColorSpace", MessageType.None);

            if (!pressed1) GUI.backgroundColor = Color.yellow;
            else GUI.backgroundColor = Color.green;

            if (GUILayout.Button("current 'ColorSpace' is: " + SColorSpace()))
            {
                if (PlayerSettings.colorSpace == ColorSpace.Linear)
                    PlayerSettings.colorSpace = ColorSpace.Gamma;
                else
                    PlayerSettings.colorSpace = ColorSpace.Linear;

                EditorUtility.DisplayDialog("Confirmation", "'ColorSpace' changed to " + PlayerSettings.colorSpace,
                    "Ok");
                pressed1 = !pressed1;
            }

            GUI.backgroundColor = Color.clear;
            EditorGUILayout.HelpBox("2. Maximum QualityLevel", MessageType.None);

            if (!pressed2) GUI.backgroundColor = Color.yellow;
            else GUI.backgroundColor = Color.green;

            if (GUILayout.Button("current 'QualityLevel' is: " + QualityLevel()))
            {
                var names        = QualitySettings.names;
                var currentlevel = QualitySettings.GetQualityLevel();
                if (currentlevel < names.Length - 1)
                    QualitySettings.SetQualityLevel(names.Length - 1, true);
                else
                    QualitySettings.SetQualityLevel(defaultqualitylevel, true);

                EditorUtility.DisplayDialog("Confirmation", "'QualityLevel' changed to " + QualityLevel(), "Ok");
                pressed2 = !pressed2;
            }

            GUI.backgroundColor = Color.clear;
            EditorGUILayout.HelpBox("3. Disabled AntiAliasing", MessageType.None);

            if (!pressed3)
            {
                if (QualitySettings.antiAliasing == 0)
                    GUI.backgroundColor = Color.green;
                else
                    GUI.backgroundColor = Color.yellow;
            }
            else
            {
                if (QualitySettings.antiAliasing == 0)
                    GUI.backgroundColor = Color.green;
                else
                    GUI.backgroundColor = Color.yellow;
            }

            if (GUILayout.Button("current 'AntiAliasing' is: " + AntiAlias()))
            {
                if (QualitySettings.antiAliasing == 0)
                    QualitySettings.antiAliasing = 2;
                else
                    QualitySettings.antiAliasing = 0;

                EditorUtility.DisplayDialog("Confirmation", "'AntiAliasing' changed to " + AntiAlias(), "Ok");
                pressed3 = !pressed3;
            }

            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Ignore"))
            {
                EditorPrefs.SetBool(prefkey, true);
                Close();
            }

            if (GUILayout.Button("Close Prompt"))
            {
                EditorPrefs.SetBool(prefkey, true);
                Close();
            }
        }

        private string AntiAlias()
        {
            if (QualitySettings.antiAliasing == 0)
                valuename = "disabled";
            else
                valuename = "2x (default)";

            return valuename;
        }

        private string QualityLevel()
        {
            var names        = QualitySettings.names;
            var currentlevel = QualitySettings.GetQualityLevel();
            if (currentlevel < names.Length - 1)
                valuename = "Not a Maximum";
            else
                valuename = "Maximum";

            return valuename;
        }

        private string SColorSpace()
        {
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
                valuename = "Linear";
            else
                valuename = "Gamma";

            return valuename;
        }
    }
}