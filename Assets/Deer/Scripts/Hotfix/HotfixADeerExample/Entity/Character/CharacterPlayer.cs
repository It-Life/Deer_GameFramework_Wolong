// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-19 23-03-42  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-19 23-03-42  
//版 本 : 0.1 
// ===============================================
using GameFramework;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    public class CharacterPlayer : Character
    {
        public EntityEnum EntityType = EntityEnum.CharacterPlayer;

        private bool m_isMoveing = false;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            GameEntry.Messenger.RegisterEvent(EventName.EVENT_CS_GAME_MOVE_DIRECTION, OnHandleMoveDirectionCallback);
            GameEntry.Messenger.RegisterEvent(EventName.EVENT_CS_GAME_MOVE_END, OnHandleMoveEndCallback);
            GameEntry.Messenger.RegisterEvent(EventName.EVENT_CS_GAME_START_JUMP, OnHandleJumpCallback);
            CharacterPlayerData entityData = userData as CharacterPlayerData;
            if (entityData == null)
                return;
            if (entityData.IsOwner)
            {
                IsOwner = true;
                //Transform transCamTarget = CachedTransform.Find("CamTaget");
                GameEntry.Camera.FollowAndFreeViewTarget(CachedTransform, CachedTransform);
            }

        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            GameEntry.Messenger.UnRegisterEvent(EventName.EVENT_CS_GAME_MOVE_DIRECTION, OnHandleMoveDirectionCallback);
            GameEntry.Messenger.UnRegisterEvent(EventName.EVENT_CS_GAME_MOVE_END, OnHandleMoveEndCallback);
            GameEntry.Messenger.UnRegisterEvent(EventName.EVENT_CS_GAME_START_JUMP, OnHandleJumpCallback);
            GameEntry.Camera.FollowAndFreeViewTarget(null, null);
        }

        private void Update()
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                m_isMoveing = true;
                MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
                messengerInfo.param1 = Input.GetAxisRaw("Horizontal");
                messengerInfo.param2 = Input.GetAxisRaw("Vertical");
                GameEntry.Messenger.SendEvent(EventName.EVENT_CS_GAME_MOVE_DIRECTION, messengerInfo);
            }
            else
            {
                if (m_isMoveing)
                {
                    MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
                    messengerInfo.param1 = false;
                    GameEntry.Messenger.SendEvent(EventName.EVENT_CS_GAME_MOVE_END, messengerInfo);
                    m_isMoveing = false;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnHandleJumpCallback(null);
            }
        }

        private object OnHandleMoveDirectionCallback(object pSender)
        {
            MessengerInfo messengerInfo = (MessengerInfo)pSender;
            if (messengerInfo == null) return null;
            if (!IsOwner)
            {
                return null;
            }
            Vector2 direction = new Vector2((float)messengerInfo.param1, (float)messengerInfo.param2);
            IsJoyStickControl = true;
            SetMoveMode(direction.magnitude >= 0.9f ? MoveMode.ForwardRun : MoveMode.Forward);
            JoyStickDirection = GameObjectUtils.GetFrezzeModeDirection(direction.x, direction.y);
            if (!StateController.IsInState(JumpState))
            {
                MessengerInfo messengerInfo1 = ReferencePool.Acquire<MessengerInfo>();
                messengerInfo1.param1 = MoveType.MoveToDir;
                messengerInfo1.param2 = JoyStickDirection;
                messengerInfo1.param3 = 1f;
                MoveState.SetParam(messengerInfo1);
                StateController.OnChangeState(MoveState);
            }
            if (CharacterData.JumpCanMove)
            {
                //CharacterControllerManager.OnMove();
            }
            return null;
        }
        private object OnHandleMoveEndCallback(object pSender)
        {
            MessengerInfo messengerInfo = (MessengerInfo)pSender;
            if (messengerInfo == null) return null;
            if (!IsOwner)
            {
                return null;
            }
            IsJoyStickControl = false;
            JoyStickDirection = Vector3.zero;
            StateController.OnChangeState(IdleState);
            return null;
        }
        private object OnHandleJumpCallback(object pSender)
        {
            if (!CanJump())
            {
                return null;
            }
            SetMoveMode(MoveState.MoveMode == MoveMode.ForwardRun ? MoveMode.JumpRun : MoveMode.Jump);
            CharacterControllerManager.CharacterGravitySpeed = CharacterData.JumpPower;
            StateController.OnChangeState(JumpState);
            return null;
        }
    }
}