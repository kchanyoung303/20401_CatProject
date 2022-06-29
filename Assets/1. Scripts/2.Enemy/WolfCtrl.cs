using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WolfCtrl : MonoBehaviour
{
    public GameObject hudDamageText;
    public Transform hudPos;
    public GameObject dropfood;
    public List<GameObject> food = new List<GameObject>();
    public float waterValue;
    public float hungryValue;
    public enum SkullState { None, Idle, Move, Wait, GoTarget, Atk, Damage, Die }
    public float DelaySecond = 1.2f;

    public Slider hpBar;
    [Header("�⺻ �Ӽ�")]

    public SkullState skullState = SkullState.None;

    public float spdMove = 1f;
    public float spdRun = 1f;
    private float NowSpd;

    public GameObject targetCharactor = null;

    public Transform targetTransform = null;

    public Vector3 posTarget = Vector3.zero;
 
    private Animation skullAnimation = null;
    private Transform skullTransform = null;
    public GameObject EffectPosition = null;
    [Header("�ִϸ��̼� Ŭ��")]
    public AnimationClip IdleAnimClip = null;
    public AnimationClip MoveAnimClip = null;
    public AnimationClip AtkAnimClip = null;
    public AnimationClip DamageAnimClip = null;
    public AnimationClip DieAnimClip = null;


    [Header("�����Ӽ�")]
    public float hpvalue = 100f;
    public float hp = 100f;
    public float attackDamage;
    public float AtkRange = 1.5f;
    public GameObject effectDamage = null;
    public GameObject AtkPlayerEffect = null;
    public GameObject effectDie = null;

    private Tweener effectTweener = null;
    private SkinnedMeshRenderer skinnedMeshRenderer = null;
    public CapsuleCollider AtkCapsuleCollider = null;
    private PlayerCtrl playerctrl;
    private GameObject player;

    void OnDmgAnmationFinished()
    {
        Debug.Log("Dmg Animation finished");
    }



    /// <summary>
    /// �ִϸ��̼� �̺�Ʈ�� �߰����ִ� ��. 
    /// </summary>
    /// <param name="clip">�ִϸ��̼� Ŭ�� </param>
    /// <param name="funcName">�Լ��� </param>
    void OnAnimationEvent(AnimationClip clip, string funcName)
    {
        AnimationEvent retEvent = new AnimationEvent();
        retEvent.functionName = funcName;
        retEvent.time = clip.length - 0.4f;
        clip.AddEvent(retEvent);
    }


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerctrl = player.GetComponent<PlayerCtrl>();
        skullState = SkullState.Idle;

        NowSpd = spdMove;
        skullAnimation = GetComponent<Animation>();
        skullTransform = GetComponent<Transform>();


        skullAnimation[IdleAnimClip.name].wrapMode = WrapMode.Loop;
        skullAnimation[MoveAnimClip.name].wrapMode = WrapMode.Loop;
        skullAnimation[AtkAnimClip.name].wrapMode = WrapMode.Once;
        skullAnimation[DamageAnimClip.name].wrapMode = WrapMode.Once;

        skullAnimation[DamageAnimClip.name].layer = 10;
        skullAnimation[DieAnimClip.name].wrapMode = WrapMode.Once;
        skullAnimation[DieAnimClip.name].layer = 10;


        OnAnimationEvent(DamageAnimClip, "OnDmgAnmationFinished");

        skinnedMeshRenderer = this.skullTransform.Find("Wolf 1").GetComponent<SkinnedMeshRenderer>();

        
    }
    public void OnAtkAnmationFinished()
    {
        SoundManager.Instance.SetEffectSoundClip(4);
        Debug.Log("Atk Animation finished");
        playerctrl.GetComponent<PlayerCtrl>().GetDamege(attackDamage);

        Instantiate(AtkPlayerEffect, playerctrl.transform.position, Quaternion.identity);

    }

    void CkState()
    {
        switch (skullState)
        {
            case SkullState.Idle:

                setIdle();
                break;
            case SkullState.GoTarget:
            case SkullState.Move:
                setMove();
                break;
            case SkullState.Atk:
                setAtk();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

        CkState();
        AnimationCtrl();
        HpBarUpdate();
    }
    void HpBarUpdate()
    {
        hpBar.value = (float)hp / (float)hpvalue;
    }

    void setIdle()
    {
        if (targetCharactor == null)
        {
            posTarget = new Vector3(skullTransform.position.x + Random.Range(-10f, 10f),
                                    skullTransform.position.y + 1f,
                                    skullTransform.position.z + Random.Range(-10f, 10f)
                );
            Ray ray = new Ray(posTarget, Vector3.down);
            RaycastHit infoRayCast = new RaycastHit();
            if (Physics.Raycast(ray, out infoRayCast, Mathf.Infinity) == true)
            {
                posTarget.y = infoRayCast.point.y;
            }

            //skullState = SkullState.Move;
        }
        else
        {
            skullState = SkullState.GoTarget;
        }
    }


    void setMove()
    {
        //����� ������ �� ������ ���� 
        Vector3 distance = Vector3.zero;
        //��� ������ �ٶ󺸰� ���� �ִ��� 
        Vector3 posLookAt = Vector3.zero;

        //����
        switch (skullState)
        {
            // ���ƴٴϴ� ���
            case SkullState.Move:
                //���� ���� ��ġ ���� ���ΰ� �ƴϸ�
                if (posTarget != Vector3.zero)
                {
                    //��ǥ ��ġ���� �ذ� �ִ� ��ġ ���� ���ϰ�
                    distance = posTarget - skullTransform.position;

                    //���࿡ �����̴� ���� �ذ��� ��ǥ�� �� ���� ���� ���� 
                    if (distance.magnitude < AtkRange)
                    {
                        //��� ���� �Լ��� ȣ��
                        StartCoroutine(setWait());
                        //���⼭ ����
                        return;
                    }

                    //��� ������ �ٶ� �� ����. ���� ����
                    posLookAt = new Vector3(posTarget.x,
                                            //Ÿ���� ���� ���� ��찡 ������ y�� üũ
                                            skullTransform.position.y,
                                            posTarget.z);
                }
                break;
            //ĳ���͸� ���ؼ� ���� ���ƴٴϴ�  ���
            case SkullState.GoTarget:
                //��ǥ ĳ���Ͱ� ���� ��
                if (targetCharactor != null)
                {

                    //��ǥ ��ġ���� �ذ� �ִ� ��ġ ���� ���ϰ�
                    distance = targetCharactor.transform.position - skullTransform.position;
                    //���࿡ �����̴� ���� �ذ��� ��ǥ�� �� ���� ���� ���� 
                    if (distance.magnitude < AtkRange)
                    {
                        //���ݻ��·� �����մ�.
                        skullState = SkullState.Atk;
                        //���⼭ ����
                        return;
                    }
                    //��� ������ �ٶ� �� ����. ���� ����
                    posLookAt = new Vector3(targetCharactor.transform.position.x,
                                            //Ÿ���� ���� ���� ��찡 ������ y�� üũ
                                            skullTransform.position.y,
                                            targetCharactor.transform.position.z);
                }
                break;
            default:
                break;


        }

        //�ذ� �̵��� ���⿡ ũ�⸦ ���ְ� ���⸸ ����(normalized)
        Vector3 direction = distance.normalized;

        //������ x,z ��� y�� ���� �İ� ���Ŷ� ����
        direction = new Vector3(direction.x, 0f, direction.z);

        //�̵��� ���� ���ϱ�






            Vector3 amount = direction * NowSpd * Time.deltaTime;
            skullTransform.Translate(amount, Space.World);



        //ĳ���� ��Ʈ���� �ƴ� Ʈ���������� ���� ��ǥ �̿��Ͽ� �̵�

        //ĳ���� ���� ���ϱ�
        skullTransform.LookAt(posLookAt);

    }

    /// <summary>
    /// ��� ���� ���� �� 
    /// </summary>
    /// <returns></returns>
    IEnumerator setWait()
    {
        Debug.Log("Wolf wait");
        //���¸� ��� ���·� �ٲ�
        skullState = SkullState.Wait;
        //����ϴ� �ð��� �������� �ʰ� ����
        float timeWait = Random.Range(0.5f,2);
        //��� �ð��� �־� ��.
        yield return new WaitForSeconds(timeWait);
        //��� �� �ٽ� �غ� ���·� ����
        skullState = SkullState.Idle;

    }

    /// <summary>
    /// �ִϸ��̼��� ��������ִ� �� 
    /// </summary>
    void AnimationCtrl()
    {
        //���¿� ���� �ִϸ��̼� ����
        switch (skullState)
        {
            //���� �غ��� �� �ִϸ��̼� ��.
            case SkullState.Wait:
            case SkullState.Idle:
                //�غ� �ִϸ��̼� ����
                skullAnimation.CrossFade(IdleAnimClip.name);
                break;
            //������ ��ǥ �̵��� �� �ִϸ��̼� ��.
            case SkullState.Move:
            case SkullState.GoTarget:
                //�̵� �ִϸ��̼� ����
                NowSpd = spdRun;
                skullAnimation.CrossFade(MoveAnimClip.name);
                break;
            //������ ��
            case SkullState.Atk:
                Invoke("AtkDelay", 0.2f);
                break;
            //�׾��� ��
            case SkullState.Die:
                //���� ���� �ִϸ��̼� ����
                //EnemyDieDelay();
                break;
            default:
                break;

        }
    }
    ///<summary>
    ///�þ� ���� �ȿ� �ٸ� Trigger �Ǵ� ĳ���Ͱ� ������ ȣ�� �ȴ�.
    ///�Լ� ������ ��ǥ���� ������ ��ǥ���� �����ϰ� �ذ��� Ÿ�� ��ġ�� �̵� ��Ų�� 
    ///</summary>
    void AtkDelay()
    {
        this.skullAnimation.CrossFade(AtkAnimClip.name);
    }

    void OnCkTarget(GameObject target)
    {
        //��ǥ ĳ���Ϳ� �Ķ���ͷ� ����� ������Ʈ�� �ְ� 
        targetCharactor = target;
        //��ǥ ��ġ�� ��ǥ ĳ������ ��ġ ���� �ֽ��ϴ�. 
        targetTransform = targetCharactor.transform;

        //��ǥ���� ���� �ذ��� �̵��ϴ� ���·� ����
        skullState = SkullState.GoTarget;

    }
    /// <summary>
    /// �ذ� ���� ���� ���
    /// </summary>
    void setAtk()
    {
        //�ذ�� ĳ���Ͱ��� ��ġ �Ÿ� 
        float distance = Vector3.Distance(targetTransform.position, skullTransform.position); //���̴�
        NowSpd = spdMove;
        //���� �Ÿ����� �� ���� �Ÿ��� �־� ���ٸ� 
        if (distance > AtkRange + 0.5f)
        {
            //Ÿ�ٰ��� �Ÿ��� �־����ٸ� Ÿ������ �̵� 
            skullState = SkullState.GoTarget;
        }
    }


    /// <summary>
    /// �ذ� �ǰ� �浹 ���� 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //���࿡ �ذ��� ĳ���� ���ݿ� �¾Ҵٸ�
        if (other.gameObject.CompareTag("PlayerAtk") == true)
        {
            //�ذ� ü���� 10 ���� 
            hp -= playerctrl.AtkDamege;
            SoundManager.Instance.SetEffectSoundClip(0);
            GameObject hudText = Instantiate(hudDamageText);
            hudText.transform.position = -hudPos.position;
            if (hp > 0)
            {
                //�ǰ� ����Ʈ 
                Instantiate(effectDamage, other.transform.position, Quaternion.identity);

                //ü���� 0 �̻��̸� �ǰ� �ִϸ��̼��� ���� �ϰ� 
                skullAnimation.CrossFade(DamageAnimClip.name);

                //�ǰ� Ʈ���� ����Ʈ
                effectDamageTween();
            }
            else
            {
                //0 ���� ������ �ذ��� ���� ���·� �ٲپ��  
                skullAnimation.CrossFade(DieAnimClip.name);
                skullState = SkullState.Die;
                StartCoroutine("DieDelay");

            }
        }

    }

    IEnumerator DieDelay()
    {
        int randomfood = Random.Range(1, 3);
        Debug.Log(randomfood);
        yield return new WaitForSeconds(DelaySecond);
        //���� ���� �̺�Ʈ 
        SoundManager.Instance.SetEffectSoundClip(1);
        Instantiate(effectDie, skullTransform.position, Quaternion.identity);

        //���� ���� 
        Destroy(gameObject);

        if(randomfood==2)
        {
            Instantiate(dropfood, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
        

    }
    /// <summary>
    /// �ǰݽ� ���� ������ ��½��½ ȿ���� �ش�
    /// </summary>
    void effectDamageTween()
    {

        if (hp > 0)

        {
            //Ʈ���� ������ �� Ʈ�� �Լ��� ����Ǹ� ������ ������ �� �� �־ 
            //Ʈ�� �ߺ� üũ�� �̸� ������ ���ش�

            //��½�̴� ����Ʈ ������ �������ش�
            Color colorTo = Color.red;


            skinnedMeshRenderer.material.DOColor(colorTo, 0f).OnComplete(OnDamageTweenFinished);

        }
        else
        {

        }
    }

    /// <summary>
    /// �ǰ�����Ʈ ����� �̺�Ʈ �Լ� ȣ��
    /// </summary>
    void OnDamageTweenFinished()
    {
        //Ʈ���� ������ �Ͼ������ Ȯ���� ������ �����ش�
        skinnedMeshRenderer.material.DOColor(Color.white, 2f);
    }
}
