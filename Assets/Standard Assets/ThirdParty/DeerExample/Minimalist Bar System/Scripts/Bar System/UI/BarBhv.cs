using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Minimalist.Bar.Utility;

namespace Minimalist.Bar.UI
{
    public class BarBhv : QuantitySubscriberBhv
    {
        // Private properties
        private BarCanvasBhv BarCanvas => _barCanvas == null || !Application.isPlaying ? GetComponentInParent<BarCanvasBhv>() : _barCanvas;
        private MainBorderBhv MainBorder => _mainBorder == null || !Application.isPlaying ? GetComponentInChildren<MainBorderBhv>() : _mainBorder;
        private BackgroundBhv Background => _background == null || !Application.isPlaying ? GetComponentInChildren<BackgroundBhv>() : _background;
        private ForegroundBhv Foreground => _foreground == null || !Application.isPlaying ? GetComponentInChildren<ForegroundBhv>() : _foreground;
        private FillManagerBhv FillManager => _fillManager == null || !Application.isPlaying ? GetComponentInChildren<FillManagerBhv>() : _fillManager;
        private GlowBhv Glow => _glow == null || !Application.isPlaying ? GetComponentInChildren<GlowBhv>() : _glow;
        private ShadowBhv Shadow => _shadow == null || !Application.isPlaying ? GetComponentInChildren<ShadowBhv>() : _shadow;
        private LabelManagerBhv LabelManager => _labelManager == null ? GetComponentInChildren<LabelManagerBhv>(true) : _labelManager;
        private SegmentBorderLayoutBhv SegmentBorderLayout => _segmentBorderLayout == null || !Application.isPlaying ? GetComponentInChildren<SegmentBorderLayoutBhv>() : _segmentBorderLayout;
        private float MaxBorderWidth => Mathf.Min(Width, Height) / 2f;
        private float SegmentBorderWidth => _mainBorderWidth * _segmentBorderWidthProportion;

        // Private serialized fields
        [Header("Colors:")]
        [SerializeField] private BarColors _barColors;

        [Header("Borders:")]
        [Tooltip("The width of the bar's frame / main border.")]
        [SerializeField, Min(0)] private float _mainBorderWidth = 5f;
        [Tooltip("The width of segment borders as a proportion of the main border's width (only applies if the subscribed quantity is segmented).")]
        [SerializeField, Range(0, 1)] private float _segmentBorderWidthProportion = .5f;

        [Header("Pseudo-lighting:")]
        [Tooltip("The proportion of the bar's foreground that the pseudo-glow covers.")]
        [SerializeField, Range(0, 1)] private float _glowProportion = .15f;
        [Tooltip("The proportion of the bar's foreground that the pseudo-shadow covers.")]
        [SerializeField, Range(0, 1)] private float _shadowProportion = .25f;

        [Header("Label:")]
        [SerializeField] private bool _displayLabel = true;
        [Tooltip("The Font Asset containing the glyphs that can be rendered for this text.")]
        [SerializeField] private TMP_FontAsset _labelFontAsset;
        [Tooltip("Styles to apply to the text such as Bold or Italic.")]
        [SerializeField] private FontStyles _labelFontStyle;
        [Tooltip("The size the text will be rendered at in points.")]
        [SerializeField, Range(10, 100)] private float _labelFontSize = 25;
        [Tooltip("The space between the text and the edge of its container.")]
        [SerializeField] private LabelMargins _labelMargins;
        [Tooltip("Horizontal and vertical aligment of the text within its container.")]
        [SerializeField] private TextAlignmentOptions _labelAligmentOptions = TextAlignmentOptions.Center;

        [Header("Fill Animation:")]
        [Tooltip("The time (in seconds) it takes for the bar's fastest fill (increment in increments, main in decrements) to react to a change in the subscribed quantity's fill amount.")]
        [SerializeField, Min(.05f)] private float _fillFastestTime = .25f;
        [Tooltip("The time (in seconds) it takes for the bar's main fill to react to an increment in the subscribed quantity's fill amount.")]
        [SerializeField] private float _fillIncrementCatchUpTime = .5f;
        [Tooltip("The time (in seconds) it takes for the bar's decrement fill to react to a decrement in the subscribed quantity's fill amount.")]
        [SerializeField] private float _fillDecrementCatchUpTime = 1f;

        [Header("Flash Animation:")]
        [Tooltip("The number of flash cycles per second.")]
        [SerializeField, Min(.1f)] private float _flashFrequency = 1;
        [Tooltip("The number of flash cycles per animation.")]
        [SerializeField, Range(1, 5)] private int _flashRepetitions = 1;

        // Private fields
        private BarCanvasBhv _barCanvas;
        private MainBorderBhv _mainBorder;
        private BackgroundBhv _background;
        private ForegroundBhv _foreground;
        private FillManagerBhv _fillManager;
        private SegmentBorderLayoutBhv _segmentBorderLayout;
        private GlowBhv _glow;
        private ShadowBhv _shadow;
        private LabelManagerBhv _labelManager;

        protected override void SpecifyQuantityEventActions()
        {
            OnQuantityTypeChangedAction = UpdateName;

            OnQuantityAmountChangedAction = UpdateBar;

            OnQuantityInvalidAmountAction = FlashBar;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            InstantiateBarCanvasIfNeeded();

            MakeAllChildrenUnpickable();

            MakeAllChildrenNotEditable();
        }

