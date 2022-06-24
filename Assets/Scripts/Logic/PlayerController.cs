using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Factory;

public class PlayerController : MonoBehaviour
{
    private const float MinScaleY=0.7f, MaxScaleY = 1f;// platform压缩与弹起的高度限制
    private const float YDownSpeed = 0.005f, YUpSpeed = 0.05f;// platform压缩与弹起的速度
    private const float JumpY = 8f;// 角色跳起的最高高度
    private const float JumpHorizontalSpeed = 25f;// 角色跳起的横向速度
    private const float FailedPosY = 1f;// 角色失败下坠的高度
    private const float endPointMoveSpeed = 0.05f;

    private static Vector2 platformSize = new Vector2(GameMgr.PlatformPrefabSize.x, GameMgr.PlatformPrefabSize.z);// platform水平大小

    private float jumpTime = 0f;//角色跳跃动画需要的时长
    private float cam2playerDistance=0f;//相机与角色之间的距离
    private bool isPress = false;
    private Vector3 startPoint, midPoint, endPoint;//贝塞尔曲线的三个点
    private Vector3 originPlatformPos;//platform初始位置
    private Vector3 cameraOriginPos;//相机初始位置
    private Vector3 cam2playerDir;//相机与角色之间的方向
    private Rect[] jugeArea = null;// 落点判断区域,0代表current，1代表next

    private Transform cameraTrs = null;
    private Transform curPlatform = null, nextPlatform = null;
    private Transform character = null;
    
    private void Awake()
    {
        if (cameraTrs == null)
        {
            cameraTrs = Camera.main.transform;
        }
        cameraOriginPos = cameraTrs.position;
    }

    public void Init()
    {
        InitPlatform();
        isPress = false;
        GameMgr.Inst.CanControll = true;
        jumpTime = 0;
        InitPlayer();
        jugeArea = new Rect[2] { new Rect(), new Rect() };
        InitCameraPos();
        CameraFocus();
        Panel_Game.touch.PointerDown += PressDown;
        Panel_Game.touch.PointerUp += PressUp;
    }

    void InitPlayer()
    {
        if (character == null)
        {
            PlayerInfo info = new PlayerInfo(DatabaseMgr.Inst.CharacterID, transform, Vector3.one, Vector3.zero);
            character = CreateFactory.Inst.Create(FactoryType.Character, info).transform;
            Vector3 playerPos = new Vector3(0, curPlatform.localScale.y + character.localScale.y, 0);
            character.localPosition = playerPos;
        }
        else
        {
            character.Reset();
            character.localPosition = new Vector3(0, curPlatform.localScale.y + character.localScale.y, 0);
        }
    }

    void InitPlatform()
    {
        if (curPlatform != null)
        {
            DestroyImmediate(curPlatform.gameObject);
            curPlatform = null;
        }
        curPlatform = GameMgr.Inst.GetFirstPlatform();

        if (nextPlatform != null)
        {
            DestroyImmediate(nextPlatform.gameObject);
            nextPlatform = null;
        }
        nextPlatform = GameMgr.Inst.GetNextPlatform();
    }

    #region OnPress
    void CharacterPressDown()
    {
        Vector3 scale = character.localScale;
        float scaleY = Mathf.Clamp(scale.y - YDownSpeed, MinScaleY, MaxScaleY);
        scale.Set(scale.x, scaleY, scale.z);
        character.localScale = scale;
    }

    void CharacterPressUp()
    {
        Vector3 scale = character.localScale;
        float scaleY = Mathf.Clamp(scale.y + YUpSpeed, MinScaleY, MaxScaleY);
        scale.Set(scale.x, scaleY, scale.z);
        character.localScale = scale;
    }

    void PlayerFollow()
    {
        //PlayerPos=platform高度/2f+人物高度/2f
        float platY = curPlatform.localPosition.y + curPlatform.localScale.y / 2f + character.localScale.y;
        character.localPosition = new Vector3(character.localPosition.x, platY, character.localPosition.z);
    }

    void PressDown()
    {
        RefreshThreePoint();
        GetCurrentArea();
        GetNextArea();
        originPlatformPos = curPlatform.localPosition;
        isPress = true;
    }

    void PressUp()
    {
        UIMgr.Inst.CanTouch = false;
        Jump();
        PlatformTween();
        isPress = false;
    }
    #endregion

