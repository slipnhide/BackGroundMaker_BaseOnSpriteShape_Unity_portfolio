using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
//[RequireComponent(typeof(LineRenderer))]

enum Addtype { InsertStart,InsertEnd,InsertMid};
struct Editinfo
{
   public Addtype addtype;
    public int StartPoint;
    public int EndPoint;
}


public class GetLength : MonoBehaviour
{
    List<Vector3> vectors=new List<Vector3>();
    List<Vector3> LastPoints=new List<Vector3>();
   // LineRenderer line;
    [Range(0.05f,1)]
    public float Range = 0.5f;
    int index;
    public bool Debuging;
    public bool withLine;
    private void awake()
    {
       // line = GetComponent<LineRenderer>();
    }

    void Line()
    {

        if (withLine)
        {
            GetComponent<LineRenderer>().enabled = true;
            GetComponent<LineRenderer>().positionCount = vectors.Count;

            for (int i = 0; i < vectors.Count; i++)
            {
                GetComponent<LineRenderer>().SetPosition(i, vectors[i]);
                // Debug.Log(vectors[i]);
            }
        }
        else
        {
            GetComponent<LineRenderer>().enabled = false;
            for (int i = 0; i < vectors.Count; i++)
                Debug.DrawRay(vectors[i], Vector3.up * 0.1f);
        }
    }
    void GetDiffPoint(SpriteShapeController shape, int index1, int index2)
    {
        Editinfo editinfo = new Editinfo();
        int[] value = new int[4];
        bool SameStart = false; // 참이라면 이전 포인트중 가운데에서 시작 ,거짓이라면 추가해서 들어가야함
        int InStartPoint = 0;
        bool SameEnd = false; //참이라면 포인트 이후 부분 삭제, 거짓이라면 기존 크기에서 뒤에 추가
        int InEndPoint = 0;
        //둘다 거짓이라면 아예 다른 범위니 새로 시작

        //처음 같은점 찾기-탄젠트 포함( 다른점 이전기록은 전부 날림)
        // LastPoints.Find
        List<Vector3> now = null;
        now = GetsplineList(now, shape, index1, index2);

        if (LastPoints.Contains(now[0]))
        {
            InStartPoint = LastPoints.IndexOf(now[0]);
            if (InStartPoint == 0)
                SameStart = true;
            else
            SameStart = false;
        }
        else
            SameStart = false;
        if (LastPoints.Contains(now[now.Count-1]))
        {
            InEndPoint = LastPoints.IndexOf(now[now.Count - 1]);
            if (InEndPoint == (now.Count - 1))
                SameEnd = true;
            else
                SameEnd = false;
        }
        else
            SameEnd = false;

        if (SameStart && SameEnd) //중간이 바뀐경우
        {
            editinfo.addtype = Addtype.InsertMid;
            editinfo.StartPoint=0;
        }




        // 다른점 찾기

    }
    List<Vector3> GetsplineList(List<Vector3> list, SpriteShapeController shape, int index1, int index2)
    {
        if (index1 < 0 || index2 < 0)
        {
            Debug.Log("GetsplineList.Error:index1 < 0 || index2 < 0 :  " + "index1 :  " + index1 + "index2 :  " + index2);
            return new List<Vector3>();
        }
        else if (index1 == index2)
        {
            Debug.Log("GetsplineList.Error:index1 == index2 :  " + "index1 :  " + index1 + "index2 :  " + index2);
            return new List<Vector3>();
        }
        else if (index1 > index2)
        {
            int temp = index2;
            index2 = index1;
            index1 = temp;
        }

        list.Clear();
        Spline spline=shape.spline;
        Vector3 Base= shape.transform.position;

        list.Add(spline.GetPosition(index1) + Base);
        if (spline.GetTangentMode(index1) != ShapeTangentMode.Linear)
        {
            list.Add(spline.GetRightTangent(index1) + spline.GetPosition(index1) + Base);
        }
        index1++;
        //Debug.Log("셋팅:" );
        for (; index1 < index2; index1++)
        {
            Debug.Log(index1);
            if (spline.GetTangentMode(index1) != ShapeTangentMode.Linear)
                list.Add(spline.GetLeftTangent(index1) + spline.GetPosition(index1) + Base);
            list.Add(spline.GetPosition(index1) + Base);

            GetBezier(list);
            list.Clear();

            list.Add(spline.GetPosition(index1) + Base);
            if (spline.GetTangentMode(index1) != ShapeTangentMode.Linear)
                list.Add(spline.GetRightTangent(index1) + Base + spline.GetPosition(index1) + Base);
        }

        if (spline.GetTangentMode(index2) != ShapeTangentMode.Linear)
            list.Add(spline.GetLeftTangent(index2) + spline.GetPosition(index2) + Base);
        list.Add(spline.GetPosition(index2) + Base);


        return list;
    }

