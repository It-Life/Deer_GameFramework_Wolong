// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-08-24 16-32-15
//修改作者:杜鑫
//修改时间:2022-08-24 16-32-15
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MeshRenderEditor : Editor
{
    [MenuItem("DeerTools/CombineMesh")]
    public static void EditorTest()
    {
        CombineMesh();
    }
    /// <summary>
    /// 图片缓存路径
    /// </summary>
    private static string JpgPath = "Assets/texture.jpg";
    /// <summary>
    /// material缓存路径
    /// </summary>
    private static string MaterialPath = "Assets/mat.mat";
    static void CombineMesh()
    {
        #region 如果在游戏运行时动态合并网格,在脚本Start里运行这块代码就可以了
        GameObject gameObject = Selection.gameObjects[0];
        MeshFilter[] mfChildren = gameObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[mfChildren.Length];

        MeshRenderer[] mrChildren = gameObject.GetComponentsInChildren<MeshRenderer>();
        Material[] materials = new Material[mrChildren.Length];

        MeshRenderer mrSelf = gameObject.AddComponent<MeshRenderer>();
        MeshFilter mfSelf = gameObject.AddComponent<MeshFilter>();

        Texture2D[] textures = new Texture2D[mrChildren.Length];
        for (int i = 0; i < mrChildren.Length; i++)
        {
            materials[i] = mrChildren[i].sharedMaterial;
            Texture2D tx = materials[i].GetTexture("_MainTex") as Texture2D;

            Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.ARGB32, false);
            tx2D.SetPixels(tx.GetPixels(0, 0, tx.width, tx.height));
            tx2D.Apply();
            textures[i] = tx2D;
        }

        Material materialNew = new Material(materials[0].shader);
        materialNew.CopyPropertiesFromMaterial(materials[0]);
        mrSelf.sharedMaterial = materialNew;

        Texture2D texture = new Texture2D(1024, 1024);
        materialNew.SetTexture("_MainTex", texture);
        Rect[] rects = texture.PackTextures(textures, 10, 1024);

        for (int i = 0; i < mfChildren.Length; i++)
        {
            if (mfChildren[i].transform == gameObject.transform)
            {
                continue;
            }
            Rect rect = rects[i];

            Mesh meshCombine = mfChildren[i].mesh;
            Vector2[] uvs = new Vector2[meshCombine.uv.Length];
            //把网格的uv根据贴图的rect刷一遍
            for (int j = 0; j < uvs.Length; j++)
            {
                uvs[j].x = rect.x + meshCombine.uv[j].x * rect.width;
                uvs[j].y = rect.y + meshCombine.uv[j].y * rect.height;
            }
            meshCombine.uv = uvs;
            combine[i].mesh = meshCombine;
            combine[i].transform = mfChildren[i].transform.localToWorldMatrix;
            mfChildren[i].gameObject.SetActive(false);
        }

        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combine, true, true);//合并网格
        mfSelf.mesh = newMesh;
        #endregion 下面的代码是吧生成的Material,和合并的图片缓存在本地


        FileWriteTexture(texture, JpgPath);
        CreateMaterial(materialNew);
        LoadTextureAlter(JpgPath);
        MaterialSetTexture(MaterialPath, JpgPath);

    }
    /// <summary>
    /// 把合并好的图片缓存在"Assets/texture.jpg"路径下
    /// </summary>
    /// <param name="texture"></param>
    static void FileWriteTexture(Texture2D texture, string jpgPath)
    {
        var bytes = texture.EncodeToPNG();
        FileStream file = File.Open(jpgPath, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();
        texture.Apply();
    }
    /// <summary>
    /// 合并后的材质球缓存到MaterialPath路径下,刷新
    /// </summary>
    /// <param name="materialNew"></param>
    static void CreateMaterial(Material materialNew)
    {
        AssetDatabase.CreateAsset(materialNew, MaterialPath);
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 加载缓存合并图片,修改里面的参数
    /// </summary>
    static void LoadTextureAlter(string path)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Default;
        textureImporter.isReadable = true;
        AssetDatabase.ImportAsset(path);
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 缓存的Material存在上面贴图丢失的现象,重新绑定贴图数据
    /// </summary>
    static void MaterialSetTexture(string materialPath, string jpgPath)
    {
        Material LoadMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        Texture2D Loadtextur = AssetDatabase.LoadAssetAtPath<Texture2D>(jpgPath);
        LoadMaterial.SetTexture("_MainTex", Loadtextur);
        AssetDatabase.Refresh();
    }
}
