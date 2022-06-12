using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WolfCtrl : MonoBehaviour
{
    //�ذ� ����
    public enum SkullState { None, Idle, Move, Wait, GoTarget, Atk, Damage, Die }
    public float DelaySecond = 1f;

    //�ذ� �⺻ �Ӽ�
    [Header("�⺻ �Ӽ�")]
    //�ذ� �ʱ� ����
    public SkullState skullState = SkullState.None;
    //�ذ� �̵� �ӵ�
    public float spdMove = 1f;
    //�ذ��� �� Ÿ��
    public GameObject targetCharactor = null;
    //�ذ��� �� Ÿ�� ��ġ���� (�Ź� �� ã������)
    public Transform targetTransform = null;
    //�ذ��� �� Ÿ�� ��ġ(�Ź� �� ã����)
    public Vector3 posTarget = Vector3.zero;

    //�ذ� �ִϸ��̼� ������Ʈ ĳ�� 
    private Animation skullAnimation = null;
    //�ذ� Ʈ������ ������Ʈ ĳ��
    private Transform skullTransform = null;

    [Header("�ִϸ��̼� Ŭ��")]
    public AnimationClip IdleAnimClip = null;
    public AnimationClip MoveAnimClip = null;
    public AnimationClip AtkAnimClip = null;
    public AnimationClip DamageAnimClip = null;
    public AnimationClip DieAnimClip = null;

    [Header("�����Ӽ�")]
    //�ذ� ü��
    public int hp = 100;
    //�ذ� ���� �Ÿ�
    public float AtkRange = 1.5f;
    //�ذ� �ǰ� ����Ʈ
    public GameObject effectDamage = null;
    //�ذ� ���� ����Ʈ
    public GameObject effectDie = null;

    private Tweener effectTweener = null;
    private SkinnedMeshRenderer skinnedMeshRenderer = null;


    void OnAtkAnmationFinished()
    {
        Debug.Log("Atk Animation finished");
    }

    void OnDmgAnmationFinished()
    {
        Debug.Log("Dmg Animation finished");
    }

    void OnDieAnmationFinished()
    {
        Debug.Log("Die Animation finished");

        effectDamageTween();
    }

    /// <summary>
    /// �ִϸ��̼� �̺�Ʈ�� �߰����ִ� ��. 
    /// </summary>
    /// <param name="clip">�ִϸ��̼� Ŭ�� </param>
    /// <param name="funcName">�Լ��� </param>
    void OnAnimationEvent(AnimationClip clip, string funcName)
    {
        //�ִϸ��̼� �̺�Ʈ�� ����� �ش�
        AnimationEvent retEvent = new AnimationEvent();
        //�ִϸ��̼� �̺�Ʈ�� ȣ�� ��ų �Լ���
        retEvent.functionName = funcName;
        //�ִϸ��̼� Ŭ�� ������ �ٷ� ������ ȣ��
        retEvent.time = clip.length - 0.1f;
        //�� ������ �̺�Ʈ�� �߰� �Ͽ���
        clip.AddEvent(retEvent);
    }


    // Start is called before the first frame update
    void Start()
    {
        //ó�� ���� ������
        skullState = SkullState.Idle;

        //�ִϸ���, Ʈ������ ������Ʈ ĳ�� : �������� ã�� ������ �ʰ�
        skullAnimation = GetComponent<Animation>();
        skullTransform = GetComponent<Transform>();

        //�ִϸ��̼� Ŭ�� ��� ��� ����
        skullAnimation[IdleAnimClip.name].wrapMode = WrapMode.Loop;
        skullAnimation[MoveAnimClip.name].wrapMode = WrapMode.Loop;
        skullAnimation[AtkAnimClip.name].wrapMode = WrapMode.Once;
        skullAnimation[DamageAnimClip.name].wrapMode = WrapMode.Once;

        //�ִϸ��̼� ������ ���� ũ�� �ø�
        skullAnimation[DamageAnimClip.name].layer = 10;
        skullAnimation[DieAnimClip.name].wrapMode = WrapMode.Once;
        skullAnimation[DieAnimClip.name].layer = 10;

        //���� �ִϸ��̼� �̺�Ʈ �߰�
        OnAnimationEvent(AtkAnimClip, "OnAtkAnmationFinished");
        OnAnimationEvent(DamageAnimClip, "OnDmgAnmationFinished");
        OnAnimationEvent(DieAnimClip, "OnDieAnmationFinished");

        //��Ų�Ž� ĳ��
        skinnedMeshRenderer = skullTransform.Find("Wolf 1").GetComponent<SkinnedMeshRenderer>();
    }

    /// <summary>
    /// �ذ� ���¿� ���� ������ �����ϴ� �Լ� 
    /// </summary>
    void CkState()
    {
        switch (skullState)
        {
            case SkullState.Idle:
                //�̵��� ���õ� RayCast��
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
    }

    /// <summary>
    /// �ذ� ���°� ��� �� �� ���� 
    /// </summary>
    void setIdle()
    {
        if (targetCharactor == null)
        {
            posTarget = new Vector3(skullTransform.position.x + Random.Range(-10f, 10f),
                                    skullTransform.position.y + 1000f,
                                    skullTransform.position.z + Random.Range(-10f, 10f)
                );
            Ray ray = new Ray(posTarget, Vector3.down);
            RaycastHit infoRayCast = new RaycastHit();
            if (Physics.Raycast(ray, out infoRayCast, Mathf.Infinity) == true)
            {
                posTarget.y = infoRayCast.point.y;
            }
            skullState = SkullState.Move;
        }
        else
        {
            skullState = SkullState.GoTarget;
        }
    }

    /// <summary>
    /// �ذ� ���°� �̵� �� �� �� 
    /// </summary>
    void setMove()
    {
        //����� ������ �� ������ ���� 
        Vector3 distance = Vector3.zero;
        //��� ������ �ٶ󺸰� ���� �ִ��� 
        Vector3 posLookAt = Vector3.zero;

        //�ذ� ����
        switch (skullState)
        {
            //�ذ��� ���ƴٴϴ� ���
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
        Vector3 amount = direction * spdMove * Time.deltaTime;

        //ĳ���� ��Ʈ���� �ƴ� Ʈ���������� ���� ��ǥ �̿��Ͽ� �̵�
        skullTransform.Translate(amount, Space.World);
        //ĳ���� ���� ���ϱ�
        skullTransform.LookAt(posLookAt);

    }

    /// <summary>
    /// ��� ���� ���� �� 
    /// </summary>
    /// <returns></returns>
    IEnumerator setWait()
    {
        //�ذ� ���¸� ��� ���·� �ٲ�
        skullState = SkullState.Wait;
        //����ϴ� �ð��� �������� �ʰ� ����
        float timeWait = Random.Range(1f, 3f);
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
        //�ذ��� ���¿� ���� �ִϸ��̼� ����
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
        skullAnimation.CrossFade(AtkAnimClip.name);
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
            hp -= 10;
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
                EnemyDieDelay();
            }
        }
    }
    IEnumerator EnemyDieDelay()
    {
        yield return new WaitForSeconds(DelaySecond);

        OnDieAnmationFinished();
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
            //���� ���� �̺�Ʈ 
            Instantiate(effectDie, skullTransform.position, Quaternion.identity);

            //���� ���� 
            Destroy(gameObject);
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