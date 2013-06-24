using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Genetic
{
    class LoadingState : GenState
    {
        public GenText LoadingText;

        public override void Create()
        {
            base.Create();

            BgColor = Color.Black;

            LoadingText = new GenText("LOADING", Camera.CameraView.Width * 0.5f, Camera.CameraView.Height * 0.5f, 1, 24);
            LoadingText.Color = Color.PaleVioletRed;
            LoadingText.TextAlignment = GenText.TextAlign.Center;
            LoadingText.Scale.X = 2f;
            LoadingText.Scale.Y = 2f;
            LoadingText.HasShadow = true;
            LoadingText.ShadowColor = Color.Lime;
            Add(LoadingText);

            Camera.Flash(1f, 1f, Color.Black);
        }

        public override void Update()
        {
            base.Update();

            LoadingText.Rotation = GenU.SineWave(0, 5, 20);

            if (GenG.StateLoaded)
                Camera.Fade(1f, Color.Black, StartState);
        }

        public void StartState()
        {
            GenG.StartLoadedState();
        }
    }
}
