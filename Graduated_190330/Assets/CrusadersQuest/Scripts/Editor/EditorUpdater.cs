using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Anima2D;

namespace CrusadersQuestReplica 
{
	[InitializeOnLoad][ExecuteInEditMode]
	public class EditorUpdater
	{
		static bool s_Dirty = true;
		static string s_UndoName = "";
		static bool s_DraggingATool = false;
		static List<SpriteMeshInstance> s_SMI = new List<SpriteMeshInstance>();
		static bool s_InAnimationMode = false;
		static float s_OldAnimationTime = 0f;
		static float s_LastUpdate = 0f;
		static int s_LastNearestControl = -1;

		static EditorUpdater()
		{
			EditorApplication.update += Update;
			SceneView.onSceneGUIDelegate += OnSceneGUI;
			EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;

			Undo.undoRedoPerformed += UndoRedoPerformed;
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		static void HierarchyWindowChanged()
		{
			s_SMI = GameObject.FindObjectsOfType<SpriteMeshInstance>().ToList();
		}

		static void UndoRedoPerformed()
		{
			/*foreach(Bone2D bone in s_Bones)
			{
				if(bone)
				{
					bone.attachedIK = null;
				}
			}*/

			SetDirty();

			EditorApplication.delayCall += () => { SceneView.RepaintAll(); };
		}

		public static void SetDirty()
		{
			SetDirty("");
		}

		public static void SetDirty(string undoName)
		{
			s_UndoName = undoName;
			s_Dirty = true;
		}

		public static void Update(string undoName, bool record)
		{
			List<SpriteMeshInstance> updatedIKs = new List<SpriteMeshInstance>();

			/*for (int i = 0; i < s_SMI.Count; i++)
			{
                SpriteMeshInstance smi = s_SMI[i];
				
				if(smi && !updatedIKs.Contains(smi))
				{
					List<SpriteMeshInstance> smiList = IkUtils.UpdateIK(smi,undoName,record);

					if(smiList != null)
					{
						updatedIKs.AddRange(smiList);
						updatedIKs = updatedIKs.Distinct().ToList();
					}
				}
			}*/
		}

		static void AnimationModeCheck()
		{
			if(s_InAnimationMode != AnimationMode.InAnimationMode())
			{
				SetDirty();
				s_InAnimationMode = AnimationMode.InAnimationMode();
			}
		}

		static void AnimationWindowTimeCheck()
		{
			float currentAnimationTime = AnimationWindowExtra.currentTime;
			
			if(s_OldAnimationTime != currentAnimationTime)
			{
				SetDirty();
			}
			
			s_OldAnimationTime = currentAnimationTime;
		}

		static void OnSceneGUI(SceneView sceneview)
		{
			if(!s_DraggingATool &&
			   GUIUtility.hotControl != 0 &&
			   !ToolsExtra.viewToolActive)
			{
				s_DraggingATool = Event.current.type == EventType.MouseDrag;
			}

			Anima2D.Gizmos.OnSceneGUI(sceneview);

			if(s_LastNearestControl != HandleUtility.nearestControl)
			{
				s_LastNearestControl = HandleUtility.nearestControl;
				SceneView.RepaintAll();
			}
		}

		static void OnLateUpdate()
		{
			if(AnimationMode.InAnimationMode())
			{
				SetDirty();

				UpdateSprites();
			}
		}

		static void Update()
		{
			EditorUpdaterProxy.Instance.onLateUpdate -= OnLateUpdate;
			EditorUpdaterProxy.Instance.onLateUpdate += OnLateUpdate;

			if(s_DraggingATool)
			{	
				s_DraggingATool = false;

				string undoName = "Move";

				if(Tools.current == Tool.Rotate) undoName = "Rotate";
				if(Tools.current == Tool.Scale) undoName = "Scale";
                
				for (int i = 0; i < Selection.transforms.Length; i++)
				{
					Transform transform = Selection.transforms [i];
					Control control = transform.GetComponent<Control> ();
					if(control && control.isActiveAndEnabled && control.bone)
					{
						Undo.RecordObject(control.bone.transform,undoName);
						
						control.bone.transform.position = control.transform.position;
						control.bone.transform.rotation = control.transform.rotation;
						
						BoneUtils.OrientToChild(control.bone.parentBone,false,undoName,true);
					}

					Ik2D ik2D = transform.GetComponent<Ik2D>();
					if(ik2D && ik2D.record)
					{
						IkUtils.UpdateIK(ik2D,undoName,true);
					}
				}

				SetDirty();
			}

			AnimationModeCheck();
			AnimationWindowTimeCheck();

			//IkUtils.UpdateAttachedIKs(s_SMI);

			UpdateSprites();
		}

		static void UpdateSprites()
		{
			if(!s_Dirty)
			{
				return;
			}

			if(s_LastUpdate == Time.realtimeSinceStartup)
			{
				return;
			}

			Update(s_UndoName,false);
			
			s_Dirty = false;
			s_UndoName = "";
			s_LastUpdate = Time.realtimeSinceStartup;
		}
	}
}
