/* ================================================
 * Introduction：xxx 
 * Creator：杜鑫 
 * CreationTime：2022-03-18 17-16-14
 * CreateVersion：0.1
 *  =============================================== */
using GameFramework;
using Pathfinding;
using System;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(FunnelModifier))]
    [Serializable]
    public partial class Character : EntityLogicBase
    {
        private CharacterData m_CharacterData;
        public CharacterData CharacterData { get { return m_CharacterData; } private set { m_CharacterData = value; } }

        public GameObject CharacterModel { get; private set; }
        public Animator Animator { get; private set; }
        public StateController StateController { get; set; }
        public AIStateController AIStateController { get; set; }

        public CharacterControllerManager CharacterControllerManager { get; set; }
        public bool IsOwner { get; set; }

        [SerializeField]
        private Character m_MyCurrentTarget;
        /// <summary>当前的目标</summary>
        public Character MyCurrentTarget
        {
            get { return m_MyCurrentTarget; }
            set
            {
                m_MyCurrentTarget = value;
            }
        }

        private bool m_IsPauseFram;
        /// <summary>
        /// 速度
        /// </summary>
        protected internal Vector3 m_Velocity;
        /// <summary>
        /// 是否顿帧，顿帧只针对受击，击飞，技能位移有效果
        /// </summary>
        public bool IsPauseFram
        {
            get
            {
                return m_IsPauseFram;
            }
            set
            {
                m_IsPauseFram = value;
            }
        }
        /// <summary>
        /// 顿帧时，动画和移动速度减慢的百分比
        /// </summary>
        public float PauseFrameAniPercent { get; set; }
        /// <summary>
        /// 摇杆方向
        /// </summary>
        public Vector3 JoyStickDirection { get; set; }
        /// <summary>
        /// 是否是摇杆控制
        /// </summary>
        public bool IsJoyStickControl { get; set; } = false;
        /// <summary>
        /// 是否使用自定义面向
        /// </summary>
        public bool UseCustomFace { get; set; } = false;

        /// <summary>
        /// 角色面向限制
        /// </summary>
        public SceneCharacterFace UseFaceType = SceneCharacterFace.Free;

        private MoveDirection m_MoveDirection;

        /// <summary>
        /// 使用两个方向
        /// </summary>
        public bool UserTwoDirction = false;
        /// <summary>
        /// 角色朝向
        /// </summary>
        public MoveDirection MoveDirection
        {
            get
            {
                return m_MoveDirection;
            }
            set
            {
                switch (UseFaceType)
                {
                    case SceneCharacterFace.Free:
                    case SceneCharacterFace.LeftAndRight:
                    default:
                        m_MoveDirection = value;
                        break;
                    case SceneCharacterFace.Right:
                        m_MoveDirection = MoveDirection.Right;
                        break;
                    case SceneCharacterFace.Left:
                        m_MoveDirection = MoveDirection.Left;
                        break;
                }
            }
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            CharacterData = (CharacterData)userData;
            if (CharacterData == null) { Logger.Error("characterData is invalid."); return; }
            CachedTransform.position = CharacterData.Position;
            CharacterModel = CachedTransform.Find("Model").gameObject;
            Animator = CharacterModel.GetComponent<Animator>();
            CharacterController controller = CachedTransform.GetComponent<CharacterController>();
            controller.enabled = true;
            CharacterControllerManager = new CharacterControllerManager(this, controller);
            m_Seeker = CachedTransform.GetComponent<Seeker>();
            GetCharacterState();
            StateController = new StateController(this);
            StateController.OnChangeState(IdleState);
            InitAIStateController();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (!enabled || !Visible)
            {
                return;
            }
            if (CharacterControllerManager != null)
            {
                CharacterControllerManager.Update(elapseSeconds, realElapseSeconds);
            }
            if (StateController != null)
            {
                StateController.OnUpdate(elapseSeconds, realElapseSeconds);
            }
            if (AIStateController != null)
            {
                AIStateController.OnUpdate(elapseSeconds, realElapseSeconds);
            }
        }
        public void SetMoveMode(MoveMode moveMode)
        {
            MoveState.MoveMode = moveMode;
        }

        private void InitAIStateController()
        {
            if (AIStateController != null)
            {
                AIStateController.Clear();
                AIStateController = null;
            }
            AIStateController = new AIStateController(this);
            if (CharacterData.AIStates != null && CharacterData.AIStates.Count > 0)
            {
                AIState[] aIStates = new AIState[CharacterData.AIStates.Count];
                for (int i = 0; i < CharacterData.AIStates.Count; i++)
                {
                    string type = CharacterData.AIStates[i];
                    Type aiStateType = Utility.Assembly.GetType(type);
                    if (aiStateType == null)
                    {
                        Logger.Error($"Can not find aiState type '{type}'.");
                        return;
                    }

                    AIState aIState = (AIState)Activator.CreateInstance(aiStateType);
                    if (aIState == null)
                    {
                        Logger.Error($"Can not create aiState instance '{type}'.");
                        return;
                    }
                    aIState.StateId = i;
                    aIStates[i] = aIState;
                }
                AIStateController.Initialize(this, aIStates);
                AIStateController.OnStartAIState(CharacterData.InitAIStateId);
            }
        }


        public void SetFaceDir(Vector3 vec3Dir)
        {
            if (vec3Dir == Vector3.zero)
            {
                return;
            }
            if (vec3Dir.magnitude > 0)
            {
                Quaternion freeRotation = Quaternion.LookRotation(vec3Dir);
                float diferenceRotation = freeRotation.eulerAngles.y - CachedTransform.eulerAngles.y;
                float eulerY = CachedTransform.eulerAngles.y;
                if (diferenceRotation < 0 || diferenceRotation > 0)
                {
                    eulerY = freeRotation.eulerAngles.y;
                }
                Vector3 euler = new Vector3(0, eulerY, 0);
                CachedTransform.rotation = Quaternion.Slerp(CachedTransform.rotation, Quaternion.Euler(euler), Time.deltaTime * CharacterData.TurningSpeed);
            }
        }
        public bool CanJump()
        {
            if (!CharacterControllerManager.IsGround)
            {
                return false;
            }
            if (StateController.IsInState(JumpState))
            {
                return false;
            }
            return true;
        }
        #region 寻路相关
        private Seeker m_Seeker;
        public Seeker Seeker { get { return m_Seeker; } set { m_Seeker = value; } }
        /// <summary>
        /// 是否在寻路中
        /// </summary>
        public bool IsInFinding { get; private set; }

        /// <summary>
        /// 路径
        /// </summary>
        public ABPath AStartPath { get; set; }
        /// <summary>
        /// 移动到某个点
        /// </summary>
        /// <param name="toPos">目标点</param>
        /// <param name="findCallBack">回调 true 表示寻路成功  false表示寻路失败</param>
        /// <param name="IgnoreWall">是否忽略墙移动</param>
        public void MoveToPoint(Vector3 toPos, GameFrameworkAction<bool, Vector3> findCallBack)
        {
            if (IsInFinding)
            {
                return;
            }
            if (Vector3.Distance(CachedTransform.position, toPos) < 0.2f)
            {
                findCallBack?.Invoke(false, CachedTransform.position);
                return;
            }

            IsInFinding = true;
            m_Seeker.StartPath(CachedTransform.position, toPos, (Path p) =>
            {
                IsInFinding = false;
                if (!p.error)
                {
                    AStartPath = (ABPath)p;
                    if (Vector3.Distance(AStartPath.endPoint, new Vector3(AStartPath.originalEndPoint.x, AStartPath.endPoint.y, AStartPath.originalEndPoint.z)) > 0.5f)
                    {
                        Logger.Warning(string.Format("目标点 '{0}' 超出寻路范围不能到达, 选择了最近的目标点 '{1}'", toPos, AStartPath.endPoint));
                        findCallBack?.Invoke(true, AStartPath.endPoint);
                    }
                    else
                    {
                        findCallBack?.Invoke(true, AStartPath.endPoint);
                    }
                }
                else
                {
                    Logger.ColorInfo(ColorType.greenyellow, "寻路出错");
                    AStartPath = null;
                    findCallBack?.Invoke(false, toPos);
                }
            });
        }
        #endregion
    }
}