    public void Set(SpriteShapeController shape, int index1,int index2)
    {
        vectors.Clear();
        index = 0;
        List<Vector3> Bezier=new List<Vector3>();
        Spline spline = shape.spline;
        Vector3 Base = shape.transform.position;
       //Debug.Log("인덱스 1:"+index1+"  2:"+ index2);
        if (index1 < 0 || index2 < 0)
            return;
        else if (index1 == index2)
            return;
        else if (index1 > index2)
        {
            int temp = index2;
            index2 = index1;
            index1 = temp;
        }

       // vectors.Add(spline.GetPosition(index1) + Base);
        Bezier.Add(spline.GetPosition(index1) + Base);
        if (spline.GetTangentMode(index1) != ShapeTangentMode.Linear)
        {
            Bezier.Add(spline.GetRightTangent(index1) + spline.GetPosition(index1) + Base);
        }
        index1++;
        //Debug.Log("셋팅:" );
        for (; index1 < index2; index1++)
        {
            Debug.Log(index1);
            if(spline.GetTangentMode(index1) != ShapeTangentMode.Linear)
                Bezier.Add(spline.GetLeftTangent(index1) + spline.GetPosition(index1) + Base);
            Bezier.Add(spline.GetPosition(index1) + Base);

            GetBezier(Bezier);
            Bezier.Clear();
            
            Bezier.Add(spline.GetPosition(index1) + Base);
            if (spline.GetTangentMode(index1) != ShapeTangentMode.Linear)
                Bezier.Add(spline.GetRightTangent(index1) + Base+spline.GetPosition(index1) + Base);
        }
        
        if (spline.GetTangentMode(index2) != ShapeTangentMode.Linear)
            Bezier.Add(spline.GetLeftTangent(index2) + spline.GetPosition(index2) + Base);
        Bezier.Add(spline.GetPosition(index2) + Base);

        GetBezier(Bezier);
        Bezier.Clear();
        // Debug.Log("index1: " + index1 + " index2:" + index2);

        //Debug.Log("리스트 수:"+vectors.Count);
        if(Debuging)
        Line();
        Bezier = null;
    }
    void GetBezier(List<Vector3> list)
    {
        //vectors = new List<Vector3>();

        //Debug.Log("Bezier");
        for (float i = 0; i < 1+Range; i += Range)
        {
            vectors.Add( GetVector3(list,i));
            Debug.Log("index: "+index+" i: "+i);
            index++;
        }
    }
    int[] Combination(int a)
    {
        //int[] value = new int[4];
        //int[] _2 = new int[2] { 1,1};
        //int[] _3 = new int[3] { 1,2, 1 };
        //int[] _4 = new int[4] { 1,3,3, 1 };
        //switch (a)
        //{
        //    case 2:
        //        value = _2;
        //        break;
        //    case 3:
        //        value = _3;
        //        break;
        //    case 4:
        //        value = _4;
        //        break;
        //}
        int[] value = null;

        switch (a)
        {
            case 2:
                value = new int[2] { 1, 1 };
                break;
            case 3:
                value = new int[3] { 1, 2, 1 };
                break;
            case 4:
                value = new int[4] { 1, 3, 3, 1 };
                break;
        }


        return value;
    }
    //int Factorial()

    Vector3 GetVector3(List<Vector3> list,float t)
    {
        Debug.Log("GetVector3");
        Vector3 value;
        value = Vector3.zero;

        if (t > 1 || t < 0)
        {
            Debug.Log("t값 오류");
            t = Mathf.Clamp(t, 0, 1);
        }
        
        
        //0~x개의 값이 점이 있을때 
        //P.(x-m)*(t-1)^m * t^(x-m)*파스칼 삼각형 계수 을 m=x 부터 m=0 까지 더해주면 베지어 곡선의 B(t)
        //ex)3차 베지어 곡선(점4개) B(t)=P0(1-T)^3 + 3*p1*T*(1-T)^2 + 3*p2*T^2*(1-T)^1+P3(T)^3
        int m = list.Count;
        int[] coefficient = new int[4];
        coefficient = Combination(list.Count);
        Debug.Log("list.Count: " + list.Count + "  coefficient.count : " + coefficient.Length);
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log("i: "+i);
            value += (
                list[i] * coefficient[i] * Mathf.Pow(1 - t, m - 1 - i) * Mathf.Pow(t, i)
                ) ;
        }
        coefficient = null;

        Debug.Log("value: "+ value);
        Debug.Log("GetVector3_End");

        return value;
    }
    List<Vector3> find(float need)
    {
        float distance=0;
        List<Vector3> value=null;
        {
            float d;
            for (int i = 0; i < vectors.Count-1; i++)
            {
                d = need - distance;
                if (need<0)
                {
                    Vector3 way = vectors[i + 1] - vectors[i];
                    value.Add(vectors[i] + way.normalized * d);
                }
                distance += Vector3.Distance(vectors[i + 1], vectors[i]);

            }
        };
        return value;
    }

}
