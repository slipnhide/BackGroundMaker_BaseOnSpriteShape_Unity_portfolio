using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RandomUseType 
{
    OnlyPreFab,OnlySprite,UseBoth
}
public enum UseType
{
    Prefab,Sprite
}

[System.Serializable]
public class OneLayers : MonoBehaviour
{
    //내부 관리 변수
    [SerializeField]
    public Sprite_Shape_Master Main;
    [SerializeField]
    int LastStartSpline = 0;
    [SerializeField]
    int LastEndSpline;
    [SerializeField]
    float LastDistance ;
    bool LastIsRotate = true;
    bool LastUseThisIsRotate = false;
    [SerializeField]
    bool LastRandomize = false;
    [SerializeField]
    public UseType LastuseType = UseType.Prefab;
    [SerializeField]
    public RandomUseType LastrandomUseType = RandomUseType.UseBoth;
    public Color32 LastSelcolor = new Color32(255, 255, 255, 255);
    public Vector3 Lastoffset = Vector3.zero;
    public Vector3 LastWolrdOffset = Vector3.zero;
    [SerializeField]
    public int LastSelPreFab = 0;
    [SerializeField]
    public int LastSelSprite = 0;
    public float LastFirstBounus = 0;
    public bool LastPositionType = false;
    public Vector3 LastScale = Vector3.one;
    Vector3 LastRandomOffSet = Vector3.zero;
    Vector3 LastWorldRandomOffSet = Vector3.zero;
    [SerializeField]
    public Sortlayer sortlayer;// = new Sortlayer();// { SortId = 0, LastSortId = 0 };
    //부모에서 조절하는 변수
    public Layers_master parent;
    public Vector3 ParentWolrdOffset = Vector3.zero;
    public Vector3 ParentOffSet = Vector3.zero;
    public Vector3 ParentScale = Vector3.one;
    public Color ParentColor = Color.white;
    public float Depth = 0;
    public Vector3 ParentRandomOffSet = Vector3.zero;
    public Vector3 ParentWorldRandomOffSet = Vector3.zero;
    //부모에서 조절하는 변수확인용
    public Vector3 LastParentWolrdOffset = Vector3.zero;
    public Vector3 LastParentOffSet = Vector3.zero;
    public Vector3 LastParentScale = Vector3.one;
    public Color LastParentColor = Color.white;
    public float LastDepth = 0;
    Vector3 LastParentRandomOffSet = Vector3.zero;
    Vector3 LastParentWorldRandomOffSet = Vector3.zero;

    public Vector3 RandomOffSet = Vector3.zero;
    public Vector3 WorldRandomOffSet = Vector3.zero;
    //외부 노출 변수
    public string Memo;
    // public bool PositionType = false;
    [SerializeField]
    public UseType useType = UseType.Prefab;
    [SerializeField]
    public RandomUseType randomUseType = RandomUseType.UseBoth;
    public Color32 Selcolor = new Color32(255, 255, 255, 255);
    public float FirstBounus = 0;
    public Vector3 offset = Vector3.zero;
    public Vector3 WolrdOffset = Vector3.zero;
    public Vector3 Scale = Vector3.one;
    [SerializeField]
    public int StartSpline = 0;
    [SerializeField]
    public int EndSpline = 1;
    public bool IsRotate = true;
    [SerializeField]
    public float Distance = .1f;
    public bool UseThisIsRotate = false;
    [SerializeField]
    public bool Randomize = false;
    [SerializeField]
    public int SelPreFab = 0;
    [SerializeField]
    public int SelSprite = 0;
    //실제 생성되는 오브젝트
    [SerializeField]
    public List<GameObject> Images = new List<GameObject>();
    //오브젝트에게 전달하기 위한 프리팹,스프라이트 목록
    [SerializeField]
    public List<GameObject> PreFabs = new List<GameObject>();
    [SerializeField]
    public List<Sprite> sprites = new List<Sprite>();
    //shapeMaster에서 받아온 trasform 정보
    [SerializeField]
    List<Vector3[]> ObjectsTF = new List<Vector3[]>();

    //디버깅용
    public GameObject test;

    //인스펙터창 전용
    public Vector2 scroll = Vector2.zero;

