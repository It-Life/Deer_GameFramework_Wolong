using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.SampleScene
{
    public class GameManager : Singleton<GameManager>
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}