using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    [SerializeField] Team team = Team.NONE;
    [SerializeField] bool autoMaxHealth = true;

    public float health
    {
        get { return _health; }
        set
        {       
            _health = value;
            if (autoOpenUI == null) return;
            if (value < maxHealth) autoOpenUI.SetActive(true);  //血不是滿的就要顯示血量條
            else autoOpenUI.SetActive(false);

        }
    }
    float _health = 0;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float maxHitTime = 3f;
    float timeSincelastHit = 0f;

    [SerializeField] Image hpBarFill = null;
    [SerializeField] Image hpBarCenter = null;
    [SerializeField] Color noPowerColor = Color.red;
    [SerializeField][Range(0.01f , 1)] float hpColorChangeSpeed = 1.5f;
    [SerializeField] TextMeshProUGUI hpText = null;
    bool isDead = false;
    [SerializeField] GameObject hitVFX = null;
    [SerializeField] GameObject autoOpenUI = null;
    [SerializeField] GameObject healingVFX = null;

    float lastHP = 0f;

    private void Start()
    {
        if(autoOpenUI != null) autoOpenUI.SetActive(false);

        if(autoMaxHealth) health = maxHealth;

    }

    void Update()
    {
        float currentHP = health / maxHealth;
        if(lastHP != currentHP )   //只有在血量有變化時才更新bar
        {
            if(hpBarFill != null) hpBarFill.fillAmount = health / maxHealth;

            if(hpText != null) hpText.text = (health / maxHealth * 100).ToString("F0") + "%";

            if(hpBarCenter != null)
            {
                Color centerColor = hpBarCenter.color;
                hpBarCenter.color = Color.Lerp(centerColor, noPowerColor, Time.deltaTime * hpColorChangeSpeed);
            }

            lastHP = currentHP;
        }   
    }

    public void TakeDamage(DamageInfo di, DamageType damageType)
    {
        if (isDead) return;
        if (damageType == DamageType.瞬間傷害) timeSincelastHit = maxHitTime;

        health -= di.damage;
        timeSincelastHit += Time.deltaTime;

        if(timeSincelastHit >= maxHitTime)
        {
            SendMessage("Hit",di, SendMessageOptions.DontRequireReceiver);
            timeSincelastHit = 0;
            if(hitVFX != null) Instantiate(hitVFX , transform.position  + new Vector3(0f,1f,0f), Quaternion.identity);

        }
        
        if (health <= 0 )
        {
            isDead = true;
            health = 0;
            SendMessage("Die", di, SendMessageOptions.DontRequireReceiver);
        }


    }

    public void ChangeHealth(float change)
    {
        if (health <= 0) return;

        health += change;
        health = Mathf.Clamp(health, 0f, maxHealth);

        if (change > 0 && healingVFX != null && health < maxHealth)
        {
            GameObject temp = Instantiate(healingVFX,transform.position,Quaternion.identity);
            temp.transform.parent = transform;
            temp.transform.Rotate(-90f, 0f, 0f);
        }

        if(health <= 0)  //如果在變化血量之後hp小於0
        {
            DamageInfo di = new DamageInfo();
            di.sourcePos = Vector3.zero;
            di.damage = 1;
            isDead = true;
            SendMessage("Die", di, SendMessageOptions.DontRequireReceiver);
        }

    }


    //從SendMessage呼叫
    public bool IsDead()
    {
        return isDead;
    }

    public bool IsMyEnemy(Health other)
    {
        if (other.team == Team.NONE) return true; //如果他沒有隊伍

        if (other.team != this.team) return true; //如果他跟我不同隊伍

        //若以上條件都不符合
        return false;
    }

}
public struct DamageInfo
{
    public float damage;
    public Vector3 sourcePos;
}


public enum Team
{
    /// <summary>邊緣人隊伍</summary>
    NONE,
    /// <summary>玩家隊</summary>
    PLAYER,
    /// <summary>骷髏隊</summary>
    SKELETON
}