using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Transform))]
public class OneObject : MonoBehaviour
{
    public bool IsStand;
    public float Range;
    public float Distance;
    public Vector3 offset;
    public int SortLayer;
    public int OrderLayer;
    public Vector3 Last;

    public void MadeObject()
    {
    }

}
