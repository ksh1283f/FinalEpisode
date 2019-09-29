using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAugmenterData {
    public int Id { get; private set; }
    public int Level { get; private set; }
    public int Augmenter { get; private set; }

    public StatAugmenterData (int id, int level, int augmenter) {
        Id = id;
        Level = level;
        Augmenter = augmenter;
    }
}