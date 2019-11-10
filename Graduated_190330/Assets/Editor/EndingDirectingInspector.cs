using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EndingDirecting))]
public class EndingDirectingInspector : Editor
{
    EndingDirecting endingDirecting = null;
    private void OnEnable()
    {
        endingDirecting = (EndingDirecting)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        endingDirecting.SuccessDataIndex = EditorGUILayout.IntSlider(endingDirecting.SuccessDataIndex, 0, endingDirecting.SuccessDataList.Count);
        EditorGUILayout.BeginHorizontal();
        {
            if(GUILayout.Button("Insert SuccessData at end"))
            {
                endingDirecting.InsertSuccessData();
            }

            if (GUILayout.Button("Insert SuccessData at dataIndex"))
            {
                endingDirecting.InsertFailedDataAtDataIndex();
            }

            if(GUILayout.Button("Remove SuccessData"))
            {
                if(EditorUtility.DisplayDialog("유저 메세지",
                    string.Concat(endingDirecting.SuccessDataIndex,"번째 데이터를 정말로 삭제하시겠습니까?\nDataType: ",endingDirecting.SuccessDataList[endingDirecting.SuccessDataIndex].DirectingDataType),
                    "예","아니오"))
                {
                    endingDirecting.RemoveSuccessData();
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        endingDirecting.FailedDataIndex = EditorGUILayout.IntSlider(endingDirecting.FailedDataIndex, 0, endingDirecting.FailedDataList.Count);
        EditorGUILayout.BeginHorizontal();
        {
            if(GUILayout.Button("Insert FailedData at end"))
            {
                endingDirecting.InsertFailedData();
            }

            if (GUILayout.Button("Insert FailedData at dataIndex"))
            {
                endingDirecting.InsertFailedDataAtDataIndex();
            }

            if(GUILayout.Button("Remove FailedData"))
            {
                if(EditorUtility.DisplayDialog("유저 메세지",
                    string.Concat(endingDirecting.FailedDataIndex,"번째 데이터를 정말로 삭제하시겠습니까?\nDataType: ",endingDirecting.FailedDataList[endingDirecting.FailedDataIndex].DirectingDataType),
                    "예","아니오"))
                {
                    endingDirecting.RemoveFailedData();
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        endingDirecting.AnotherSuccessDataIndex = EditorGUILayout.IntSlider(endingDirecting.AnotherSuccessDataIndex, 0, endingDirecting.AnotherSuccessDataList.Count);
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Insert anotherData at end"))
            {
                endingDirecting.InsertAnotherData();
            }

            if (GUILayout.Button("Insert anotherData at dataIndex"))
            {
                endingDirecting.InsertAnotherDataAtDataIndex();
            }

            if (GUILayout.Button("Remove anotherData"))
            {
                if (EditorUtility.DisplayDialog("유저 메세지",
                    string.Concat(endingDirecting.FailedDataIndex, "번째 데이터를 정말로 삭제하시겠습니까?\nDataType: ", endingDirecting.FailedDataList[endingDirecting.FailedDataIndex].DirectingDataType),
                    "예", "아니오"))
                {
                    endingDirecting.RemoveAnotherdData();
                }
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}