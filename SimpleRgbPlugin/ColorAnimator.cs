﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SimpleRgbPlugin
{
    public sealed class ColorAnimator : IAnimator<Color>
    {
        private readonly Timer animationTimer = new(1000f / 16f);

        private Color fromColor;
        private Color lastColor;
        private Color toColor;

        private float animationDuration = 0f;
        private float animationPercentage = 0f;

        public ColorAnimator(Color initialColor)
        {
            animationTimer.Interval = 16;
            animationTimer.Elapsed += AnimationTimer_Elapsed;

            lastColor = initialColor;
        }

        private void AnimationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lastColor = AnimationCommon.AnimateColor(fromColor, toColor, animationPercentage, EasingFunctions.EaseInOutQuart);
            ValueAnimate?.Invoke(this, lastColor);

            animationPercentage += (float)animationTimer.Interval / animationDuration;

            if (animationPercentage >= 1.0f)
            {
                animationTimer.Stop();
                ValueAnimateFinished?.Invoke(this, lastColor);
            }
        }

        public void Animate(Color to, float duration)
        {
            animationTimer.Stop();

            if (duration <= 0f)
            {
                lastColor = to;
                ValueAnimate?.Invoke(this, to);
                return;
            }

            animationPercentage = 0.0f;

            fromColor = lastColor;
            toColor = to;
            animationDuration = duration;

            animationTimer.Start();
        }

        public void Dispose()
        {
            animationTimer.Stop();

            animationTimer.Elapsed -= AnimationTimer_Elapsed;
            animationTimer.Dispose();
        }

        public event IAnimator<Color>.ValueAnimateEventHandler ValueAnimate;
        public event IAnimator<Color>.ValueAnimateFinishedEventHandler ValueAnimateFinished;
    }
}