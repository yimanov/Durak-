// Cape Guy, 2015. Use at your own risk.
// http://answers.unity3d.com/questions/286571/can-i-disable-live-recompile.html
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace EditorHelper
{
    /// <summary>
    /// This script exits play mode whenever script compilation is detected during an editor update.
    /// </summary>
    [InitializeOnLoad] // Make static initialiser be called as soon as the scripts are initialised in the editor (rather than just in play mode).
    public class EndActivityOnScriptCompile
    {
        public static event Action EndActivity;
        private static bool _isActivityEnd = false;
        // Static initialiser called by Unity Editor whenever scripts are loaded (editor or play mode)
        static EndActivityOnScriptCompile()
        {
            Unused(_instance);
            _instance = new EndActivityOnScriptCompile();
        }

        private EndActivityOnScriptCompile()
        {

            EditorApplication.update += OnEditorUpdate;
        }

        ~EndActivityOnScriptCompile()
        {
            _isActivityEnd = false;
            EditorApplication.update -= OnEditorUpdate;
            // Silence the unused variable warning with an if.
            _instance = null;
        }

        // Called each time the editor updates.
        private static void OnEditorUpdate()
        {
            if (!_isActivityEnd && EditorApplication.isCompiling)
            {
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                }

                Debug.LogWarning("Exiting activity due to script compilation.");
                EndActivity?.Invoke();
                _isActivityEnd = true;
            }
        }

        // Used to silence the 'is assigned by its value is never used' warning for _instance.
        private static void Unused<T>(T unusedVariable) { }

        public static EndActivityOnScriptCompile _instance = null;
    }
}
#endif