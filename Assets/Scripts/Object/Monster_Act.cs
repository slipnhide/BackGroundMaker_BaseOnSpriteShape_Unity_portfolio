using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Act : MonoBehaviour
{

    protected MonsterFSM fsm;
    // Start is called before the first frame update
    protected void Start()
    {
        fsm = gameObject.GetComponent<MonsterFSM>();
        
    }

    protected virtual void Attack()
    { }

    protected virtual void Move()
    { }

    protected virtual void Dead()
    { }

    protected virtual void wander()
    {}

}