    public void NewMake()
    {
        CheckAtMinimum();
        if (transform.childCount == 0)
        {
            Images.Clear();
            ObjectsTF = GetVector3s();
            for (int i = 0; i < ObjectsTF.Count; i++)
            {
                GameObject go = MakeObject(ObjectsTF[i], i.ToString());
                Images.Add(go);
            }
        }
        else
         for (int i = 0; i < transform.childCount; i++)
         {
           Images.Add(transform.GetChild(i).gameObject);
         }

        ChgPosition();
        ChgAllNomal();
        ChgColor();
        ChgOffset();
        ChgScale();
        ChgAllSortingLayer();
    }
    public void ChgPosition()
    {
        //Debug.Log("ChgPosition()");
        // List<Vector3[]> newobjects = GetVector3s();
        int OldCount = Images.Count;
        int Lastindex;
        CheckAtMinimum();
        ObjectsTF = GetVector3s();
        int NewCount = ObjectsTF.Count;
        Lastindex = Mathf.Min(NewCount, OldCount);
        //Debug.Log("NewCount:" + NewCount + "   OldCount" + OldCount + "   Lastindex" + Lastindex + "  Image.count:" + Images.Count);
        for (int i = 0; i < Lastindex; i++)
        {
            //Debug.Log(newobjects[i][0]);
            //   ObjectsTF[i][0] = newobjects[i][0];
            //  ObjectsTF[i][1] = newobjects[i][1];
            Images[i].transform.position = ObjectsTF[i][0];
            //Debug.Log(Images[i].transform.position);
        }
        // Debug.Log("이동통과;");
        if (NewCount > OldCount)
        {
            //   Debug.Log("추가진입;");
            for (int i = Lastindex; i < NewCount; i++)
            {
                // ObjectsTF.Add(newobjects[i]);
                Images.Add(MakeObject(ObjectsTF[i], i.ToString()));
            }
            //  Debug.Log("추가통과;");
        }
        //삭제해야될때
        else if (NewCount < OldCount)
        {
            // Debug.Log("삭제해야될때");
            //  Debug.Log("Lastindex" + Lastindex + "_Images:" + Images.Count);
            //  Debug.Log("newobjects.Count" + newobjects.Count + "   ObjectsTF.Count" + ObjectsTF.Count);
            for (int i = Lastindex; i < OldCount; i++)
            {
                // ObjectsTF.RemoveAt(ObjectsTF.Count-1);
                DestroyImmediate(Images[Images.Count - 1]);
                Images.RemoveAt(Images.Count - 1);
            }
        }

        //Debug.Log("움직임통과1;");
        ChgAllNomal();
        //Debug.Log("움직임통과2;");
        ChgColor();
       // Debug.Log("움직임통과3;");
        ChgOffset();
        //Debug.Log("움직임통과4;");
        ChgScale();
        // Debug.Log("움직임통과5;");
        ChgAllSortingLayer(); 

    }
    public void CheckValue()
    {

        //스크립트가 아닌 유니티에서 이미지 오브젝트의 삭제 혹은 임의의 오브젝트 생성시 문제
        if (transform.childCount == 0)
        {
            ObjectsTF.Clear();
            Images.Clear();
        }

        if (ObjectsTF.Count == 0)
        {
            // Debug.LogError("ObjectsTF.Count");
            ObjectsTF.Clear();
            //if (transform.childCount == 0)
                ObjectsTF = GetVector3s();
            //else
            //{
            //    Vector3[] vector3s = new Vector3[3];
            //    for (int i = 0; i < transform.childCount; i++)
            //    {
            //        vector3s[0] = transform.GetChild(i).transform.position;
            //        vector3s[1] = transform.GetChild(i).transform.eulerAngles;
            //        vector3s[2] = transform.GetChild(i).transform.localScale;
            //        ObjectsTF.Add(vector3s);
            //    }
            //    vector3s = null;
            //}
        }
        if (Images.Count == 0)
        {
            Images.Clear();
            Vector3[] vector3s = new Vector3[3];
            for (int i = 0; i < transform.childCount; i++)
            {
                Images.Add(transform.GetChild(i).gameObject);
                //Images.Add(transform.Find(i.ToString()).gameObject);
            }
            vector3s = null;
        }
        //하위 오브젝트 갯수랑 데이터 정보랑 맞지 않을때(오브젝트가 임의로 삭제되었을때)
        //비활성화중-실험필요
        {/*
            if (Images.Count != transform.childCount)
            {
                Debug.Log("안맞음");
                bool check = false;
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (!check)
                        if (!string.Equals(i.ToString(), transform.GetChild(i).name))
                        {
                            Images.RemoveAt(i);
                            ObjectsTF.RemoveAt(i);
                            check = true;
                            Debug.Log("i: " + i + "에서 삭제됨");
                        }
                    if (check)
                        transform.GetChild(i).name = i.ToString();
                }
            }*/
        }


        if (LastDistance != Distance)
        {
            if (Distance <= 0)
                Distance = 0.1f;
            Debug.Log("Distance Changed");
            Debug.Log("LastDistance" + LastDistance + "  Distance: " + Distance);
            //if(ObjectsTF.Count!=0)
            //Debug.Log("2Distance Changed2");
            ChgPosition();
            LastDistance = Distance; 
        }
        if (LastFirstBounus != FirstBounus)
        {
            Debug.Log("FirstBounus_Changed" + FirstBounus);
            ChgPosition();
            LastFirstBounus = FirstBounus;
        }

        if ((LastStartSpline != StartSpline) || (LastEndSpline != EndSpline))
        {
           // Debug.Log("StartSpline Changed");
           // Debug.Log("EndSpline Changed  LastEndSpline: " + LastEndSpline + "  EndSpline:" + EndSpline);
            LastStartSpline = StartSpline;
            LastEndSpline = EndSpline;
            ChgPosition();

        }
        if (IsRotate != LastIsRotate)
        {
           // Debug.Log("IsRotate Changed");
            ChgAllNomal();
            LastIsRotate = IsRotate;
        }
        if (Lastoffset != offset
            ||LastParentOffSet!=ParentOffSet
            ||LastDepth!=Depth)
        {
            ChgOffset();
            Lastoffset = offset;
            LastParentOffSet = ParentOffSet;
            LastDepth = Depth;
        }
        if ((LastWolrdOffset != WolrdOffset)
            ||LastParentWolrdOffset!=ParentWolrdOffset)
        {
            ChgOffset();
            LastWolrdOffset = WolrdOffset;
            LastParentWolrdOffset = ParentWolrdOffset;
        }
        if (LastScale != Scale
            ||LastParentScale!=ParentScale)
        {
            ChgScale();
            LastScale = Scale;
            LastParentScale = ParentScale;
        }
        if (LastUseThisIsRotate != UseThisIsRotate)
        {
            ChgAllNomal();
          //  Debug.Log("UseThisIsRotate Changed LastUseThisIsRotate" + LastUseThisIsRotate + "   UseThisIsRotate" + UseThisIsRotate);
            LastUseThisIsRotate = UseThisIsRotate;
        }
        if (LastRandomize != Randomize
            || LastuseType != useType
            || LastrandomUseType != randomUseType)
        {
            LastRandomize = Randomize;
            LastuseType = useType;
            LastrandomUseType = randomUseType;

            ChgImage();
          //  Debug.Log("ImageThigSome Changed");
        }
        if( LastSelPreFab != SelPreFab || LastSelSprite != SelSprite)
        {
            LastSelPreFab = SelPreFab;
            LastSelSprite = SelSprite;
            if(!Randomize)
            ChgImage();
        }


        if (LastSelcolor.r != Selcolor.r
            || LastSelcolor.g != Selcolor.g
            || LastSelcolor.b != Selcolor.b
            || LastSelcolor.a != Selcolor.a
            ||LastParentColor.a!=ParentColor.a
            || LastParentColor.r != ParentColor.r
            || LastParentColor.b != ParentColor.b
            || LastParentColor.g != ParentColor.g)
        {
            ChgColor();
            LastSelcolor = Selcolor;
            LastParentColor = ParentColor;
        }
        if (LastRandomOffSet != RandomOffSet
            || LastParentRandomOffSet != ParentRandomOffSet
            || LastWorldRandomOffSet != WorldRandomOffSet
            || LastParentWorldRandomOffSet != ParentWorldRandomOffSet)
        {
            ChgOffset();
            {
                LastRandomOffSet = RandomOffSet;
                LastParentRandomOffSet = ParentRandomOffSet;
                LastWorldRandomOffSet = WorldRandomOffSet;
                LastParentWorldRandomOffSet = ParentWorldRandomOffSet;
             }
        }
        {
            if (sortlayer.LastSortId != sortlayer.SortId)
            {
                
                Debug.Log("Layer Chage");
                ChgAllSortingLayer();
                sortlayer.LastSortId = sortlayer.SortId;
            }
        }
    }
    public void DestroyAll()
    {
        int k = Images.Count;
        for (int i = 0; i < k; i++)
        {
            ObjectsTF.RemoveAt(ObjectsTF.Count - 1);
            DestroyImmediate(Images[Images.Count - 1]);
            Images.RemoveAt(Images.Count - 1);
        }
    }

