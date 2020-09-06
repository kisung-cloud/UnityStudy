using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//클래스는 System.Serializable이라는 어트리뷰트(Attribute)를 명시해야
//인스펙터 뷰에 노출됨
[System.Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
}

public class playerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;
    private float r = 0.0f;

    //접근해야 하는 컴포넌트는 반드시 변수에 할당한 후 사용
    private Transform tr;
    //이동 속도 변수(public으로 선언되어 Inspector에 노출됨)
    public float moveSpeed = 10.0f;
    //회전 속도 변수
    public float rotSpeed = 80.0f;

    //인스펙터 뷰에 표시할 애니메이션 클래스 변수
    public PlayerAnim playerAnim;
    //animation 컴포넌트를 저장하기위한 변수
    [System.NonSerialized] //private와 같은역할
    public Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        //스크립트가 실행된 후 처음 수행되는 Start 함수에서 TransForm 컴포넌트를 할당
        tr = GetComponent<Transform>();

        //animation 컴포넌트를 변수에 할당
        anim = GetComponent<Animation>();
        //animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        anim.clip = playerAnim.idle;
        anim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");

        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate(이동 방향 * 속도 * 변위값 * Time.deltaTime, 기준좌표)
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        //Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * rotSpeed * Time.deltaTime * r);

        Debug.Log("h=" + h.ToString());
        Debug.Log("v=" + v.ToString());
    }
}
