using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public bool IsOnGround;
    Rigidbody2D rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            IsOnGround = true;
           // if (gameObject.GetComponentInParent<Animator>().GetBool("Jump"))
            {
           //     gameObject.GetComponentInParent<Animator>().SetBool("Jump", false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            IsOnGround = false;
        }

    }

}
