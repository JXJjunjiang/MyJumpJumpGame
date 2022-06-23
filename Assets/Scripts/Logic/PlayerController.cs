using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Factory;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Q弹的最大高度
    /// </summary>
    private readonly Vector3 BumpMaxOffset = new Vector3(0, 0.2f, 0);
    /// <summary>
    /// platform压缩与弹起的高度限制
    /// </summary>
    private const float MinScaleY=0.7f, MaxScaleY = 1f;
    /// <summary>
    /// platform压缩与弹起的速度
    /// </summary>
    private const float YDownSpeed = 0.005f, YUpSpeed = 0.05f;
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
    private static Vector2 platformSize = new Vector2(GameMgr.PlatformPrefabSize.x, GameMgr.PlatformPrefabSize.z);
    /// <summary>
    /// 角色失败的高度
    /// </summary>
    private const float FailedPosY = 1f;
    /// <summary>
    /// 当前plat和下一个plat
    /// </summary>
    private Transform curPlatform, nextPlatform;
    /// <summary>
    /// 落点判断区域,0代表当前，1代表下一个
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
    private Vector3 startPoint, midPoint, endPoint;
    private float jumpTime = 0;
    private Transform cameraTrs;
    private Vector3 originPos;
    private Vector3 cameraPos;
    private const float endPointMoveSpeed = 0.05f;

    private void Awake()
    {
        if (cameraTrs == null)
        {
            cameraTrs = Camera.main.transform;
        }
        cameraPos = cameraTrs.position;
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
        Vector3 playerPos = new Vector3(0, curPlatform.localScale.y + character.localScale.y, 0);
        if (character == null)
        {
            PlayerInfo info = new PlayerInfo(DatabaseMgr.CharacterID, transform, Vector3.one, playerPos);
            character = CreateFactory.Inst.Create(FactoryType.Character, info).transform;
        }
        else
        {
            character.Reset();
            character.position = playerPos;
        }
    }

    void InitPlatform()
    {
        if (curPlatform != null)
        {
            DestroyImmediate(curPlatform.gameObject);
            curPlatform = null;
        }
        curPlatform = GameMgr.Inst.SpawnPlatform(Vector3.zero);
        if (nextPlatform != null)
        {
            DestroyImmediate(nextPlatform.gameObject);
            nextPlatform = null;
        }
        Vector3 nextPos = GameMgr.Inst.GetNextPlatformPos(Vector3.zero);
        nextPlatform = GameMgr.Inst.SpawnPlatform(nextPos);
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
        float platY = curPlatform.localPosition.y + curPlatform.localScale.y / 2f + character.localScale.y;//TODO 这个模型不知道为什么需要多加0.5，所以暂时不/2
        character.localPosition = new Vector3(character.localPosition.x, platY, character.localPosition.z);
    }

    void PressDown()
    {
        RefreshThreePoint();
        GetCurrentArea();
        GetNextArea();
        originPos = curPlatform.localPosition;
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
        seq.Append(curPlatform.DOLocalMoveY(originPos.y-0.2f,0.15f));
        seq.Append(curPlatform.DOLocalMoveY(originPos.y, 0.2f));
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
        if (!IsInSameArea())//如果非同一平台
        {
            /*分为四种情况。1.超出 2.未及 3.完全不在 4.完全在*/
            bool isBeyond = IsBeyond();
            bool isMiss = IsMiss();
            if (!isBeyond && !isMiss)
            {
                //TODO 完全在
                Debug.Log("完全在");
                curPlatform = GameMgr.Inst.SearchPool(nextPlatform.name);
                Vector3 nextPos = GameMgr.Inst.GetNextPlatformPos(curPlatform.position);
                nextPlatform = GameMgr.Inst.SpawnPlatform(nextPos);
                CameraFocusJog();
                EventHandler.ScoreTween_Dispatch(1);
                if (DatabaseMgr.IsMatchAnyHeight())
                {
                    UIMgr.HideUI(UIPanel.Game);
                    UIMgr.OpenUI<Panel_HeightTips>(UIPanel.HeightTips);
                }
                GameMgr.Inst.CanControll = true;
            }
            else if (isBeyond && isMiss)
            {
                //TODO 完全不在
                Debug.Log("完全不在");
                PlayerFall(() => { UIMgr.OpenUI<Pop_Fail>(UIPanel.Fail); });
                GameMgr.Inst.CanControll = false;
            }
            else if (isBeyond)
            {
                //TODO 超出
                Debug.Log("超出");
                PlayerForwardRotate();
                PlayerFall(() => { UIMgr.OpenUI<Pop_Fail>(UIPanel.Fail); });
                GameMgr.Inst.CanControll = false;
            }
            else if (isMiss)
            {
                //TODO 未及
                Debug.Log("未及");
                PlayerBackRotate();
                PlayerFall(() => { UIMgr.OpenUI<Pop_Fail>(UIPanel.Fail); });
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
        Debug.Log(point);
        return !jugeArea[1].Contains(point);
    }

    bool IsMiss()
    {
        Vector2 playerPos = new Vector2(character.localPosition.x, character.localPosition.z);
        Vector2 playerSize = new Vector2(character.localScale.x, character.localScale.z);
        Vector2 dir = (new Vector2(nextPlatform.localPosition.x, nextPlatform.localPosition.z) - new Vector2(curPlatform.localPosition.x, curPlatform.localPosition.z)).normalized;
        Vector3 modelSize = character.GetComponent<MeshFilter>().GetModelSize();
        Vector2 point = playerPos - (playerSize * new Vector2(modelSize.x, modelSize.z) * 0.5f).x * dir;
        Debug.Log(point);
        return !jugeArea[1].Contains(point);
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
        cam2playerDir = (cameraPos - character.position).normalized;
        cam2playerDistance = Vector3.Distance(character.position, cameraPos);
    }
    #endregion

    public void Uninit()
    {
        
    }
}
