using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AutoBindTest : MonoBehaviour
{

    private int count = 0;


    void Start()
    {
        GetBindComponents(gameObject);
        m_Btn_Test2.onClick.AddListener(OnBtnClick);
    }

    private void OnBtnClick()
    {
        count++;
        m_Txt_Test3.text = "点击了按钮" + count + "次";
        m_Img_Test1.gameObject.SetActive(!m_Img_Test1.gameObject.activeInHierarchy);
    }
}
