using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Layers_master))]
public class Layers_master_Edit : Editor
{
    Layers_master layers_Master;
    List<GameObject> layer = new List<GameObject>();
    List<bool> ShowInfoFade = new List<bool>();
    bool ShowAllInfo;
    bool DeleteCheck = false;
    GameObject forAddPrefab;
    Sprite forAddSprite;
    Vector2 Scroll_Sprite_Choose, Scroll_PreFabs_Choose = Vector2.zero;

    private void OnEnable()
    {
    }
    public override void OnInspectorGUI()
    {
    }

    void oneLayer_Inspector()
    {
        for (int i = 0; i < layers_Master.layers.Count; i++)
        {

            layers_Master.layers[i].name = GUILayout.TextArea(layers_Master.layers[i].name, GUILayout.Width(150));
            ShowInfoFade[i] = EditorGUILayout.BeginFoldoutHeaderGroup(ShowInfoFade[i], "설정보기");
            if (ShowInfoFade[i])
            {
                OneLayers one = layer[i].GetComponent<OneLayers>();
                //OneLayers_test oneLayers_Test=layer[i].GetComponent<OneLayers_test>();
                //OneLayers_test.OnInspectorGUI();
                {
                  //  one.PositionType = EditorGUILayout.Toggle("PositioningType", one.PositionType);
                    one.StartSpline = Mathf.Clamp(EditorGUILayout.IntField("시작스플라인", one.StartSpline), 0, one.Main.Spline.GetPointCount() - 2);
                    one.EndSpline = Mathf.Clamp(EditorGUILayout.IntField("끝스플라인", one.EndSpline), one.StartSpline + 1, one.Main.Spline.GetPointCount() - 1);
                    GUILayout.Space(10);
                    one.Distance = Mathf.Clamp(EditorGUILayout.FloatField("Distance", one.Distance), 0.5f, 800);
                    one.FirstBounus = EditorGUILayout.FloatField("FirstBounus", one.FirstBounus);
                    GUILayout.Space(20);
                    one.UseThisIsRotate = EditorGUILayout.BeginToggleGroup("UseThisIsRotate", one.UseThisIsRotate);
                    one.IsRotate = EditorGUILayout.Toggle("IsRotate", one.IsRotate);
                    EditorGUILayout.EndToggleGroup();
                    one.offset = EditorGUILayout.Vector3Field("Offset", one.offset, GUILayout.Width(400));
                    one.WolrdOffset = EditorGUILayout.Vector3Field("WolrdOffset", one.WolrdOffset, GUILayout.Width(400));
                    one.Scale = EditorGUILayout.Vector3Field("Scale", one.Scale,GUILayout.Width(400));
                    GUILayout.Space(10);
                    one.Selcolor = EditorGUILayout.ColorField(one.Selcolor);
                    GUILayout.Space(20);
                }
                //이하 프리팹과 이미지 설정 부분
                one.Randomize = GUILayout.Toggle(one.Randomize, "Randomize");
                //랜덤할지 말지 한다면 어느 종류로 섞을지 안한다면 어느 종류 쓸지
                if (!one.Randomize)
                {
                    one.useType = (UseType)EditorGUILayout.EnumPopup("사용할 종류", one.useType);
                }
                else
                {
                    one.randomUseType = (RandomUseType)EditorGUILayout.EnumPopup("랜덤시 사용할 종류", one.randomUseType);
                }

                //쓰는 타입에 따라 이미지나 프리팹 혹은 양쪽 설정 화면
                if ((!one.Randomize && one.useType == UseType.Prefab) || (one.Randomize && (one.randomUseType == RandomUseType.OnlyPreFab || one.randomUseType == RandomUseType.UseBoth)))
                {
                    //현제 가진 프리팹 목록 보여주고 랜덤이 아니라면 선택된 프리팹 이미지 보여줌
                    //현제 프리팹 목록중 가장 처음에 추가할수있는 칸 표시
                    if (one.PreFabs == null) one.PreFabs = new List<GameObject>();
                    {
                        GUILayout.BeginHorizontal();
                        if (!one.Randomize && one.PreFabs.Count > 0 && one.SelPreFab >= 0 && one.SelPreFab < one.PreFabs.Count)
                        {
                            GUILayout.Label("현제 선택한 프리팹");

                            one.PreFabs[one.SelPreFab] = ((GameObject)EditorGUILayout.ObjectField(one.PreFabs[one.SelPreFab], typeof(GameObject), GUILayout.Width(150), GUILayout.Height(70)));
                            //  GUILayout.Box(one.PreFabs[one.SelPreFab].GetComponent<SpriteRenderer>().sprite.texture);

                        }
                        if (GUILayout.Button("선택프리팹삭제하기 현제 선택프리팹:" + one.SelPreFab))
                        {
                            one.RemoveSelPrefab();
                            Scroll_PreFabs_Choose = Vector2.zero;
                        }
                        GUILayout.EndHorizontal();
                        // Debug.Log(one.SelPreFab + "삭제");
                    }
                    //프리팹 생성과 리스트목록
                    {
                        GUILayout.Label("AddPrefab       ListofPrefab");
                        GUILayout.BeginHorizontal();
                        forAddPrefab = (GameObject)EditorGUILayout.ObjectField(forAddPrefab, typeof(GameObject), GUILayout.Width(70), GUILayout.Height(70));
                        if (forAddPrefab != null)
                        {
                            one.AddPrefab(forAddPrefab);
                            forAddPrefab = null;
                        }
                        //CanSelectedBar = EditorGUILayout.BeginScrollView(CanSelectedBar, GUILayout.Width((one.PreFabs.Count * 300) + 300),GUILayout.Height(200));
                        Scroll_PreFabs_Choose = EditorGUILayout.BeginScrollView(Scroll_PreFabs_Choose, GUILayout.Width(300));
                        GUILayout.BeginHorizontal();
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
                            for (int k = 0; k < one.PreFabs.Count; k++)
                            {
                                if (one.PreFabs[k] != null)
                                {
                                    if (one.PreFabs[k].GetComponent<SpriteRenderer>() != null)
                                    {
                                        if (GUILayout.Button(one.PreFabs[k].GetComponent<SpriteRenderer>().sprite.texture, GUILayout.Width(70), GUILayout.Height(60)))
                                            one.SelPreFab = k;
                                    }
                                    else
                                    {
                                        if (GUILayout.Button(Texture2D.blackTexture, GUILayout.Width(70), GUILayout.Height(60)))
                                            one.SelPreFab = k;
                                    }
                                }
                                else
                                {
                                    if (GUILayout.Button(Texture2D.blackTexture, GUILayout.Width(70), GUILayout.Height(60)))
                                        one.SelPreFab = k;
                                }
                            }
                            GUILayout.EndHorizontal();
                            EditorGUILayout.EndScrollView();
                            GUILayout.EndHorizontal();
                        }

                    }
                }
                //쓰는 타입에 따라 이미지나 프리팹 혹은 양쪽 설정 화면
                if ((!one.Randomize && one.useType == UseType.Sprite) || (one.Randomize && (one.randomUseType == RandomUseType.OnlySprite || one.randomUseType == RandomUseType.UseBoth)))
                {
                    {
                        if (one.sprites == null) one.sprites = new List<Sprite>();
                        {
                            GUILayout.BeginHorizontal();
                            if (!one.Randomize && one.sprites.Count > 0 && one.SelSprite >= 0 && one.SelSprite < one.sprites.Count)
                            {
                                GUILayout.Label("현제 선택한 이미지");

                                one.sprites[one.SelSprite] = ((Sprite)EditorGUILayout.ObjectField(one.sprites[one.SelSprite], typeof(Sprite), GUILayout.Width(150), GUILayout.Height(70)));
                                //  GUILayout.Box(one.PreFabs[one.SelPreFab].GetComponent<SpriteRenderer>().sprite.texture);

                            }
                            if (GUILayout.Button("선택이미지삭제하기 현제 선택이미지:" + one.SelSprite))
                            {
                                one.RemoveSelSprite();
                                Scroll_Sprite_Choose = Vector2.zero;
                            }
                            GUILayout.EndHorizontal();
                            // Debug.Log(one.SelPreFab + "삭제");
                        }
                        GUILayout.Label("AddSprite       ListOfSprite");
                        GUILayout.BeginHorizontal();
                        forAddSprite = (Sprite)EditorGUILayout.ObjectField(forAddSprite, typeof(Sprite), GUILayout.Width(70), GUILayout.Height(70));
                        if (forAddSprite != null)
                        {
                            one.AddSprite(forAddSprite);
                            forAddSprite = null;
                        }
                        Scroll_Sprite_Choose = EditorGUILayout.BeginScrollView(Scroll_Sprite_Choose, GUILayout.Width(300));
                        GUILayout.BeginHorizontal();
                        {
                            //ver2  버튼 눌러서 선택하게
                            for (int k = 0; k < one.sprites.Count; k++)
                            {
                                if (one.sprites[k] != null)
                                {
                                    if (GUILayout.Button(one.sprites[k].texture, GUILayout.Width(70), GUILayout.Height(60))) one.SelSprite = k;
                                }
                                else
                                    if (GUILayout.Button(Texture2D.blackTexture, GUILayout.Width(70), GUILayout.Height(60))) one.SelSprite = k;
                            }
                        }
                        GUILayout.EndHorizontal();
                        EditorGUILayout.EndScrollView();
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.Space(10);
                DeleteCheck = EditorGUILayout.BeginToggleGroup("WannaDeleteLayer", DeleteCheck);
                if (EditorGUILayout.Toggle("LayerDelete", false))
                {
                    DeleteCheck = false;
                    layers_Master.DeleteLayer(i);
                }
                EditorGUILayout.EndToggleGroup();
                GUILayout.Space(10);
                if (GUILayout.Button("ReMake"))
                    one.NewMake();
                if (GUILayout.Button("DeleteAllObject"))
                    one.DestroyAll();
                one.CheckValue();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        if (GUILayout.Button("전체리스트삭제"))
        {
            layers_Master.SetList_Test();
        }
    }
}
