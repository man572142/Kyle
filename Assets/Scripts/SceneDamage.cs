using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDamage : MonoBehaviour
{
    [SerializeField] float damage = 20f;
    [SerializeField] DamageType damageType = DamageType.瞬間傷害;



    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health == null) return;

        if(damageType == DamageType.瞬間傷害)
        {
            DamageInfo di = new DamageInfo();
            di.damage = damage;
            di.sourcePos = Vector3.zero;

            health.TakeDamage(di,damageType);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health == null) return;

        if (damageType == DamageType.持續傷害)
        {
            DamageInfo di = new DamageInfo();
            di.damage = damage * Time.fixedDeltaTime;
            di.sourcePos = Vector3.zero;

            health.TakeDamage(di , damageType);

        }

    }

    

}

public enum DamageType : int
{
    瞬間傷害,
    持續傷害,
}
