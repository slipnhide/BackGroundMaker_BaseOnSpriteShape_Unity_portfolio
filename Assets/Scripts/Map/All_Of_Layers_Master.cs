using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class All_Of_Layers_Master
{
    // public static All_Of_Layers_Master Instance=null;

    public static List<Layers_master> List_layers_Masters = new List<Layers_master>();

    public static void Find_All_of_Layers_Master()
    {
        List_layers_Masters.Clear();
        Layers_master[] list = Resources.FindObjectsOfTypeAll<Layers_master>();
        for (int i = 0; i < list.Length; i++)
        {

            List_layers_Masters.Add(list[i]);
        }
    }
    public static void Add_Layer_Master(Layers_master layers_Master)
    {
        List_layers_Masters.Add(layers_Master);
    }
    public static void Delete_Layer_Master(int i)
    {
        if (List_layers_Masters.Count != 0 && List_layers_Masters.Count < i)
            List_layers_Masters.RemoveAt(i);
    }
}
