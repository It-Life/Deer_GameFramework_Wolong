// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-05 18-48-32
//修改作者:杜鑫
//修改时间:2022-06-05 18-48-32
//版 本:0.1 
// ===============================================
using GameFramework;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Main.Runtime.Procedure
{
    public class ProcedureInitResources : ProcedureBase
    {
        public override bool UseNativeDialog => true;

        private bool m_InitResourcesComplete = false;
        private bool m_InitAssembliesComplete = false;
        private bool m_InitConfigComplete = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_InitResourcesComplete = false;
            GameEntryMain.Assemblies.InitAssembliesVersion(OnInitAssembliesComplete);    
            GameEntryMain.LubanConfig.InitConfigVersion(OnInitConfigComplete);    
            // 注意：使用单机模式并初始化资源前，需要先构建 AssetBundle 并复制到 StreamingAssets 中，否则会产生 HTTP 404 错误
            GameEntryMain.Resource.InitResources(OnInitResourcesComplete);
        }
        

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_InitResourcesComplete)
            {
                // 初始化资源未完成则继续等待
                return;
            }
            if (!m_InitAssembliesComplete)
            {
                return;
            }

            if (!m_InitConfigComplete)
            {
                return;   
            }
            ChangeState<ProcedureLoadAssembly>(procedureOwner);
        }

        private void OnInitResourcesComplete()
        {
            m_InitResourcesComplete = true;
            Log.Info("Init resources complete.");
        }
        private void OnInitAssembliesComplete()
        {
            m_InitAssembliesComplete = true;
            Log.Info("Init assemblies complete.");
        }

        private void OnInitConfigComplete()
        {
            m_InitConfigComplete = true;
            Log.Info("Init config complete.");
        }
    }
}