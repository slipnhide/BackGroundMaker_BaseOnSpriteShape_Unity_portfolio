using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
public struct Bezie
{
    public List<Vector3> vector3s { get; set; }
    public float Range;
}
public struct LSpline
{
    public List<Vector3> Tangent;
    public Vector3 Point;
    public bool IsTangent;
}
[System.Serializable]
public class Sprite_Shape_Master : MonoBehaviour
{
    
    //기본 설정 변수들
    public Vector3 scale =Vector3.one;
    public Vector3 offset=Vector3.zero;
    public bool IsOffsetWorldOffset=false;
    public bool IsRotate = true;
    public float t_precision = 0.01f;
    public float StartDistance = 0f;
    
    //관리용 변수
    Vector3 Lastscale = Vector3.one;
    Vector3 Lastoffset = Vector3.zero;
    bool LastIsOffsetWorldOffset= false;
    bool LastIsRotate = true;
    float Last_t_precision = 0.5f;
    float LastStartDistance = 0f;

    //디버깅용 변수
    //public GameObject Prefab;
    //public bool Test_;
    //public bool LineTest;
    //public Transform[] test;

    //내부관리 변수
    public SpriteShapeController spriteShape;
    public Spline Spline;
    public List<Bezie> Bezies = new List<Bezie>();
    
    //움직임 체크용
    public List<LSpline> LastSpline = new List<LSpline>();
    public Vector3[] Lasttransform=new Vector3[3];
    // public OneLayers oneLayers;
    public Layers_master layers_Master;



