using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class Test : MonoBehaviour
{
    public SpriteShapeController spriteShapeController;
    public int index;
    public bool useNormals = false;
    public bool runtimeUpdate = false;
    [Header("Offset")]
    public float yoffset = 0.0f;
    public bool localoffset = false;
    private Spline spline;
    int LastSpritePointCount;
    bool LastUseNomals;
    private Vector3 LastPosition;

    private void Awake()
    {
        spline = spriteShapeController.spline;
    }

    // Update is called once per frame
    void Update()
    {
        //게임 실행중이 이거나 런타임셋이 true일때
        if (EditorApplication.isPlaying || runtimeUpdate)
        {
            spline = spriteShapeController.spline;
            if ((spline.GetPointCount() != 0) && (LastSpritePointCount != 0))
            {
                index = Mathf.Clamp(index, 0, spline.GetPointCount() - 1);
                if (spline.GetPointCount() != LastSpritePointCount)
                {
                    if (spline.GetPosition(index) != LastPosition)
                    {
                        index += spline.GetPointCount() - LastSpritePointCount;
                        //index=index+spline.GetPointCount()-LastSpritePointCount
                    }
                }

                if ((index <= spline.GetPointCount() - 1) && (index >= 0))
                {
                    if (useNormals)
                    {
                        if (spline.GetTangentMode(index) != ShapeTangentMode.Linear)
                        {
                            Vector3 Lt = Vector3.Normalize(spline.GetLeftTangent(index) - spline.GetRightTangent(index));
                            Vector3 Rt = Vector3.Normalize(spline.GetRightTangent(index) - spline.GetLeftTangent(index));

                            float a = Angle(Vector3.left, Lt);
                            float b = Angle(Lt, Rt);
                            float c = a + (b * 0.5f);
                            if (b > 0)
                                c = (180 + c);
                            transform.rotation = Quaternion.Euler(0, 0, c);
                        }
                    }
                    else
                        transform.rotation = Quaternion.Euler(0, 0, 0);

                    Vector3 offsetVector;
                    if (localoffset)
                    {
                        offsetVector = (Vector3)Rotate(Vector2.up, transform.localEulerAngles.z) * yoffset;
                    }
                    else
                    {
                        offsetVector = Vector2.up * yoffset;
                    }
                    transform.position = spriteShapeController.transform.position + spline.GetPosition(index) + offsetVector;
                    LastPosition = spline.GetPosition(index);

                }

            }
        }
        LastSpritePointCount = spline.GetPointCount(); 
    }
    float Angle(Vector3 a, Vector3 b)
    {
        float dot = Vector3.Dot(a, b);
        float det = (a.x * b.y) - (b.x * a.y);

        return Mathf.Atan2(det, dot) * Mathf.Rad2Deg;
    }
    Vector2 Rotate(Vector2 v, float degrees)
    {

        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
}







