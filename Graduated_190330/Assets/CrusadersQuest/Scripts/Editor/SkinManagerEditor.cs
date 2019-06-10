using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using Anima2D;

namespace CrusadersQuestReplica
{
    [CustomEditor(typeof(SkinManager))]
    public class SkinManagerEditor : Editor
    {
        ReorderableList mList = null;

        List<string> m_DuplicatedPaths;

        void OnEnable()
        {
            m_DuplicatedPaths = GetDuplicatedPaths((target as SkinManager).transform);

            SetupList();
        }

        void SetupList()
        {
            SerializedProperty skinListProperty = serializedObject.FindProperty("m_Skins");

            if (skinListProperty != null)
            {
                mList = new ReorderableList(serializedObject, skinListProperty, true, true, true, true);

                mList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {

                    SerializedProperty poseProperty = mList.serializedProperty.GetArrayElementAtIndex(index);

                    rect.y += 1.5f;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 120, EditorGUIUtility.singleLineHeight), poseProperty, GUIContent.none);

                    EditorGUI.BeginDisabledGroup(!poseProperty.objectReferenceValue);

                    if (GUI.Button(new Rect(rect.x + rect.width - 115, rect.y, 55, EditorGUIUtility.singleLineHeight), "Save"))
                    {
                        if (EditorUtility.DisplayDialog("Overwrite Pose", "Overwrite '" + poseProperty.objectReferenceValue.name + "'?", "Apply", "Cancel"))
                        {
                            SkinUtils.SavePose(poseProperty.objectReferenceValue as Skin, (target as SkinManager).transform);
                            mList.index = index;
                        }
                    }

                    if (GUI.Button(new Rect(rect.x + rect.width - 55, rect.y, 55, EditorGUIUtility.singleLineHeight), "Load"))
                    {
                        SkinUtils.LoadPose(poseProperty.objectReferenceValue as Skin, (target as SkinManager).transform);
                        mList.index = index;
                    }

                    EditorGUI.EndDisabledGroup();
                };

                mList.drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Skins");
                };

                mList.onSelectCallback = (ReorderableList list) => { };
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();

            if (mList != null)
            {
                mList.DoLayoutList();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create new skin", GUILayout.Width(150)))
            {
                EditorApplication.delayCall += CreateNewSkin;
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (m_DuplicatedPaths.Count > 0)
            {
                string helpString = "Warning: duplicated skin paths found.\nPlease use unique skin paths:\n\n";

                foreach (string path in m_DuplicatedPaths)
                {
                    helpString += path + "\n";
                }

                EditorGUILayout.HelpBox(helpString, MessageType.Warning, true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        void CreateNewSkin()
        {
            serializedObject.Update();

            Skin newSkin = ScriptableObjectUtility.CreateAssetWithSavePanel<Skin>("Create a skin asset", "skin.asset", "asset", "Create a new Skin");

            mList.serializedProperty.arraySize += 1;

            SerializedProperty newElement = mList.serializedProperty.GetArrayElementAtIndex(mList.serializedProperty.arraySize - 1);

            newElement.objectReferenceValue = newSkin;

            serializedObject.ApplyModifiedProperties();

            SkinUtils.SavePose(newSkin, (target as SkinManager).transform);
        }

        List<string> GetDuplicatedPaths(Transform root)
        {
            List<string> paths = new List<string>(50);
            List<string> duplicates = new List<string>(50);
            List<SpriteMeshInstance> spriteMeshInstances = new List<SpriteMeshInstance>(50);

            root.GetComponentsInChildren<SpriteMeshInstance>(true, spriteMeshInstances);

            for (int i = 0; i < spriteMeshInstances.Count; i++)
            {
                SpriteMeshInstance spriteMeshInstance = spriteMeshInstances[i];

                if (spriteMeshInstance)
                {
                    string bonePath = SpriteMeshUtils.GetSpriteMeshPath(root, spriteMeshInstance);

                    if (paths.Contains(bonePath))
                    {
                        duplicates.Add(bonePath);
                    }
                    else
                    {
                        paths.Add(bonePath);
                    }
                }
            }

            return duplicates;
        }
    }
}
