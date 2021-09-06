using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public struct Sortlayer
{
    [SerializeField]
    public int SortId;
    [SerializeField]
    public int LastSortId;
}
[System.Serializable]
public class Layers_master : MonoBehaviour
{
    //하위 레이어생성 시 전달하기위한 정보
    public Sprite_Shape_Master Main;
    //public int BaseStartNum=0;
    //public int BaseEndNum = 1;
    //public float BaseDistance = 2;
    //public Vector3 BaseOffset=Vector3.zero;

    //하위레이어에 영향 주는 변수
    public Vector3 ParentWolrdOffset =Vector3.zero;
    public Vector3 ParentOffSet = Vector3.zero;
    public Vector3 ParentScale = Vector3.one;
    public Color ParentColor = Color.white;
    public float Eachdepth = 0;
    public float StartDepth = 0;
    public Vector3 ParentRandomOffSet = Vector3.zero;
    public Vector3 ParentWorldRandomOffSet = Vector3.zero;
    public Sortlayer Basesortlayer = new Sortlayer { SortId = 0, LastSortId = 0 };
    //하위레이어에 영향 주는 변수 정보의 변화량감지
    public Vector3 LastParentWolrdOffset = Vector3.zero;
    public Vector3 LastParentOffSet = Vector3.zero;
    public Vector3 LastParentScale = Vector3.one;
    public Color LastParentColor = Color.white;
    public float LastEachdepth = 0;
    public float LastStartDepth = 0;
    Vector3 LastParentRandomOffSet = Vector3.zero;
    Vector3 LastParentWorldRandomOffSet = Vector3.zero;

    //직접 하위레이어들 동시 조절하기위한 정보
    public int SSpline = 0;
    public int ESpline = 1;
    public float Distance = 1;
    public float FirstBounus = 0;
    //public bool PositioningType = false;
    public bool UseThisIsRotate=false;
    public bool Rotate = true;
    public Vector3 WorldOffset = Vector3.zero;
    public Vector3 Offset = Vector3.zero;
    public Vector3 Scale = Vector3.one;
   // public Vector3 Rotation = Vector3.zero;
    public Color color = Color.white;
    public bool Randomize = false;
    public UseType useType = UseType.Prefab;
    public RandomUseType randomUse = RandomUseType.OnlyPreFab;



    //직접 하위레이어들 동시 조절하기위한 정보의 변화량감지
    public int LastSSpline = 0;
    public int LastESpline = 1;
    public float LastDistance = 1;
    public float LastFirstBounus = 0;
   // public bool LastPositioningType = false;
    public bool LastUseThisIsRotate = false;
    public bool LastRotate = true;
    Vector3 LastWorldOffset = Vector3.zero;
    Vector3 LastOffset = Vector3.zero;
    Vector3 LastScale = Vector3.one;
    Vector3 LastRotation = Vector3.zero;
    Color Lastcolor = Color.white;
    bool LastRandomize = false;
    UseType LastuseType = UseType.Prefab;
    RandomUseType LastrandomUse = RandomUseType.OnlyPreFab;

    //하위 레이어 갯수 변화 감지용
    int LastLayerNum=0;

    public List<GameObject> layers =new List<GameObject>();
    public void AllRemake()
    {
       // if (Layers.Count != 0)
         //   for (int i = 0; i < Layers.Count; i++)
            {
         //       Layers[i].ChgPosition();
            }
    }

