using System.Collections.Generic;
using UnityEngine;

public class BotCollider : MonoBehaviour
{
    [SerializeField] private KartController parent;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject hair;
    [SerializeField] private List<GameObject> self;

    [SerializeField]
    private Animator headAnimator;

    [SerializeField]
    private Animator hairAnimator;

    private Vector3 angle;
    private bool rotateToRight;
    private bool shouldRotate = false;

    private void Update()
    {
        if (!shouldRotate)
        {
            return;
        }

        if (angle == Vector3.zero)
        {
            if (!rotateToRight)
            {
                hairAnimator.Play("HairToLeft_Inverse");
                headAnimator.Play("HeadToLeft_Inverse");
            }
            else
            {
                hairAnimator.Play("HairToRight_Inverse");
                headAnimator.Play("HeadToRight_Inverse");
            }
        }
        else
        {
            if (angle.x < 0)
            {
                hairAnimator.Play("HairToLeft");
                headAnimator.Play("HeadToLeft");
                rotateToRight = false;
            }
            else
            {
                hairAnimator.Play("HairToRight");
                headAnimator.Play("HeadToRight");
                rotateToRight = true;
            }
            //Debug.LogError(angle);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kart") && !self.Contains(other.gameObject))
        {
            //Debug.LogError(other);
            Vector3 direction = other.transform.position - transform.position;
            angle = direction;
            shouldRotate = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        angle = Vector3.zero;
    }
}
