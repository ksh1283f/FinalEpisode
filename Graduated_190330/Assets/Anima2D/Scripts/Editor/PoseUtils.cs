using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Anima2D
{
	public static class PoseUtils
	{
		public static void SavePose(Pose pose, Transform root)
		{
			List<Bone2D> bones = new List<Bone2D>(50);

			root.GetComponentsInChildren<Bone2D>(true,bones);

			SerializedObject poseSO = new SerializedObject(pose);
			SerializedProperty entriesProp = poseSO.FindProperty("m_BoneEntries");

			poseSO.Update();
			entriesProp.arraySize = bones.Count;

			for (int i = 0; i < bones.Count; i++)
			{
				Bone2D bone = bones [i];

				if(bone)
				{
					SerializedProperty element = entriesProp.GetArrayElementAtIndex(i);
					element.FindPropertyRelative("path").stringValue = BoneUtils.GetBonePath(root,bone);
					element.FindPropertyRelative("localPosition").vector3Value = bone.transform.localPosition;
					element.FindPropertyRelative("localRotation").quaternionValue = bone.transform.localRotation;
					element.FindPropertyRelative("localScale").vector3Value = bone.transform.localScale;
				}
			}

			poseSO.ApplyModifiedProperties();
            


            List<Ik2D> iks = new List<Ik2D>(50);

            root.GetComponentsInChildren<Ik2D>(true, iks);

            SerializedObject poseS1 = new SerializedObject(pose);
            SerializedProperty entriesProp1 = poseS1.FindProperty("m_IkEntries");

            poseS1.Update();
            entriesProp1.arraySize = iks.Count;

            for (int i = 0; i < iks.Count; i++)
            {
                Ik2D ik = iks[i];

                if (ik)
                {
                    SerializedProperty element = entriesProp1.GetArrayElementAtIndex(i);
                    element.FindPropertyRelative("path").stringValue = IkUtils.GetIkPath(root, ik);
                    element.FindPropertyRelative("localPosition").vector3Value = ik.transform.localPosition;
                    element.FindPropertyRelative("localRotation").quaternionValue = ik.transform.localRotation;
                    element.FindPropertyRelative("localScale").vector3Value = ik.transform.localScale;
                }
            }

            poseS1.ApplyModifiedProperties();
        }

        public static void LoadPose(Pose pose, Transform root)
		{
			SerializedObject poseSO = new SerializedObject(pose);
			SerializedProperty entriesProp = poseSO.FindProperty("m_BoneEntries");

			List<Ik2D> iks = new List<Ik2D>();

			for (int i = 0; i < entriesProp.arraySize; i++)
			{
				SerializedProperty element = entriesProp.GetArrayElementAtIndex(i);

				Transform boneTransform = root.Find(element.FindPropertyRelative("path").stringValue);

				if(boneTransform)
				{
					Bone2D boneComponent = boneTransform.GetComponent<Bone2D>();

					if(boneComponent && boneComponent.attachedIK && !iks.Contains(boneComponent.attachedIK))
					{
						iks.Add(boneComponent.attachedIK);
					}

					Undo.RecordObject(boneTransform,"Load Pose");

					boneTransform.localPosition = element.FindPropertyRelative("localPosition").vector3Value;
					boneTransform.localRotation = element.FindPropertyRelative("localRotation").quaternionValue;
					boneTransform.localScale = element.FindPropertyRelative("localScale").vector3Value;
				}
			}

			/* 현재 불필요한 코드
            for (int i = 0; i < iks.Count; i++)
			{
				Ik2D ik = iks[i];

				if(ik && ik.target)
				{
					Undo.RecordObject(ik.transform,"Load Pose");

					ik.transform.position = ik.target.endPosition;

					if(ik.orientChild && ik.target.child)
					{
						ik.transform.rotation = ik.target.child.transform.rotation;
					}
				}
			}
            */



            SerializedProperty entriesProp1 = poseSO.FindProperty("m_IkEntries");

            for (int i = 0; i < entriesProp1.arraySize; i++)
            {
                SerializedProperty element = entriesProp1.GetArrayElementAtIndex(i);

                Transform ikTransform = root.Find(element.FindPropertyRelative("path").stringValue);

                if (ikTransform)
                {
                    Ik2D ikComponent = ikTransform.GetComponent<Ik2D>();

                    Undo.RecordObject(ikTransform, "Load Pose");

                    ikTransform.localPosition = element.FindPropertyRelative("localPosition").vector3Value;
                    /*아래 스케일과 로테이션은 테스트후 제거*/
                    ikTransform.localRotation = element.FindPropertyRelative("localRotation").quaternionValue;
                    ikTransform.localScale = element.FindPropertyRelative("localScale").vector3Value;
                }
            }
            EditorUpdater.SetDirty("Load Pose");
		}
	}
}
