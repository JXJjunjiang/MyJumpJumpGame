using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Controll : MonoBehaviour
{
    private const float MinScaleY = 0.2f, MaxScaleY = 2f;
    private const float YDownSpeed = 0.1f, YUpSpeed = 0.3f;
    private const float JumpY = 8f;
    private const float JumpHorizontalSpeed = 15f;
    private static Vector2 platformSize;
    private const float FailedPosY = 1f;

    private Transform curPlatform, nextPlatform;
    private Rect[] jugeArea;
    private Transform character;

    private Vector3 cam2playerDir;
    private float cam2playerDistance;

    bool isPress;
    bool isJump;
    bool canControll;
    Vector3 startPoint, midPoint, endPoint;
    float jumpTime = 0;

    private void Start()
    {
        curPlatform = GameObject.Find("Platform").transform;
        nextPlatform = null;
        character = GameObject.Find("Player").transform;
        platformSize.Set(GameManager.Inst.PlatformPrefabSize.x, GameManager.Inst.PlatformPrefabSize.z);
        jugeArea = new Rect[2] { new Rect(), new Rect() };//第0个代表现在的platform，第1个代表下一个platform

        GameStart();
    }

    void GameStart()
    {
        Spawn(curPlatform);
        canControll = true;
        cam2playerDir = (Camera.main.transform.position - character.position).normalized;
        cam2playerDistance = Vector3.Distance(character.position, Camera.main.transform.position);
        Debug.Log(character.position + cam2playerDir * cam2playerDistance);
    }

    void GameEnd()
    {
        canControll = false;
    }

    void Spawn(Transform next)
    {
        JumpTrs jt = GameManager.Inst.GeneratePlatform(next);
        curPlatform = jt.currentTrs;
        nextPlatform = jt.nextTrs;
    }

    void PlatformDown()
    {
        //设置scale
        Vector3 scale = curPlatform.localScale;
        float scaleY = Mathf.Clamp(scale.y - YDownSpeed, MinScaleY, MaxScaleY);
        scale.Set(scale.x, scaleY, scale.z);
        curPlatform.localScale = scale;
        //设置position
        SetPlatformYPos(scale);
    }

    void PlatformUp()
    {
        //设置scale
        Vector3 scale = curPlatform.localScale;
        float scaleY = Mathf.Clamp(scale.y + YUpSpeed, MinScaleY, MaxScaleY);
        scale.Set(scale.x, scaleY, scale.z);
        curPlatform.localScale = scale;
        //判断是否可以起跳
        if (Mathf.Abs(MaxScaleY-scale.y)<=0.1f)
        {
            isPress = false;
        }
        //设置position
        SetPlatformYPos(scale);
    }

    void PlayerFollow()
    {
        Vector3 pos = character.localPosition;
        pos.Set(pos.x, curPlatform.localScale.y + character.localScale.y, pos.z);
        character.localPosition = pos;
    }

    void SetPlatformYPos(Vector3 scale)
    {
        Vector3 pos = curPlatform.localPosition;
        pos.Set(pos.x, scale.y / 2f, pos.z);
        curPlatform.localPosition = pos;
    }

    private void Update()
    {
        if (!canControll)
            return;

        if (Input.GetMouseButtonDown(0))//按下鼠标时刷新贝塞尔的三个点
        {
            RefreshThreePoint();
            GetCurrentArea();
            GetNextArea();
        }

        if (Input.GetMouseButton(0))
        {
            PlatformDown();
            isPress = true;
            SetThreePoint();
        }
        else if(isPress)
        {
            PlatformUp();
            if (!isPress)
            {
                Jump();
            }
        }

        if (isPress)
        {
            PlayerFollow();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(startPoint,0.2f);
        Gizmos.DrawSphere(midPoint, 0.2f);
        Gizmos.DrawSphere(endPoint, 0.2f);
    }

    #region EndPointMove

    void RefreshThreePoint()
    {
        startPoint = character.position;
        Vector3 temp = new Vector3(nextPlatform.localPosition.x, startPoint.y, nextPlatform.localPosition.z);
        Vector3 direction = (temp - startPoint).normalized;
        endPoint = character.position + direction;
        float midX = startPoint.x+(endPoint.x - startPoint.x) / 2f;
        float midZ = startPoint.z+(endPoint.z - startPoint.z) / 2f;
        midPoint.Set(midX, JumpY, midZ);
    }
    void SetThreePoint()
    {
        Vector3 temp = new Vector3(nextPlatform.localPosition.x, startPoint.y, nextPlatform.localPosition.z);
        Vector3 direction = (temp - startPoint).normalized;
        endPoint += (direction * 0.1f);
        float midX = startPoint.x + (endPoint.x - startPoint.x) / 2f;
        float midZ = startPoint.z + (endPoint.z - startPoint.z) / 2f;
        midPoint.Set(midX, JumpY, midZ);
    }
    #endregion

    #region Jump
    void Jump()
    {
        float distance = Vector3.Distance(endPoint, startPoint);
        jumpTime =  distance/ JumpHorizontalSpeed;
        canControll = false;
        DOTween.To((t) =>
        {
            SetCharacterCurveMove(t);
            SetCharacterRotate(t);
        }, 0f, 1f, jumpTime).onComplete = OnJumpFinish;
    }

    void SetCharacterCurveMove(float t)
    {
        Vector3 targetPoint = GetBezierPoint(t);
        character.position = targetPoint;
    }

    void SetCharacterRotate(float t)
    {
        Vector3 rotation = character.localEulerAngles;
        rotation.Set(rotation.x, rotation.y, t * 360f);
        character.localEulerAngles = rotation;
    }

    public Vector3 GetBezierPoint(float t)
    {
        Vector3 a = startPoint;
        Vector3 b = midPoint;
        Vector3 c = endPoint;

        Vector3 aa = a + (b - a) * t;
        Vector3 bb = b + (c - b) * t;
        return aa + (bb - aa) * t;
    }
    #endregion

    #region JumpFinish
    void OnJumpFinish()
    {
        if (IsInArea())
        {
            Spawn(nextPlatform);
            canControll = true;
            CameraFocus();
        }
    }
    #endregion

    #region Area
    void GetNextArea()
    {
        Vector3 nextPos = nextPlatform.localPosition;
        GetArea(nextPos, 1);
    }

    void GetCurrentArea()
    {
        Vector3 curPos = curPlatform.localPosition;
        GetArea(curPos, 0);
    }

    void GetArea(Vector3 pos,int index)
    {
        Vector2 rectStartPos = new Vector2(pos.x - platformSize.x / 2, pos.z - platformSize.y / 2);
        jugeArea[index].Set(rectStartPos.x, rectStartPos.y, platformSize.x, platformSize.y);
    }
    
    bool IsInArea()
    {
        for (int i = 0; i < jugeArea.Length; i++)
        {
            if (jugeArea[i].Contains(new Vector2(endPoint.x, endPoint.z)))
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    #region Camera
    void CameraFocus()
    {
        Vector3 camEndPos = character.position + cam2playerDistance * cam2playerDir;
        DOTween.To((t) =>
        {
            CameraMove(camEndPos, t);
        }, 0, 1, 0.5f);
    }

    void CameraMove(Vector3 camEndPos,float t)
    {
        Vector3 dir = (camEndPos - Camera.main.transform.position).normalized;
        float distance = Vector3.Distance(camEndPos, Camera.main.transform.position);
        Camera.main.transform.position += dir * distance * t;
    }
    #endregion
}
