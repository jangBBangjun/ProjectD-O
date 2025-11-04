using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_controller : MonoBehaviour
{
    public string TriggerName;
    public float MoveSpeed;
    private Animator thisAnim;
    private Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        thisAnim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var v = Input.GetAxis("Vertical");
        var h = Input.GetAxis("Horizontal");

        thisAnim.SetFloat("Speed", Mathf.Abs(v + h));
        
        Vector3 direction = new Vector3(h, 0, v) * -1;
        if(direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            rigid.rotation = Quaternion.Euler(0, angle, 0);
        }
        rigid.position += direction * MoveSpeed * Time.fixedDeltaTime;              

        if (Input.GetButtonDown("Jump"))
        {           
                       thisAnim.SetTrigger(TriggerName);
            thisAnim.SetBool("IsATK", true);
            StartCoroutine(ATKreset(.5f));
        }      
        IEnumerator ATKreset(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            thisAnim.SetBool("IsATK", false);
        }
    }
}
