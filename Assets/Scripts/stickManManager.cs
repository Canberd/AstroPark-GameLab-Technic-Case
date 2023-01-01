using System;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class stickManManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem blood;
    private Animator StickManAnimator;
    private void Start()
    {
        StickManAnimator = GetComponent<Animator>();
        StickManAnimator.SetBool("run", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("red") && other.transform.parent.childCount > 0)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);

            Instantiate(blood, transform.position, Quaternion.identity);
        }

        switch (other.tag)
        {
            case "red":
                if (other.transform.parent.childCount > 0)
                {
                    Destroy(other.gameObject);
                    Destroy(gameObject);
                }
                break;

            case "jump":

                transform.DOJump(transform.position, 1f, 1, 1f).SetEase(Ease.Flash).OnComplete(() => PlayerManager.PlayerManagerInstance.FormatStickMan(0.45f));

                break;
        }

        if (other.CompareTag("stair"))
        {
            Debug.Log("stair");
            transform.parent.parent = null; // for instance tower_0
            transform.parent = null; // stickman
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Collider>().isTrigger = false;
            StickManAnimator.SetBool("run", false);

            if (PlayerManager.PlayerManagerInstance.stickmanParent.childCount == 1)
            {
                other.GetComponent<Renderer>().material.DOColor(new Color(0.4f, 0.98f, 0.65f), 0.5f).SetLoops(1000, LoopType.Yoyo)
                    .SetEase(Ease.Flash);
            }

            if (PlayerManager.PlayerManagerInstance.stickmanParent.childCount <= 1)
            {
                PlayerManager.PlayerManagerInstance.isForwardMove = false;
                PlayerManager.PlayerManagerInstance.moveTheCamera = false;

                menuManager.Instance.ChangePanel(menuManager.Instance.levelCompletedPanel);
            }
        }

        if (other.CompareTag("FormatPoint"))
        {
            Debug.LogError("FORMAT POINT!");

            PlayerManager.PlayerManagerInstance.FormatStickMan(18);
        }
        if (other.CompareTag("FinishLine"))
        {
            PlayerManager.PlayerManagerInstance.isForwardMove = false;
            PlayerManager.PlayerManagerInstance.moveTheCamera = false;

            menuManager.Instance.ChangePanel(menuManager.Instance.levelCompletedPanel);
            Debug.LogError("LEVEL COMPLETED!");
        }
        if (other.CompareTag("CollectStickman"))
        {
            transform.parent = PlayerManager.PlayerManagerInstance.stickmanParent;
        }
    }
}