    private void Update()
    {
        if (!GameMgr.Inst.CanControll)
            return;

        if (isPress)
        {
            CharacterPressDown();
            PlayerFollow();
            SetThreePoint();
        }
        else
        {
            CharacterPressUp();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(startPoint,0.2f);
        Gizmos.DrawSphere(midPoint, 0.2f);
        Gizmos.DrawSphere(endPoint, 0.2f);
    }

    void PlatformTween()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(curPlatform.DOLocalMoveY(originPlatformPos.y - 0.2f, 0.15f));
        seq.Append(curPlatform.DOLocalMoveY(originPlatformPos.y, 0.2f));
        seq.Play();
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
        endPoint += (direction * endPointMoveSpeed);
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
        if (!IsInSameArea())
        {
            bool isBeyond = IsBeyond();
            bool isMiss = IsMiss();
            if (!isBeyond && !isMiss)
            {
                Debug.Log("完全在");
                curPlatform = GameMgr.Inst.GetCurrentPlatform();
                nextPlatform = GameMgr.Inst.GetNextPlatform();
                CameraFocusJog();
                EventHandler.ScoreTween_Dispatch(1);
                if (DatabaseMgr.Inst.IsMatchAnyHeight())
                {
                    UIMgr.HideUI(UIPanelType.Game);
                    UIMgr.OpenUI<Panel_HeightTips>(UIPanelType.HeightTips);
                }
                GameMgr.Inst.CanControll = true;
            }
            else
            {
                if (isBeyond && isMiss)
                {
                    Debug.Log("完全不在");
                }
                else if (isBeyond)
                {
                    Debug.Log("超出");
                    PlayerForwardRotate();
                }
                else if (isMiss)
                {
                    Debug.Log("未及");
                    PlayerBackRotate();
                }
                PlayerFall(() => { UIMgr.OpenUI<Pop_Fail>(UIPanelType.Fail); });
                DatabaseMgr.Inst.Height = 0;
                GameMgr.Inst.CanControll = false;
            }
            UIMgr.Inst.CanTouch = true;
        }
    }

    void PlayerFall(System.Action callback)
    {
        character.DOLocalMoveY(1, 0.5f).onComplete = () => callback?.Invoke();
    }

    void PlayerForwardRotate()
    {
        Vector3 dir = (nextPlatform.localPosition - curPlatform.localPosition).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);
        character.localRotation = rotation;
        Vector3 eulerAngle = character.localEulerAngles;
        character.DOLocalRotate(new Vector3(-90f, eulerAngle.y, 0), 0.5f);
    }

    void PlayerBackRotate()
    {
        Vector3 dir = (nextPlatform.localPosition - curPlatform.localPosition).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);
        character.localRotation = rotation;
        Vector3 eulerAngle = character.localEulerAngles;
        character.DOLocalRotate(new Vector3(-90f, eulerAngle.y, 0), 0.5f);
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

    bool IsInSameArea()
    {
        return jugeArea[0].Contains(new Vector2(endPoint.x, endPoint.z));
    }

    bool IsBeyond()
    {
        Vector2 playerPos = new Vector2(character.localPosition.x, character.localPosition.z);
        Vector2 playerSize = new Vector2(character.localScale.x, character.localScale.z);
        Vector2 dir = (new Vector2(nextPlatform.localPosition.x, nextPlatform.localPosition.z) - new Vector2(curPlatform.localPosition.x, curPlatform.localPosition.z)).normalized;
        Vector3 modelSize = character.GetComponent<MeshFilter>().GetModelSize();
        Vector2 point = playerPos + (playerSize * new Vector2(modelSize.x, modelSize.z) * 0.5f).x * dir;
        return !jugeArea[1].Contains(point);
    }

    bool IsMiss()
    {
        Vector2 playerPos = new Vector2(character.localPosition.x, character.localPosition.z);
        Vector2 playerSize = new Vector2(character.localScale.x, character.localScale.z);
        Vector2 dir = (new Vector2(nextPlatform.localPosition.x, nextPlatform.localPosition.z) - new Vector2(curPlatform.localPosition.x, curPlatform.localPosition.z)).normalized;
        Vector3 modelSize = character.GetComponent<MeshFilter>().GetModelSize();
        Vector2 point = playerPos - (playerSize * new Vector2(modelSize.x, modelSize.z) * 0.5f).x * dir;
        return !jugeArea[1].Contains(point);
    }
    #endregion

    #region Camera
    void InitCameraPos()
    {
        if (cameraTrs == null)
        {
            cameraTrs = Camera.main.transform;
        }
        cam2playerDir = (cameraOriginPos - character.position).normalized;
        cam2playerDistance = Vector3.Distance(character.position, cameraOriginPos);
    }

    /// <summary>
    /// 相机注视缓动
    /// </summary>
    void CameraFocusJog()
    {
        Vector3 camEndPos = character.position + cam2playerDistance * cam2playerDir;
        cameraTrs.DOMove(camEndPos, 0.5f);
    }

    /// <summary>
    /// 相机直接注视
    /// </summary>
    void CameraFocus()
    {
        Vector3 camEndPos = character.position + cam2playerDistance * cam2playerDir;
        cameraTrs.position = camEndPos;
    }
    #endregion

    public void Uninit()
    {
        
    }
}