    //포인터의 움직임 확인-SetLastSpline- 연결 하여 확인위한 LastSpline 업데이트  -변경된 포인터 존재할경우 부분을 MakeBezies로 베지에 함수 업데이트
    public void Checkspline()
    {
        if (layers_Master == null && !(GetComponent<Layers_master>()==null))
            layers_Master = GetComponent<Layers_master>();

        if (LastSpline.Count != 0)
            //라인은 베지에 를 기반으로 움직임으로 베지에 기반으로 변경(회전에 대해서는 추후에 수정-수정시 베지에 생성에도 변경해야할수도-베지에리스트는 월드좌표를 기준으로 움직입니다.)
            if (Lasttransform[0] != transform.position)
            {
                for (int i = 0; i < Bezies.Count; i++)
                {
                    for (int k = 0; k < Bezies[i].vector3s.Count; k++)
                        Bezies[i].vector3s[k] += (transform.position- Lasttransform[0]);
                }
                layers_Master.ChgPosition_call();
            }
            else if (LastSpline.Count > spriteShape.spline.GetPointCount())
            {
                //삭제된경우
                Debug.Log("spline Is Deleted ");
                int Deletepoint = -1;
                for (int i = 0; i < spriteShape.spline.GetPointCount(); i++)
                {
                    if (LastSpline[i].Point != Spline.GetPosition(i))
                    {
                        Debug.Log("Deleted Point: " + i);
                        Deletepoint = i;
                        break;
                    }
                }

                //가장 끝번호가 삭제된 경우
                if (Deletepoint == -1)
                {
                    Deletepoint = LastSpline.Count - 1;
                    Debug.Log("Deleted Point: " + Deletepoint);
                    //Debug.Log("삭제전 Bezies.Count:  " + Bezies.Count);
                    Bezies.RemoveAt(Deletepoint - 1);
                    //Debug.Log("삭제후 Bezies.Count:  " + Bezies.Count);
                }
                //그외에 삭제 -양끝은 Bezies새로 설정할 필요없음
                else
                {
                    Debug.Log("Deleted Point: " + Deletepoint);
                    Bezies.RemoveAt(Deletepoint);
                    if (Deletepoint != 0)
                        MakeBezies(Deletepoint - 1, Deletepoint);

                }
                SetLastSpline();
                layers_Master.DeletedPointedAt(Deletepoint);
                //layers_Master.ChgPosition_call();

            }
            else if (LastSpline.Count < spriteShape.spline.GetPointCount())
            {
                //추가시
                //0이 새로 생기지는 않는다고 가정
                int InsertPoint = -1;
                for (int i = 0; i < LastSpline.Count; i++)
                {
                    if (LastSpline[i].Point != Spline.GetPosition(i))
                    {
                        InsertPoint = i;
                        Debug.Log("InsertPoint: " + i);
                        break;
                    }
                }
                //가장 끝에 새로 생성된경우
                if (InsertPoint == -1)
                {
                    Bezies.Add(new Bezie());
                    MakeBezies(Spline.GetPointCount() - 2, Spline.GetPointCount()-1);
                    Debug.Log("InsertPoint: " + (Spline.GetPointCount() - 1));
                }
                //중간에 생긴경우
                else
                {
                    Bezies.Insert(InsertPoint, new Bezie());
                    MakeBezies(InsertPoint - 1, InsertPoint + 1);
                }
                Debug.Log("spline Is Add ");
                layers_Master.InsertPointedAt(InsertPoint);
                //layers_Master.ChgPosition_call();
            }
            else
            {
                //포인트의 새 생성이나 삭제가 아니라면 포인트의 움직임이나 탄젠트의 모드변경 이나 움직임 확인
                for (int i = 0; i < LastSpline.Count; i++)
                {
                    if (LastSpline[i].IsTangent != (Spline.GetTangentMode(i) != ShapeTangentMode.Linear))
                    {
                        Debug.Log("탄젠트 모드 바뀜" + " 위치: " + i);
                        if (i == 0)
                            MakeBezies(0, i + 1);
                        else if (i == LastSpline.Count - 1)
                            MakeBezies(i - 1, i);
                        else
                            MakeBezies(i - 1, i + 1);
                        layers_Master.ChgPosition_call();
                        break;
                    }
                    else
                    {
                        //포인트 위치 바뀌엇는지 확인
                        if (LastSpline[i].Point != Spline.GetPosition(i))
                        {
                           // Debug.Log("Point 움직임" + " 위치: " + i);
                            if (i == 0)
                                MakeBezies(0, i + 1);
                            else if (i == LastSpline.Count - 1)
                                MakeBezies(i - 1, i);
                            else
                                MakeBezies(i - 1, i + 1);
                            layers_Master.ChgPosition_call();
                            break;
                        }
                        //탄젠트 모드라면 양쪽확인
                        if (LastSpline[i].IsTangent)
                        {
                            if ((LastSpline[i].Tangent[0] != Spline.GetLeftTangent(i))
                                ||(LastSpline[i].Tangent[1] != Spline.GetRightTangent(i)) )
                            {
                                //Debug.Log("Tangent 움직임" + " 위치: " + i);
                                if (i == 0)
                                    MakeBezies(0, i + 1);
                                else if (i == (LastSpline.Count - 1))
                                    MakeBezies(i - 1, i);
                                else
                                {
                                    Debug.Log("Tangent 움직임" + " 위치: " + i);
                                    MakeBezies(i - 1, i + 1);
                                }
                                layers_Master.ChgPosition_call();
                                break;
                            }
                        }
                    }
                }

            }

        if (Lastscale != scale)
        {
            Lastscale = scale;
            layers_Master.ChgPosition_call();
        }
        if (Lastoffset != offset)
        {
            Lastoffset = offset;
            layers_Master.ChgPosition_call();
        }
        if (LastIsOffsetWorldOffset != IsOffsetWorldOffset)
        {
            LastIsOffsetWorldOffset = IsOffsetWorldOffset;
            layers_Master.ChgPosition_call();
        }
        if (LastIsRotate != IsRotate)
        {
            LastIsRotate = IsRotate;
            layers_Master.ChgPosition_call();
        }
        if (Last_t_precision != t_precision)
        {
            Last_t_precision = t_precision;
            layers_Master.ChgPosition_call();
        }
        if (LastStartDistance != StartDistance)
        {
            LastStartDistance = StartDistance;
            layers_Master.ChgPosition_call();
        }


        Set_LastTransform();
        SetLastSpline();
        

    }
    public void SetLastSpline()
    {
        // Debug.Log("SetLastSpline");
        LSpline spline = new LSpline();
        spline.Tangent = new List<Vector3>();
        spline.Point = new Vector3();
        LastSpline.Clear();
        for (int i = 0; i < Spline.GetPointCount(); i++)
        {
            spline.Point = Spline.GetPosition(i);
            if (Spline.GetTangentMode(i) != ShapeTangentMode.Linear)
            {
                spline.IsTangent = true;
                spline.Tangent.Add(Spline.GetLeftTangent(i));
                spline.Tangent.Add(Spline.GetRightTangent(i));
            }

            LastSpline.Add(spline);
            spline = new LSpline();
            spline.Tangent = new List<Vector3>();
            spline.Point = new Vector3();
        }
        // Debug.Log("LastSpline.Count " + LastSpline.Count);
        //  Check();
    }
    void Set_LastTransform()
    {
        //Lasttransform = new Vector3[3];
        Lasttransform[0] = transform.position;
        Lasttransform[1] = transform.rotation.eulerAngles;
        Lasttransform[2] = transform.localScale;
    }