        private void OnTransformParentChanged()
        {
            InstantiateBarCanvasIfNeeded();
        }

        private void InstantiateBarCanvasIfNeeded()
        {
            //if (BarCanvas == null)
            //{
            //    GameObject barCanvasObject = new GameObject();

            //    Transform anchorTransform = RectTransform.parent;

            //    _barCanvas = barCanvasObject.AddComponent<BarCanvasBhv>();

            //    RectTransform.SetParent(barCanvasObject.transform);

            //    _barCanvas.transform.SetParent(anchorTransform);

            //    _barCanvas.AnchorTransform = anchorTransform;

            //    _barCanvas.UpdateCanvas();
            //}
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            ValidateDimensions();

            ValidateFillTimes();

            UpdateDimensions();

            UpdateColors();

            UpdateSegmentDelimeters();

            UpdateLabelManager();
        }

        private void UpdateLabelManager()
        {
            LabelManager.IsActive = _displayLabel;

            LabelManager.FontAsset = _labelFontAsset;

            LabelManager.FontStyle = _labelFontStyle;

            LabelManager.FontSize = _labelFontSize;
            
            LabelManager.Alignment = _labelAligmentOptions;

            LabelManager.Margins = new Vector4(_labelMargins.Left, _labelMargins.Top, _labelMargins.Right, _labelMargins.Bottom);
        }

        private void ValidateDimensions()
        {
            _mainBorderWidth = Mathf.Clamp(_mainBorderWidth, 0, MaxBorderWidth);

            _glowProportion = Mathf.Clamp(_glowProportion, 0, 1 - _shadowProportion);

            _shadowProportion = Mathf.Clamp(_shadowProportion, 0, 1 - _glowProportion);
        }

        private void ValidateFillTimes()
        {
            _fillIncrementCatchUpTime = Mathf.Max(_fillIncrementCatchUpTime, _fillFastestTime);

            _fillDecrementCatchUpTime = Mathf.Max(_fillDecrementCatchUpTime, _fillFastestTime);
        }

        private void UpdateDimensions()
        {
            MainBorder.SizeDelta = _mainBorderWidth * Vector2.one;

            MainBorder.BorderWidth = _mainBorderWidth;

            Background.SizeDelta = -_mainBorderWidth * Vector2.one;

            Foreground.SizeDelta = -_mainBorderWidth * Vector2.one;

            Glow.LocalScale = new Vector3(1, _glowProportion, 1);

            Shadow.LocalScale = new Vector3(1, _shadowProportion, 1);

            LabelManager.SizeDelta = -_mainBorderWidth * Vector2.one;
        }

        public void UpdateColors()
        {
            FillManager.Main.Gradient = _barColors.MainFill;

            FillManager.Increment.Color = _barColors.IncrementFill;

            FillManager.Decrement.Color = _barColors.DecrementFill;

            Background.Color = _barColors.Background;

            MainBorder.Color = _barColors.Border;

            SegmentBorderLayout.Color = _barColors.Border;

            Glow.Color = _barColors.Glow;

            Shadow.Color = _barColors.Shadow;

            LabelManager.Main.FontColor = _barColors.ForegroundLabelColor;

            LabelManager.Increment.FontColor = _barColors.IncrementFill.Negative();

            LabelManager.Decrement.FontColor = _barColors.DecrementFill.Negative();

            LabelManager.Background.FontColor = _barColors.BackgroundLabelColor;
        }

        private void Awake()
        {
            _barCanvas = BarCanvas;

            _mainBorder = MainBorder;

            _background = Background;

            _foreground = Foreground;

            _fillManager = FillManager;

            _segmentBorderLayout = SegmentBorderLayout;

            _glow = Glow;

            _shadow = Shadow;

            _labelManager = LabelManager;
        }

        private void Start()
        {
            this.OnValidate();
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateDimensions();

            UpdateSegmentDelimeters();
        }

        private void UpdateSegmentDelimeters()
        {
            if (Quantity != null)
            {
                SegmentBorderLayout.UpdateSegmentDelimeters(Quantity, SegmentBorderWidth);
            }
        }

        public void UpdateName()
        {
            if (BarSystemManager.Instance.automaticObjectNaming)
            {
                name = (Quantity == null ? "" : Quantity.Type.ToString() + " ") + "Bar (" + BarCanvas.RenderMode.ToString() + ")";
            }
        }

        private void UpdateBar()
        {
            if (!this.IsActive)
            {
                return;
            }

            FillManager.UpdateFill(Quantity.FillAmount, _fillFastestTime, _fillIncrementCatchUpTime, _fillDecrementCatchUpTime);

            LabelManager.UpdateLabel(Quantity.Amount, Quantity.MaximumAmount);

            SegmentBorderLayout.UpdateSegmentDelimeters(Quantity, SegmentBorderWidth);
        }

        public void FlashBar()
        {
            if (!this.IsActive)
            {
                return;
            }

            FillManager.Flash(_barColors.FlashColor, _flashRepetitions, _flashFrequency);
        }
    }
}