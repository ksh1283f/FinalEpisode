using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace CrusadersQuestReplica
{
    public class ContextMenu
    {
        [MenuItem("Assets/Create/CrusadersQuest/Skin")]
        public static void CreateSkin(MenuCommand menuCommand)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (System.IO.File.Exists(path))
            {
                path = System.IO.Path.GetDirectoryName(path);
            }

            path += "/";

            if (System.IO.Directory.Exists(path))
            {
                path += "New pose.asset";

                ScriptableObjectUtility.CreateAsset<Skin>(path);
            }
        }
    }
}
