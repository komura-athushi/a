using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] GameObject swordBone;
    [SerializeField] GameObject[] swords;
    [SerializeField] CharacterController characterController;
    private int swordNumber = 0;
    [SerializeField] float moveSpeedScalar = 0.0f;
    private const float runSpeed = 2.3f;
    //前の移動ベクトル
    private Vector3 oldvelocity = Vector3.zero;
    //移動速度
    private Vector3 moveSpeed = Vector3.zero;
    private float interpolationFactor = 5.0f;
    [SerializeField] float attackIdleTimeLimit = 0.15f;
    [SerializeField] float movementRestriction = 0.1f;
    [SerializeField] GameObject assasinGirl;
    Vector3 oldPosition = Vector3.zero;
    [SerializeField] GameObject rootBone;
    enum EnPlayerState
    {
        enPlayerState_Idle = 0,
        enPlayerState_Walk = 1,
        enPlayerState_Run = 2,
        enPlayerState_Attack = 3
    };
    private EnPlayerState playerState = EnPlayerState.enPlayerState_Idle;
    bool IsEnableMove()
    {
        return playerState != EnPlayerState.enPlayerState_Attack;
    }
    // Start is called before the first frame update
    void Start()
    {

       
    }
   
    void Move()
    {
        //移動不可なら移動させない。
        if (IsEnableMove() == false)
        {
            return;
        }
        moveSpeed.x = 0.0f;
        moveSpeed.z = 0.0f;

        // WASD入力から、XZ平面(水平な地面)を移動する方向(velocity)を得ます
        Vector3 stickR = Vector3.zero;
        stickR.x = Input.GetAxis("Horizontal");
        stickR.z = Input.GetAxis("Vertical");
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0.0f;
        Vector3 right = Camera.main.transform.right;
        right.y = 0.0f;
        forward = forward * stickR.z;
        right = right * stickR.x;
        Vector3 velocity = forward;
        velocity += right;
        velocity = Vector3.Slerp(oldvelocity, velocity, Time.deltaTime * interpolationFactor);
        if(velocity.sqrMagnitude <= 0.01f * 0.01f)
        {
            return;
        }
        oldvelocity = velocity;
        moveSpeed = velocity * moveSpeedScalar;
        // 速度ベクトルの長さを1秒でvelocityだけ進むように調整します
        characterController.Move(moveSpeed * Time.deltaTime);
       
    }
    void Rotation()
    {
        if (Mathf.Abs(moveSpeed.x) < 0.01f
        && Mathf.Abs(moveSpeed.z) < 0.01f)
        {
            //velocity.xとvelocity.zの絶対値がともに0.001以下ということは
            //このフレームではキャラは移動していないので旋回する必要はない。
            return;
        }
        Vector3 velocityXZ = moveSpeed;
        velocityXZ.y = 0.0f;
        Quaternion rotation = Quaternion.identity;
        rotation.SetLookRotation(velocityXZ);

        transform.rotation = rotation;
    }
    void PlayAnimation()
    {
        switch (playerState)
        {
            case EnPlayerState.enPlayerState_Idle:
                playerAnimator.SetInteger("playerAnimationState", (int)EnPlayerState.enPlayerState_Idle);
                break;
            case EnPlayerState.enPlayerState_Walk:
                playerAnimator.SetInteger("playerAnimationState", (int)EnPlayerState.enPlayerState_Walk);
                break;
            case EnPlayerState.enPlayerState_Run:
                playerAnimator.SetInteger("playerAnimationState", (int)EnPlayerState.enPlayerState_Run);
                break;
            case EnPlayerState.enPlayerState_Attack:
                playerAnimator.SetInteger("playerAnimationState", (int)EnPlayerState.enPlayerState_Attack);
                break;
        }
    }
    void ProcessCommonStateTransition()
    {
        if(Input.GetButtonDown("Jump"))
        {
            moveSpeed.x = 0.0f;
            moveSpeed.z = 0.0f;
            var newOldVelocity = oldvelocity.normalized;
            newOldVelocity *= 0.01f;
            oldvelocity = newOldVelocity;
            playerState = EnPlayerState.enPlayerState_Attack;
            return;
        }

        //スティックの入力があれば
        if (Mathf.Abs(moveSpeed.x) > movementRestriction
        || Mathf.Abs(moveSpeed.z) > movementRestriction)
        {
            Vector3 velocityXZ = moveSpeed;
            velocityXZ.y = 0.0f;
          
            if (velocityXZ.sqrMagnitude >= (runSpeed * runSpeed))
            {
                playerState = EnPlayerState.enPlayerState_Run;
                
            }
            else
            {
                playerState = EnPlayerState.enPlayerState_Walk;
            }
        }
        else
        {
            playerState = EnPlayerState.enPlayerState_Idle;
        }
    }
    void ManagePlayerState()
    {
        switch (playerState)
        {
            case EnPlayerState.enPlayerState_Idle:
                ProcessCommonStateTransition();
                break;
            case EnPlayerState.enPlayerState_Walk:
                ProcessCommonStateTransition();
                break;
            case EnPlayerState.enPlayerState_Run:
                ProcessCommonStateTransition();
                break;
            case EnPlayerState.enPlayerState_Attack:
                if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
                {
                    if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
                    {
                        ProcessCommonStateTransition();
                    }
                }
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Move();
        Rotation();
        PlayAnimation();
        ManagePlayerState();

        /*if (Input.GetKey(KeyCode.A))
        {
            playerAnimator.SetInteger("playerAnimationState", 0);
            swords[0].SetActive(true);
            swords[1].SetActive(false);

            swordNumber = 0;
        }
        */
        if (Input.GetButtonDown("Fire1"))
        {
            //playerAnimator.SetInteger("playerAnimationState", 1);
            //swords[1].SetActive(true);
            //swords[0].SetActive(false);

            //swordNumber = 1;
        }
        swords[swordNumber].transform.position = swordBone.transform.position;
        swords[swordNumber].transform.rotation = swordBone.transform.rotation;
        if(playerState == EnPlayerState.enPlayerState_Attack)
        {
            Vector3 rootPosition = rootBone.transform.position;
            Vector3 vecocity = rootPosition - oldPosition;
            characterController.Move(vecocity);
            Vector3 assasinGirlVelocity = characterController.transform.position - rootPosition;
            assasinGirl.transform.position += assasinGirlVelocity;
            Debug.Log(rootPosition);
            //transform.position = rootBone.transform.position;
        }
        else
        {
            assasinGirl.transform.position = transform.position;
        }
      
        assasinGirl.transform.rotation = transform.rotation;
        oldPosition = rootBone.transform.position;
    }
}