    //현제베지에 리스트가 0일경우는 생성 그 이후는 직접 대입 - 리스트의 추가 삭제는 Checkspline진행후 넘어옴
    public void MakeBezies(int S, int E)
    {
        bool IsZero;
        {
            if (Bezies.Count == 0)
            {
                IsZero = true;
            }
            else IsZero = false;

            if (Spline.GetPointCount() <= E)
            {
                Debug.Log("오류" + "Spline.GetPointCount(): " + Spline.GetPointCount() + "  S:  " + S + "  E:" + E);
                return;
            }
        }

        Bezie bezie = new Bezie();
        bezie.vector3s = new List<Vector3>();
        //기존에 벡터화 한 베지에곡선의 리스트가 존재하는지 안하는지 확인후
        //없다면 추가하면서 넣고 아니라면 변경
        if (IsZero)
        {
            for (int i = S; i < E; i++)
            {
                Bezies.Add(new Bezie());
                bezie = Bezies[i];
                bezie = MakeBezie(i, bezie);
                Bezies[i] = bezie;
            }
        }
        else
        {
            for (int i = S; i < E; i++)
            {
                bezie = Bezies[i];
                bezie = MakeBezie(i, bezie);
                Bezies[i] = bezie;

            }
        }

        Bezie MakeBezie(int i, Bezie _bezie)
        {
            // i번째 스플라인과 i+1스플라인을 베지에 곡선좌표로 저장합니다.
            if (i >= Spline.GetPointCount())
                Debug.LogError("MakeBezie_잘못된 인덱스: i: " + i);
            _bezie.vector3s = new List<Vector3>();
            //스프라이트 쉐이프의 기본 위치
            Vector3 ShapeObjectPos = gameObject.transform.position;
            //가장 왼쪽 스플라인 추가후 라이너가 아니라면 오른쪽 점또한 추가
            _bezie.vector3s.Add(Spline.GetPosition(i)+ ShapeObjectPos);
            if (Spline.GetTangentMode(i) != ShapeTangentMode.Linear)
                _bezie.vector3s.Add(Spline.GetRightTangent(i)+Spline.GetPosition(i)+ ShapeObjectPos);
            //이후 오른쪽 스플라인이 라이너라면 스플라인의 왼쪽 점을 추가한뒤 오른쪽 스플라인 위치 추가
            if (Spline.GetTangentMode(i + 1) != ShapeTangentMode.Linear)
                _bezie.vector3s.Add(Spline.GetLeftTangent(i + 1) + Spline.GetPosition(i+1) + ShapeObjectPos);
            _bezie.vector3s.Add(Spline.GetPosition(i + 1)+ ShapeObjectPos);
            _bezie.Range = GetBezieDistance(_bezie);
            return _bezie;
        }
    }
    //MakeBezies나 GetVector3WithDstInBezie에서 사용됨, GetPointWithT 를 경유해서 해당 베지에형체의 거리 반환
    public float GetBezieDistance(Bezie bezie)
    {
        if (bezie.vector3s.Count < 2 || bezie.vector3s.Count > 4)
        {
            Debug.LogError("GetBezieDistance 인덱스오류 bezie.vector3s.Count: "+ bezie.vector3s.Count);
            return -1;
        }
        float value = 0;
        if (bezie.vector3s.Count == 2)
        {
            value = Vector2.Distance(bezie.vector3s[1], bezie.vector3s[0]);
        }
        else
            //t_precision: 정밀도 외부 설정 변수
         for (float i = 0; i <= 1 - t_precision; i += t_precision)
        {
        value += Vector2.Distance(GetPointWithT(i, bezie), GetPointWithT(i + t_precision, bezie));
        }
        return value;
    }
    //베지에의 t일때의 벡터 상태를 반환
    Vector3 GetPointWithT(float T, Bezie bezie)
    {
        //Debug.Log("GetPointWithTEnter");
        Vector3 value = Vector3.zero;
        if (bezie.vector3s.Count < 2 || bezie.vector3s.Count > 4)
        {
            Debug.LogError("GetPointWithT 인덱스오류 bezie.vector3s.Count: " + bezie.vector3s.Count);
            return value;
        }
        T=Mathf.Clamp(T, 0, 1);

        float a = (1 - T);
        float b = T;
        //float U2_a = Mathf.Pow(a, 2);
        //float U2_b = Mathf.Pow(b, 2);
        //Debug.Log(bezie.vector3s.Count);
        switch (bezie.vector3s.Count)
        {
            case 2:
                value = a * bezie.vector3s[0]
                + b * bezie.vector3s[1];
                break;
            case 3:
                //value = U2_a * bezie.vector3s[0]
                //+ 2 * a * b * bezie.vector3s[1]
                //+ U2_b * bezie.vector3s[2];

                {
                    Vector3 A = Vector3.Lerp(bezie.vector3s[0], bezie.vector3s[1], T);
                    Vector3 B = Vector3.Lerp(bezie.vector3s[1], bezie.vector3s[2], T);
                    value = Vector3.Lerp(A, B, T);
                }

                break;
            case 4:
                // value = bezie.vector3s[0] * U2_a * a
                //+ 3 * bezie.vector3s[1] * b * U2_a
                //+ 3 * bezie.vector3s[2] * U2_b * b
                //+ bezie.vector3s[3] * U2_b * b;
                {
                    Vector3 A = Vector3.Lerp(bezie.vector3s[0], bezie.vector3s[1], T);
                    Vector3 B = Vector3.Lerp(bezie.vector3s[1], bezie.vector3s[2], T);
                    Vector3 C = Vector3.Lerp(bezie.vector3s[2], bezie.vector3s[3], T);
                    Vector3 A_b = Vector3.Lerp(A, B, T);
                    Vector3 B_c = Vector3.Lerp(B, C, T);
                    value = Vector3.Lerp(A_b, B_c, T);
                }

                break;
        }
        return value;
    }


