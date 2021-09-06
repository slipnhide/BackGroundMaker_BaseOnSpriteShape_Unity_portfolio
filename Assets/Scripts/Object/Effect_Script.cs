using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Script : MonoBehaviour
{
    public  enum owner { Player}
    public enum EffectType { Player_Attack_Unbreak }

    void Destroy() { Destroy(gameObject); }
    void Unable() { gameObject.SetActive(false); }
    void Able() { gameObject.SetActive(true); }
    public owner who;
    public EffectType type;
    Effect_Script(owner _who, EffectType _type) { who = _who;type = _type; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int _who = (int)who;
        int _type = (int)type;
        switch (_who)
        {
            case 0:
                {
                    if (collision.tag == "Monster")
                    { 
                    }
                    switch (_type)
                    {
                        case 0:
                            Unable();
                            break;
                    }
                }
                break;
        }

    }



}
