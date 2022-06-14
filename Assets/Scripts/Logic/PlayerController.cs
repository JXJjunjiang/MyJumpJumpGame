using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private Transform curPlatform, nextPlatform;
    private Transform character;
    private Rigidbody characterRigi;
    /// <summary>
    /// 相机到角色的方向
    /// </summary>
    private Vector3 cam2playerDir;
    /// <summary>
    /// 相机到角色的距离
    /// </summary>
    private float cam2playerDistance;

    private bool isJumping;
    private Transform cameraTrs;
    private Vector3 direction;
    private float speed;

    public void Init()
    {
        InitPlatform();
        speed = 1f;
        isJumping = false;
        InitPlayer();
        InitCameraPos();
        CameraFocus();
    }

    void InitPlayer()
    {
        if (character == null)
        {
            var obj = Instantiate<GameObject>(Loader.LoadGame("Player"));
            character = obj.transform;
            character.SetParent(transform);
            character.Reset();
        }
        character.position = new Vector3(0, curPlatform.localScale.y + character.localScale.y, 0);
    }

    void InitPlatform()
    {
        if (curPlatform != null)
        {
            DestroyImmediate(curPlatform.gameObject);
            curPlatform = null;
        }
        curPlatform = GameManager.Inst.SpawnPlatform(Vector3.zero);
        if (nextPlatform != null)
        {
            DestroyImmediate(nextPlatform.gameObject);
            nextPlatform = null;
        }
        Vector3 nextPos = GameManager.Inst.GetNextPlatformPos(Vector3.zero);
        nextPlatform = GameManager.Inst.SpawnPlatform(nextPos);
    }

    private void Update()
    {
        if (!GameManager.CanControll)
            return;

        if (Panel_Game.touch.IsHold&&!Panel_Game.touch.IsDown&&!isJumping)
        {
            isJumping = true;
            direction = (character.position - nextPlatform.position).normalized;
            Jump(Panel_Game.touch.PressTime);
        }
    }

    void Jump(float holdTime)
    {
        characterRigi.AddForce(direction * speed * holdTime, ForceMode.Impulse);
        //TODO 持续检测rigidbody直到停止或者超过多少秒为止
        StartCoroutine(CheckRigidbody());
    }

    #region JumpFinish
    void OnJumpFinish()
    {
        
    }

    IEnumerator CheckRigidbody()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (Vector3.Distance( characterRigi.velocity,Vector3.zero)<=0.1f)
            {
                isJumping = false;
                break;
            }
        }
        //TODO
    }
    #endregion

    #region Camera
    void CameraFocusJog()
    {
        Vector3 camEndPos = character.position + cam2playerDistance * cam2playerDir;
        cameraTrs.DOMove(camEndPos, 0.5f);
    }

    void CameraFocus()
    {
        Vector3 camEndPos = character.position + cam2playerDistance * cam2playerDir;
        cameraTrs.position = camEndPos;
    }

    void InitCameraPos()
    {
        if (cameraTrs==null)
        {
            cameraTrs = Camera.main.transform;
        }
        Vector3 pos = new Vector3(15f, 15, -13);
        cameraTrs.position = pos;
        cam2playerDir = (pos - character.position).normalized;
        cam2playerDistance = Vector3.Distance(character.position, pos);
    }
    #endregion

    public void Uninit()
    {
        
    }
}
