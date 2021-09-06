using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(OneLayers))]
public class OneLayers_test : Editor
{
    GameObject forAddPrefab;
    Sprite forAddSprite;
    Vector2 Scroll_PreFabs_Choose=Vector2.zero;
    Vector2 Scroll_Sprite_Choose = Vector2.zero;
    OneLayers one;
    private void OnEnable()
    {
        one = target as OneLayers;
    }
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
    }
}
