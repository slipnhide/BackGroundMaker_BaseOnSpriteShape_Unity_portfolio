using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
[CustomEditor(typeof(Sprite_Shape_Master))]
public class Sprite_Shape_Master_Edit : Editor
{
    Sprite_Shape_Master Control;
    bool fade;
    // SpriteShapeController sprite_Shape;
    // Spline spline;
    //float test=0.1f;

    private void OnEnable()
    {
        //Debug.Log("OnEnable");
        Control = target as Sprite_Shape_Master;
        Control.spriteShape = Control.GetComponent<SpriteShapeController>();
        Control.Spline = Control.spriteShape.spline;
        if(Control.Bezies.Count==0)
        Control.MakeBezies(0, Control.Spline.GetPointCount() - 1);

    }
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        GUILayout.Label("Please,Keep Open it");
            Control.Checkspline();
        EditorUtility.SetDirty(target);
        {
            fade = EditorGUILayout.BeginFoldoutHeaderGroup(fade, "옵션보기");
            if (fade)
            {
                GUILayout.Space(10);
                Control.layers_Master = ((Layers_master)EditorGUILayout.ObjectField(Control.layers_Master, typeof(Layers_master)));
                Control.t_precision = Mathf.Clamp(EditorGUILayout.FloatField("정밀도", Control.t_precision), 0.001f, 0.5f);
                Control.IsOffsetWorldOffset = EditorGUILayout.Toggle("IsOffsetWorldOffset", Control.IsOffsetWorldOffset);
                Control.offset = EditorGUILayout.Vector3Field("offset", Control.offset);
                Control.scale = EditorGUILayout.Vector3Field("Scale", Control.scale);
                Control.IsRotate = EditorGUILayout.Toggle("IsRotate", Control.IsRotate);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}