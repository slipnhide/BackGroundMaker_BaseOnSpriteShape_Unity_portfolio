using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFSM : MonoBehaviour
{
    Rigidbody2D rigid;
    public Transform Target;
    Animator anim;
    public enum State { Idle,Move,Attack,Dead}
    public State state;
    float sight;
    bool Find;
    bool IsHeLeft;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        Target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        CheckClose();
    }

    void CheckClose()
    {
        if ((transform.position - Target.position).magnitude < sight)
            Find = true;
        else
            Find = false;
    }

    public void ChgState(State _state)
    {
        state = _state;
        int temp = (int)state;
        switch (temp)
        {
            case 0:
                //아이들
                break;
            case 1:
                //이동
                break;
            case 2:
                //공격
                break;
            case 3:
                //사망
                break;

        }
    }


}
