using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Anima2D;

namespace CrusadersQuestReplica
{
    public class Skin : ScriptableObject
    {
        [Serializable]
        public class SkinEntry
        {
            public string path;
            public SpriteMesh skin;
        }

        [SerializeField]
        List<SkinEntry> m_SkinEntries;
        [SerializeField]
        List<SkinEntry> m_FaceEntries;
    }
}
