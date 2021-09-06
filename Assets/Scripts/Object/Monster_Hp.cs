using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Hp : MonoBehaviour
{

    public int MHp;
    int Hp;

    public void Hit(int Dmg)
    {
        Hp -= Dmg;
        if (Hp < 0)
            GetComponent<MonsterFSM>().state = MonsterFSM.State.Dead;
    }
}
