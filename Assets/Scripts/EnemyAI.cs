using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : AYEStatusBehaviour<AIStatus>
{
    [SerializeField] Health myHealth = null;
    [SerializeField] EnemyMovement myMovement = null;
    [SerializeField] Animator animator = null;
    [Tooltip("�̤j�ݩR�ɶ�")]
    [SerializeField] float maxIdleTime = 5f;
    [Tooltip("�̤p�ݩR�ɶ�")]
    [SerializeField] float minIdleTime = 3f;
    [SerializeField] Image healthBarUI = null;

    [Header("����")]
    [Tooltip("�̤j�榸���̮ɶ� :�v�T�樫�Z���Ψ����W�v")]
    [SerializeField] float maxWalkTime = 5f;
    [SerializeField] float maxHomeDistance = 10f;
    [SerializeField] float miniHomeDistance = 2f;
    Vector3 homePosition = Vector3.zero;

    [Header("�l�y")]
    [Tooltip("�l�y�d��b�|")]
    [SerializeField] float huntingRange = 5f;
    float currentHuntingRange = 0f;
    [Tooltip("�l�y��������")]
    [SerializeField] float huntingAngle = 90f;
    float currentHuntingAngle = 0f;
    float huntPower = 3f;
    [SerializeField][Range(0.1f,1f)] float huntingRecoverLevel = 0.5f;
    [Tooltip("�̤j�l�y��O:�Y�L��O�N�|����l�y")]
    [SerializeField] float maxHuntPower = 10f;


    [Header("����")]
    [Tooltip("�����d��b�|")]
    [SerializeField] float attackRange = 2f;
    [Tooltip("�����d�򮰧Ψ���")]
    [SerializeField] float attackAngle = 60f;
    [Tooltip("�w�]�����O: �Y�ʵe�L���Ѽƭ�,�N�ϥΦ���")]
    [SerializeField] float defaultAttackDamage = 15f;
    [Tooltip("�������A **�����P�ʵe�ƶq�ۦP**")]
    [SerializeField] int attackTypeAmount = 4;
    Health targetHealth = null;
    [SerializeField] Image debugImage = null;
    [SerializeField] CreateItem createItem = null;
    Vector3 maybeAttackPos = Vector3.zero;

    [SerializeField] bool revenger = true;
    [SerializeField] bool willGuarding = false;
    [SerializeField] bool willCallFriend = true;
    [SerializeField] bool mainCaller = false;
    [SerializeField] float huntingTime = 5f;
    [SerializeField] float callFriednsRange = 20f;
    [SerializeField] float getUpDistance = 20f;
    /// <summary>�O�_��Q�B�ͩI�s</summary>
    bool isCalled = false;
    /// <summary>�u�\���W��������V</summary>
    [SerializeField] List<Renderer> renderList = new List<Renderer>();
    private void Awake()
    {
        SaveManager.instance.Act_OnSave += Save;

        AddStatus(AIStatus.Sleep, ToSleep, Sleeping, GetUp);
        AddStatus(AIStatus.Idle, ToIdle, Idling, StopIdle);
        AddStatus(AIStatus.Guard, ToGuard, Guarding, StopGuard);
        AddStatus(AIStatus.Home, GoHome, Homing, StopGoingHome);
        AddStatus(AIStatus.Hunt, StartHunt, Hunting, StopHunt);
        AddStatus(AIStatus.Attack, ToAttack, Attacking, StopAttack);
        AddStatus(AIStatus.Hit, GetHit, Hitting, StopHit);
        AddStatus(AIStatus.Die, ToDie, Dying, Dead);
        AddStatus(AIStatus.LookBack, LookBack, Looking, StopLooking);
        AddStatus(AIStatus.Searching, Search, Searching, StopSearching);
        AddStatus(AIStatus.CallFriends, CallFriends, Calling, StopCalling);
        
        homePosition = this.transform.position;
        currentHuntingAngle = huntingAngle;
        currentHuntingRange = huntingRange;
        debugImage.enabled = false;
        healthBarUI.enabled = true;
    }

    private void OnDestroy()
    {
        SaveManager.instance.Act_OnSave -= Save;
    }

    void Save()
    {
        if (SaveManager.instance.gameData.aiDatas != null)
        {
            //�������{������ƥH�K���m���e (�]��List���e�S��k�Q�л\)
            for (int i = 0; i < SaveManager.instance.gameData.aiDatas.Count; i++)
            {
                if (SaveManager.instance.gameData.aiDatas[i].objName == this.gameObject.name)
                {
                    SaveManager.instance.gameData.aiDatas.RemoveAt(i);
                    break;
                }
            }
        }
        


        AIdata tempData = new AIdata();
        tempData.objName = this.gameObject.name;
        tempData.pos = new Vector4(
            transform.position.x,
            transform.position.y,
            transform.position.z,
            transform.rotation.eulerAngles.y);
        tempData.hp = myHealth.health;
        SaveManager.instance.gameData.aiDatas.Add(tempData);
    }

    #region �ʦL

    Transform player = null;

    void ToSleep()
    {
        transform.Rotate(0f, Random.Range(0f, 360f), 0f,Space.Self);
        


        if (SaveManager.instance.gameData.deadEnemy != null)
        {
            for (int i = 0; i < SaveManager.instance.gameData.deadEnemy.Count; i++)
            {
                if (SaveManager.instance.gameData.deadEnemy[i] == this.gameObject.name) Destroy(gameObject);//�p�G�w�g���F�N�����R��
            }
        }
        else Debug.Log("deadEnemy null");

        //�q�s�ɤ����J�U�����
        if (SaveManager.instance.gameData.aiDatas == null) return;
        for (int i = 0; i < SaveManager.instance.gameData.aiDatas.Count; i++)
        {
            AIdata aiTemp = SaveManager.instance.gameData.aiDatas[i];
            if (aiTemp.objName == this.gameObject.name)
            {
                transform.position = aiTemp.pos;
                transform.rotation = Quaternion.Euler(0f, aiTemp.pos.w, 0f);
                myHealth.health = aiTemp.hp;

                break;
            }
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;

    }
    float nextFindPlayerTime = 0f;
    void Sleeping()
    {
        if(Time.time > nextFindPlayerTime)
        {
            
            nextFindPlayerTime = Time.time + Random.Range(1f, 2f);
            float distance = Vector3.Distance(player.position, transform.position);
            if(getUpDistance > distance)
            {
                if (mainCaller)
                {
                    maybeAttackPos = player.position;
                    status = AIStatus.CallFriends;
                }
                else status = AIStatus.Idle;

            }
        }
    }
    void GetUp()
    {
        animator.SetTrigger("GetUp");
    }



    #endregion

    #region �ݩR
    void ToIdle()
    {
        //Debug.Log("IDLE");
    }
    void Idling()
    {
        if(IsTime(Random.Range(minIdleTime , maxIdleTime ) , 1))
        {
            status = AIStatus.Guard;
        }

        if(currentHuntingAngle != huntingAngle) //�N���観�J��ĤH,�l�y�d��w��360,�{�b�C�C�^�_�ܹw�]��(���p�G�S�i��l�y ���ױN�S�ܬ�360)
        {
            currentHuntingAngle = Mathf.Lerp(currentHuntingAngle, huntingAngle, Time.deltaTime * huntingRecoverLevel);
        }
        
    }
    void StopIdle()
    {

    }
    #endregion

    #region �u��
    void ToGuard()
    {
        transform.Rotate(0f, Random.Range(0f, 360f), 0f);
        animator.SetBool("isWalk", true);
        myMovement.Walk(true);
    }
    void Guarding()
    {
        if(statusTime > maxWalkTime)
        {
            float distance = Vector3.Distance(transform.position, homePosition);
            if (distance > maxHomeDistance)
            {
                status = AIStatus.Home;
            }
            else
            {
                status = AIStatus.Idle;
            }  
        }
    }
    void StopGuard()
    {
        animator.SetBool("isWalk", false);
        myMovement.Walk(false);
    }
    #endregion

    #region �^�a
    void GoHome()
    {
        Vector3 tempPosition = new Vector3(homePosition.x, this.transform.position.y, homePosition.z);
        transform.LookAt(tempPosition);
        animator.SetBool("isWalk", true);
        myMovement.Walk(true);
        isCalled = false;
    }
    void Homing()
    {
        float distance = Vector3.Distance(transform.position, homePosition);

        if(distance < miniHomeDistance)
        {
            status = AIStatus.Idle;
        }

        if(statusTime > maxWalkTime)  //�P�̤j�樫�ɶ��ۦP�]�w
        {
            status = AIStatus.Idle;
        }

    }
    void StopGoingHome()
    {
        myMovement.Walk(false);
        animator.SetBool("isWalk", false);
    }
    #endregion

    #region �l�y

    void StartHunt()
    {
        animator.SetBool("isRun", true);
        myMovement.Running(true);
        //Debug.Log("HUNT");
        //�@���}�l�l�y�N���|�������,���D�b�i�Jidle���A�ɼĤH�w���}�d��
        currentHuntingRange = currentHuntingRange * 2;
        currentHuntingAngle = 360f;  
    }
    void Hunting()
    {
        if(targetHealth == null)
        {
            status = AIStatus.Idle;
            return;
        }
        Vector3 targetPos = LookTarget(targetHealth.transform.position);
        transform.LookAt(targetPos);

        float targetDistance = Vector3.Distance(transform.position, targetPos);

        if (targetDistance < attackRange)
        {
            status = AIStatus.Attack;
        }

        if (huntPower <= 0) status = AIStatus.Idle;
        else huntPower -= Time.deltaTime;

    }
    void StopHunt()
    {
        animator.SetBool("isRun", false);
        myMovement.Running(false);
    }

    #endregion

    #region ����
    void ToAttack()
    {
        int attackType = Random.Range(0, attackTypeAmount);
        animator.SetInteger("AttackType", attackType);
        animator.SetTrigger("Attack");
        
        
    }
    void Attacking()
    {
        if (statusTime > 2f)  //�W�L2��S�����ĤH,�N�ਭ�ݤ@��
        {
            if (willGuarding) status = AIStatus.LookBack;
            else status = AIStatus.Idle;

        } 
    }
    void StopAttack()
    {
        animator.ResetTrigger("Attack");
    }

    #endregion

    #region �Q��

    void GetHit()
    {
        animator.SetTrigger("Hit");
        
        //Debug.Log("HIT");
        //debugImage.enabled = true;
        //Invoke("CloseDebugImage", 1f);
    }
    void Hitting()
    {
        if(statusTime > 1.5f)  //�Q����Y�b1.5���S�����ĤH,�N�i�J�j��
        {
            if (revenger) status = AIStatus.Searching;  //�p�G�O�_���̴N�|�h�j���ĤH,�_�h�u�|�^��ݩR
            else status = AIStatus.Idle;

        }
    }
    void StopHit()
    {

    }
    #endregion

    #region ����

    void ToDie()
    {
        animator.ResetTrigger("Hit");
        animator.SetTrigger("Die");
        healthBarUI.enabled = false;
        if(createItem != null)
        {
            createItem.GetAllItem();
        }
        SaveManager.instance.gameData.deadEnemy.Add(this.name);
    }
    /// <summary>�u�\����N���i��</summary>
    float p = 1f;
    void Dying()
    {

        p -= Time.deltaTime * 0.2f;
        if(p>1f)
        {
            
            Destroy(gameObject);
        }
        else
        {
            SetRenderFire(p);
        }
    }
    void Dead()
    {
        

    }
    #endregion

    #region �^�Y

    void LookBack()
    {
        animator.SetTrigger("LookBack");
        //Debug.Log("LOOKBACK");
    }
    void Looking()
    {
        if(statusTime > 1.2f)
        {
            float r = Random.Range(0f, 100f);
            if(r < 50f)
            {
                status = AIStatus.LookBack;
            }
            else     //�p�G���٬O�S�����ĤH,�N�^�a,�ë�_�i�H�I�B�ަ񪺪��A
            { 
                status = AIStatus.Home;
                isCalled = false;
            }
        }
    }
    void StopLooking()
    {
        animator.ResetTrigger("LookBack");
    }

    #endregion

    #region �j�M�ĤH

    void Search()
    {
        transform.LookAt(maybeAttackPos);
        animator.SetBool("isRun",true);
        //Debug.Log("SEARCH");
    }
    void Searching()
    {
        if(statusTime > 5f)
        {
            currentHuntingRange = currentHuntingRange * 2;
            currentHuntingAngle = 360f;
            status = AIStatus.LookBack;
        }
    }
    void StopSearching()
    {
        animator.SetBool("isRun", false);
    }

    #endregion

    #region �I�B�ަ�

    void CallFriends()
    {
        animator.SetTrigger("CallFriends");
        SoundManager.instance.Play(SoundType.CallEnemy, transform.position);
    }
    void Calling()
    {
        if(statusTime > 1f)
        {
            Collider[] allColliders = Physics.OverlapSphere(transform.position, callFriednsRange);
            foreach (Collider other in allColliders)
            {
                if (other.GetInstanceID() == transform.GetInstanceID()) continue;

                EnemyAI friendAI = other.GetComponent<EnemyAI>();
                if (friendAI == null) continue;

                Health myFriendHP = friendAI.GetComponent<Health>();
                if (myFriendHP == null) continue;

                if (!myHealth.IsMyEnemy(myFriendHP))
                {
                    friendAI.BeingCalled(maybeAttackPos);
                }
            }

            status = AIStatus.Hunt;
        }
    }
    void StopCalling()
    {
        animator.ResetTrigger("CallFriends");
    }

    public void BeingCalled(Vector3 targetLocation)
    {
        maybeAttackPos = targetLocation;
        status = AIStatus.Searching;
        isCalled = true;
    }

    #endregion

    #region �q��
    float nextLookTime = 0f;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // if(A && (B || C ))
        if(Time.time > nextLookTime && (status == AIStatus.Idle || status == AIStatus.Guard || status == AIStatus.Home || status == AIStatus.LookBack || status == AIStatus.Searching))    //�u���b�o�X�Ӷ��q�~�|�j���ĤH
        {
            nextLookTime = Time.time + Random.Range(0.3f, 0.5f);   // �ϥH�U�{���X�C�j 0.3~0.5��~����@��,�H�`�ٮį�
            Collider[] targets = Physics.OverlapSphere(transform.position, currentHuntingRange);
            foreach (Collider target in targets)
            {
                Health colliderHealth = target.GetComponent<Health>();

                if (!CheckTarget(colliderHealth)) continue;  //�T�{�O�_���ͩR?�O�_�O�ۤv?�O�_�w��?�O�_�O�ĤH? ���@����^��false���N���L�j��

                float angle = GetTargetAngle(target.transform);
                if (angle < currentHuntingAngle * 0.5f)  //�p�G�b�������פ�(+-��Ӥ�V�����׳��n��)
                {
                    targetHealth = colliderHealth;

                    if (huntPower > 2f)  //���ĤH�åB����O����, 50%�|�I�B�ަ� 50%�|�ۤv�h�l
                    {
                        float r = Random.Range(0, 100);
                        if(r > 50 && !isCalled  && willCallFriend)  ///�Y�����æ^�Y�����٬O�S�����ĤH,isCalled�|���^false ��_�i�H�I�B�ަ񪺪��A
                        {
                            maybeAttackPos = targetHealth.transform.position;
                            status = AIStatus.CallFriends;
                        }
                        else
                        {
                            status = AIStatus.Hunt;  
                        }                       
                    }                   
                }

            }
        }

        if(status != AIStatus.Hunt)  //�p�G���b�l�y���A�N�w�C�^�_��O
        {
            if(huntPower < huntingTime && huntPower < maxHuntPower)
            {
                huntPower += Time.fixedDeltaTime;
            }    
        }
        
    }

    private bool CheckTarget(Health target)
    {
        if (target == null) return false;   //�p�G�O�L�ͩR��Nfalse
        if (!target.IsMyEnemy(myHealth)) return false;  //�p�G���O�ڪ��ĤH�Nfalse
        //if (target.transform.GetInstanceID() == this.transform.GetInstanceID()) return false;  //�p�G�O�ۤv�Nfalse                  
        if (target.IsDead())  //�p�G�w�g���F�N��targetHealth �M�Ũæ^��false
        {
            targetHealth = null;
            return false;
        }
        return true;  //���󳣳q�L�N�^��true
    }

    private float GetTargetAngle(Transform target)
    {
        Vector3 myDirection = transform.forward;
        Vector3 targetDirection = target.position - transform.position;
        float angle = Vector3.Angle(myDirection, targetDirection);
        return angle;
    }

    Vector3 LookTarget(Vector3 pos)
    {
        Vector3 temp = pos;
        temp.y = this.transform.position.y;
        return temp;
    }

    #endregion

    //�q�ʵe�I�s
    public void Attack(float animationDamage)
    {
        if (status != AIStatus.Attack) return;
        if (targetHealth == null || targetHealth.IsDead()) return;

        float distance = Vector3.Distance(this.transform.position, targetHealth.transform.position);
        float angle = GetTargetAngle(targetHealth.transform);

        if (distance < attackRange  && angle < attackAngle)
        {
            //float damage = 0f;
            //if (animationDamage != 0) damage = animationDamage;
            //else damage = defaultAttackDamage;

            DamageInfo di = new DamageInfo();
            di.damage = defaultAttackDamage;
            di.sourcePos = transform.position;

            targetHealth.TakeDamage(di,DamageType.�����ˮ`);
        }
        SoundManager.instance.Play(SoundType.SkeletonAttack, transform.position);
    }

    //�qHealth��sendMessage�I�s
    public void Hit(DamageInfo di)
    {   
        transform.LookAt(LookTarget(di.sourcePos));
        maybeAttackPos = di.sourcePos;
        status = AIStatus.Hit;
        SoundManager.instance.Play(SoundType.SkeletonHit, transform.position);
    }

    public void Die(DamageInfo di)
    {
        transform.LookAt(LookTarget(di.sourcePos));
        status = AIStatus.Die;     
    }
    
    void CloseDebugImage()
    {
        debugImage.enabled = false;
    }



    void SetRenderFire(float p)
    {
        for(int i = 0; i < renderList.Count; i ++)
        {
            renderList[i].material.SetFloat("_P", p);  //_p�Ӧ�shader��reference

        }
    }



}

public enum AIStatus
{
    /// <summary>�ݩR</summary>
    Idle,
    /// <summary>�H������</summary>
    Guard,
    /// <summary>�¦ۤv�X�ͦa�e�i�����F����</summary>
    Home,
    /// <summary>�l��,�µۥؼЫe�i</summary>
    Hunt,
    /// <summary>���q����</summary>
    Attack,
    Hit,
    Die,
    /// <summary>�^�Y���ĤH</summary>
    LookBack,
    /// <summary>�j�M�ĤH</summary>
    Searching,
    CallFriends,
    Sleep

}



