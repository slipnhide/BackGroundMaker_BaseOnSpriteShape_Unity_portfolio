using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class All_Of_Layers_Master_Window : EditorWindow
{
    //내부 관리 변수
    int SpaceBetween = 10;
    int Space_LayerSel = 70;
    Vector2 Scroll_Layers_Master = Vector2.zero;
    Vector2 Scroll_Sel_Layer = Vector2.zero;
    Vector2 Scroll_Layer_Info = Vector2.zero;
    Vector2 Scroll_PreFabs_Choose = Vector2.zero;
    Vector2 Scroll_Sprite_Choose = Vector2.zero;
    Color BaseColor = Color.white;
    Color ChooseColor = Color.green;

    //선택구분변수
    bool Option = false;
    int SelMasterNum = 0;
    int LastSelMasterNum = -1;
    int SelMenu = 0;
    int SelLayer = 0;
    int LastSelLayer = -1;
    bool SelBasicInfo = true;
    bool MemoChgMode = false;
    bool LayerDeleteCheck = false;
    bool LayerMasterAdjustOptionSel = false;

    //수정용 변수
    GameObject forAddPrefab=null;
    Sprite forAddSprite = null;
    string NameChg;
    string Memo;

    //코드줄이기위한 변수
    OneLayers target;
    Layers_master master;
    List<Layers_master> list = All_Of_Layers_Master.List_layers_Masters;

    [MenuItem("Example/MyWindow")]
    static void Init()
    {

        All_Of_Layers_Master_Window window = GetWindow<All_Of_Layers_Master_Window>(typeof(All_Of_Layers_Master_Window));
        // instance.Find_All_of_Layers_Master();
        window.Show();
    }
    private void OnGUI()
    {
        GUI.color = BaseColor;
        //if (!EditorApplication.isPlaying)
        {
            if (GUILayout.Button("option", GUILayout.Height(20), GUILayout.Width(70))) Option = !Option;
            if (Option)
            {
                Management();
            }
            else
            {
                All_Of_Layers_Master.Find_All_of_Layers_Master();
                ChoosLayer_Master();
                //메뉴 선택
                ChooseMenu();
                //이하 메뉴에 따라 구조 출력
                if (SelMenu == 0)
                    ControlInsideLayer();
                else if (SelMenu == 1)
                    adjustAllLayers();
                else if (SelMenu == 2)
                    SetLayerAddValue();

                Save();
                //없다면 유니티 실행중에 문제도 없지만 유니티 실행후 해당윈도우 기능만 사용하면 프로젝트에 달라진점을 인지못한다 고로 저장도 불가능하다
                if (GUI.changed)
                {
                    Debug.Log("list.Count" + list.Count);
                    Save();
                }
            }
        }
    }
    void ControlInsideLayer()
    {
        // 오류 체크
        {
            if (!(list[SelMasterNum].Check()))
            {
                if (list[SelMasterNum].GetComponent<Sprite_Shape_Master>())
                    list[SelMasterNum].Main = list[SelMasterNum].GetComponent<Sprite_Shape_Master>();
                else
                {
                    //GUILayout.TextArea("Sprite_Shape_Master Is Not Selected");
                    GUILayout.Label("Sprite_Shape_Master Is Not Selected");
                    return;
                }
            }
            if (list[SelMasterNum].layers.Count - 1 < SelLayer) SelLayer = 0;

            list[SelMasterNum].ReSetLayer_num();
        }

        //레이어 선택문
        SelectLayer();

        //레이어 선택 오류 체크
        if (list[SelMasterNum].layers.Count == 0) return;

        //레이어 순서 변경
        {
            GUILayout.BeginHorizontal("Box");
            if (GUILayout.Button("앞으로", GUILayout.Width(200)))SelLayer = list[SelMasterNum].LayerUp(SelLayer);
            if (GUILayout.Button("뒤로", GUILayout.Width(200))) { SelLayer = list[SelMasterNum].LayerDown(SelLayer); }
            if (GUILayout.Button("선택레이어복사", GUILayout.Width(200))) { list[SelMasterNum].LayerCopy(SelLayer); }
                GUILayout.EndHorizontal();
        }


        target = list[SelMasterNum].layers[SelLayer].GetComponent<OneLayers>();
        //매프레임마다 컴포넌트 호출 막기위한 작업+ 레이어가 바뀔때마다 이뤄질 일들
        if (LastSelLayer != SelLayer)
        {
            WenChgSelLayer();
        }
        //보고싶은 정보선택
        {
            GUILayout.BeginHorizontal("Box");
            if (GUILayout.Button("기본정보", GUILayout.Width(300))) SelBasicInfo = true;
            if (GUILayout.Button("이미지설정", GUILayout.Width(300))) SelBasicInfo = false;
            GUILayout.EndHorizontal();
        }

        //이름,이름변경,삭제확인,삭제버튼
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("SelectedLayer :  " + target.gameObject.name, GUILayout.MaxWidth(150));
            // NameChg= EditorGUILayout.DelayedTextField(NameChg, GUILayout.MaxWidth(80));
            NameChg = EditorGUILayout.TextField(NameChg, GUILayout.MaxWidth(80));
            if (GUILayout.Button("NameChge", GUILayout.MaxWidth(80))) { target.gameObject.name = NameChg; NameChg = string.Empty; GUI.FocusControl(null); }
            LayerDeleteCheck = GUILayout.Toggle(LayerDeleteCheck, "삭제하기", GUILayout.MaxWidth(80));
            if (LayerDeleteCheck)
                if (GUILayout.Button("선택레이어 삭제하기", GUILayout.MaxWidth(150))) 
                {
                    list[SelMasterNum].DeleteLayer(SelLayer); SelLayer = 0; LayerDeleteCheck = false;
                    if (list[SelMasterNum].layers.Count != 0)
                        target = list[SelMasterNum].layers[SelLayer].GetComponent<OneLayers>();
                    else
                        target = null;
                    return; 

                }
            GUILayout.EndHorizontal();
        }
        //선택에따라 정보 
        ShowInfo();

        void ShowInfo()
        {
            Scroll_Layer_Info = EditorGUILayout.BeginScrollView(Scroll_Layer_Info);
            if (SelBasicInfo)
                LayerInside_Basic();
            else
                LayerInside_Img();
            EditorGUILayout.EndScrollView();
        }
        //레이어마스터 내부의 레이어 선택하는버튼
        void SelectLayer()
        {

            Scroll_Sel_Layer = GUILayout.BeginScrollView(Scroll_Sel_Layer, GUILayout.Height(Space_LayerSel+20));
            GUILayout.BeginHorizontal();
            //스크롤바 시작
            //Debug.Log(list.Count);
            if (GUILayout.Button("레이어 추가", GUILayout.Height(Space_LayerSel), GUILayout.Width(Space_LayerSel)))
            {
                if (list[SelMasterNum].layers.Count == 0)
                {
                   if(list[SelMasterNum].AddLayers())
                    target = target = list[SelMasterNum].layers[0].GetComponent<OneLayers>();
                }
                else list[SelMasterNum].AddLayers();
            }
              //  Debug.Log("버튼");
            for (int i = 0; i < list[SelMasterNum].layers.Count; i++)
            {
                if (SelLayer == i)
                    GUI.color = ChooseColor;
                if (GUILayout.Button(list[SelMasterNum].layers[i].name, GUILayout.Height(Space_LayerSel), GUILayout.Width(Space_LayerSel)))
                    SelLayer = i;
                if (SelLayer == i)
                    GUI.color = BaseColor ;
            }
            // Debug.Log("SelLayer" + SelLayer);
            //스크롤바 끝
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        //레이어 내부 메뉴
        void LayerInside_Basic()
        {
            //메모보기,변경
            {

                //MemoChgMode = GUILayout.Toggle(MemoChgMode, "메모 변경모드", GUILayout.MaxWidth(80));
                //if (MemoChgMode)
                //{
                //    Memo= GUILayout.TextArea(Memo);
                //    if (GUILayout.Button("수정하기", GUILayout.Height(70), GUILayout.Width(70))) { target.Memo = Memo; Memo = string.Empty; MemoChgMode = false; }
                //}
                //else
                GUILayout.Label("레이어 메모", GUILayout.Width(600));
                target.Memo=GUILayout.TextArea(target.Memo, GUILayout.Width(600));
            }
            
            GUILayout.Space(SpaceBetween);
            {
                
                //스플라인조정,거리관련 조정
                {
                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.BeginHorizontal("Box");
                        target.StartSpline = Mathf.Clamp(EditorGUILayout.IntField("StarSpline", target.StartSpline, GUILayout.MaxWidth(180)), 0, target.Main.Spline.GetPointCount() - 2);
                        target.EndSpline = Mathf.Clamp(EditorGUILayout.IntField("EndSpline", target.EndSpline, GUILayout.MaxWidth(200)), target.StartSpline, target.Main.Spline.GetPointCount() - 1);
                     //   target.PositionType = EditorGUILayout.Toggle("PositioningType", target.PositionType, GUILayout.MaxWidth(200));
                        GUILayout.EndHorizontal();
                    }
                    {
                        GUILayout.BeginHorizontal("Box");
                        target.Distance = Mathf.Clamp(EditorGUILayout.FloatField("Distance", target.Distance, GUILayout.MaxWidth(200)), 0.5f, 9999);
                        target.FirstBounus = EditorGUILayout.FloatField("FirstBounus", target.FirstBounus, GUILayout.MaxWidth(200));
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                }
                
                GUILayout.Space(SpaceBetween);
                //벡터관련
                {
                    GUILayout.BeginVertical("Box");
                    {
                        target.WolrdOffset = EditorGUILayout.Vector3Field("WolrdOffset", target.WolrdOffset, GUILayout.Width(400));
                        target.offset = EditorGUILayout.Vector3Field("Offset", target.offset, GUILayout.Width(400));
                        target.Scale = EditorGUILayout.Vector3Field("Scale", target.Scale, GUILayout.Width(400));
                        GUILayout.BeginHorizontal("Box");
                        target.UseThisIsRotate = EditorGUILayout.Toggle("UseThisIsRotate", target.UseThisIsRotate);
                        if (target.UseThisIsRotate) target.IsRotate = EditorGUILayout.Toggle("IsRotate", target.IsRotate);
                        GUILayout.EndHorizontal();

                    }
                    GUILayout.EndVertical();
                }
                GUILayout.Space(SpaceBetween);
                //컬러
                {
                    target.Selcolor = EditorGUILayout.ColorField("기본 색 변경", target.Selcolor, GUILayout.Width(400));
                }
                GUILayout.Space(SpaceBetween);
                //레이어조정
                {
                    var layers = SortingLayer.layers;
                    string[] layersName = new string[layers.Length];
                    int[] layerNum = new int[layers.Length];
                    for (int i = 0; i < layers.Length; i++)
                    {
                        layersName[i] = layers[i].name;
                        layerNum[i] = layers[i].id;
                    }
                    GUILayout.Label(SortingLayer.IDToName(target.sortlayer.SortId));
                    target.sortlayer.SortId = (int)EditorGUILayout.IntPopup(target.sortlayer.SortId, layersName, layerNum);
                    
                }
            }
        }


        void LayerInside_Img()
        {
            //문제될소지 검사
            target.CheckAtMinimum();
            //랜덤할지 말지 한다면 어느 종류로 섞을지 안한다면 어느 종류 쓸지
            { 
            target.Randomize = GUILayout.Toggle(target.Randomize, "Randomize");
                if (!target.Randomize)
                    target.useType = (UseType)EditorGUILayout.EnumPopup("사용할 종류", target.useType);
                else
                    target.randomUseType = (RandomUseType)EditorGUILayout.EnumPopup("랜덤시 사용할 종류", target.randomUseType);
            }
            //프리팹 보여주기
            {
                //프리팹 보여줄지 검사
                if ((!target.Randomize && target.useType == UseType.Prefab) || (target.Randomize && (target.randomUseType == RandomUseType.OnlyPreFab || target.randomUseType == RandomUseType.UseBoth)))
                {
                    //현제 선택한 프리팹과 선택한 프리팹 삭제 버튼
                    {
                        GUILayout.Label("현제 선택한 프리팹");
                        GUILayout.BeginHorizontal();
                        target.PreFabs[target.SelPreFab] = ((GameObject)EditorGUILayout.ObjectField(target.PreFabs[target.SelPreFab], typeof(GameObject), GUILayout.Width(150), GUILayout.Height(70)));
                        //GUILayout.Box(one.PreFabs[one.SelPreFab].GetComponent<SpriteRenderer>().sprite.texture);
                        if (GUILayout.Button("선택프리팹삭제하기:" + target.SelPreFab, GUILayout.Width(150), GUILayout.Height(70)))
                        {
                            target.RemoveSelPrefab();
                            Scroll_PreFabs_Choose = Vector2.zero;
                        }
                        GUILayout.EndHorizontal();
                        
                    }

                    //프리팹 생성과 리스트목록
                    {
                        GUILayout.Label("AddPrefab       ListofPrefab");
                        //프리팹 생성
                        {
                            GUILayout.BeginHorizontal();
                            forAddPrefab = (GameObject)EditorGUILayout.ObjectField(forAddPrefab, typeof(GameObject), GUILayout.Width(70), GUILayout.Height(70));
                            if (forAddPrefab != null)
                            {
                                target.AddPrefab(forAddPrefab);
                                forAddPrefab = null;
                            }
                        }

                        //프리팹 목록
                        Scroll_PreFabs_Choose = EditorGUILayout.BeginScrollView(Scroll_PreFabs_Choose, GUILayout.Width(300),GUILayout.Height(0));
                        {
                            //ver 1 프리팹이미지를을 보여주는 버전
                            //for (int k = 0; k < one.PreFabs.Count; k++)
                            //{
                            //    if (one.PreFabs[k].GetComponent<SpriteRenderer>() != null)
                            //        // GUILayout.Box(one.PreFabs[k].GetComponent<SpriteRenderer>().sprite.texture, GUILayout.Width(70), GUILayout.Height(70));
                            //        one.PreFabs[k] = ((GameObject)EditorGUILayout.ObjectField(one.PreFabs[k], typeof(GameObject), GUILayout.Width(70), GUILayout.Height(70)));
                            //    else
                            //        GUILayout.Box(Texture2D.blackTexture, GUILayout.Width(70), GUILayout.Height(70));
                            //}
                            //ver2  버튼 눌러서 선택하게
                            GUILayout.BeginHorizontal();
                            for (int k = 0; k < target.PreFabs.Count; k++)
                            {
                                if (target.PreFabs[k] != null)
                                {
                                    //프리팹이 스프라이프가 없는거라면 그냥 빈 텍스쳐로 보여주고 아니라면 이미지를 텍스쳐로 보여줌
                                    if (target.PreFabs[k].GetComponent<SpriteRenderer>() != null)
                                    {
                                        if (GUILayout.Button(target.PreFabs[k].GetComponent<SpriteRenderer>().sprite.texture, GUILayout.Width(70), GUILayout.Height(60)))
                                            target.SelPreFab = k;
                                    }
                                    else
                                    {
                                        if (GUILayout.Button(Texture2D.blackTexture, GUILayout.Width(70), GUILayout.Height(60)))
                                            target.SelPreFab = k;
                                    }
                                }
                                else
                                {//프리팹이 null일때는 그냥 빈 텍스쳐로
                                    if (GUILayout.Button(Texture2D.blackTexture, GUILayout.Width(70), GUILayout.Height(60)))
                                        target.SelPreFab = k;
                                }
                                //}
                            }
                            GUILayout.EndHorizontal();
                            EditorGUILayout.EndScrollView();
                            GUILayout.EndHorizontal();
                        }

                    }
                }
            }
            //스프라이트 보여주기
            {
                if ((!target.Randomize && target.useType == UseType.Sprite) || (target.Randomize && (target.randomUseType == RandomUseType.OnlySprite || target.randomUseType == RandomUseType.UseBoth)))
                {
                    //선택한이미지 ,이미지 삭제
                        {
                              GUILayout.Label("현제 선택한 이미지");
                              GUILayout.BeginHorizontal();
                              target.sprites[target.SelSprite] = ((Sprite)EditorGUILayout.ObjectField(target.sprites[target.SelSprite], typeof(Sprite), GUILayout.Width(150), GUILayout.Height(70)));
                             //  GUILayout.Box(one.PreFabs[one.SelPreFab].GetComponent<SpriteRenderer>().sprite.texture);

                            if (GUILayout.Button("선택이미지삭제하기 현제 선택이미지:" + target.SelSprite, GUILayout.Width(300), GUILayout.Height(70)))
                            {
                            target.RemoveSelSprite();
                            Scroll_Sprite_Choose = Vector2.zero;
                            }
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.Label("AddSprite       ListOfSprite");
                        GUILayout.BeginHorizontal();
                        forAddSprite = (Sprite)EditorGUILayout.ObjectField(forAddSprite, typeof(Sprite), GUILayout.Width(70), GUILayout.Height(70));
                        if (forAddSprite != null)
                        {
                            target.AddSprite(forAddSprite);
                            forAddSprite = null;
                        }
                        Scroll_Sprite_Choose = EditorGUILayout.BeginScrollView(Scroll_Sprite_Choose, GUILayout.Width(300), GUILayout.Height(80));
                        {
                        GUILayout.BeginHorizontal();
                        //ver2  버튼 눌러서 선택하게
                        for (int k = 0; k < target.sprites.Count; k++)
                            {
                                if (target.sprites[k] != null)
                                {
                                    if (GUILayout.Button(target.sprites[k].texture, GUILayout.Width(70), GUILayout.Height(60))) target.SelSprite = k;
                                }
                                else
                                    if (GUILayout.Button(Texture2D.blackTexture, GUILayout.Width(70), GUILayout.Height(60))) target.SelSprite = k;
                            }
                        }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.EndScrollView();
                    GUILayout.EndHorizontal();
                }
            }
        }
    }


    void Management()
    {
        BaseColor = EditorGUILayout.ColorField("Basecolor",BaseColor);
        ChooseColor = EditorGUILayout.ColorField("ChooseColor", ChooseColor);
        SpaceBetween = EditorGUILayout.IntField("메뉴간 공간", SpaceBetween);
        Space_LayerSel= EditorGUILayout.IntField("레이어 메뉴 크기", Space_LayerSel);
        /*    int SpaceBetween = 10;
    int Space_LayerSel = 70;*/
    }
    void WhenChgMasterLayer()
    {
        LastSelMasterNum = SelMasterNum;
        SelLayer = 0; LastSelLayer = -1;
        Memo = string.Empty;
        NameChg = string.Empty;
        GUI.FocusControl(null);
        target = null;
        Scroll_Sel_Layer = Vector2.zero;
        Scroll_Layer_Info = Vector2.zero;
        Scroll_PreFabs_Choose = Vector2.zero;
        Scroll_Sprite_Choose = Vector2.zero;
        SelMenu = 0;
        SelBasicInfo = true;
        MemoChgMode = false;
        LayerDeleteCheck = false;
        {
            Debug.Log("checked");
            list[SelMasterNum].Check();

        }
    }
    void WenChgSelLayer()
    {
        LastSelLayer = SelLayer;
        MemoChgMode = false;
        Scroll_PreFabs_Choose = Vector2.zero;
        GUI.FocusControl(null);
    }
    void ChoosLayer_Master()
    {
        {
            Scroll_Layers_Master = GUILayout.BeginScrollView(Scroll_Layers_Master, GUILayout.Height(75));
            GUILayout.BeginHorizontal();
            //스크롤바 시작
            // Debug.Log(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                if (SelMasterNum == i)
                    GUI.color = ChooseColor;
                if (GUILayout.Button(list[i].name, GUILayout.Height(70), GUILayout.Width(70)))
                    SelMasterNum = i;
                if (SelMasterNum == i)
                    GUI.color = BaseColor;
            }
            if (LastSelMasterNum != SelMasterNum) WhenChgMasterLayer();
            //스크롤바 끝
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }
    }
    void ChooseMenu()
    {
        {
            GUILayout.BeginHorizontal();
            const int menunum = 3;
            string[] Menu = new string[menunum] { "레이어 내부조정", "레이어 일괄조정", "레이어 추가시 기본설정" };
            for (int k = 0; k < menunum; k++)
            {
                if (SelMenu == k)
                    GUI.color = ChooseColor;
                if (GUILayout.Button(Menu[k], GUILayout.Width(200), GUILayout.Height(70))) SelMenu = k;
                if (SelMenu == k)
                    GUI.color = BaseColor;
            }
            // Debug.Log("선택메뉴: " + SelMenu);
            GUILayout.EndHorizontal();
        }
    }
    void adjustAllLayers()
    {
        {
            master = list[SelMasterNum];

            {
                GUILayout.BeginHorizontal();

                if (LayerMasterAdjustOptionSel == true)
                    GUI.color = ChooseColor;
                if (GUILayout.Button("레이어 마스터 조작"))
                {
                    LayerMasterAdjustOptionSel = true;
                }
                if (LayerMasterAdjustOptionSel == true)
                    GUI.color = BaseColor;

                if (LayerMasterAdjustOptionSel == false)
                    GUI.color = ChooseColor;
                if (GUILayout.Button("레이어내부 일괄변경"))
                {
                    LayerMasterAdjustOptionSel = false;
                }
                if (LayerMasterAdjustOptionSel == false)
                    GUI.color = BaseColor;
                GUILayout.EndHorizontal();
            }


            //스플라인조정,거리관련 조정
            if (LayerMasterAdjustOptionSel)
            {
                LayerMasterValue();
            }
            else
            {
                DirectLayerInsideControl();
            }
            master.Check();


            void DirectLayerInsideControl()
            {
                {
                    GUILayout.Label("현제창에서 변경할 경우 현제 하위 레이어들의 설정을 전체 변경합니다");
                    GUILayout.Space(SpaceBetween);
                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.BeginHorizontal("Box");
                        master.SSpline = Mathf.Clamp(EditorGUILayout.IntField("StarSpline", master.SSpline, GUILayout.MaxWidth(200)), 0, master.Main.Spline.GetPointCount() - 2);
                        master.ESpline = Mathf.Clamp(EditorGUILayout.IntField("EndSpline", master.ESpline, GUILayout.MaxWidth(200)), master.SSpline, master.Main.Spline.GetPointCount() - 1);
                      //  master.PositioningType = EditorGUILayout.Toggle("PositioningType", master.PositioningType, GUILayout.MaxWidth(200));
                        GUILayout.EndHorizontal();
                    }
                    {
                        GUILayout.BeginHorizontal("Box");
                        master.Distance = Mathf.Clamp(EditorGUILayout.FloatField("Distance", master.Distance, GUILayout.MaxWidth(200)), 0.5f, 9999);
                        master.FirstBounus = EditorGUILayout.FloatField("FirstBounus", master.FirstBounus, GUILayout.MaxWidth(200));
                        GUILayout.EndHorizontal();
                    }

                    {
                        // master.Basesortlayer.SortId=EditorGUILayout
                    }

                    GUILayout.EndVertical();
                }
                GUILayout.Space(SpaceBetween);
                //벡터관련
                {
                    GUILayout.BeginVertical("Box");
                    {
                        master.WorldOffset = EditorGUILayout.Vector3Field("WorldOffset", master.WorldOffset, GUILayout.Width(400));
                        master.Offset = EditorGUILayout.Vector3Field("Offset", master.Offset, GUILayout.Width(400));
                        master.Scale = EditorGUILayout.Vector3Field("Scale", master.Scale, GUILayout.Width(400));
                        GUILayout.BeginHorizontal("Box");
                        master.UseThisIsRotate = EditorGUILayout.Toggle("UseThisIsRotate", master.UseThisIsRotate);
                        if (master.UseThisIsRotate) master.Rotate = EditorGUILayout.Toggle("IsRotate", master.Rotate);
                        GUILayout.EndHorizontal();

                    }
                    GUILayout.EndVertical();
                }
                GUILayout.Space(SpaceBetween);
                //컬러
                {
                    master.color = EditorGUILayout.ColorField("기본 색 변경", master.color, GUILayout.Width(400));
                }
                //랜덤체크
                {
                    master.Randomize = GUILayout.Toggle(master.Randomize, "Randomize");
                    if (!master.Randomize)
                        master.useType = (UseType)EditorGUILayout.EnumPopup("사용할 종류", master.useType, GUILayout.Width(400));
                    else
                        master.randomUse = (RandomUseType)EditorGUILayout.EnumPopup("랜덤시 사용할 종류", master.randomUse, GUILayout.Width(400));
                }
                //소팅 레이어 
                {
                    var layers = SortingLayer.layers;
                    string[] layersName = new string[layers.Length];
                    int[] layerNum = new int[layers.Length];
                    for (int i = 0; i < layers.Length; i++)
                    {
                        layersName[i] = layers[i].name;
                        layerNum[i] = layers[i].id;
                    }
                    GUILayout.Label(SortingLayer.IDToName(master.Basesortlayer.SortId));
                    master.Basesortlayer.SortId = (int)EditorGUILayout.IntPopup(master.Basesortlayer.SortId, layersName, layerNum);
                    
                }
                master. Check();
            }
            void LayerMasterValue()
            {
                GUILayout.Label("현제창에서 변경할 경우 하위 레이어들의 요소와 합쳐져 반응합니다");
                GUILayout.Space(SpaceBetween);
                master.ParentWolrdOffset = EditorGUILayout.Vector3Field("WorldOffset", master.ParentWolrdOffset, GUILayout.Width(400));
                master.ParentOffSet = EditorGUILayout.Vector3Field("Offset", master.ParentOffSet, GUILayout.Width(400));
                master.ParentScale = EditorGUILayout.Vector3Field("Scale", master.ParentScale, GUILayout.Width(400));
                GUILayout.Space(SpaceBetween);
                master.StartDepth = EditorGUILayout.FloatField("StartDepth", master.StartDepth, GUILayout.Width(400));
                master.Eachdepth = EditorGUILayout.FloatField("Eachdepth", master.Eachdepth, GUILayout.Width(400));
                GUILayout.Space(SpaceBetween);
                master.ParentRandomOffSet=EditorGUILayout.Vector3Field("RandomOffSet", master.ParentRandomOffSet, GUILayout.Width(400));
                master.ParentWorldRandomOffSet = EditorGUILayout.Vector3Field("WorldRandomOffSet", master.ParentWorldRandomOffSet, GUILayout.Width(400));
                GUILayout.Space(SpaceBetween);
                master.ParentColor = EditorGUILayout.ColorField("기본 색 변경", master.ParentColor, GUILayout.Width(400));
                master.Check();
            }
        }
    }
    void SetLayerAddValue()
    {
        
    }

    void Save()
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int k = 0; k < list[i].layers.Count; k++)
            {
                EditorUtility.SetDirty(list[i].layers[k].GetComponent<OneLayers>());

            }
            EditorUtility.SetDirty(list[i]);
            EditorUtility.SetDirty(list[i].GetComponent<Layers_master>().Main);
            EditorUtility.SetDirty(this);
        }
    }
}
