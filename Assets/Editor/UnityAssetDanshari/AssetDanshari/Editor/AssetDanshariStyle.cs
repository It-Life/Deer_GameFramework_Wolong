using UnityEditor;
using UnityEngine;

namespace AssetDanshari
{
    public class AssetDanshariStyle
    {
        public class Style
        {
            public GUIContent help = new GUIContent("帮助");
            public GUIContent about = new GUIContent("关于");
            public GUIContent exportCsv = new GUIContent("导出 CSV");
            public GUIContent forceText = new GUIContent("Asset Serialization must be ForceText");
            public GUIContent assetReferenceTitle = new GUIContent("检查列表");
            public GUIContent assetReferenceAsset = new GUIContent("资源目录", "存放资源的文件夹路径");
            public GUIContent assetReferenceAssetCommon = new GUIContent("公共资源目录", "整理资源时所放置的公共文件夹路径");
            public GUIContent assetReferenceReference = new GUIContent("引用目录", "使用到资源的预制文件夹路径");
            public GUIContent assetReferenceCheckDup = new GUIContent("检查重复");
            public GUIContent assetReferenceCheckRef = new GUIContent("引用查找");
            public GUIContent assetReferenceDepend = new GUIContent("被引用查找");

            public GUIContent duplicateTitle = new GUIContent("重复资源检查");
            public GUIContent duplicateWaiting = new GUIContent("等待进行检查重复资源");
            public GUIContent duplicateNothing = new GUIContent("所检查的文件夹没有重复资源");
            public GUIContent duplicateManualAdd = new GUIContent("手动添加");
            public GUIContent duplicateHeaderContent = new GUIContent("名称");
            public GUIContent duplicateHeaderContent2 = new GUIContent("路径");
            public GUIContent duplicateHeaderContent3 = new GUIContent("大小");
            public GUIContent duplicateHeaderContent4 = new GUIContent("创建时间");
            public GUIContent duplicateContextOnlyUseThis = new GUIContent("仅使用此资源，其余删除");
            public string duplicateContextMoveComm = "移入公共目录/";
            public string duplicateGroup = "文件数：{0}";

            public GUIContent dependenciesTitle = new GUIContent("资源被引用查找");
            public GUIContent dependenciesWaiting = new GUIContent("等待进行资源被引用查找");
            public GUIContent dependenciesNothing = new GUIContent("资源文件夹没有任何数据");
            public GUIContent dependenciesHeaderContent2 = new GUIContent("被引用的路径");
            public GUIContent dependenciesDelete = new GUIContent("删除选中资源");
            public GUIContent dependenciesFilter = new GUIContent("过滤为空");

            public GUIContent referenceTitle = new GUIContent("引用查找");
            public GUIContent referenceWaiting = new GUIContent("等待进行引用查找");
            public GUIContent referenceNothing = new GUIContent("文件夹没有任何数据");
            public GUIContent referenceHeaderContent2 = new GUIContent("引用的资源");
            public GUIContent referenceHeaderContent3 = new GUIContent("隶属");

            public GUIContent iconDelete = EditorGUIUtility.IconContent("AS Badge Delete");
            public GUIContent iconNew = EditorGUIUtility.IconContent("AS Badge New");

            public string progressTitle = "正在处理";
            public string errorTitle = "错误信息";
            public string continueStr = "继续执行";
            public string cancelStr = "取消";
            public string sureStr = "确定";
            public string progressFinish = "处理结束";
            public string deleteFile = "正在删除文件...";
            public GUIContent expandAll = new GUIContent("展开");
            public GUIContent expandAll2 = new GUIContent("全部展开");
            public GUIContent expandAll3 = new GUIContent("全部展开除最后一层");
            public GUIContent collapseAll = new GUIContent("折叠");
            public GUIContent collapseAll2 = new GUIContent("全部折叠");
            public GUIContent collapseAll3 = new GUIContent("仅折叠最后一层");
            public GUIContent locationContext = new GUIContent("定位");
            public GUIContent explorerContext = new GUIContent("打开所在文件夹");
            public GUIContent nameHeaderContent = new GUIContent("名称");
            public Texture2D folderIcon = EditorGUIUtility.FindTexture("Folder Icon");

            public GUIStyle labelStyle;

            public void InitGUI()
            {
                if (labelStyle == null)
                {
                    labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.alignment = TextAnchor.MiddleRight;
                }
            }
        }

        private static Style sStyle;

        public static Style Get()
        {
            if (sStyle == null)
            {
                sStyle = new Style();
            }

            return sStyle;
        }
    }
}