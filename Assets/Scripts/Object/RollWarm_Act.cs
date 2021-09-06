using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollWarm_Act : Monster_Act
{
    Animator anim;
    Rigidbody2D rigid;
    float AttackSpeed = 8;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        rigid=GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Attack()
    {
        anim.SetInteger("Move", 2);
        if(fsm.Target.position.x>fsm.Target.position.x)
        this.rigid.AddForce(Vector2.left * AttackSpeed);
    }
}