    void ChgImage()
    {
        CheckAtMinimum();
        for (int i = 0; i < Images.Count; i++)
        {
            GameObject temp = Images[i];
            Images.RemoveAt(i);
            GameObject newtemp = Make_EmptyObject(temp);
            Images.Insert(i, newtemp);
        }
        Vector3[] Tr = new Vector3[3];
        for (int i = 0; i < Images.Count; i++)
        {
            Tr[0] = Images[i].transform.position;
            Tr[2] = Images[i].transform.localScale;
            Tr[1] = Images[i].transform.localRotation.eulerAngles;
            GameObject go = MakeObject(Tr, Images[i].name);
            GameObject temp = Images[i];
           // ChgOffset();
            Images.RemoveAt(i);
            DestroyImmediate(temp);
            Images.Insert(i, go);
        }
        ChgAllNomal();
        ChgColor();
        ChgOffset();
        ChgScale();
    }
    List<Vector3[]> GetVector3s()
    {
        CheckAtMinimum();
            return Main.MakeVectorWithDistance(Distance, StartSpline, EndSpline, FirstBounus);
    }
    GameObject MakeObject(Vector3[] Tr, string name)
    {
        Debug.Log("MakeObject");
        GameObject go;
        if (Randomize)
        {
            //랜덤모드-프리팹모드
            if (randomUseType == RandomUseType.OnlyPreFab)
                go = MakeObject_UsePrefab(Tr, name);
            //랜덤모드-스프라이트모드
            else if (randomUseType == RandomUseType.OnlySprite)
                go = MakeObject_UseSprite(Tr, name);
            //랜덤모드-프리팹+스프라이트모드
            else
            {
                {
                    if (PreFabs == null)
                        PreFabs = new List<GameObject>();
                    if (sprites == null)
                        sprites = new List<Sprite>();
                    int T = PreFabs.Count + sprites.Count;
                    if (T == 0)
                        go = MakeObject_UseSprite(Tr, name);
                    else
                    {
                        if (PreFabs.Count > sprites.Count)
                        {
                            if (Random.Range(1, T + 1) < (PreFabs.Count))
                                go = MakeObject_UsePrefab(Tr, name);
                            else
                                go = MakeObject_UseSprite(Tr, name);
                        }
                        else if (PreFabs.Count < sprites.Count)
                        {
                            if (Random.Range(1, T + 1) < (sprites.Count))
                                go = MakeObject_UseSprite(Tr, name);
                            else
                                go = MakeObject_UsePrefab(Tr, name);
                        }
                        else
                        {
                            if (Random.Range(0, 2) == 0)
                                go = MakeObject_UseSprite(Tr, name);
                            else
                                go = MakeObject_UsePrefab(Tr, name);
                        }
                    }

                }
            }
        }
        else
        {
            if (useType == UseType.Prefab)
                go = MakeObject_UsePrefab(Tr, name);
            else
                go = MakeObject_UseSprite(Tr, name);
        }
        return go;
    }
    GameObject MakeObject_UsePrefab(Vector3[] Tr, string name)
    {
        GameObject go=null;
        //오류 검사하면서 오류에 맞춰서 오브젝트 생성
        {
            if (PreFabs == null) //혹시 프리팹 널상태
                PreFabs = new List<GameObject>();
            if (PreFabs.Count == 0)//Debug.Log("리스트 0");
                PreFabs.Add(null);
            if (SelPreFab > PreFabs.Count-1)
            {
                Debug.LogError("프리팹 선택값 에러");
                SelPreFab = 0;
            }

            else
            {
                if (object.ReferenceEquals(PreFabs[SelPreFab], null))
                {
                    go = new GameObject(name);
                    Debug.Log("Prefab Reference is null");
                }
                else
                {
                    if (!PreFabs[SelPreFab])
                    {
                        //프리펩이 빈상태
                        go = new GameObject(name);
                        //Debug.Log("Prefab Reference is fakenull");
                    }
                    else
                    {
                        if (Randomize)
                        {
                            go = Instantiate(PreFabs[Random.Range(0, PreFabs.Count)]);
                        }
                        else
                            go = Instantiate(PreFabs[SelPreFab]);
                        //  Debug.Log("있음");
                    }
                }
            }
        }
        // go = Instantiate(new GameObject(name), Tr[0] + transform.localPosition, Quaternion.identity);
        go.transform.position = Tr[0] + transform.localPosition;
        go.transform.localScale = Tr[2];
        go.transform.parent = gameObject.transform;
        go.name = name;
        return go;
    }
    GameObject MakeObject_UseSprite(Vector3[] Tr, string name)
    {
        GameObject go=new GameObject();
        go.AddComponent<SpriteRenderer>();
        //오류 검사하면서 오류에 맞춰서 오브젝트 생성
        {
            if (sprites == null) //혹시 프리팹 널상태
                sprites = new List<Sprite>();
            if (sprites.Count == 0)//Debug.Log("리스트 0");
                sprites.Add(null);
            if (SelSprite > sprites.Count-1)
            {

                Debug.LogError("스프라이트 선택값 에러");
                SelSprite = 0;
            }

            //0개가 아니여야 하고 널값이 아니고
            if (sprites.Count != 0)
            {
                if (!Randomize)
                {
                    //스프라이트 대상이 널은 아니고
                     if(!(object.ReferenceEquals(sprites[SelSprite], null)))
                    //대상 이미지가 비어있는상태(fakenull) 가 아닐때
                    if (sprites[SelSprite])
                        go.GetComponent<SpriteRenderer>().sprite = sprites[SelSprite];
                    //랜덤이 아닌데 대상이미지가 비어있다면 그냥패스
                }
                else
                {
                    //랜덤이라면 목록 돌면서 널이 아닌 목록중 골라서 이미지 적용

                    List<int> tt = new List<int>();
                    for (int t = 0; t < sprites.Count; t++)
                    {
                        if (!(object.ReferenceEquals(sprites[t], null)))
                            //대상 이미지가 비어있는상태(fakenull) 가 아닐때
                            if (sprites[t])
                                tt.Add(t);
                    }
                    int choice;
                    if (tt.Count != 0)
                    {
                        choice = Random.Range(0, tt.Count);
                        go.GetComponent<SpriteRenderer>().sprite = sprites[choice];
                    }
                }
            }
        }
        go.transform.position = Tr[0] + transform.localPosition;
        go.transform.localScale = Tr[2];
        go.transform.parent = gameObject.transform;
        go.name = name;
        return go;
    }
    GameObject Make_EmptyObject(GameObject go)
    {
        string name = go.gameObject.name;

        GameObject value = new GameObject(name);
        value.transform.position = go.transform.position;
        value.transform.rotation = go.transform.rotation;
        value.transform.localScale = go.transform.localScale;
        value.transform.parent = transform;
        DestroyImmediate(go);
        return value;
    }
    GameObject RandomGetSprite(GameObject _go)
    {
        Debug.Log("RandomGetSprite");
        List<int> tt = new List<int>();
        for (int t = 0; t < sprites.Count; t++)
        {
            if (!(object.ReferenceEquals(sprites[t], null)))
                //대상 이미지가 비어있는상태(fakenull) 가 아닐때
                if (sprites[t])
                    tt.Add(t);
        }
        int choice;
        if (tt.Count != 0)
        {
            choice = Random.Range(0, tt.Count);
            if (_go.GetComponent<SpriteRenderer>() == null)
                _go.AddComponent<SpriteRenderer>();
            _go.GetComponent<SpriteRenderer>().sprite = sprites[choice];
        }
        return _go;
    }
    GameObject RandomGetPrefab(GameObject _go)
    {
        List<int> tt = new List<int>();
        for (int t = 0; t < PreFabs.Count; t++)
        {
            if (!(object.ReferenceEquals(PreFabs[t], null)))
                //대상 이미지가 비어있는상태(fakenull) 가 아닐때
                if (PreFabs[t])
                    tt.Add(t);
        }
        int choice;
        if (tt.Count != 0)
        {
            choice = Random.Range(0, tt.Count);
            _go= PreFabs[choice];
        }
        return _go;
    }
    public void ChgColor()
    {
        for (int i = 0; i < Images.Count; i++)
        {
            if (Images[i].GetComponent<SpriteRenderer>() != null)
                Images[i].GetComponent<SpriteRenderer>().color = Selcolor*ParentColor;
        }
    }
    public void ChgOffset()
    {
        Vector3 RanWolrdOff = ParentWorldRandomOffSet + WorldRandomOffSet;
        Vector3 RanOff = ParentRandomOffSet + RandomOffSet;
        Vector3 ChgPosition = Vector3.zero;
        for (int i = 0; i < Images.Count; i++)
        {
            //ChgPosition.x += (Mathf.Cos(Mathf.Deg2Rad * Images[i].transform.localEulerAngles.z)) * offset.x;
            //ChgPosition.y += (Mathf.Sin(Mathf.Deg2Rad * Images[i].transform.localEulerAngles.z)) * offset.x;
            //ChgPosition.x -= (Mathf.Sin(Mathf.Deg2Rad * Images[i].transform.localEulerAngles.z)) * offset.y;
            //ChgPosition.y += (Mathf.Cos(Mathf.Deg2Rad * Images[i].transform.localEulerAngles.z)) * offset.y;
            //ChgPosition += WolrdOffset;
            //Images[i].transform.position = ObjectsTF[i][0] + ChgPosition;
            //Images[i].transform.position += WolrdOffset;
            ChgPosition.x += (Mathf.Cos(Mathf.Deg2Rad * Images[i].transform.localEulerAngles.z)) * (offset.x+ParentOffSet.x+Random.Range(0,RanOff.x));
            ChgPosition.y += (Mathf.Sin(Mathf.Deg2Rad * Images[i].transform.localEulerAngles.z)) * (offset.x + ParentOffSet.x+Random.Range(0, RanOff.x));
            ChgPosition.x -= (Mathf.Sin(Mathf.Deg2Rad * Images[i].transform.localEulerAngles.z)) * (offset.y+ParentOffSet.y + Random.Range(0, RanOff.y));
            ChgPosition.y += (Mathf.Cos(Mathf.Deg2Rad * Images[i].transform.localEulerAngles.z)) * (offset.y + ParentOffSet.y+ Random.Range(0, RanOff.y));
            ChgPosition += (WolrdOffset+ParentWolrdOffset + new Vector3(Random.Range(0,RanWolrdOff.x), Random.Range(0, RanWolrdOff.y), Random.Range(0, RanWolrdOff.z))  );
            ChgPosition.z += Depth;
            Images[i].transform.position = ObjectsTF[i][0] + ChgPosition;
            ChgPosition = Vector3.zero;
           // Images[i].transform.position += WolrdOffset;
        }

    }
    void ChgAllNomal()
    {
       // Debug.Log("ChgAllNomal_  Images.Count:"+ Images.Count+ "  ObjectsTF:"+ ObjectsTF.Count);
        if (UseThisIsRotate && !IsRotate)
            for (int i = 0; i < Images.Count; i++)
                Images[i].transform.eulerAngles = Vector3.zero;
        else
            for (int i = 0; i < Images.Count; i++)
                Images[i].transform.eulerAngles = ObjectsTF[i][1];
      //  Debug.Log("why");
    }
    void ChgScale()
    {
        Vector3 Lscale = new Vector3();
       // Debug.Log("ChgScale");
        for (int i = 0; i < Images.Count; i++)
        {
            Lscale.x = Scale.x * ParentScale.x * ObjectsTF[i][2].x;
            Lscale.y = Scale.y * ParentScale.y * ObjectsTF[i][2].y;
            Lscale.z = Scale.z * ParentScale.z * ObjectsTF[i][2].z;
            Images[i].transform.localScale = Lscale;
            Lscale = new Vector3();
        }
    }
    void ChgAllSortingLayer()
    {
       // if (sortlayer.LastSortId != sortlayer.SortId)
        {
            Debug.Log("ChgAllSortingLayer");
            Debug.Log("name" + transform.name + "  Last" + sortlayer.LastSortId + "  Now" + sortlayer.SortId);
            for (int i = 0; i < Images.Count; i++)
            {
                if (Images[i].GetComponent<SpriteRenderer>())
                {
                    Images[i].GetComponent<SpriteRenderer>().sortingLayerID = sortlayer.SortId;
                    float z = Images[i].transform.position.z;
                    Images[i].GetComponent<SpriteRenderer>().sortingOrder = Random.Range(0+((int)z*10), (int)(z+1)*10);
                }
            }
        }
    }
    public void CheckAtMinimum()
    {
        if (Main == null)
        {

            if (transform.parent.GetComponent<Layers_master>().Main)
                Main = transform.parent.GetComponent<Layers_master>().Main;
            else
                Debug.LogWarning("Onelayers_Main_Is null");
        }    
        if (sprites == null)
            sprites = new List<Sprite>();
        if (sprites.Count == 0)
            sprites.Add(null);
        if (PreFabs == null)
            PreFabs = new List<GameObject>();
        if (PreFabs.Count == 0)
            PreFabs.Add(null);
        if ((SelPreFab > PreFabs.Count - 1)
            ||( SelPreFab<0))
            SelPreFab = 0;
        if ((SelSprite > sprites.Count - 1)
            || (SelSprite < 0))
            SelSprite = 0;

        if (Images.Count != transform.childCount)
        {
            Images.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                Images.Add(transform.GetChild(i).gameObject);
            }
        }
        if (sortlayer.SortId == 0)
        {
            if ( (0 != transform.childCount))
            {
                for(int i=0; i<transform.childCount; i++)
                {
                    if(transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>())
                    {
                        Debug.Log("sortlayer.SortId: " + sortlayer.SortId);
                        Debug.Log(transform.name);
                        Debug.Log(transform.GetChild(i).GetComponent<SpriteRenderer>().name);
                        sortlayer.SortId = transform.GetChild(i).GetComponent<SpriteRenderer>().sortingLayerID;
                        Debug.Log("sortlayer.SortId: " + sortlayer.SortId);
                        sortlayer.LastSortId = sortlayer.SortId;
                        break;
                    }
                }
            }
        }
    }

    public void RemoveSelPrefab() { PreFabs[SelPreFab] = null; PreFabs.RemoveAt(SelPreFab);SelPreFab = 0; }
    public void RemoveSelSprite() { sprites[SelSprite] = null; sprites.RemoveAt(SelSprite);SelSprite = 0; }
    public void AddPrefab(GameObject _Prefab)
    {
        if (!object.ReferenceEquals(PreFabs[0], null)&& !PreFabs[0])
        {
             if (!PreFabs[0])
             {
                 PreFabs[0] = _Prefab;
             }
        }
          else
              PreFabs.Add(_Prefab);
        
    }
    public void AddSprite(Sprite _Sprite) 
    {
        if (sprites.Count == 1
            && !object.ReferenceEquals(sprites[0], null)
            && !sprites[0])
        {
                    sprites[0] = _Sprite;
        }
        else
            sprites.Add(_Sprite);
    }

    public void InsertPointedAt(int i) 
    {
        //포인트가 각 start end앞이나 해당위치에 추가된경우 한칸씩 뒤로 
        //뒤에 추가된 것은 무시
        //start포인트검사
        if (i > StartSpline && i <= EndSpline)
        {
            EndSpline++;
        }
        else if (StartSpline >= i)
        {
            {
                StartSpline++;
                EndSpline++;
            }
        }
        else if (i > EndSpline)
        {
            EndSpline++;
        }
    }
    public void DeletedPointedAt(int i)
    {
        //포인트가 각 start end 앞에 삭제된경우 한칸씩 앞으로(0>)
        //해당 포인트 지점에 삭제된경우 start는 앞으로 end는 뒤로count<
        if (i > StartSpline && i < EndSpline)
        {
            EndSpline--;
        }
        else if (StartSpline >= i)
        {
            if (StartSpline == 0)
            {
                EndSpline--;
                if (EndSpline == StartSpline)
                    EndSpline++;
            }
            else
            {
                StartSpline--;
                EndSpline--;
            }

        }
        else if (i == EndSpline)
        {
            EndSpline--;
            if (EndSpline == StartSpline)
                EndSpline++;
        }
    }
}
