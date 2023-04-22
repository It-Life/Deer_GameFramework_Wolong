using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LabelBhv : EmptyBhv
    {
        // Public properties
        public string Text
        {
            set
            {
                TextMeshPro.text = value;
            }
        }
        public TMP_FontAsset FontAsset
        {
            set
            {
                TextMeshPro.font = value;
            }
        }
        public FontStyles FontStyle
        {
            set
            {
                TextMeshPro.fontStyle = value;
            }
        }
        public float FontSize
        {
            set
            {
                TextMeshPro.fontSize = value;
            }
        }
        public Color FontColor
        {
            set
            {
                TextMeshPro.color = value;
            }
        }
        public TextAlignmentOptions Alignment
        {
            set
            {
                TextMeshPro.alignment = value;
            }
        }
        public Vector4 Margins
        {
            set
            {
                TextMeshPro.margin = value;
            }
        }

        // Protected properties
        protected TextMeshProUGUI TextMeshPro => _textMeshPro == null ? GetComponent<TextMeshProUGUI>() : _textMeshPro;

        // Private fields
        private TextMeshProUGUI _textMeshPro;

        private void Awake()
        {
            _textMeshPro = this.TextMeshPro;
        } 
    }
}
