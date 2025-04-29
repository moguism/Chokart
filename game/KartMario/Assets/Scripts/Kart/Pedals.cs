using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Pedals : MonoBehaviour
{
    public KartController kart;

    public void Accelerate()
    {
        kart.direction = 1;
    }

    public void GoBackwards()
    {
        kart.direction = -1;
    }

    public void NotMoveKart()
    {
        kart.direction = 0;
    }

    public async void Jump()
    {
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