    public bool AddLayers()
    {
        if (!MinimumCheck())
            return false;
        //Layers.Add(new OneLayers());
        GameObject go=new GameObject((layers.Count+1).ToString());
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = new Vector3(0, 0, 0);
        OneLayers one=go.AddComponent<OneLayers>();
        one.parent = this;
        one.Main = this.Main;
        one.StartSpline = SSpline;
        one.EndSpline = ESpline;
        one.Distance = Distance;
        one.offset = Offset;
        one.ParentColor = ParentColor;
        one.ParentOffSet = ParentOffSet;
        one.ParentScale = ParentScale;
        one.ParentWolrdOffset = ParentWolrdOffset;
        one.parent = this;
        //아직 레이어에 추가 하기전이므로 카운트로
        one.Depth= StartDepth + (Eachdepth * layers.Count);
        one.sortlayer.SortId = Basesortlayer.SortId;
        

        layers.Add(go);
        // names.Add("name");
        return true;
    }
    public void DeleteLayer()
    {
        //Layers.RemoveAt(i);
       // names.RemoveAt(i);
    }
    public void SetList_Test()
    {
        layers.Clear();
      //  names.Clear();
    }
    public bool Check()
    {

        if (!MinimumCheck())
            return false;


        if (layers.Count != transform.childCount)
        {
            layers.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).GetComponent<OneLayers>())
                    layers.Add(transform.GetChild(i).gameObject);
            }
        }
        OneLayers temp;
        //직접 조절하기위한 변수
        {
            if (LastSSpline != SSpline)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.StartSpline = SSpline;
                }
                LastSSpline = SSpline;
            }
            if (LastFirstBounus != FirstBounus)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.FirstBounus = FirstBounus;
                }
                LastFirstBounus = FirstBounus;
            }
            if (LastESpline != ESpline)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.EndSpline = ESpline;
                }
                LastESpline = ESpline;
            }
            if (LastFirstBounus != FirstBounus)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.FirstBounus = FirstBounus;
                }
                LastFirstBounus = FirstBounus;
            }
            if (LastOffset != Offset)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.offset = Offset;

                }
                LastOffset = Offset;
            }
            if (LastWorldOffset != WorldOffset)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.WolrdOffset = WorldOffset;
                }
                LastWorldOffset = WorldOffset;
            }

            if (LastScale != Scale)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.Scale = Scale;
                }
                LastScale = Scale;
            }
            if (Rotate != LastRotate)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.IsRotate = Rotate;
                }
                LastRotate = Rotate;
            }
            if (UseThisIsRotate != LastUseThisIsRotate)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.UseThisIsRotate = UseThisIsRotate;
                }
                LastUseThisIsRotate = UseThisIsRotate;
            }
            if (LastRandomize != Randomize)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.useType = useType;
                    temp.Randomize = Randomize;
                }
                LastRandomize = Randomize;
            }
            if (LastrandomUse != randomUse)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.randomUseType = randomUse;
                }
                LastrandomUse = randomUse;
            }
            if (useType != LastuseType)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.useType = useType;
                }
                LastuseType = useType;
            }
            if (Lastcolor.a != color.a
               || Lastcolor.b != color.b
               || Lastcolor.r != color.r
               || Lastcolor.g != color.g)
            {
                for (int i = 0; i < layers.Count; i++)
                    layers[i].GetComponent<OneLayers>().Selcolor = color;
                Lastcolor = color;
            }
            {
                if (Basesortlayer.LastSortId != Basesortlayer.SortId)
                {
                    for (int i = 0; i < layers.Count; i++)
                        layers[i].GetComponent<OneLayers>().sortlayer.SortId = Basesortlayer.SortId;
                    Basesortlayer.LastSortId = Basesortlayer.SortId;
                }
            }
            if (LastDistance != Distance)
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    temp = layers[i].GetComponent<OneLayers>();
                    temp.Distance = Distance;
                }
                LastDistance = Distance;
            }
        }

        //부모로서 주는 영향
        {
           // Debug.Log(" //부모로서 주는 영향");
            if (LastParentWolrdOffset != ParentWolrdOffset)
            {
              // Debug.Log("ParentWolrdOffset");
                for (int i = 0; i < layers.Count; i++)
                    layers[i].GetComponent<OneLayers>().ParentWolrdOffset = ParentWolrdOffset; 
                LastParentWolrdOffset = ParentWolrdOffset;
            }
            if (LastParentOffSet != ParentOffSet)
            {
                for (int i = 0; i < layers.Count; i++)
                    layers[i].GetComponent<OneLayers>().ParentOffSet = ParentOffSet; 
                LastParentOffSet = ParentOffSet;
            }
            if (LastParentScale != ParentScale)
            {
                for (int i = 0; i < layers.Count; i++)
                   layers[i].GetComponent<OneLayers>().ParentScale = ParentScale;
                LastParentScale = ParentScale;
            }
            if (LastParentColor != ParentColor)
            {
                for (int i = 0; i < layers.Count; i++)
                    layers[i].GetComponent<OneLayers>().ParentColor = ParentColor;
                LastParentColor = ParentColor;
            }

            if (LastEachdepth != Eachdepth)
            {
                SortingDepth();
                LastEachdepth = Eachdepth;
            }
            if (LastStartDepth != StartDepth)
            {
                SortingDepth();
                LastStartDepth = StartDepth;
            }
            if (LastParentRandomOffSet != ParentRandomOffSet)
            {
                for (int i = 0; i < layers.Count; i++)
                    layers[i].GetComponent<OneLayers>().ParentRandomOffSet = ParentRandomOffSet;
                LastParentRandomOffSet = ParentRandomOffSet;
            }
            if (LastParentWorldRandomOffSet != ParentWorldRandomOffSet)
            {
                for (int i = 0; i < layers.Count; i++)
                    layers[i].GetComponent<OneLayers>().ParentWorldRandomOffSet = ParentWorldRandomOffSet; 
                LastParentWorldRandomOffSet = ParentWorldRandomOffSet;
            }
        }

        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].GetComponent<OneLayers>().CheckValue();
        }


            return true;
    }
    bool MinimumCheck()
    {
        if (Main == null)
        {
            Debug.LogError("Function, Sprite_Shape_Master Is Not Selected");
            return false;
        }
        if (layers == null)
            layers = new List<GameObject>();
        if (Main == null && !(GetComponent<Sprite_Shape_Master>() == null))
            Main = GetComponent<Sprite_Shape_Master>();
        return true;
    }
    public void SortingDepth()
    {
        MinimumCheck();
        if (layers.Count != 0)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].GetComponent<OneLayers>().Depth =StartDepth+ (Eachdepth * i);
            }
        }
    }

    public void ChgPosition_call()
    {
        //Debug.Log("ChgPosition_call");
        for (int i = 0; i < layers.Count; i++)
            layers[i].GetComponent<OneLayers>().ChgPosition();
    }
    public void DeleteLayer(int i)
    {
        GameObject temp = layers[i];
        layers.RemoveAt(i);
        DestroyImmediate(temp);
    }
    public void ReSetLayer_num()
    {
        MinimumCheck();
        layers.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            layers.Add(transform.GetChild(i).gameObject);
            //layers[i].name = transform.GetChild(i).name;
        }
    }
    public void MakeAllZero()
    {
        MinimumCheck();
        ReSetLayer_num();
        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i].GetComponent<OneLayers>().WolrdOffset = Vector3.zero;
            layers[i].GetComponent<OneLayers>().offset = Vector3.zero;
        }
    }
    public int LayerDown(int i)
    {
        MinimumCheck();
        if ((layers.Count - 1 < i) || (layers.Count<2) || (i<0))
            return -1;

        GameObject temp = layers[i];
        if (i == (layers.Count - 1))
        {
            layers[i].transform.SetAsFirstSibling();
            layers.RemoveAt(i);
            layers.Insert(0, temp);
            SortingDepth();
            return 0;
        }
        else if (i == (layers.Count - 2))
        {
            layers[i].transform.SetAsLastSibling();
            layers.RemoveAt(i);
            layers.Add(temp);
            SortingDepth();
            return layers.Count - 1;

        }
        else if (i >= 0 && i < (layers.Count - 2))
        {
            layers[i].transform.SetSiblingIndex(i +1);
            layers.RemoveAt(i);
            layers.Insert(i + 1, temp);
            SortingDepth();
            return i + 1;
        }
        else
            return -1;
    }
    public int LayerUp(int i)
    {
        Debug.Log("LayerDownEnter  i: " + i);
        Debug.Log(layers[i].transform.GetSiblingIndex());
        MinimumCheck();
        if ((layers.Count - 1 < i) || (i<0))
            return -1;

        GameObject temp = layers[i];
        if (i == 0)
        {

            layers[i].transform.SetAsLastSibling();
            layers.RemoveAt(i);
            layers.Add(temp);
            SortingDepth();
            return layers.Count - 1;

        }
        else if (i > 0 && i < layers.Count)
        {
            layers[i].transform.SetSiblingIndex(i - 1);
            layers.RemoveAt(i);
            layers.Insert(i - 1, temp);
            SortingDepth();
            return i - 1;

        }
        else
            return -1;
    }

    public void LayerCopy(int k)
    {
        if (!MinimumCheck())
            return;
        OneLayers To = layers[k].GetComponent<OneLayers>();
        GameObject go = new GameObject(To.name+" Copy");
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = To.transform.localPosition;
        OneLayers Copy = go.AddComponent<OneLayers>();
        Copy.Distance = To.Distance;
        Copy.StartSpline = To.StartSpline;
        Copy.EndSpline = To.EndSpline;
        Copy.FirstBounus = To.FirstBounus;
        Copy.offset = To.offset;
        Copy.WolrdOffset = To.WolrdOffset;
        Copy.Scale = To.Scale;
        Copy.UseThisIsRotate = To.UseThisIsRotate;
        Copy.IsRotate = To.IsRotate;
        Copy.useType = To.useType;
        Copy.randomUseType = To.randomUseType;
        Copy.Randomize = To.Randomize;
        Copy.SelPreFab = To.SelPreFab;
        Copy.SelSprite = To.SelSprite;
        Copy.Selcolor = To.Selcolor;
        Copy.ParentColor = ParentColor;
        Copy.ParentOffSet = ParentOffSet;
        Copy.ParentScale = ParentScale;
        Copy.ParentWolrdOffset = ParentWolrdOffset;
        Copy.parent = this;
        Copy.Depth= StartDepth + (Eachdepth * layers.Count);
        for (int i = 0; i < To.PreFabs.Count; i++)
            Copy.PreFabs.Add(To.PreFabs[i]);
        for (int i = 0; i < To.sprites.Count; i++)
            Copy.sprites.Add(To.sprites[i]);
        Copy.sortlayer = To.sortlayer;
        Copy.NewMake();
        //딮스만은 복사 안하고 별도로
        //아직 레이어에 추가 하기전이므로 카운트로
        layers.Add(go);

    }

    public void InsertPointedAt(int k) 
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].GetComponent<OneLayers>().InsertPointedAt(k);
        }

        if (k > SSpline && k <= ESpline)
        {
            ESpline++;
            Debug.Log("EndS: " + (ESpline - 1) + "-> Ends: " + ESpline);
        }
        else if (SSpline >= k)
        {
            {
                SSpline++;
                ESpline++;
                Debug.Log("StartS: " + (SSpline - 1) + "-> Starts: " + SSpline);
                Debug.Log("EndS: " + (ESpline - 1) + "-> Ends: " + ESpline);
            }
        }
        else if (k > ESpline)
        {
            ESpline++;
        }
    }
    public void DeletedPointedAt(int k) 
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].GetComponent<OneLayers>().DeletedPointedAt(k);
        }

        {
            if (k > SSpline && k < ESpline)
            {
                ESpline--;
            }
            else if (SSpline >= k)
            {
                if (SSpline == 0)
                {
                    ESpline--;
                    if (ESpline == SSpline)
                        ESpline++;
                }
                else
                {
                    SSpline--;
                    ESpline--;
                }

            }
            else if (k == ESpline)
            {
                ESpline--;
                if (ESpline == SSpline)
                    ESpline++;
            }
        }
    }


}
