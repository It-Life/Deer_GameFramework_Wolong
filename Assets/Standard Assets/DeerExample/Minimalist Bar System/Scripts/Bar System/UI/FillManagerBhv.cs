using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimalist.Bar.UI
{
    public class FillManagerBhv : EmptyBhv
    {
        // Public properties
        public MainFillBhv Main => _mainFill == null ? GetComponentInChildren<MainFillBhv>() : _mainFill;
        public IncrementFillBhv Increment => _incrementFill == null ? GetComponentInChildren<IncrementFillBhv>() : _incrementFill;
        public DecrementFillBhv Decrement => _decrementFill == null ? GetComponentInChildren<DecrementFillBhv>() : _decrementFill;

        // Private fields
        private MainFillBhv _mainFill;
        private IncrementFillBhv _incrementFill;
        private DecrementFillBhv _decrementFill;

        private void Awake()
        {
            _mainFill = Main;

            _incrementFill = Increment;

            _decrementFill = Decrement;
        }

        public void UpdateFill(float fillAmount, float immediateFillTime, float incrementCatchUpTime, float decrementCatchUpTime)
        {
            Main.FillTime = fillAmount > Main.FillAmount ? incrementCatchUpTime : immediateFillTime;

            Main.FillAmount = fillAmount;

            Increment.FillTime = immediateFillTime;

            Increment.FillAmount = fillAmount;

            Decrement.FillTime = fillAmount < Decrement.FillAmount ? decrementCatchUpTime : immediateFillTime;

            Decrement.FillAmount = fillAmount;
        }

        public void Flash(Color flashColor, int repetitionCount, float lerpSpeed)
        {
            Main.StartFlashing(flashColor, repetitionCount, lerpSpeed);
        }
    }
}