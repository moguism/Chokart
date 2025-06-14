using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Pedals : MonoBehaviour
{
    public KartController kart;

    public void Accelerate()
    {
        if(!kart.canMove)
        {
            return;
        }
        kart.direction = 1;
    }

    public void GoBackwards()
    {
        if (!kart.canMove)
        {
            return;
        }
        kart.direction = -1;
    }

    public void NotMoveKart()
    {
        if (!kart.canMove)
        {
            return;
        }
        kart.direction = 0;
    }

    public async void Jump()
    {
        if (!kart.canMove)
        {
            return;
        }
        //if (isGrounded)
        //{
        kart.jumping = true;

        kart.kartModel.parent.DOComplete();
        kart.kartModel.parent.DOPunchPosition(transform.up * .2f, .3f, 5, 1);
        await UniTask.WaitForSeconds(0.001f);
        //}
    }

    public void StopJumping()
    {
        print("DEJO DE SALTAR");
        kart.jumping = false;
    }
}
