using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCtrl : MonoBehaviour
{
    //총알 프리팹
    public GameObject bullet;
    //탄피 추출 파티클
    public ParticleSystem cartridge;
    //총구 화염 파티클
    public ParticleSystem muzzleFlash;

    //총알 발사 좌표
    public Transform firePos;

    // Start is called before the first frame update
    void Start()
    {
        //FirePos 하위에 있는 컴포넌트 추출
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        //Bullet 프리팹을 동적으로 생성
        Instantiate(bullet, firePos.position, firePos.rotation);
        //파티클 실행
        cartridge.Play();
        //총구 화염 파티클 실행
        muzzleFlash.Play();
    }
}
