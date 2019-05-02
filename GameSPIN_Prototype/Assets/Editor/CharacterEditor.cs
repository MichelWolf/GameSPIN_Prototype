using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Character character = (Character)target;
        if (GUILayout.Button("Take Damage"))
        {
            character.TakeDamage(20);
        }
    }
}
