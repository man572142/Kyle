using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : AYEStatusBehaviour<AIStatus>
{
    [SerializeField] Health myHealth = null;
    [SerializeField] EnemyMovement myMovement = null;
    [SerializeField] Animator animator = null;
    [Tooltip("最大待命時間")]
    [SerializeField] float maxIdleTime = 5f;
    [Tooltip("最小待命時間")]
    [SerializeField] float minIdleTime = 3f;
    [SerializeField] Image healthBarUI = null;

    [Header("移動")]
    [Tooltip("最大單次閒晃時間 :影響行走距離及走停頻率")]
    [SerializeField] float maxWalkTime = 5f;
    [SerializeField] float maxHomeDistance = 10f;
    [SerializeField] float miniHomeDistance = 2f;
    Vector3 homePosition = Vector3.zero;

    [Header("追獵")]
    [Tooltip("追獵範圍半徑")]
    [SerializeField] float huntingRange = 5f;
    float currentHuntingRange = 0f;
    [Tooltip("追獵視野角度")]
    [SerializeField] float huntingAngle = 90f;
    float currentHuntingAngle = 0f;
    float huntPower = 3f;
    [SerializeField][Range(0.1f,1f)] float huntingRecoverLevel = 0.5f;
    [Tooltip("最大追獵體力:若無體力就會停止追獵")]
    [SerializeField] float maxHuntPower = 10f;


    [Header("攻擊")]
    [Tooltip("攻擊範圍半徑")]
    [SerializeField] float attackRange = 2f;
    [Tooltip("攻擊範圍扇形角度")]
    [SerializeField] float attackAngle = 60f;
    [Tooltip("預設攻擊力: 若動畫無提供數值,就使用此值")]
    [SerializeField] float defaultAttackDamage = 15f;
    [Tooltip("攻擊型態 **必須與動畫數量相同**")]
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
    /// <summary>是否剛被朋友呼叫</summary>
    bool isCalled = false;
    /// <summary>骷髏身上全部的渲染</summary>
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
            //先移除現有的資料以便重置內容 (因為List內容沒辦法被覆蓋)
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

    #region 封印

    Transform player = null;

    void ToSleep()
    {
        transform.Rotate(0f, Random.Range(0f, 360f), 0f,Space.Self);
        


        if (SaveManager.instance.gameData.deadEnemy != null)
        {
            for (int i = 0; i < SaveManager.instance.gameData.deadEnemy.Count; i++)
            {
                if (SaveManager.instance.gameData.deadEnemy[i] == this.gameObject.name) Destroy(gameObject);//如果已經死了就直接刪除
            }
        }
        else Debug.Log("deadEnemy null");

        //從存檔中載入各項資料
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

    #region 待命
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

        if(currentHuntingAngle != huntingAngle) //代表剛剛有遇到敵人,追獵範圍已變360,現在慢慢回復至預設值(但如果又進到追獵 角度將又變為360)
        {
            currentHuntingAngle = Mathf.Lerp(currentHuntingAngle, huntingAngle, Time.deltaTime * huntingRecoverLevel);
        }
        
    }
    void StopIdle()
    {

    }
    #endregion

    #region 守衛
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

    #region 回家
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

        if(statusTime > maxWalkTime)  //與最大行走時間相同設定
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

    #region 追獵

    void StartHunt()
    {
        animator.SetBool("isRun", true);
        myMovement.Running(true);
        //Debug.Log("HUNT");
        //一旦開始追獵就不會輕易放棄,除非在進入idle狀態時敵人已離開範圍
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

    #region 攻擊
    void ToAttack()
    {
        int attackType = Random.Range(0, attackTypeAmount);
        animator.SetInteger("AttackType", attackType);
        animator.SetTrigger("Attack");
        
        
    }
    void Attacking()
    {
        if (statusTime > 2f)  //超過2秒沒有找到敵人,就轉身看一看
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

    #region 被打

    void GetHit()
    {
        animator.SetTrigger("Hit");
        
        //Debug.Log("HIT");
        //debugImage.enabled = true;
        //Invoke("CloseDebugImage", 1f);
    }
    void Hitting()
    {
        if(statusTime > 1.5f)  //被打後若在1.5秒內沒有找到敵人,就進入搜索
        {
            if (revenger) status = AIStatus.Searching;  //如果是復仇者就會去搜索敵人,否則只會回到待命
            else status = AIStatus.Idle;

        }
    }
    void StopHit()
    {

    }
    #endregion

    #region 死掉

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
    /// <summary>骷髏死後燒的進度</summary>
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

    #region 回頭

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
            else     //如果都還是沒有找到敵人,就回家,並恢復可以呼朋引伴的狀態
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

    #region 搜尋敵人

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

    #region 呼朋引伴

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

    #region 通用
    float nextLookTime = 0f;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // if(A && (B || C ))
        if(Time.time > nextLookTime && (status == AIStatus.Idle || status == AIStatus.Guard || status == AIStatus.Home || status == AIStatus.LookBack || status == AIStatus.Searching))    //只有在這幾個階段才會搜索敵人
        {
            nextLookTime = Time.time + Random.Range(0.3f, 0.5f);   // 使以下程式碼每隔 0.3~0.5秒才執行一次,以節省效能
            Collider[] targets = Physics.OverlapSphere(transform.position, currentHuntingRange);
            foreach (Collider target in targets)
            {
                Health colliderHealth = target.GetComponent<Health>();

                if (!CheckTarget(colliderHealth)) continue;  //確認是否有生命?是否是自己?是否已死?是否是敵人? 任一條件回傳false都將跳過迴圈

                float angle = GetTargetAngle(target.transform);
                if (angle < currentHuntingAngle * 0.5f)  //如果在視野角度內(+-兩個方向的角度都要算)
                {
                    targetHealth = colliderHealth;

                    if (huntPower > 2f)  //找到敵人並且有體力的話, 50%會呼朋引伴 50%會自己去追
                    {
                        float r = Random.Range(0, 100);
                        if(r > 50 && !isCalled  && willCallFriend)  ///若攻擊並回頭完都還是沒有找到敵人,isCalled會切回false 恢復可以呼朋引伴的狀態
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

        if(status != AIStatus.Hunt)  //如果不在追獵狀態就緩慢回復體力
        {
            if(huntPower < huntingTime && huntPower < maxHuntPower)
            {
                huntPower += Time.fixedDeltaTime;
            }    
        }
        
    }

    private bool CheckTarget(Health target)
    {
        if (target == null) return false;   //如果是無生命體就false
        if (!target.IsMyEnemy(myHealth)) return false;  //如果不是我的敵人就false
        //if (target.transform.GetInstanceID() == this.transform.GetInstanceID()) return false;  //如果是自己就false                  
        if (target.IsDead())  //如果已經死了就把targetHealth 清空並回傳false
        {
            targetHealth = null;
            return false;
        }
        return true;  //條件都通過就回傳true
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

    //從動畫呼叫
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

            targetHealth.TakeDamage(di,DamageType.瞬間傷害);
        }
        SoundManager.instance.Play(SoundType.SkeletonAttack, transform.position);
    }

    //從Health的sendMessage呼叫
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
            renderList[i].material.SetFloat("_P", p);  //_p來自shader的reference

        }
    }



}

public enum AIStatus
{
    /// <summary>待命</summary>
    Idle,
    /// <summary>隨機巡邏</summary>
    Guard,
    /// <summary>朝自己出生地前進直到抵達為止</summary>
    Home,
    /// <summary>追補,朝著目標前進</summary>
    Hunt,
    /// <summary>普通攻擊</summary>
    Attack,
    Hit,
    Die,
    /// <summary>回頭打敵人</summary>
    LookBack,
    /// <summary>搜尋敵人</summary>
    Searching,
    CallFriends,
    Sleep

}



