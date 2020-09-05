using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerLogic : MonoBehaviour
{
    public int totalItemCount;
    public int stage;
    public Text stageCountText;
    public Text playerCountText;

    //아이템 카운트
    void Awake()
    {
        stageCountText.text = "/ " + totalItemCount;
    }

    //플레이어 카운트
    public void GetItem(int count)
    {
        playerCountText.text = count.ToString();
    }

    //경기장 밖으로 떨어지면
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(stage);
        }
    }
}