    // 결과적으로 타 클래스에서 정보를 얻어가기 위한 함수,d 는 오브젝트간의 거리 Start,End 는 리스트를 얻을 범위로 사용되는  Spline Index범위
    public List<Vector3[]> MakeVectorWithDistance(float d, int Start, int End, float FirstBounus = 0f,bool haedOnlyTop=false)
    {

        //Debug.Log("MakeVectorWithDistance");
        List<Vector3[]> value = new List<Vector3[]>();
        float Least = StartDistance;
        //베지에는 포인트 양끝중 앞 번호 포인트를 기준으로 합니다.
        //따라서 마지막 번호를 이용하는 베지에 는 있을수 없습니다
        if (Start < 0 || End > (Spline.GetPointCount() - 1))
        {
            Debug.LogError("MakeVectorWithDistance- 입력 법위 오류");
            Start=Mathf.Clamp(Start, 0, (Spline.GetPointCount() - 2));
            End=Mathf.Clamp(End, 0, (Spline.GetPointCount() - 1));
        }
        if (Bezies == null || Bezies.Count == 0)
            MakeBezies(0, Spline.GetPointCount() - 1);
        //if (UnitNum_each_Point.Count < Bezies.Count)
        //    for (int i = 0; i < Bezies.Count - UnitNum_each_Point.Count; i++)
        //        UnitNum_each_Point.Add(new int());
        //List<int> UnitNum_each_Point


        Least += FirstBounus;
        //if (Least > d) Least = d;
        for (int i = Start; i < End; i++)
        {
            if (Bezies[i].Range + Least > d)
            {
                int T = value.Count;
                value.AddRange(GetVector3WithDstInBezie(d - Least, i, d, haedOnlyTop));
                T = value.Count - T;
                Least += Bezies[i].Range - (T * d);
                if (Least < 0)
                {
                    //Debug.Log("Least < 0");
                    Least = 0;
                }
            }
            else
                Least += Bezies[i].Range;
        }

      //  for (int M = 0; M < value.Count; M++)
        {
           // Instantiate(Prefab, value[M][0],Quaternion.identity);
        }

        //Debug.Log("최종 전달:_" +  ", value.Count:_" + value.Count);
        return value;
    }
    //위의 MakeVectorWithDistance와 연동하여 사용되는 함수 a는 이전 베지에 에서 이용하지 못한 분량, i 는 베지에 목록, d는 오브젝트간 거리
    List<Vector3[]> GetVector3WithDstInBezie(float a, int i, float d, bool haedOnlyTop = false)
    {
        //a는 처음 생성될 오브젝트의 거리, i는 생성할 베지에곡선 넘버 d는 오브젝트간 거리
        List<Vector3[]> value = new List<Vector3[]>();
        bool Check = false;

        Vector3[] temp = new Vector3[3];
        float D = 0;  //한개 베지에를 이동한 총거리
        float _a = a;
        float Sum = a;
        for (float t = 0; t<=1;)
        {
            t = Mathf.Clamp(t, 0, 1);
            Vector3 L = GetPointWithT(t, Bezies[i]);
            Vector3 R = GetPointWithT(Mathf.Clamp(t + t_precision, 0, 1), Bezies[i]);
            D += Vector3.Distance(L, R);
            if (D > a)
            {
                while (D > _a)
                {
                    Vector3 Qtrn = R - L;
                    Qtrn.Normalize();
                    temp[2] = scale;
                    temp[0] = (GetPointWithT(t, Bezies[i])) + Qtrn * Sum;
                    //접선벡터
                    //로테이션은 함수의 접선인 (t가 적은쪽에서 높은쪽을 바라보는)Qtrn의+ 90도 회전 방향
                    //(a*cos(x),a*sin(x))->(a*cos(x+1/2),a*sin(x+1/2)= -a*sin(x),a*cos(x)== (a,b)->(-b,a))
                    if (!IsRotate)
                        temp[1] = Vector3.zero;
                    else
                    {
                        temp[1] = new Vector3(0, 0, (Mathf.Rad2Deg * Mathf.Atan2(Qtrn.y, Qtrn.x)));

                    }

                    //IsOffsetWorldOffset에따라 위치 변경
                    {
                        if (IsOffsetWorldOffset)
                            temp[0] += offset;
                        else if (!haedOnlyTop)
                        { 
                                temp[0].x += Mathf.Cos(Mathf.Deg2Rad * temp[1].z) * offset.x - offset.y * Mathf.Sin(Mathf.Deg2Rad * temp[1].z);
                                temp[0].y += Mathf.Sin(Mathf.Deg2Rad * temp[1].z) * offset.x + offset.y * Mathf.Cos(Mathf.Deg2Rad * temp[1].z);
                                temp[0].z = offset.z;
                        }
                        
                    }
                    value.Add(temp);
                    D -= _a;
                    _a = d;
                    Sum += d;
                    temp = new Vector3[3];
                }
            }
            Sum = 0;
            if (t + t_precision < 1)
                t += t_precision;
            else if (t + t_precision > 1 && !Check)
            {
                Check = true;
                t += t_precision;
            }
            else
                break;
        }
        return value;
    }




