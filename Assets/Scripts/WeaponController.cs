using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject Spear;
    public bool CanAttack = true;
    public float AttackCooldown = 1.5f;
    public AudioClip SpearAttackSound;
    public bool IsAttacking = false;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (CanAttack)
            {
                SpearAttack();
            }
        }
    }

    public void SpearAttack()
    {
        IsAttacking = true;
        CanAttack = false;
        Animator anim = Spear.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        AudioSource ac = GetComponent<AudioSource>();
        ac.PlayOneShot(SpearAttackSound);
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(AttackCooldown);
        CanAttack = true;
    }

    IEnumerator ResetAttackBool()
    {
        // shorter than the full spear animation time so that fish only get hit by the initial pierce and not the lingering animation
        yield return new WaitForSeconds(0.5f);
        IsAttacking = false;
    }
}
