using UnityEngine;
using System;

[CreateAssetMenu(fileName = "TestItemArray", menuName = "Scriptable Objects/TestItemArray")]
public class TestItemArray : ScriptableObject
{
    public TestItem[] items;
}
