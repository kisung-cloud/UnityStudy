﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //적 캐릭터의 상태를 표현하기 위한 열거형 변수 정의
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }
    
    //상태를 저장할 변수
    public State state = State.PATROL;

    //주인공의 위치를 저장할 변수
    private Transform playerTr;
    //적 캐릭터의 위치를 저장할 변수
    private Transform enemyTr;
    //Amimator 컴포넌트를 저장할 변수
    private Animator animator;

    //공격 사정거리
    public float attackDist = 5.0f;
    //추적 사정거리
    public float traceDist = 10.0f;

    //사망 여부를 판단할 변수
    public bool isDie = false;

    //코루틴에서 사용할 지연시간 변수
    private WaitForSeconds ws;
    //이동을 제어하는 MoveAgent 클래스를 저장할 변수
    private MoveAgent moveAgent;
    //총알 발사를 제어하는 EnemyFire 클래스를 저장할 변수
    private EnemyFire enemyFire;
    //시야각 및 추적 반경을 제어하는 EnemyFOV 클래스를 저장할 변수
    private EnemyFOV enemyFOV;


    //애니메이터 컨트롤러에 정의한 파라미터의 해시값을 미리 추출
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    void Awake()
    {
        //주인공 게임오브젝트 추출
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        //주인공의 Transform 컴포넌트 추출
        if (player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }
        //적 캐릭터의 Transform 컴포넌트 추출
        enemyTr = GetComponent<Transform>();
        //Animator 컴포넌트 추출
        animator = GetComponent<Animator>();
        //이동을 제어하는 MoveAgent 클래스를 추출
        moveAgent = GetComponent<MoveAgent>();
        //총알 발사를 제어하는 EnemyFire 클래스를 추출
        enemyFire = GetComponent<EnemyFire>();
        //시야각 및 추적 반경을 제어하는 EnemyFOV 클래스를 추출
        enemyFOV = GetComponent<EnemyFOV>();


        //코루틴의 지연시간 생성
        ws = new WaitForSeconds(0.3f);
        //Cycle Offset 값을 불규칙하게 변경
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        //Speed 값을 불규칙하게 변경
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 1.2f));
    }

    void OnEnable()
    {
        //CheckState 코루틴 함수 실행
        StartCoroutine(CheckState());
        //Action 코루틴 함수 실행
        StartCoroutine(Action());

        Damage.OnPlayerDie += this.OnPlayerDie;
    }

    void OnDisable()
    {
        Damage.OnPlayerDie -= this.OnPlayerDie;
    }

    //적 캐릭터의 상태를 검사하는 코루틴 함수
    IEnumerator CheckState()
    {
        //오브젝트 풀에 생성 시 다른 스크립트의 초기화를 위해 대기
        yield return new WaitForSeconds(1.0f);
        
        //적 캐릭터가 사망하기 전까지 도는 무한루프
        while (!isDie)
        {
            //상태가 사망이면 코루틴 함수를 종료시킴
            if (state == State.DIE) yield break;

            //주인공과 적 캐릭터 간의 거리를 계산
            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            //공격 사정거리 이내인 경우
            if (dist <= attackDist)
            {
                //주인공과의 거리에 장애물 여부를 판단
                if (enemyFOV.isViewPlayer())
                    state = State.ATTACK;       //장애물이 없으면 공격모드
                else
                    state = State.TRACE;        //장애물이 있으면 추적 모드
            }//추적 반경 및 시야각에 들어왔는지를 판단
            else if (enemyFOV.isTracePlayer())
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
            //0.3초 동안 대기하는 동안 제어권을 양보
            yield return ws;
        }
    }

    //상태에 따라 적 캐릭터의 행동을 처리하는 코루틴 함수
    IEnumerator Action()
    {
        //적 캐릭터가 사망할 때까지 무한루프
        while (!isDie)
        {
            yield return ws;
            //상태에 따라 분기 처리
            switch (state)
            {
                case State.PATROL:
                    //총알 발사 정지
                    enemyFire.isFire = false;
                    //순찰 모드를 활성화
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    //총알 발사 정지
                    enemyFire.isFire = false;
                    //주인공의 위치를 넘겨 추적 모드로 변경
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);

                    //총알 발사 시작
                    if(enemyFire.isFire == false)
                    {
                        enemyFire.isFire = true;
                    }
                    break;
                case State.DIE:
                    this.gameObject.tag = "Untagged";

                    isDie = true;
                    enemyFire.isFire = false;
                    //순찰 및 추적을 정지
                    moveAgent.Stop();
                    //사망 애니메이션의 종류를 지정
                    animator.SetInteger(hashDieIdx, Random.Range(0, 3));
                    //사망 애니메이션 실행
                    animator.SetTrigger(hashDie);
                    //Capsule Collider 컴포넌트를 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    void Update()
    {
        //Speed 파라미터에 이동속도를 전달
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        //모드 코루틴 함수를 종료시킴
        StopAllCoroutines();

        animator.SetTrigger(hashPlayerDie);
    }
}
