using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Q弹的最大高度
    /// </summary>
    private readonly Vector3 BumpMaxOffset = new Vector3(0, 0.2f, 0);
    /// <summary>
    /// platform压缩与弹起的高度限制
    /// </summary>
    private const float MaxDetalY = 0.3f;
    /// <summary>
    /// platform压缩与弹起的速度
    /// </summary>
    private const float YDownSpeed = 0.005f;
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
    private Vector3 originPos;

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

    #region OnPress
    void PlatformPressDown()
    {
        Vector3 pos = curPlatform.localPosition;
        float posY = Mathf.Clamp(pos.y - YDownSpeed, originPos.y - MaxDetalY, pos.y);
        pos.Set(pos.x, posY, pos.z);
        curPlatform.localPosition = pos;
    }

    void PlatformPressUp()
    {
        isUping = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(curPlatform.DOLocalMove(originPos + BumpMaxOffset, 0.2f));
        seq.Append(curPlatform.DOLocalMove(originPos - BumpMaxOffset / 2f, 0.1f));
        seq.Append(curPlatform.DOLocalMove(originPos, 0.1f));
        seq.Play().onComplete = () =>
        {
            isUping = false;
        };
    }

    void PlayerFollow()
    {
        //PlayerPos=platform高度/2f+人物高度/2f
        float platY = curPlatform.localPosition.y + curPlatform.localScale.y / 2f + character.localScale.y;//TODO 这个模型不知道为什么需要多加0.5，所以暂时不/2
        character.localPosition = new Vector3(character.localPosition.x, platY, character.localPosition.z);
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
            originPos = curPlatform.localPosition;
        }

        if (Panel_Game.touch.IsHold&&UIManager.CanTouch)
        {
            PlatformPressDown();
            isPress = true;
            SetThreePoint();
        }

        if(isPress&&!Panel_Game.touch.IsHold)
        {
            UIManager.CanTouch = false;
            if (!isUping)
            {
                PlatformPressUp();
            }
            if (Mathf.Abs(curPlatform.localPosition.y-originPos.y)<=0.1f)
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
        midPoint.Set(midX, endPoint.y + JumpY, midZ);
    }
    void SetThreePoint()
    {
        Vector3 temp = new Vector3(nextPlatform.localPosition.x, startPoint.y, nextPlatform.localPosition.z);
        Vector3 direction = (temp - startPoint).normalized;
        endPoint += (direction * 0.1f);
        float midX = startPoint.x + (endPoint.x - startPoint.x) / 2f;
        float midZ = startPoint.z + (endPoint.z - startPoint.z) / 2f;
        midPoint.Set(midX, endPoint.y + JumpY, midZ);
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
                EventHandler.ScoreTween_Dispatch(1);
                //等待分数动画执行完成后
                if (DatabaseMgr.IsMatchAnyHeight())
                {
                    UIManager.OpenUI<Panel_HeightTips>(UIPanel.HeightTips);
                }
            }
            GameManager.CanControll = true;
        }
        else
        {
            //TODO 判断三种情况
            //前倾
            //后倾
            //落点在平台之外

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
