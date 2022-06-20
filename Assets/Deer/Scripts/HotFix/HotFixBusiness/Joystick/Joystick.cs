// Copyright (c) Bian Shanghai
// https://github.com/Bian-Sh/UniJoystick
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace zFrame.UI
{
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Tooltip("Handle 移动最大半径")]
        public float maxRadius = 38; //Handle 移动最大半径
        [Tooltip("选择激活的轴向")]
        public Direction activatedAxis = Direction.Both; //选择激活的轴向
        [Tooltip("是否为动态摇杆")]
        [SerializeField] bool dynamic = true; // 是否为动态摇杆
        [Tooltip("是否展示指向器")]
        [SerializeField] bool showDirectionArrow = true; // 是否展示指向器
        [Tooltip("摇杆")]
        [SerializeField] Transform handle; //摇杆
        [Tooltip("背景")]
        [SerializeField] Transform backGround; //背景
        [Tooltip("指向器")]
        [SerializeField] Transform directionArrow; //指向器
        [Tooltip("事件 ： 摇杆被拖拽时")]
        public JoystickEvent OnValueChanged = new JoystickEvent(); //事件 ： 摇杆被拖拽时
        [Tooltip("事件 ： 摇杆被按下时")]
        [HideInInspector] public JoystickEvent OnPointerDown = new JoystickEvent(); // 事件： 摇杆被按下时
        [Tooltip("事件 ： 摇杆上抬起时")]
        [HideInInspector] public JoystickEvent OnPointerUp = new JoystickEvent(); //事件 ： 摇杆上抬起时
        #region Property
        public bool IsDraging { get { return fingerId != int.MinValue; } } //摇杆拖拽状态
        public int FingerId { get { return fingerId; } }
        public bool ShowDirectionArrow { get => showDirectionArrow; set => showDirectionArrow = value; }  // 是否展示指向器
        public bool IsDynamic //运行时代码配置摇杆是否为动态摇杆
        {
            set
            {
                if (dynamic != value)
                {
                    dynamic = value;
                    ConfigJoystick();
                }
            }
            get => dynamic;
        }
        #endregion

        #region MonoBehaviour functions
        void Awake() 
        {
            backGroundOriginLocalPostion = backGround.localPosition;
            //GameEntry.UI?.JoystickList.Add(this);
        }
        void FixedUpdate() => OnValueChanged.Invoke(handle.localPosition / maxRadius); 
        void OnDisable() => RestJoystick(); //意外被 Disable 各单位需要被重置
        void OnValidate() => ConfigJoystick(); //Inspector 发生改变，各单位需要重新配置，编辑器有效
        void Reset() => InitJoystick();
        void OnDestroy()
        {
            //GameEntry.UI?.JoystickList?.Remove(this);
        }
        #endregion

        #region The implement of pointer event Interface
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerId < -1 || IsDraging) return; //适配 Touch：只响应一个Touch；适配鼠标：只响应左键
            fingerId = eventData.pointerId;
            pointerDownPosition = eventData.position;
            if (dynamic)
            {
                pointerDownPosition[2] = eventData.pressEventCamera?.WorldToScreenPoint(backGround.position).z ?? backGround.position.z;
                backGround.position = eventData.pressEventCamera?.ScreenToWorldPoint(pointerDownPosition) ?? pointerDownPosition; ;
            }
            OnPointerDown.Invoke(eventData.position);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (fingerId != eventData.pointerId) return;
            Vector2 direction = eventData.position - (Vector2)pointerDownPosition; //得到BackGround 指向 Handle 的向量
            float radius = Mathf.Clamp(Vector3.Magnitude(direction), 0, maxRadius); //获取并锁定向量的长度 以控制 Handle 半径
            Vector2 localPosition = new Vector2()
            {
                x = (activatedAxis ==  Direction.Both ||activatedAxis == Direction.Horizontal) ? (direction.normalized * radius).x : 0, //确认是否激活水平轴向
                y = (activatedAxis == Direction.Both || activatedAxis == Direction.Vertical) ? (direction.normalized * radius).y : 0       //确认是否激活垂直轴向，激活就搞事情
            };
            handle.localPosition = localPosition;      //更新 Handle 位置
            if (showDirectionArrow)
            {
                if(!directionArrow.gameObject.activeInHierarchy)directionArrow.gameObject.SetActive(true);
                directionArrow.localEulerAngles = new Vector3(0, 0, Vector2.Angle(Vector2.right, localPosition) * (localPosition.y > 0 ? 1 : -1));
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (fingerId != eventData.pointerId) return;//正确的手指抬起时才会重置摇杆；
            RestJoystick();
            OnPointerUp.Invoke(eventData.position);
        }
        #endregion

        #region Assistant functions / fields / structures
        void RestJoystick()  //重置摇杆数据
        {
            backGround.localPosition = backGroundOriginLocalPostion;
            handle.localPosition = Vector3.zero;
            directionArrow.gameObject.SetActive(false);
            fingerId = int.MinValue;
        }

        void ConfigJoystick() //配置动态/静态摇杆
        {
            if (!dynamic) backGroundOriginLocalPostion = backGround.localPosition;
            //GetComponent<Image>().raycastTarget = dynamic;
           // handle.GetComponent<Image>().raycastTarget = !dynamic;
        }

         void InitJoystick() //编辑器下按需初始化摇杆
        {
            if (!backGround) backGround = transform.Find("BackGround");
            if (!handle) handle = backGround.Find("Handle");
            if (!directionArrow) directionArrow = backGround.Find("Direction");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        private Vector3 backGroundOriginLocalPostion, pointerDownPosition;
        private int fingerId = int.MinValue; //当前触发摇杆的 pointerId ，预设一个永远无法企及的值
        [System.Serializable] public class JoystickEvent : UnityEvent<Vector2> { }
        public enum Direction
        {
            Both,
            Horizontal,
            Vertical
        }
        #endregion
    }
}
