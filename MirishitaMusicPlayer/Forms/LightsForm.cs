﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MirishitaMusicPlayer.Animation;
using MirishitaMusicPlayer.Common;
using OpenRGB.NET;
using OpenRGB.NET.Models;
using Color = System.Drawing.Color;

namespace MirishitaMusicPlayer.Forms
{
    public partial class LightsForm : Form
    {
        private readonly Dictionary<int, LightTarget> lightTargets = new();
        private readonly int targetsPerRow;

        private readonly ScenarioPlayer _scenarioPlayer;

        private bool closing = false;

        public LightsForm(Song song, ScenarioPlayer scenarioPlayer)
        {
            InitializeComponent();

            List<int> targets = new();
            foreach (var item in song.Scenario.LightScenarios)
            {
                if (!targets.Contains(item.Target))
                    targets.Add(item.Target);
            }

            targets.Sort();

            targetsPanel.Controls.Clear();

            foreach (var item in targets)
            {
                LightTarget lightTarget = new(item);
                lightTargets[item] = lightTarget;

                targetsPanel.Controls.Add(lightTarget);
            }

            scenarioPlayer.LightsChanged += ScenarioPlayer_LightsChanged;
            _scenarioPlayer = scenarioPlayer;

            targetsPerRow = (int)Math.Ceiling(lightTargets.Count / 2f);
            Width = (targetsPerRow * 64) + 40 + 1;
            Height = 40 + targetsPanel.Top + 512 + 10 + 1;
        }

        private void ScenarioPlayer_LightsChanged(LightPayload lightPayload)
        {
            if (!closing || !IsDisposed)
            {
                Invoke(() =>
                {
                    lightTargets[lightPayload.Target].UpdateColors(
                        lightPayload.Color,
                        lightPayload.Color2,
                        lightPayload.Color3,
                        lightPayload.Duration);
                });
            }
        }

        private void LightsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scenarioPlayer.LightsChanged -= ScenarioPlayer_LightsChanged;

            closing = true;
        }

        private void HideLabelsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            foreach (var item in lightTargets)
            {
                item.Value.HideLabel = !checkBox.Checked;
            }

            int rows = (int)Math.Ceiling((float)lightTargets.Count / targetsPerRow);
            int hideDifference = (rows * 64);

            Height = checkBox.Checked ? Height -= hideDifference : Height += hideDifference;
        }
    }
}