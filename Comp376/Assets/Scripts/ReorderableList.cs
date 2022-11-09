using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// In Unity pre 2020, each different type you want to make a list from needs its own subclass, like UnityEvents.
/// eg :
/// [Serializable] public class IntReorderableList : ReorderableList<int> {}

[Serializable]
public class ReorderableList<T> {
    public List<T> list;
}