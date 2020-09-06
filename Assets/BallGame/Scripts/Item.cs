using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float rotateSpeed;

    void Update()
    {
        //Rotate는 매개변수 하나만 넣어주면됨
        //어떤 컴퓨터든 동일해야 하기 때문에 deltaTime
        //Space.Wolrd 하면 돌아가는거 보임
        //물리 충돌이 필요없을때는 isTrigger 체크
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
    }
}
