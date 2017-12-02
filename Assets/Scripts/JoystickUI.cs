
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputMgrName
{
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
}

public class JoystickUI : MonoBehaviour
{
    #region 常量

    public Vector3 DRAGING_BOXCOLLI_SIZE = new Vector3(900.0f, 900.0f, 1.0f);

    #endregion

    #region 变量

    /// <summary>
    /// 背景
    /// </summary>
    public UISprite mBg;

    /// <summary>
    /// touch
    /// </summary>
    public UISprite mTouch;

    /// <summary>
    /// 用来解决一边移动，一个手指有时候不能转镜头的bug(左手手指在触摸区外面的情况)
    /// </summary>
    public BoxCollider mTouchBoxColli;

    /// <summary>
    /// 移动事件
    /// </summary>
    public Action<Vector2> OnMove;

    /// <summary>
    /// 移动开始事件
    /// </summary>
    public Action OnMoveStart;

    /// <summary>
    /// 移动结束事件
    /// </summary>
    public Action OnMoveEnd;

    /// <summary>
    /// 半径
    /// </summary>
    private float mRadius;

    /// <summary>
    /// box colli初始化尺寸
    /// </summary>
    private Vector3 mTouchBoxColliInitSize;

    /// <summary>
    /// 是否触摸
    /// </summary>
    private bool mOnTouch = false;

    /// <summary>
    /// 旧的临时位置
    /// </summary>
    private Vector2 mOldTmpAxis = Vector2.zero;

    #endregion

    #region 内置函数

    // Use this for initialization
    void Start ()
    {
        UIEventListener.Get(mTouch.gameObject).onPress = OnTouchPress;
        UIEventListener.Get(mTouch.gameObject).onDrag = OnTouchDrag;

        mRadius = mBg.width * 0.5f;
        mTouchBoxColliInitSize = mTouchBoxColli.size;
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateJoystick();
	}

    #endregion

    #region 回调函数

    private void OnTouchDrag(GameObject go, Vector2 delta)
    {
        mOnTouch = true;
        mTouch.transform.localPosition += new Vector3(delta.x, delta.y, 0.0f);
        mTouch.transform.localPosition = Vector3.ClampMagnitude(mTouch.transform.localPosition, GetRadius());
    }

    private void OnTouchPress(GameObject go, bool state)
    {
        mOnTouch = state;
        if (!state)
        {
            mTouch.transform.localPosition = Vector2.zero;
        }

        if (state)
        {
            mTouchBoxColli.size = DRAGING_BOXCOLLI_SIZE;

            if (OnMoveStart != null)
                OnMoveStart();
        }
        else
        {
            mTouchBoxColli.size = mTouchBoxColliInitSize;

            if (OnMoveEnd != null)
                OnMoveEnd();
        }
    }

    #endregion

    #region 函数

    private void UpdateJoystick()
    {
        Transform touchtrans = mTouch.transform;
        Vector2 tempaxis = touchtrans.localPosition / GetRadius();
        if (!mOnTouch)
        {
            float x = Input.GetAxis(InputMgrName.Horizontal);
            float y = Input.GetAxis(InputMgrName.Vertical);

            if ((x != 0.0f || y != 0.0f) && (mOldTmpAxis.x == 0.0f && mOldTmpAxis.y == 0.0f))
            {
                mTouchBoxColli.size = DRAGING_BOXCOLLI_SIZE;

                if (OnMoveStart != null)
                    OnMoveStart();
            }
            else if ((x == 0.0f && y == 0.0f) && (mOldTmpAxis.x != 0.0f || mOldTmpAxis.y != 0.0f))
            {
                mTouchBoxColli.size = mTouchBoxColliInitSize;

                if (OnMoveEnd != null)
                    OnMoveEnd();
            }

            touchtrans.localPosition = Vector2.zero;
            if (x != 0.0f)
            {
                touchtrans.localPosition = new Vector2(GetRadius() * x, touchtrans.localPosition.y);
            }

            if (y != 0.0f)
            {
                touchtrans.localPosition = new Vector2(touchtrans.localPosition.x, GetRadius() * y);
            }

            touchtrans.localPosition = Vector3.ClampMagnitude(touchtrans.localPosition, GetRadius());
            tempaxis = touchtrans.localPosition / GetRadius();
            mOldTmpAxis = new Vector2(x, y);
        }

        if (tempaxis.x != 0.0f || tempaxis.y != 0.0f)
        {
            if (OnMove != null)
                OnMove(tempaxis);
        }
    }

    private float GetRadius()
    {
        return mRadius;
    }

    #endregion
}
