// ================================================
//描 述:
//作 者:LX
//创建时间:2022-05-10 11-53-31
//修改作者:LX
//修改时间:2022-05-10 11-53-31
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public class AutoSetModleFormat : AssetPostprocessor
{
    //模型导入之前调用
    public void OnPostprocessModel(GameObject go)
    {
        ModelImporter model = (ModelImporter)assetImporter;
        if (assetPath.Contains("Models/Static"))
        {
            model.isReadable = true;
        }
        //if (model != null)
        //{
        //    DoModelSettings(model);
        //    if (isLoopAnimation(model.name))
        //    {
        //        //由于我们采用动画分离的导出策略，每个fbx只有一个动画
        //        if (model.defaultClipAnimations.Length > 0)
        //        {
        //            List<ModelImporterClipAnimation> actions = new List<ModelImporterClipAnimation>();
        //            ModelImporterClipAnimation anim = model.defaultClipAnimations[0];
        //            anim.loopTime = true;
        //            actions.Add(anim);
        //            model.clipAnimations = actions.ToArray();
        //        }
        //    }
        //}
        #region Inner
        //bool isLoopAnimation(string objectName)
        //{
        //    bool res = false;
        //    if (objectName.Contains("wait"))
        //    {
        //        res = true;
        //    }
        //    else if (objectName.Contains("walk"))
        //    {
        //        res = true;
        //    }
        //    else if (objectName.Contains("run"))
        //    {
        //        res = true;
        //    }
        //    else if (objectName.Contains("air"))
        //    {
        //        res = true;
        //    }
        //    else if (objectName.Contains("dizziness"))
        //    {
        //        res = true;
        //    }
        //    return res;
        //}
        #endregion
        /// <summary>
        /// 模型参数自动设置
        /// </summary>
        //void DoModelSettings(ModelImporter M_model)
        //{
        //    ModelImporterMeshCompression mesh = M_model.meshCompression;
        //    //model.importBlendShapes = false;
        //    //model.importBlendShapeNormals = ModelImporterNormals.Import;
        //    //model.importNormals = ModelImporterNormals.Import;

        //    //string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        //    //ModelImporter modelimp = AssetImporter.GetAtPath(path) as ModelImporter;
        //    //modelimp.animationWrapMode = WrapMode.Once;
        //    ////modelimp.animationType = ModelImporterAnimationType.Human;
        //    //modelimp.animationCompression = ModelImporterAnimationCompression.KeyframeReductionAndCompression;
        //    //ModelImporterClipAnimation[] anims = modelimp.clipAnimations;

        //    //ModelImporter
        //    //ShaderImporter;
        //    //VideoClipImporter;
        //    //ModelImporter;
        //    //ModelImporterClipAnimation;
        //    //AudioImporter;
        //    //ModelImporter;
        //    //TextureImporter;
        //    //AssetImporter;
        //}
    }
}