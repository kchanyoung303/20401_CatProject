using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    private bool pickupActivated = false;  // ������ ���� �����ҽ� True 
    public Text actiontext;
    RaycastHit hit;
    private Item item = null;
    public float hpvalue = 100f;
    public int AtkDamege=10;

    public float watervalue = 100f;

    public float hungryvalue = 100f;
    private float Watertime = 1f; //�� �ʸ��� 
    private float Hugrytime = 2f;
    private float Watercurtime; //Ÿ�̸�
    private float Hugrycurtime;

    //ĳ���� ���� �̵� �ӵ� (�ȱ�)
    public float walkMoveSpd = 2.0f;

    //ĳ���� ���� �̵� �ӵ� (�޸���)
    public float runMoveSpd = 3.5f;

    //ĳ���� ȸ�� �̵� �ӵ� 
    public float rotateMoveSpd = 100.0f;

    //ĳ���� ȸ�� �������� ���� ������ �ӵ�
    public float rotateBodySpd = 2.0f;

    //ĳ���� �̵� �ӵ� ���� ��
    public float moveChageSpd = 0.1f;

    //���� ĳ���� �̵� ���� �� 
    private Vector3 vecNowVelocity = Vector3.zero;

    //���� ĳ���� �̵� ���� ���� 
    private Vector3 vecMoveDirection = Vector3.zero;

    //CharacterController ĳ�� �غ�
    private CharacterController controllerCharacter = null;

    //ĳ���� CollisionFlags �ʱⰪ ����
    private CollisionFlags collisionFlagsCharacter = CollisionFlags.None;

    //ĳ���� �߷°�
    private float gravity = 9.8f;

    //ĳ���� �߷� �ӵ� ��
    private float verticalSpd = 0f;

    //ĳ���� ���� ���� �÷���
    private bool stopMove = false;

    public float hp = 3f;

    [Header("�ִϸ��̼� �Ӽ�")]
    public AnimationClip animationClipIdle = null;
    public AnimationClip animationClipWalk = null;
    public AnimationClip animationClipRun = null;
    public AnimationClip animationClipAtkStep_1 = null;
    public AnimationClip animationClipAtkStep_2 = null;
    //public AnimationClip animationClipAtkStep_3 = null;
    //public AnimationClip animationClipAtkStep_4 = null;

    //������Ʈ�� �ʿ��մϴ� 
    private Animation animationPlayer = null;


    //ĳ���� ����  ĳ���� ���¿� ���� animation�� ǥ��
    public enum PlayerState { None, Idle, Walk, Run, Attack, Skill }

    [Header("ĳ���ͻ���")]
    public PlayerState playerState = PlayerState.None;

    //���� sub state �߰� 
    public enum PlayerAttackState { atkStep_1,atkStep_2, /*atkStep_3, atkStep_4*/ }

    //�⺻ ���� ���� �� �߰� 
    public PlayerAttackState playerAttackState = PlayerAttackState.atkStep_1;

    //���� ���� ���� Ȱ��ȭ�� ���� flag
    public bool flagNextAttack = false;

    private PlayerBar playerbar = null;
    //[Header("��������")]
    ////������ ���� ������
    //public TrailRenderer AtkTrailRenderer = null;

    //���⿡ �ִ� �ݶ��̴� ĳ��
    public CapsuleCollider AtkCapsuleCollider = null;


    //[Header("��ų����")]
    //public AnimationClip skillAnimClip = null;
    //public GameObject skillEffect = null;


    // Start is called before the first frame update
    void Start()
    {

        //CharacterController ĳ��
        controllerCharacter = GetComponent<CharacterController>();

        //Animation component ĳ��
        animationPlayer = GetComponent<Animation>();
        //Animation Component �ڵ� ��� ����
        animationPlayer.playAutomatically = false;
        //Ȥ�ó� ������� Animation �ִٸ�? ���߱�
        animationPlayer.Stop();

        //�ʱ� �ִϸ��̼��� ���� Enum
        playerState = PlayerState.Idle;

        //animation WrapMode : ��� ��� ���� 
        animationPlayer[animationClipIdle.name].wrapMode = WrapMode.Loop;
        animationPlayer[animationClipWalk.name].wrapMode = WrapMode.Loop;
        animationPlayer[animationClipWalk.name].wrapMode = WrapMode.Loop;
        animationPlayer[animationClipAtkStep_1.name].wrapMode = WrapMode.Once;
        animationPlayer[animationClipAtkStep_2.name].wrapMode = WrapMode.Once;
        //animationPlayer[animationClipAtkStep_3.name].wrapMode = WrapMode.Once;
        //animationPlayer[animationClipAtkStep_4.name].wrapMode = WrapMode.Once;

        //animationPlayer[skillAnimClip.name].wrapMode = WrapMode.Once;

        //�̺�Ʈ �Լ� ���� 
        SetAnimationEvent(animationClipAtkStep_1, "OnPlayerAttackFinshed");
        SetAnimationEvent(animationClipAtkStep_2, "OnPlayerAttackFinshed");
        //SetAnimationEvent(animationClipAtkStep_3, "OnPlayerAttackFinshed");
        //SetAnimationEvent(animationClipAtkStep_4, "OnPlayerAttackFinshed");

        //SetAnimationEvent(skillAnimClip, "OnSkillAnimFinished");
    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(this.transform.position + Vector3.up * 0.5f, this.transform.forward * 5f, Color.red);
        EatBird();
        EatWolf();
        DrinkWater();
        item = GetComponent<Item>();
        playerbar = GetComponent<PlayerBar>();
        //ĳ���� �̵� 
        Move();
        // Debug.Log(getNowVelocityVal());
        //ĳ���� ���� ���� 
        vecDirectionChangeBody();

        //���� ���¿� ���߾ �ִϸ��̼��� ��������ݴϴ�
        AnimationClipCtrl();

        //�÷��̾� ���� ���ǿ� ���߾� �ִϸ��̼� ��� 
        ckAnimationState();

        //���� ���콺 Ŭ������ ���� ���Ӱ���
        InputAttackCtrll();

        //�߷� ����
        setGravity();

        //���ݰ��� ������Ʈ ����
        AtkComponentCtrl();

        minusWater();
        minusHungry();
    }
    void minusWater()
    {
        Watercurtime += Time.deltaTime;
        if (Watertime <= Watercurtime&&watervalue>0f)
        {
            watervalue -= 1f;
            Watercurtime = 0f;
        }
    }
    void minusHungry()
    {
        Hugrycurtime += Time.deltaTime;
        if(Hugrytime <= Hugrycurtime&&hungryvalue>0f)
        {
            hungryvalue -= 1f;
            Hugrycurtime = 0f;
        }
    }

    void GetDamege(int Damege)
    {
        hp -= Damege;
    }
    /// <summary>
    /// �̵��Լ� �Դϴ� ĳ����
    /// </summary>
    void Move()
    {
        if (stopMove == true)
        {
            return;
        }

        Transform CameraTransform = Camera.main.transform;
        //���� ī�޶� �ٶ󺸴� ������ ����� � �����ΰ�.
        Vector3 forward = CameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;

        //forward.z, forward.x
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

        //Ű�Է� 
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        //�ɸ��Ͱ� �̵��ϰ��� �ϴ� ���� 
        Vector3 targetDirection = horizontal * right + vertical * forward;

        //���� �̵��ϴ� ���⿡�� ���ϴ� �������� ȸ�� 

        vecMoveDirection = Vector3.RotateTowards(vecMoveDirection, targetDirection, rotateMoveSpd * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
        vecMoveDirection = vecMoveDirection.normalized;
        //ĳ���� �̵� �ӵ�
        float spd = walkMoveSpd;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            playerState = PlayerState.Run;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            playerState = PlayerState.Walk;
        }
        //���࿡ playerState�� Run�̸� 
        if (playerState == PlayerState.Run)
        {
            spd = runMoveSpd;
        }
        else if (playerState == PlayerState.Walk)
        {
            spd = walkMoveSpd;
        }

        //�߷� ����
        Vector3 vecGravity = new Vector3(0f, verticalSpd, 0f);


        // ������ �̵� ��
        Vector3 moveAmount = (vecMoveDirection * spd * Time.deltaTime) + vecGravity;

        collisionFlagsCharacter = controllerCharacter.Move(moveAmount);


    }

    public void EatNMeat()
    {
        watervalue += GetComponent<Item>().Watervalue;
        hungryvalue += GetComponent<Item>().Hugryvalue;
    }

    /// <summary>
    /// ���� �� �ɸ��� �̵� �ӵ� �������� ��  
    /// </summary>
    /// <returns>float</returns>
    float getNowVelocityVal()
    {
        //���� ĳ���Ͱ� ���� �ִٸ� 
        if (controllerCharacter.velocity == Vector3.zero)
        {
            //��ȯ �ӵ� ���� 0
            vecNowVelocity = Vector3.zero;
        }
        else
        {

            //��ȯ �ӵ� ���� ���� /
            Vector3 retVelocity = controllerCharacter.velocity;
            retVelocity.y = 0.0f;

            vecNowVelocity = Vector3.Lerp(vecNowVelocity, retVelocity, moveChageSpd * Time.fixedDeltaTime);

        }
        //�Ÿ� ũ��
        return vecNowVelocity.magnitude;
    }

    /// <summary>
	/// GUI SKin
	/// </summary>
    private void OnGUI()
    {
        var labelStyle = new GUIStyle();
        labelStyle.fontSize = 15;
        labelStyle.normal.textColor = Color.red;

        //if (controllerCharacter != null && controllerCharacter.velocity != Vector3.zero)
        //{

            //ĳ���� ���� �ӵ�
            float _getVelocitySpd = getNowVelocityVal();
            GUILayout.Label("�ȱ� �ӵ� : " + walkMoveSpd.ToString(), labelStyle);

            //���� ĳ���� ���� + ũ��
            GUILayout.Label("�޸��� �ִ� �ӵ� : " + runMoveSpd.ToString(), labelStyle);

            //����  ����� ũ�� �ӵ�
            GUILayout.Label("������� ũ�� �ӵ� : " + vecNowVelocity.magnitude.ToString(), labelStyle);
        GUILayout.Label("���� ����� : " + hungryvalue.ToString(), labelStyle);
        GUILayout.Label("���� ���� : " + watervalue.ToString(), labelStyle);
        //}
        GUILayout.Label("���� HP : " + hp, labelStyle);

        GUILayout.Label("���� ���� �ð� : " + Watercurtime, labelStyle);
    }
    /// <summary>
    /// ĳ���� ���� ���� ���� �Լ�
    /// </summary>
    void vecDirectionChangeBody()
    {
        //ĳ���� �̵� ��
        if (getNowVelocityVal() > 0.0f)
        {
            //�� ����  �ٶ���� �ϴ� ���� ���?
            Vector3 newForward = controllerCharacter.velocity;
            newForward.y = 0.0f;

            //�� ĳ���� ���� ���� 
            transform.forward = Vector3.Lerp(transform.forward, newForward, rotateBodySpd * Time.deltaTime);

        }
    }


    /// <summary>
    ///  �ִϸ��̼� ��������ִ� �Լ�
    /// </summary>
    /// <param name="clip">�ִϸ��̼�Ŭ��</param>
    void playAnimationByClip(AnimationClip clip)
    {
        //ĳ�� animation Component�� clip �Ҵ�
        //        animationPlayer.clip = clip;
        animationPlayer.GetClip(clip.name);
        //����
        animationPlayer.CrossFade(clip.name);
    }

    /// <summary>
    /// ���� ���¿� ���߾� �ִϸ��̼��� ���
    /// </summary>
    void AnimationClipCtrl()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                playAnimationByClip(animationClipIdle);
                break;
            case PlayerState.Walk:
                playAnimationByClip(animationClipWalk);
                break;
            case PlayerState.Run:
                playAnimationByClip(animationClipRun);
                break;
            case PlayerState.Attack:
                stopMove = true;
                //���ݻ��¿� ���� �ִϸ��̼��� ���
                AtkAnimationCrtl();
                break;
            case PlayerState.Skill:
                //playAnimationByClip(skillAnimClip);
                //stopMove = true;
                break;
        }
    }

    /// <summary>
    ///  ���� ���¸� üũ���ִ� �Լ�
    /// </summary>
    void ckAnimationState()
    {
        //���� �ӵ� ��
        float nowSpd = getNowVelocityVal();

        //���� �÷��̾� ����
        switch (playerState)
        {
            case PlayerState.Idle:
                if (nowSpd > 0.0f)
                {
                    playerState = PlayerState.Walk;
                }
                break;
            case PlayerState.Walk:
                //2.0 �ȱ� max �ӵ�
                if (nowSpd > runMoveSpd-1f)
                {
                    playerState = PlayerState.Run;
                }
                else if (nowSpd < 0.01f)
                {
                    playerState = PlayerState.Idle;
                }
                break;
            case PlayerState.Run:
                if (nowSpd < 0.5f)
                {
                    playerState = PlayerState.Walk;
                }

                if (nowSpd < 0.01f)
                {
                    playerState = PlayerState.Idle;
                }
                break;
            case PlayerState.Attack:
                break;
            case PlayerState.Skill:
                break;
        }
    }

    /// <summary>
    /// ���콺 ���� ��ư���� ���� �ϴ�  �Լ� 
    /// </summary>
    void InputAttackCtrll()
    {
        //���콺 Ŭ���� �Ͽ�����?
        if (Input.GetMouseButton(0) == true)
        {
            Debug.Log("InputAttackCtrll : " + playerState);
            //�÷��̾ ���ݻ���?
            if (playerState != PlayerState.Attack)
            {
                //�÷��̾ ���ݻ��°� �ƴϸ� ���ݻ��·� ����
                playerState = PlayerState.Attack;

                //���ݻ��� �ʱ�ȭ
                playerAttackState = PlayerAttackState.atkStep_1;
            }
            else
            {
                //�÷��̾� ���°� ���ݻ����̰� ���� ���°� Ȱ��ȭ �϶�
                //���� ���¿� ���� �з�
                switch (playerAttackState)
                {
                    case PlayerAttackState.atkStep_1:
                        if (animationPlayer[animationClipAtkStep_1.name].normalizedTime > 0.01f)
                        {
                            flagNextAttack = true;
                        }
                        break;
                    //case PlayerAttackState.atkStep_2:
                    //    if (animationPlayer[animationClipAtkStep_2.name].normalizedTime > 0.05f)
                    //    {
                    //        flagNextAttack = true;
                    //    }
                    //    break;
                    //case PlayerAttackState.atkStep_3:
                    //    if (animationPlayer[animationClipAtkStep_3.name].normalizedTime > 0.5f)
                    //    {
                    //        flagNextAttack = true;
                    //    }
                    //    break;
                    //case PlayerAttackState.atkStep_4:
                    //    if (animationPlayer[animationClipAtkStep_4.name].normalizedTime > 0.5f)
                    //    {
                    //        flagNextAttack = true;
                    //    }
                    //    break;
                    default:
                        break;
                }
            }
        }

        //���콺 ������ ��ư�� �����ٸ�
        if (Input.GetMouseButtonDown(1) == true)
        {
            //���� ĳ���� ���°� �������̸�
            if (playerState == PlayerState.Attack)
            {
                //���ݻ����� 1 �⺻ ���·�
                playerAttackState = PlayerAttackState.atkStep_1;
                flagNextAttack = false;
            }

            //�÷��̾� ���¸� ��ų ���·� ���� �Ѵ�
            playerState = PlayerState.Skill;
        }
    }


    //��ų �ִϸ��̼� ����� ������ �� �̺�Ʈ 
    void OnSkillAnimFinished()
    {
        //���� ĳ���� ��ġ ����
        Vector3 pos = transform.position;

        //ĳ���� �� ���� 2.0���� ������ �Ÿ� 
        pos += transform.forward * 2f;

        ////�� ��ġ�� ��ų ����Ʈ�� ���δ�. 
        //Instantiate(skillEffect, pos, Quaternion.identity);

        //�������� ��� ���·� �д�. 
        playerState = PlayerState.Idle;
    }





    /// <summary>
    ///  ���� �ִϸ��̼� ����� ������ ȣ��Ǵ� �ִϸ��̼� �̺�Ʈ �Լ�
    /// </summary>
    void OnPlayerAttackFinshed()
    {
        //���࿡ fightNext == true
        if (flagNextAttack == true)
        {
            //fight �ʱ�ȭ
            flagNextAttack = false;

            Debug.Log(playerAttackState);

            //���� ���� �ִϸ��̼� ���¿� ���� ���� �ִϸ��̼� ���°��� �ֱ�
            //switch (playerAttackState)
            //{

            //    case PlayerAttackState.atkStep_1:
            //        playerAttackState = PlayerAttackState.atkStep_2;

            //        Debug.Log(playerAttackState);
            //        break;
            //    case PlayerAttackState.atkStep_2:
            //        playerAttackState = PlayerAttackState.atkStep_3;
            //        break;
            //    case PlayerAttackState.atkStep_3:
            //        playerAttackState = PlayerAttackState.atkStep_4;
            //        break;
            //    case PlayerAttackState.atkStep_4:
            //        playerAttackState = PlayerAttackState.atkStep_1;
            //        break;
            //}
        }
        else
        {

            stopMove = false;

            playerState = PlayerState.Idle;

            playerAttackState = PlayerAttackState.atkStep_1;
        }
    }

    /// <summary>
    /// �ִϸ��̼� Ŭ�� ����� ���� ���� �ִϸ��̼� �̺�Ʈ �Լ��� ȣ��
    /// </summary>
    /// <param name="clip">AnimationClip</param>
    /// <param name="FuncName">event function</param>
    void SetAnimationEvent(AnimationClip animationclip, string funcName)
    {
        //���ο� �̺�Ʈ ����
        AnimationEvent newAnimationEvent = new AnimationEvent();

        //�ش� �̺�Ʈ�� ȣ�� �Լ��� funcName
        newAnimationEvent.functionName = funcName;

        newAnimationEvent.time = animationclip.length - 0.15f;

        animationclip.AddEvent(newAnimationEvent);
    }

    /// <summary>
    /// ���� �ִϸ��̼� ���
    /// </summary>
    void AtkAnimationCrtl()
    {
        //���� ���ݻ��°�?
        switch (playerAttackState)
        {
            case PlayerAttackState.atkStep_1:
                playAnimationByClip(animationClipAtkStep_1);
                playerState = PlayerState.None;
                break;
            //case PlayerAttackState.atkStep_2:
            //    playAnimationByClip(animationClipAtkStep_2);
            //    break;
                //case PlayerAttackState.atkStep_3:
                //    playAnimationByClip(animationClipAtkStep_3);
                //    break;
                //case PlayerAttackState.atkStep_4:
                //    playAnimationByClip(animationClipAtkStep_4);
                //    break;
        }
    }

    /// <summary>
    ///  ĳ���� �߷� ����
    /// </summary>
    void setGravity()
    {
        if ((collisionFlagsCharacter & CollisionFlags.CollidedBelow) != 0)
        {
            verticalSpd = 0f;
        }
        else
        {
            verticalSpd -= gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// ���ݰ��� ������Ʈ ����
    /// </summary>
    void AtkComponentCtrl()
    {
        switch (playerState)
        {
            case PlayerState.Attack:    
            case PlayerState.Skill:
                //AtkTrailRenderer.enabled = true;
                AtkCapsuleCollider.enabled = true;
                break;
            default:
                //AtkTrailRenderer.enabled = false;
                AtkCapsuleCollider.enabled = false;
                break;
        }
    }
    void DrinkWater()
    {
        Ray ray = new Ray(this.transform.position - Vector3.forward * 1.5f + Vector3.up * 0.5f, (this.transform.forward - this.transform.up));
        Debug.DrawRay(this.transform.position - Vector3.forward * 1.5f + Vector3.up * 0.5f, (this.transform.forward - this.transform.up) * 1f, Color.blue);
        if (Physics.Raycast(ray, out hit, 1f))
        {
            if (hit.transform.CompareTag("Water"))
            {
                ItemInfoAppear(2);
                if (Input.GetKeyDown(KeyCode.E))
                {

                    WaterPath();
                    ItemInfoAppear(2);
                }
            }
        }
        else
        {
            ItemInfoDisappear(2);
        }
    }
    void WaterPath()
    {
        if (watervalue > 80f)
        {
            watervalue = 100f;
        }
        else
        {
            watervalue += 80f;
        }
    }
    void EatBird()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.5f, this.transform.forward);
        if (Physics.Raycast(ray, out hit, 5f))
        {

            if (hit.transform.CompareTag("BirdFood"))
            {
                ItemInfoAppear(1);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Destroy(hit.transform.gameObject);
                    ItemInfoDisappear(1);
                    BirdFoodPath();
                    Debug.Log("Birdpath");

                }
            }
        }
        else
        {
            ItemInfoDisappear(1);
        }


    }
    void EatWolf()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.5f, this.transform.forward);
        if (Physics.Raycast(ray, out hit, 5f))
        {

            if (hit.transform.CompareTag("WolfFood"))
            {
                ItemInfoAppear(1);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Destroy(hit.transform.gameObject);
                    ItemInfoDisappear(1);
                    WolfFoodPath();

                }
            }
        }
        else
        {
            ItemInfoDisappear(1);
        }
    }

    void BirdFoodPath()
    {
        if (hungryvalue > 90f)
        {
            hungryvalue = 100f;

        }
        else
        {
            hungryvalue += 10f;
        }
        if (watervalue >= 0)
        {
            watervalue -= 10f;
        }
    }

    void WolfFoodPath()
    {
        Debug.Log("WolfPath");
        int randomStat = Random.Range(1, 3);

        switch(randomStat)
        {
            case 1:
                runMoveSpd += 1f;
                break;
            case 2:
                if(runMoveSpd > walkMoveSpd+3f)
                {
                    walkMoveSpd += 1f;
                }
                else
                {
                    runMoveSpd += 1f;
                }
                break;
            
        }
    }

    private void ItemInfoAppear(int value)
    {
        pickupActivated = true;
        actiontext.gameObject.SetActive(true);

        switch (value)
        {
            case 1:
                actiontext.text = "������ ȹ�� " + "<color=yellow>" + "E" + "</color>";
                break;
            case 2:
                actiontext.text = "�� ���ñ� " + "<color=blue>" + "E" + "</color>";
                break;
        }

    }
    private void ItemInfoDisappear(int value)
    {
        pickupActivated = false;
        actiontext.gameObject.SetActive(false);
    }
}