    //이하 디버깅용
    //public void Check()
    //{
    //    Debug.Log(Bezies.Count);
    //    for (int i = Bezies.Count; i < 7; i++)
    //    {
    //        test[i].gameObject.SetActive(false);
    //    }
    //    for (int i = 0; i < Bezies.Count; i++)
    //    {
    //        test[i].gameObject.SetActive(true);
    //        test[i].position = Bezies[i].vector3s[0];
    //    }
    //    //Debug.Log("i: "+i+"  "+Bezies[i].vector3s[0]+"  "+ Bezies[i].vector3s[Bezies[i].vector3s.Count-1]);
    //}
    //void MakeBeziePoint()
    //{
    //    string name="Name:";
    //    for (int i = 0; i < Bezies.Count; i++)
    //    {
    //        for (int k = 0; k < Bezies[i].vector3s.Count; k++)
    //        {
    //            name += i.ToString();
    //            name += "_";
    //            name += k.ToString();

    //            Instantiate(Prefab, Bezies[i].vector3s[k], Quaternion.identity).name=name;
    //            name= "Name:";
    //        }
    //    }
    //   // LineTest = false;
    //}
    //void MakeLastPoint()
    //{
    //    string name = "astPoint:";
    //    for (int i = 0; i < LastSpline.Count; i++)
    //    {
    //            name += i.ToString();

