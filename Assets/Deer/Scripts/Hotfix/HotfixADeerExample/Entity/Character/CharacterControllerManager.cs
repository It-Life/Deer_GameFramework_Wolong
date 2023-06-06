/* ================================================
 * Introduction：角色管理器 
 * Creator：杜鑫 
 * CreationTime：2022-03-18 18-50-30
 * CreateVersion：0.1
 *  =============================================== */
using System;
using UnityEngine;

namespace HotfixBusiness.Entity
{
    /// <summary>
    /// 角色管理器
    /// </summary>
    public class CharacterControllerManager
    {
        private Character m_Owner;
        /// <summary>
        /// 是否在地面上
        /// </summary>
        public bool IsGround { get; set; } = false;
        /// <summary>
        /// 重力大小
        /// </summary>
        [SerializeField]
        private float Gravity { get; set; } = 23f;
        /// <summary>
        /// 角色重力速度
        /// </summary>
        [SerializeField]
        public float CharacterGravitySpeed { get; set; }
        /// <summary>
        /// 角色控制器
        /// </summary>
        [SerializeField]
        private CharacterController m_CharacterController;

        public CharacterController Controller { get { return m_CharacterController; } }

        /// <summary>
        /// 真正的移动方向
        /// </summary>
        private Vector3 m_MoveDir = Vector3.zero;
        /// <summary>
        /// 是否可移动
        /// </summary>
        public bool IsCanMove { get; set; } = true;
        /// <summary>
        /// 恒定位移速度
        /// </summary>
        public float ConstantSpeed { get; set; }

        public CharacterControllerManager(Character owner, CharacterController characterController)
        {
            m_Owner = owner;
            m_CharacterController = characterController;
            ConstantSpeed = m_Owner.CharacterData.ConstantSpeed;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (!m_Owner.CharacterData.UnUseGravity)
            {
                UpdateGravity(elapseSeconds);
            }


            m_MoveDir += Vector3.right * ConstantSpeed * elapseSeconds;
            if (m_MoveDir != Vector3.zero && !IsCanMove)
            {
                m_MoveDir.x = 0;
                m_MoveDir.z = 0;
            }
            if (m_MoveDir != Vector3.zero)
            {
                if (m_Owner.IsPauseFram)
                {
                    m_CharacterController.Move(m_MoveDir * m_Owner.PauseFrameAniPercent);
                }
                else
                {
                    m_CharacterController.Move(m_MoveDir);
                }
            }
            m_MoveDir = Vector3.zero;
        }
        /// <summary>
        /// 角色坠落
        /// </summary>
        private void UpdateGravity(float elapseSeconds)
        {
            if (m_CharacterController == null || !m_CharacterController.enabled)
            {
                return;
            }

            if (!m_CharacterController.isGrounded)
            {
                IsGround = GetCharacterIsInGround();
                if (m_Owner.IsPauseFram)
                {
                    CharacterGravitySpeed -= elapseSeconds * Gravity * m_Owner.PauseFrameAniPercent;
                }
                else
                {
                    CharacterGravitySpeed -= elapseSeconds * Gravity;
                }
            }
            else
            {
                IsGround = true;
            }

            if (m_CharacterController.isGrounded)
            {
                CharacterGravitySpeed = 0;
            }

            if (m_Owner.gameObject.activeInHierarchy && CharacterGravitySpeed != 0)
            {
                Move(new Vector3(0, CharacterGravitySpeed * elapseSeconds, 0));
            }
        }
        /// <summary>
        /// 获取角色是否在地面上
        /// </summary>
        /// <returns></returns>
        private bool GetCharacterIsInGround()
        {
            Vector3 startPos = m_Owner.CachedTransform.position + Vector3.up * 2f;

#if UNITY_EDITOR
            Debug.DrawLine(startPos, m_Owner.CachedTransform.position, Color.blue);
#endif
            return Physics.Raycast(startPos, Vector3.down, 2.1f, 1 << Constant.Leyer.GroundId);
        }
        #region 角色运动处理
        /// <summary>
        /// 控制器移动
        /// </summary>
        /// <param name="dir"></param>
        public void Move(Vector3 dir)
        {
            m_MoveDir += dir;
        }
        /// <summary>
        /// 角色移动
        /// </summary>
        /// <param name="dir">方向</param>
        /// <param name="speed">速度</param>
        public void OnMove(Vector3 dir, float speed)
        {
            if (dir == Vector3.zero)
            {
                return;
            }
            dir = dir.normalized;
            m_Owner.m_Velocity = dir * speed;
            m_Owner.m_Velocity += Vector3.up * CharacterGravitySpeed;
            dir *= speed * Time.deltaTime;
            Move(dir);
        }


        #endregion
    }
}