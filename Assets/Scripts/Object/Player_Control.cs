using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Control : MonoBehaviour
{
    Rigidbody2D rigid;
    Vector2 OriginCollider2D_OffSet;
    Vector2 OriginCollider2D_Size;
    public GameObject foot;
    Animator anim;
    SpriteRenderer sr;

    Vector2 Move;

    float jump;
    float speed=8;
    float jumpPower=30;
    float jumpingKeyDuration=0.3f;
    float jumpKeyDownTime;

    bool IsJumping;
    bool IsSit;

    float act1;
    float act1CoolTime=.7f;
    bool IsAtking;
    float act1UsedTime;
    public GameObject slash;


    float act2;
    enum Act { Idle,Move,Attack}
    enum animLayer { Base,Sit}

    void ChgAnimLayer(int t)
    {
        Debug.Log(anim.layerCount);
        for (int i = 0; i < anim.layerCount; i++)
            anim.SetLayerWeight(i, 0);
        anim.SetLayerWeight(t, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        OriginCollider2D_OffSet = GetComponent<BoxCollider2D>().offset;
        OriginCollider2D_Size = GetComponent<BoxCollider2D>().size;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        Move = Vector2.zero;
        act1UsedTime = Time.time;
        slash.GetComponent<Effect_Script>().who = Effect_Script.owner.Player;
        slash.GetComponent<Effect_Script>().type = Effect_Script.EffectType.Player_Attack_Unbreak;
    }

    // Update is called once per frame
    void Update()
    {
        Control();
        Acting();
    }

    void Control()
    {
        Move.x = Input.GetAxis("Horizontal");
         Move.y = Input.GetAxis("Vertical");
        jump = Input.GetAxis("Jump");
        if (jump > 0)
            Debug.Log("점프");
        act1 = Input.GetAxis("Fire1");
       
       
    }
    void Move_Act()
    {
        void ChgStance(animLayer animLayer)
        {
            int num = (int)animLayer;
            switch (num)
            {
                case 0:
                    //베이스
                    ChgAnimLayer(num);
                    GetComponent<BoxCollider2D>().offset = OriginCollider2D_OffSet;
                    GetComponent<BoxCollider2D>().size = OriginCollider2D_Size;
                    break;
                case 1:
                    ChgAnimLayer(num);
                    GetComponent<BoxCollider2D>().size = OriginCollider2D_Size / 2;
                   // GetComponent<BoxCollider2D>().offset = new Vector2(OriginCollider2D_OffSet.x, OriginCollider2D_OffSet.y+ ((OriginCollider2D_Size.y) / 2));
                    break;
            }
        }

        if (!IsSit && Move.y < 0)
            if (foot.GetComponent<CheckGround>().IsOnGround)
            {
                IsSit = true;
                ChgStance(animLayer.Sit);
            }
            else
            {
              //  IsSit = false;
               // ChgStance(animLayer.Base);
            }
        else if(IsSit && Move.y >=0)
        {
            IsSit = false;
            ChgStance(animLayer.Base);
        }

        if(!IsSit)
        transform.Translate(new Vector3(Move.x,0,0)*Time.deltaTime*speed);
        else
            transform.Translate(new Vector3(Move.x, 0, 0) * Time.deltaTime * speed*0.5f);

        if(!anim.GetBool("Melee_Attack"))
        if (Move.x != 0)
        {
            anim.SetBool("Move", true);

            if (Move.x < 0)
                sr.flipX = true;
            else if (Move.x > 0)
                sr.flipX = false;

        }
        else
            anim.SetBool("Move", false);


    }
    void jumping()
    {
        if (IsJumping && foot.GetComponent<CheckGround>().IsOnGround)
        {
            anim.SetBool("Jump", false);
            IsJumping = false;
        }
        if (jump != 0)
            if (foot.GetComponent<CheckGround>().IsOnGround)
            {
                jumpKeyDownTime = Time.time;
                IsJumping = true;
                anim.SetBool("Jump", true);
            }

        if (IsJumping)
        {
            if (Time.time < jumpingKeyDuration + jumpKeyDownTime)
            {
                rigid.AddForce(Vector2.up * jump * jumpPower);
                Debug.Log("발돋움");
            }
            anim.SetFloat("Velocity.Y", rigid.velocity.y);
        }

    }
    void Acting1()
    {
        if (act1 > 0&&!IsSit)
            if (act1CoolTime + act1UsedTime < Time.time)
            {
            act1UsedTime = Time.time;
            anim.SetBool("Melee_Attack",true);
                slash.SetActive(true);
                slash.GetComponent<SpriteRenderer>().flipX = sr.flipX;
            Debug.Log("attack");
            }
    }
    void Aac1End() {anim.SetBool("Melee_Attack", false); }
    void Acting()
    {
        Acting1();
        Move_Act();
        jumping();
    }
}