    //            Instantiate(Prefab, LastSpline[i].Point, Quaternion.identity).name = name;
    //            name = "LastPoint:";

    //    }
    //    // LineTest = false;
    //}

    //실험용
    public List<Vector3[]> MakeVectorWithDistance2(float d, int Start, int End, float FirstBounus = 0f)
    {

        //Debug.Log("MakeVectorWithDistance");
        List<Vector3[]> value = new List<Vector3[]>();
        float Least = StartDistance;
        if (Bezies == null || Bezies.Count == 0)
            MakeBezies(0, Spline.GetPointCount() - 1);

        Start = Mathf.Clamp(Start, 0, (Spline.GetPointCount() - 3));
            End=Mathf.Clamp(End, 0, Spline.GetPointCount() - 2);
        Least += FirstBounus;
        //if (Least > d) Least = d;
        for (int i = End; i>=Start; i--)
        {
            if (Bezies[i].Range + Least > d)
            {
                // Debug.Log("Bezies[" + i + "].Range > d:  " + Bezies[i].Range + Least + "  >  " + d);
                //d-sum 만큼 위치 구함
                //더이상 장소 없으면 남은 거리 sum에 더하고 넘어감
                //value = (GetVector3WithDstInBezie(d - Least, i, d));
                int T = value.Count;
                value.AddRange(GetVector3WithDstInBezie2(d - Least, i, d));
                //value.AddRange(GetVector3WithDstInBezie(Least, i, d));
                // Debug.Log("i:_" + i + ", value.Count:_" + value.Count);
                T = value.Count - T;
                //Least += Bezies[i].Range - (GetVector3WithDstInBezie(d-Least, i, d).Count) * d;
                //  Debug.Log("Least(" + Least + ")+=" + "Bezies[" + i + "].Range(" + Bezies[i].Range + ")-T(" + T + ")*D(" + d + ")="+ (Least+Bezies[i].Range - T * d));
                Least += Bezies[i].Range - T * d;
                if (Least < 0)
                    Least = 0;
                //sum -= (Bezies[i].Range - Least);
                //Debug.Log("Least: " + Least);
            }
            else
            {
                //Debug.Log("작동안함");
                Least += Bezies[i].Range;
            }
        }

        Debug.Log("최종 전달:_" + ", value.Count:_" + value.Count);
        return value;
    }
    //위의 MakeVectorWithDistance와 연동하여 사용되는 함수 a는 이전 베지에 에서 이용하지 못한 분량, i 는 베지에 목록, d는 오브젝트간 거리
    List<Vector3[]> GetVector3WithDstInBezie2(float a, int i, float d)
    {
        // Debug.Log("GetVector3WithDstInBezie_ a:" + a + "  i:" + i + "  d:" + d);
        List<Vector3[]> value = new List<Vector3[]>();
        bool Check = false;

        Vector3[] temp = new Vector3[3];
        float D = 0;
        float S = 0;
        float _a = a;
        float Sum = a;
        //Debug.Log("i" + i+ "   a" + a+ "   d" + d);
        int n = 0;
        for (float t = 1; t >= 0 + t_precision; t -= t_precision)
        {
        one:;
          //  Debug.Log("i: " + i + " t: " + t);
            //Debug.Log("t: " + t);
            //value.Add(new Vector3[3]);
            //value[n] = temp;
            // Debug.Log("t: " + t);
           t= Mathf.Clamp(t, 0, 1);
            Vector3 L = GetPointWithT(t, Bezies[i]);
            Vector3 R = GetPointWithT(Mathf.Clamp(t - t_precision, 0, 1), Bezies[i]);
            D += Vector3.Distance(L, R);
            //S += Vector3.Distance(L,R);
            // Debug.Log("D"+D);
            //if (D > a)
            while (D > _a)
            {
                Vector3 Qtrn = L - R;
                Qtrn.Normalize();
                temp[2] = scale;
                //temp[0]=( GetPointWithT(t + t_precision, Bezies[i]));
                temp[0] = (GetPointWithT(t, Bezies[i])) - Qtrn * Sum;
                //접선벡터
                //로테이션은 함수의 접선인 (t가 적은쪽에서 높은쪽을 바라보는)Qtrn의+ 90도 회전 방향 (a*cos(x),a*sin(x))->(a*cos(x+1/2),a*sin(x+1/2)= -a*sin(x),a*cos(x)== (a,b)->(-b,a))
                if (!IsRotate)
                    temp[1] = Vector3.zero;
                else
                {
                    temp[1] = new Vector3(0, 0, (Mathf.Rad2Deg * Mathf.Atan2(Qtrn.y, Qtrn.x)));

                    //  Debug.Log("각도 조절:" + temp[1]);
                }

                //IsOffsetWorldOffset에따라 위치 변경
                {
                    if (IsOffsetWorldOffset)
                        temp[0] += offset;
                    else
                    {
                        temp[0].x += Mathf.Cos(Mathf.Deg2Rad * temp[1].z) * offset.x - offset.y * Mathf.Sin(Mathf.Deg2Rad * temp[1].z);
                        temp[0].y += Mathf.Sin(Mathf.Deg2Rad * temp[1].z) * offset.x + offset.y * Mathf.Cos(Mathf.Deg2Rad * temp[1].z);
                        temp[0].z = offset.z;
                    }
                }
                // temp[0] += transform.position;

                value.Add(temp);
                //Debug.Log(i + "번째 베지에의" + t + "에서 생성 생성위치: " + temp[0]);
                // Debug.Log("value.count" + value.Count);
                D -= _a;
                _a = d;
                Sum += d;
                temp = new Vector3[3];
            }

            //if (Bezies[i].Range - S < _a)
            //{
            //    Debug.Log("Bezies["+i+"].Range" + Bezies[i].Range + "-S" + S + "<_a" + _a);
            //    break;
            //}
            n++;
            S += Sum;
            Sum = 0;
            if (t - t_precision < t_precision)
                if (t - t_precision > 0 && !Check)
                {
                    t -= t_precision;
                    Check = true;
                    goto one;
                }
            /*            if (t+t_precision>1-t_precision)
            if (t + t_precision < 1 && !Check)*/
    }

        //Debug.Log(value.Count + "  " + value[0][0]);
        //for (int t = 0; t < value.Count; t++)
        //{
        //    Debug.Log(t + ":  " + value[t][0]);
        //    Instantiate(Prefab, value[t][0], Quaternion.identity);
        //}
        return value;
    }
}

