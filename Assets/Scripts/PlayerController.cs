using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Animator animator = null;
    [SerializeField] Rigidbody myRigidbody = null;
    float mouseX;
    float totalMouseX;
    float mouseY;
    float keyX;
    float keyY;
    float runValue;
    float walkValue;
    float lastMoveTime = 0;
    public Health myHealth = null;
    [SerializeField] Transform lookPosition = null;
    [SerializeField] Animator dieUI = null;
    [SerializeField] Transform camAim = null;
    [SerializeField] CinemachineVirtualCamera virtualCamera = null;
    float defaultFOV = 0f;
    [SerializeField] Transform mainCamera = null;
    [SerializeField] FootStepsManager footSteps = null;
    [SerializeField] [Range(0.1f, 1f)] float timeScale = 1f;
    [SerializeField] LayerMask targetLayer = default;
    [SerializeField] Image energyBar = null;
    [SerializeField] GameObject fullPowerVFX = null;
    GameObject fullPowerTempVFX = null;

    [Header("����")]
    [SerializeField] [Range(50, 200)] float rotationSpeed = 1f;    
    [SerializeField] float maxCamRotation = 60f;
    bool isTouchingGround = true;
    [SerializeField] float slopeExtraForce = 5f;
    [SerializeField] LayerMask slopeLayer = default;
    CapsuleCollider capsuleCollider;
    
    
    bool isDead = false;

    [Header("�����Ѽ�")]
    [SerializeField] float attackRadius = 2f;
    [SerializeField] float attackAngle = 180f;
    [SerializeField] float attackDamage = 30f;
    [SerializeField] float shootDamage = 50f;
    [SerializeField] [Range(5f, 40f)] float shootDecreaseFOV = 10f;
    [SerializeField] float shootPowerFactor = 30f;
    [SerializeField] float maxShootPower = 100f;
    [Header("�Z��")]
    [SerializeField] GameObject sword = null;
    [SerializeField] GameObject bow = null;
    [SerializeField] GameObject bullet = null;
    [SerializeField] Transform bulleLocation = null;
    
    Vector3 shootTargetPos = Vector3.zero;
    
    float shootPower = 0f;
    float totalAddPercent = 1f;

    bool isReadyShoot = false;

    bool isAiming
    {
        get { return _isShoot; }
        set  //����isShoot = Input.GetKey(KeyCode.Mouse1);
        {
            _isShoot = value;
            bow.SetActive(value);
            sword.SetActive(!value);
        }
    }

    bool _isShoot = false;

    //�����
    static public PlayerController instance = null;

    private void Awake()
    {
        instance = this;
        SaveManager.instance.LoadData();
    }

    private void Start()
    {
        ItemInfoWindow.instance.Act_UseStuff += UseItem;
        GameManager.instance.Act_StuffChange += ItemPowerUp;

        //SaveManager.instance.LoadData();
        this.transform.position = SaveManager.instance.gameData.playerPos;
        myHealth.health = SaveManager.instance.gameData.playerHealth;

        //�q�s�ɨt�θ��J�D��
        GameManager.instance.LoadPlayerItem(SaveManager.instance.gameData.haveItemList);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Locked;
        defaultFOV = virtualCamera.m_Lens.FieldOfView;

        //Time.timeScale = 0.5f;
        capsuleCollider = GetComponent<CapsuleCollider>();
    }



    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetLookAtPosition(lookPosition.position);
        // 100%�ϥ�ik����
        animator.SetLookAtWeight(1f);
    }

    private void OnDestroy()
    {
        if(ItemInfoWindow.instance != null)
        {
            ItemInfoWindow.instance.Act_UseStuff -= UseItem;
        }
        if(GameManager.instance != null)
        {
            GameManager.instance.Act_StuffChange -= ItemPowerUp;
        }
        
    }

    void Update()
    {
        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) Restart();
            return;
        }

        if(isTouchingGround)
        {
            KeyMovement();
            KeyRun();
            KeyWalk();
            MouseRotation();
            MouseAttack();
            MouseAim();
        }
        
        

        //if (countAttackTime) attackCounter += Time.deltaTime;

        energyBar.fillAmount = shootPower / maxShootPower;
    }

    public void SetAirAnimate(bool state)
    {
        animator.SetBool("isAir", state);
        isTouchingGround = !state;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Pickup")
        {
            GameManager.instance.AddItem(int.Parse(collision.gameObject.name));
            SoundManager.instance.Play(SoundType.Pickup, transform.position);
            Destroy(collision.gameObject);
        }

        //�����P�I���骺�۹�y�b�t�רӧP�w�O�_�L��
        float yVelocity = Mathf.Abs(collision.relativeVelocity.y);
        if (yVelocity > 15f)
        {
            float fallDamage = yVelocity - 5f;
            // ��ۤv�y���ˮ`
            DamageInfo di = new DamageInfo();
            di.damage = fallDamage;
            di.sourcePos = this.transform.position;
            myHealth.TakeDamage(di, DamageType.�����ˮ`);
        }

    }



    #region �����欰
    private void MouseAim()
    {
        isAiming = Input.GetKey(KeyCode.Mouse1);

        if (isAiming)
        {
            animator.SetBool("isShoot", true);
            animator.SetFloat("ShootHeight", (mouseY + maxCamRotation) / maxCamRotation / 2);
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, defaultFOV - shootDecreaseFOV, Time.deltaTime * 4f);

            if (GameManager.instance.GetAmontById(0) > 0)  //�p�G�٦��b
            {
                if (Input.GetKey(KeyCode.Mouse0))  //������ֿn�O�q
                {
                    if (!isReadyShoot)
                    {
                        SoundManager.instance.Play(SoundType.PrepareShoot, transform.position);
                        isReadyShoot = true;
                    }
                    IncreaseShootEnergy();
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))  //�����}�o�g
                {
                    Shoot();
                    isReadyShoot = false;
                    SoundManager.instance.Stop(SoundType.PrepareShoot);
                    Destroy(fullPowerTempVFX);
                }
            }
        }
        else
        {
            animator.SetBool("isShoot", false);
            virtualCamera.m_Lens.FieldOfView = defaultFOV;
            if (fullPowerTempVFX != null) Destroy(fullPowerTempVFX);
            shootPower = 0f;
            //�����n�������ʧ@
            SoundManager.instance.Stop(SoundType.PrepareShoot);
            isReadyShoot = false;
        }
    }
    private void MouseAttack()
    {
        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isAiming) return;  //�p�G���b�˷ǴN�����Z������

            if (animState.IsTag("Hit")) return;  //�p�G���b�Q���Nreturn

            if (animState.IsTag("A1"))  //�p�G���bA1 �N�i�H��A2
            {
                animator.ResetTrigger("Attack3");
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Attack2");
            }
            else if (animState.IsTag("A2"))  //�p�G���bA2 �N�i�H��A3
            {
                animator.ResetTrigger("Attack");
                animator.ResetTrigger("Attack2");
                animator.SetTrigger("Attack3");
            }
            else if (animState.IsTag("A3"))  //�p�G���bA3 �i�H���^A1
            {                
                animator.ResetTrigger("Attack2");
                animator.ResetTrigger("Attack3");
                animator.SetTrigger("ComboAttack");
            }
            else //�p�G���O A1 A2 A3 �N��A1  //�qany state
            {
                animator.SetTrigger("Attack");
            }

        }
    }
    private void Shoot()
    { 
        if (shootPower < 50f) //�O�q�Ӥp�N���g���k0
        {
            shootPower = 0f;
            return;
        }
        if (shootPower >= maxShootPower) shootPower = maxShootPower;

        DamageInfo di = new DamageInfo();
        di.damage = shootDamage * totalAddPercent;
        di.sourcePos = transform.position;

        bulleLocation.LookAt(shootTargetPos);
        GameObject bulletTemp = Instantiate(this.bullet, bulleLocation.position, bulleLocation.rotation);
        bulletTemp.GetComponent<Bullet>().Shoot(shootTargetPos, di, this.GetInstanceID(), myHealth, shootPower);
        //lastAttackTime = Time.time;
        shootPower = 0f;
        GameManager.instance.DecreaseItem(0); //��֥�ƶq

        SoundManager.instance.Play(SoundType.Shoot, transform.position);
    }

    private void IncreaseShootEnergy()
    {
        shootPower += Time.deltaTime * shootPowerFactor;
        if (fullPowerTempVFX == null)
        {
            fullPowerTempVFX = Instantiate(fullPowerVFX, bulleLocation.position, Quaternion.identity);
            fullPowerTempVFX.transform.parent = bulleLocation;
        }     
    }

    #endregion
    private void FixedUpdate()
    {
        myRigidbody.AddTorque(0f, totalMouseX * rotationSpeed, 0f);
        totalMouseX = 0f;

        RaycastHit hit;
        bool isHit = Physics.Raycast(mainCamera.position, mainCamera.TransformDirection(Vector3.forward), out hit, 100f,targetLayer);
        if(isHit)
        {
            Debug.DrawLine(mainCamera.position, hit.point, Color.red);
            shootTargetPos = hit.point;
        }
        else
        {
            shootTargetPos = mainCamera.TransformPoint(0f, 0f, 100f);
        }


        RaycastHit hitSlope;
        Vector3 center = transform.position + (Vector3.up * capsuleCollider.height / 2);
        bool isSlope = Physics.Raycast(center, transform.forward, out hitSlope, 10f, slopeLayer);


        //Debug.DrawRay(center, transform.forward, Color.yellow,10f);
        
        if (isSlope)
        {
            float slopeDistance = Vector3.Distance(center, hitSlope.point);            
            myRigidbody.AddForce(1f / slopeDistance * Vector3.down * (slopeExtraForce + runValue), ForceMode.VelocityChange);
            //�b�שY�W�[�V�U���O,�O�ר��׫׻P�O�_�b�]�B�Ҽv�T
        }
    }

    public void Restart()
    {
        //SaveManager.instance.LoadData();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0 /*SaveManager.instance.gameData.sceneName*/);

    }
    #region ���ʦ欰
    private void KeyRun()
    {
        runValue = Mathf.Lerp(runValue, Input.GetKey(KeyCode.LeftShift)? 1f:0f , Time.deltaTime * 5f);
        animator.SetFloat("Run", runValue);
    }
    private void KeyWalk()
    {
        walkValue = Mathf.Lerp(walkValue, Input.GetKey(KeyCode.LeftControl) ? 1f : 0f, Time.deltaTime * 5f);
        animator.SetFloat("Walk", walkValue);
    }
    private void KeyMovement()
    {
        keyX = Mathf.Lerp(keyX ,Input.GetAxisRaw("Horizontal"), Time.deltaTime * 5f);
        keyY = Mathf.Lerp(keyY, Input.GetAxisRaw("Vertical") , Time.deltaTime * 5f);

        if(Mathf.Abs(keyX) + Mathf.Abs(keyY) > 0.1f)
        {
            animator.SetBool("isWalk", true);
            lastMoveTime = Time.time;
        }
        else
        {
            if (Time.time - lastMoveTime > 0.5f)  //����ʶW�L0.5f�~��isWalk�]��fasle�קK�L���W�c�����ʵe�ɭP�e�������Z
            {
                animator.SetBool("isWalk", false);
            }
        }

        ManageFootSteps(keyX, keyY);


        animator.SetFloat("X", keyX);
        animator.SetFloat("Y", keyY);
    }

    private void ManageFootSteps(float keyX, float keyY)
    {
        if(Input.GetKey(KeyCode.LeftShift))  //�Ĩ�
        {
            footSteps.SetFootStepMode(3, 1f);
            return;
        }

        if(keyY > 0.5f && keyX > 0.5f) //���k�]
        {
            footSteps.SetFootStepMode(2,1f);
        }
        else if(keyY > 0.5f && keyX < -0.5f)  //�����]
        {
            footSteps.SetFootStepMode(1,1f);
        }
        else if (keyY < 0.5f && keyY > 0f)  //�C�U��
        {
            footSteps.SetFootStepMode(0,keyY+0.3f);
        }
        else if( keyY < 0f)  //��h
        {
            footSteps.SetFootStepMode(99, 0.5f);
        }
        else
        {
            footSteps.SetFootStepMode(0, 1f);
        }

    }

    private void MouseRotation()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY += Input.GetAxis("Mouse Y") * Time.deltaTime *rotationSpeed * -1;

        totalMouseX += mouseX;

        mouseY = Mathf.Clamp(mouseY, -maxCamRotation, maxCamRotation);

        camAim.transform.localRotation = Quaternion.Euler(mouseY, 0f, 0f);

        //������v���˷��I
        //camAim.Rotate(-mouseY, 0f, 0f);
    }
#endregion

    #region �ϥΪ��~
    public void ItemPowerUp()
    {
        totalAddPercent = 0f;
        for (int i = 0; i < GameManager.instance.myItemList.Count; i++)
        {
            ItemData itemData = ItemDataManager.instance.GetDataByID(GameManager.instance.myItemList[i].id);

            totalAddPercent += itemData.addDamagePercent * GameManager.instance.myItemList[i].amount;
        }

        totalAddPercent = 1 + (totalAddPercent / 100f);
    }

    public void UseItem(int id)
    {

        ItemData itemData = ItemDataManager.instance.GetDataByID(id);
        if (itemData.costType == CostType.COST_ONE)
        {
            myHealth.ChangeHealth(itemData.hpChangeByUse);
        }

    }
    #endregion

    void Hit(DamageInfo di)  //�qHealth ��SendMessage�I�s
    {
        animator.SetTrigger("Hit");

        Vector3 localPos = this.transform.InverseTransformPoint(di.sourcePos);

        animator.SetFloat("HitX", localPos.x);
        animator.SetFloat("HitY", localPos.y);
        SoundManager.instance.Play(SoundType.PlayerHit,transform.position);
    }
    public void Die()
    {
        isDead = true;
        GetComponent<Animator>().ResetTrigger("Hit");   //���񦺱��ʵe�e���O��hit�S���b����
        GetComponent<Animator>().SetTrigger("Die");
        if (dieUI != null) dieUI.SetTrigger("PlayDieUI");
        Cursor.lockState = CursorLockMode.None;
        SoundManager.instance.Play(SoundType.PlayerDie, transform.position);
        FindObjectOfType<MusicPlayer>().PlaybackControl(false);

    }

    //�q�ʵe�I�s
    public void Attack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius);
        for(int i = 0; i < colliders.Length; i++)
        {
            Vector3 myDirection = transform.forward;
            Vector3 targetDirection = colliders[i].transform.position - transform.position;
            float angle = Vector3.Angle(myDirection, targetDirection);
            if (angle > attackAngle) continue;

            Health targetHealth = colliders[i].GetComponent<Health>();
            if (targetHealth == null || targetHealth.IsDead()) continue;  //�w���ΨS�ͩR�������L
            if (!targetHealth.IsMyEnemy(myHealth)) continue; //���O�ڪ��ĤH�N���L

            ProcessDamage(targetHealth);
            
        }
        SoundManager.instance.Play(SoundType.SwordAttack, transform.position);
    }

    public void CircleAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            //�d����������Ҽ{����
            Health targetHealth = colliders[i].GetComponent<Health>();
            if (targetHealth == null || targetHealth.IsDead()) continue;  //�w���ΨS�ͩR�������L
            if (!targetHealth.IsMyEnemy(myHealth)) continue; //���O�ڪ��ĤH�N���L

            ProcessDamage(targetHealth);
            
        }
        SoundManager.instance.Play(SoundType.SwordAttack, transform.position);
    }

    //�q�ʵe�I�s
    public void PlaySound(string soundType)
    {
        SoundType temp = (SoundType)System.Enum.Parse(typeof(SoundType), soundType);
        SoundManager.instance.Play(temp, transform.position);
    }



    private void ProcessDamage(Health targetHealth)
    {
        DamageInfo di = new DamageInfo();
        di.damage = attackDamage * totalAddPercent;
        di.sourcePos = transform.position;
        targetHealth.TakeDamage(di, DamageType.�����ˮ`);
    }
}
