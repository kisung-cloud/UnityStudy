using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playerball : MonoBehaviour
{
    public float jumpPower;
    public int itemCount;
    public GameManagerLogic manager;
    bool isJump;
    Rigidbody rigid;
    AudioSource audio;

    void Awake()
    {
        isJump = false;
        rigid = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump") && !isJump)
        {
            isJump = true;
            rigid.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
        }
    }

    //물리 기반으로 움직이기 (그러기위해선 Rigidbody 필요)
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rigid.AddForce(new Vector3(h, 0, v), ForceMode.Impulse);
    }

    //충돌
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "floor")
        {
            isJump = false;
        }
    }

    //아이템 충돌
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            itemCount++;
            audio.Play();
            //닿으면 삭제
            other.gameObject.SetActive(false);
            manager.GetItem(itemCount);
        }
        else if(other.tag == "finish")
        {
            if(itemCount == manager.totalItemCount)
            {
                //game Clear && Next stage
                if(manager.stage == 2)
                {
                    //SceneManager.LoadScene("ballGame_0");
                    SceneManager.LoadScene(0);
                }
                else
                {
                    //SceneManager.LoadScene("ballGame_" + (manager.stage + 1).ToString());
                    SceneManager.LoadScene(manager.stage + 1);
                }
            }
            else
            {
                //restart;
                //SceneManager.LoadScene("ballGame_"+ manager.stage.ToString());
                SceneManager.LoadScene(manager.stage);
            }
            
        }
    }
}
