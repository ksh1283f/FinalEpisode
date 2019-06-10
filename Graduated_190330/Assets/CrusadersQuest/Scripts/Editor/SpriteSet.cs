using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Anima2D;

namespace CrusadersQuestReplica
{
    public class SpriteSet : ScriptableObject
    {
        [Serializable]
        public class SkinEntry
        {
            public string path;
            public SpriteMesh spriteMesh;
        }

        [SerializeField]
        List<SkinEntry> m_SkinEntry;
    }
}
