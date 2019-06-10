using UnityEngine;
using UnityEditor;
using System.Collections;
using Anima2D;
using System.Collections.Generic;

namespace CrusadersQuestReplica
{
    public static class SkinUtils
    {
        public static void SavePose(Skin skin, Transform root)
        {
            List<SpriteMeshInstance> skinSprites = new List<SpriteMeshInstance>(50);

            root.GetComponentsInChildren<SpriteMeshInstance>(true, skinSprites);

            SerializedObject skinObject = new SerializedObject(skin);
            SerializedProperty entriesProp = skinObject.FindProperty("m_SkinEntries");


            skinObject.Update();
            entriesProp.arraySize = skinSprites.Count;

            for (int i = 0; i < skinSprites.Count; i++)
            {
                SpriteMeshInstance spriteMeshInstance = skinSprites[i];

                if (spriteMeshInstance)
                {
                    SerializedProperty element = entriesProp.GetArrayElementAtIndex(i);
                    element.FindPropertyRelative("path").stringValue = SpriteMeshUtils.GetSpriteMeshPath(root, spriteMeshInstance);
                    element.FindPropertyRelative("skin").objectReferenceValue = spriteMeshInstance.spriteMesh;
                }
            }

            skinObject.ApplyModifiedProperties();
            List<SpriteMesh> faces = new List<SpriteMesh>(50);
            faces.AddRange(root.GetComponentInChildren<SpriteMeshAnimation>().frames);

            SerializedObject face0s = new SerializedObject(skin);
            SerializedProperty entriesProp1 = face0s.FindProperty("m_FaceEntries");


            face0s.Update();
            entriesProp1.arraySize = faces.Count;
            for (int i = 0; i < faces.Count; i++)
            {
                SpriteMesh faceMesh = faces[i];

                if (faceMesh)
                {
                    SerializedProperty element = entriesProp1.GetArrayElementAtIndex(i);
                    element.FindPropertyRelative("path").stringValue = SpriteMeshUtils.GetSpriteMeshPath(root, root.GetComponentInChildren<SpriteMeshAnimation>());
                    element.FindPropertyRelative("skin").objectReferenceValue = faceMesh;
                }
            }



            face0s.ApplyModifiedProperties();
        }

        public static void LoadPose(Skin skin, Transform root)
        {
            SerializedObject skinSO = new SerializedObject(skin);
            SerializedProperty entriesProp = skinSO.FindProperty("m_SkinEntries");
            
            List<Skin> skins = new List<Skin>();

            for (int i = 0; i < entriesProp.arraySize; i++)
            {
                SerializedProperty element = entriesProp.GetArrayElementAtIndex(i);

                Transform skinTransform = root.Find(element.FindPropertyRelative("path").stringValue);

                if (skinTransform)
                {
                    SpriteMeshInstance skinComponent = skinTransform.GetComponent<SpriteMeshInstance>();

                    Undo.RecordObject(skinTransform.GetComponent<SpriteMeshInstance>(), "Load Skin");
                    skinComponent.spriteMesh = element.FindPropertyRelative("skin").objectReferenceValue as SpriteMesh;
                    
                }
            }
            
            SerializedProperty entriesProp1 = skinSO.FindProperty("m_FaceEntries");

            List<SpriteMesh> faces = new List<SpriteMesh>();

            for (int i = 0; i < entriesProp1.arraySize; i++)
            {
                SerializedProperty element = entriesProp1.GetArrayElementAtIndex(i);
                faces.Add(element.FindPropertyRelative("skin").objectReferenceValue as SpriteMesh);
            }
            SpriteMeshAnimation sma = root.Find(entriesProp1.GetArrayElementAtIndex(0).FindPropertyRelative("path").stringValue).GetComponent<SpriteMeshAnimation>();
            if (faces.Count>=1)
            {
                sma.frames = faces.ToArray();
                sma.frame = 0;
                
                Undo.RecordObject(sma, "Load Skin");
            }
            EditorUtility.SetDirty(sma);
            //EditorUpdater.SetDirty("Load Skin");

        }
    }
}
