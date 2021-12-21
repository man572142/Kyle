using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    List<GameObject> inFire = new List<GameObject>();
    float nextAddHealthTime = 5f;
    [SerializeField] float healPoints = 10f;
    [SerializeField] float healRate = 3f;
    [SerializeField] bool healPlayer = false;
    [SerializeField] float maxHealPlayerTime = 10f;
    [SerializeField] ParticleSystem particle = null;
    [SerializeField] Light myLight = null;
    [SerializeField] float lightShineRate = 0.1f;
    [SerializeField] Gradient healPlayerColor;
    [SerializeField] Gradient defaultColor;
    [SerializeField] Color playerLightColor;
    [SerializeField] Color defaultLightColor;
    float nextLightShineTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        inFire.Add(other.gameObject);



        if(other.gameObject.tag == "Bullet")
        {
            healPlayer = true;
            var ps = particle.colorOverLifetime;
            ps.color = healPlayerColor;
            myLight.color = playerLightColor;
            Invoke("StopHealingPlayer", maxHealPlayerTime);

        }
    }

    void StopHealingPlayer()
    {
        myLight.color = defaultLightColor;
        healPlayer = false;
        var ps = particle.colorOverLifetime;
        ps.color = defaultColor;
    }


    private void OnTriggerExit(Collider other)
    {
        inFire.Remove(other.gameObject);
    }
    private void Update()
    {
        if(Time.time > nextAddHealthTime)
        {
            nextAddHealthTime = Time.time + healRate;

            for(int i = 0; i < inFire.Count; i++)
            {
                if (inFire[i] == null) continue;

                Health healTarget = inFire[i].GetComponent<Health>();
                if (healTarget == null) continue;


                if(healTarget.gameObject.tag == "Player")  //如果是Player並且設定為要補Player
                {
                    if(healPlayer) healTarget.ChangeHealth(healPoints);

                }
                else
                {
                    healTarget.ChangeHealth(healPoints);
                }

            }
        }

        if(Time.time > nextLightShineTime)
        {
            nextLightShineTime = Time.time + lightShineRate;
            myLight.intensity = Random.Range(0.7f, 1.2f);
        }
        

    }


}
