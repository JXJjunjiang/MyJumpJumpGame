using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// platform压缩与弹起的高度限制
    /// </summary>
    private const float MinScaleY = 0.4f, MaxScaleY = 2f;
    /// <summary>
    /// platform压缩与弹起的速度
    /// </summary>
    private const float YDownSpeed = 0.01f, YUpSpeed = 0.3f;
    /// <summary>
    /// 角色跳起的最高高度
    /// </summary>
    private const float JumpY = 8f;
    /// <summary>
    /// 角色跳起的横向速度
    /// </summary>
    private const float JumpHorizontalSpeed = 25f;
    /// <summary>
    /// platform水平大小
    /// </summary>
    private static Vector2 platformSize = new Vector2(GameManager.PlatformPrefabSize.x, GameManager.PlatformPrefabSize.z);
    /// <summary>
    /// 角色失败的高度
    /// </summary>
    private const float FailedPosY = 1f;
    /// <summary>
    /// 当前plat和下一个plat
    /// </summary>
    private Transform curPlatform, nextPlatform;
    /// <summary>
    /// 落点判断区域
    /// </summary>
    private Rect[] jugeArea;
    /// <summary>
    /// 角色
    /// </summary>
    private Transform character;
    /// <summary>
    /// 相机到角色的方向
    /// </summary>
    private Vector3 cam2playerDir;
    /// <summary>
    /// 相机到角色的距离
    /// </summary>
    private float cam2playerDistance;

    private bool isPress;
    private bool isUping;
    private Vector3 startPoint, midPoint, endPoint;
    private float jumpTime = 0;
    private Transform cameraTrs;

    public void Init()
    {
        InitPlatform();
        isPress = false;
        jumpTime = 0;
        InitPlayer();
        jugeArea = new Rect[2] { new Rect(), new Rect() };
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
        character.position = new Vector3(0, 2f + character.localScale.y, 0);
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

    #region OnPress
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
        isUping = true;
        float y = Mathf.Abs(curPlatform.localScale.y - MaxScaleY);
        Vector3 scale = curPlatform.localScale;
        DOTween.To((t) =>
        {
            Vector3 newScale = scale;
            newScale.Set(scale.x, scale.y + y * t, scale.z);
            curPlatform.localScale = newScale;
            SetPlatformYPos(newScale);
        }, 0, 1, 0.3f).SetEase(Ease.InOutBack,3f).onComplete = () => 
        {
            curPlatform.localScale = new Vector3(platformSize.x, MaxScaleY, platformSize.y);
            isUping = false;
        };
    }

    void PlayerFollow()
    {
        Vector3 pos = character.position;
        pos.Set(pos.x, curPlatform.localScale.y + character.localScale.y, pos.z);
        character.position = pos;
    }

    void SetPlatformYPos(Vector3 scale)
    {
        Vector3 pos = curPlatform.localPosition;
        pos.Set(pos.x, scale.y / 2f, pos.z);
        curPlatform.localPosition = pos;
    }
    #endregion

    private void Update()
    {
        if (!GameManager.CanControll)
            return;

        if (Panel_Game.touch.IsDown&&UIManager.CanTouch)
        {
            RefreshThreePoint();
            GetCurrentArea();
            GetNextArea();
        }

        if (Panel_Game.touch.IsHold&&UIManager.CanTouch)
        {
            PlatformDown();
            isPress = true;
            SetThreePoint();
        }

        if(isPress&&!Panel_Game.touch.IsHold)
        {
            UIManager.CanTouch = false;
            if (!isUping)
            {
                PlatformUp();
            }
            if (Mathf.Abs(MaxScaleY - curPlatform.localScale.y) <= 0.1f)
            {
                isPress = false;
            }
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
        /*只要在区域内，都会重置控制权；但还在原来的平台上就不会生成，也不会移动相机*/
        if (IsInArea())
        {
            if (!IsInSameArea())
            {
                curPlatform = GameManager.Inst.SearchPool(nextPlatform.name);
                Vector3 nextPos = GameManager.Inst.GetNextPlatformPos(curPlatform.position);
                nextPlatform = GameManager.Inst.SpawnPlatform(nextPos);
                CameraFocusJog();
                EventHandler.ScoreTween_Dispatch(5);
            }
            GameManager.CanControll = true;
        }
        else
        {
            GameManager.CanControll = false;
            PlayerFall(() => { UIManager.OpenUI<Pop_Fail>(UIPanel.Fail); });
        }
        UIManager.CanTouch = true;
    }

    void PlayerFall(System.Action callback)
    {
        character.DOLocalMoveY(1, 0.5f).onComplete = () => callback?.Invoke();
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

    bool IsInSameArea()
    {
        return jugeArea[0].Contains(new Vector2(endPoint.x, endPoint.z));
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
        Vector3 pos = new Vector3(10, 13, -6);
        cameraTrs.position = pos;
        cam2playerDir = (pos - character.position).normalized;
        cam2playerDistance = Vector3.Distance(character.position, pos);
    }
    #endregion

    public void Uninit()
    {
        
    }
}
