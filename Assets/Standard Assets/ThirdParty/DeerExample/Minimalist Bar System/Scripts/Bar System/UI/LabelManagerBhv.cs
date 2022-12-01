using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Minimalist.Bar.UI
{
    public class LabelManagerBhv : EmptyBhv
    {
        // Public properties
        public string Text
        {
            set
            {
                foreach (LabelBhv label in Labels)
                {
                    label.Text = value;
                }
            }
        }
        public TMP_FontAsset FontAsset
        {
            set
            {
                foreach (LabelBhv label in Labels)
                {
                    label.FontAsset = value;
                }
            }
        }
        public FontStyles FontStyle
        {
            set
            {
                foreach (LabelBhv label in Labels)
                {
                    label.FontStyle = value;
                }
            }
        }
        public float FontSize
        {
            set
            {
                foreach (LabelBhv label in Labels)
                {
                    label.FontSize = value;
                }
            }
        }
        public TextAlignmentOptions Alignment
        {
            set
            {
                foreach (LabelBhv label in Labels)
                {
                    label.Alignment = value;
                }
            }
        }
        public Vector4 Margins
        {
            set
            {
                foreach (LabelBhv label in Labels)
                {
                    label.Margins = value;
                }
            }
        }
        public MainLabelBhv Main => _mainLabel == null ? GetComponentInChildren<MainLabelBhv>() : _mainLabel;
        public IncrementLabelBhv Increment => _incrementLabel == null ? GetComponentInChildren<IncrementLabelBhv>() : _incrementLabel;
        public DecrementLabelBhv Decrement => _decrementLabel == null ? GetComponentInChildren<DecrementLabelBhv>() : _decrementLabel;
        public BackgroundLabelBhv Background => _backgroundLabel == null ? GetComponentInChildren<BackgroundLabelBhv>() : _backgroundLabel;

        // Private properties
        private LabelBhv[] Labels => _labels == null || _labels.Length == 0 ? this.GetComponentsInChildren<LabelBhv>() : _labels;

        // Private fields
        private LabelBhv[] _labels;
        private MainLabelBhv _mainLabel;
        private IncrementLabelBhv _incrementLabel;
        private DecrementLabelBhv _decrementLabel;
        private BackgroundLabelBhv _backgroundLabel;

        private void Awake()
        {
            _labels = Labels;

            _mainLabel = Main;

            _incrementLabel = Increment;

            _decrementLabel = Decrement;

            _backgroundLabel = Background;
        }

        public void UpdateLabel(float amount, float maximum)
        {
            string leadingSpaces = new string(' ', maximum.ToString("f0").Length - amount.ToString("f0").Length);

            Text = leadingSpaces + amount.ToString("f0") + "/" + maximum.ToString("f0");
        }
    }
}
