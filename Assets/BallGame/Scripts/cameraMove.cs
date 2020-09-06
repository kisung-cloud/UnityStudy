using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour
{
    Transform playerTransform;
    Vector3 offset;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - playerTransform.position; 
    }

    void LateUpdate()
    {
        //offset 해줘야 벡터 일정거리 유지하면서 따라다님
        transform.position = playerTransform.position + offset;
    }
}
