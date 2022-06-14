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
    private float curTime, MaxTime = 1.5f;
    private float pressTime;

    public void Init()
    {
        InitPlatform();
        speed = 10f;
        isJumping = false;
        InitPlayer();
        InitCameraPos();
        CameraFocus();
        Panel_Game.touch.PointerDown += OnPressDown;
        Panel_Game.touch.PointerUp += OnPressUp;
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
        characterRigi = character.GetComponent<Rigidbody>();
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

    }

    void OnPressDown()
    {
        pressTime = Time.unscaledTime;
    }

    void OnPressUp()
    {
        pressTime = Time.unscaledTime - pressTime;
        direction = (nextPlatform.position - character.position+new Vector3(0,10,0)).normalized;
        Jump(pressTime);
    }

    void Jump(float holdTime)
    {
        characterRigi.AddForce(direction * speed * holdTime, ForceMode.Impulse);
    }

    #region JumpFinish
    void OnJumpFinish()
    {
        
    }

    IEnumerator CheckRigidbody()
    {
        curTime = Time.unscaledTime;
        while (Time.unscaledTime-curTime<MaxTime)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (characterRigi.velocity==Vector3.zero)
            {
                isJumping = false;
                break;
            }
        }
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
        Panel_Game.touch.PointerDown -= OnPressDown;
        Panel_Game.touch.PointerUp -= OnPressUp;
    }
}
