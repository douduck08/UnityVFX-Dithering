using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DouduckLib;

namespace DouduckLibEditor {
    public class DitherMapGeneratorEditorWindow : EditorWindow {

        static DitherMapGenerator s_ditherMapGenerator;

        [SerializeField] DitherMapGenerator ditherMapGenerator;
        [SerializeField] Texture2D ditherMapTexture;

        SerializedObject serializedObject;
        SerializedProperty generatorProperty;
        SerializedProperty textureProperty;

        [MenuItem (EditorUtil.MenuItemPathRoot + "Dither Map Generator", false, EditorUtil.EditorWindows)]
        public static void ShowWindow () {
            EditorWindow.GetWindow (typeof (DitherMapGeneratorEditorWindow), false, "Dither Map Generator");
        }

        void OnFocus () {
            if (ditherMapGenerator == null) {
                if (s_ditherMapGenerator == null) {
                    s_ditherMapGenerator = new DitherMapGenerator ();
                }
                ditherMapGenerator = s_ditherMapGenerator;
                ditherMapTexture = ditherMapGenerator.CreateTexture (16);

                serializedObject = new SerializedObject (this);
                generatorProperty = serializedObject.FindProperty ("ditherMapGenerator");
                // textureProperty = serializedObject.FindProperty ("ditherMapTexture");
            }
        }

        void OnGUI () {
            EditorGUI.BeginChangeCheck ();
            EditorGUILayout.PropertyField (generatorProperty, true);
            if (EditorGUI.EndChangeCheck ()) {
                serializedObject.ApplyModifiedProperties ();
                ditherMapGenerator.iteration = ditherMapGenerator.iteration;
                ditherMapGenerator.Apply (ditherMapTexture);
            }

            EditorGUILayout.LabelField (string.Format ("Dither Map Texture: Size = {0}x{0}", ditherMapGenerator.dimensions));

            EditorGUILayout.BeginHorizontal ();
            if (GUILayout.Button ("Export PNG")) {
                var size = ditherMapGenerator.dimensions;
                var tmp = ditherMapGenerator.CreateTexture (size);
                EditorUtil.SaveAsPNG (tmp, "new dither map texture");
            }
            if (GUILayout.Button ("Export JPG")) {
                var size = ditherMapGenerator.dimensions;
                var tmp = ditherMapGenerator.CreateTexture (size);
                EditorUtil.SaveAsJPG (tmp, "new dither map texture");
            }
            if (GUILayout.Button ("Copy string")) {
                EditorGUIUtility.systemCopyBuffer = DitherMapGenerator.GetString (ditherMapGenerator.iteration);
            }
            EditorGUILayout.EndHorizontal ();

            GUI.enabled = false;
            var resolution = Mathf.Min (position.width, position.height - EditorGUI.GetPropertyHeight (generatorProperty, true)) - 46f;
            EditorGUILayout.ObjectField (ditherMapTexture, typeof (Texture2D), false, GUILayout.Height (resolution), GUILayout.Width (resolution));
            GUI.enabled = true;
        }

    }
}