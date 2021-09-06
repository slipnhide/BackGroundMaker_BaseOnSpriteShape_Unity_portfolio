using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class OneLayer : MonoBehaviour
{
    public List<OneObject> oneObjects;
    public Vector3 Base;
    int Layer;
    float Distance;
    public Vector3 Start;
    public Vector3 End;
    public List<Vector3> vector3s;
    public Vector3 Way;
    public List<GameObject> ThisLayers;

    void Initialize()
    {
        Start = vector3s[0];
        End = vector3s[vector3s.Count - 1];
    }
    void Make()
    {
        Vector3 cur = Start;
        for (int i = 0; i < vector3s.Count-1; i++)
        {
            Way = vector3s[i + 1] - cur;
            cur = vector3s[i + 1];

            for (int t = 0; t < oneObjects.Count; i++)
            {
                OneObject oneObject = oneObjects[t].GetComponent<OneObject>();
                if (oneObject.Distance - oneObject.Range > Way.magnitude)
                {
                    GameObject go = new GameObject("temp");
                    ThisLayers.Add(go);
                   // go.transform.position=oneObject.Last
                }
            }

        }
    }

}
