// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-19 23-03-42  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-19 23-03-42  
//版 本 : 0.1 
// ===============================================
namespace HotfixBusiness.Entity
{
    public partial class Character
    {
        public IdleState IdleState { get; set; }
        public MoveState MoveState { get; set; }
        public JumpState JumpState { get; set; }
        public void GetCharacterState()
        {
            MoveState = new MoveState();
            IdleState = new IdleState();
            JumpState = new JumpState();
        }
    }
}