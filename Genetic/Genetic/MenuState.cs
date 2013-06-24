using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Genetic.Input;

namespace Genetic
{
    public class MenuState : GenState
    {
        public GenText StartGameText;

        public override void Create()
        {
            base.Create();

            Camera.BgColor = Color.IndianRed;

            StartGameText = new GenText("Press Start", Camera.CameraView.Width / 2, Camera.CameraView.Height / 2, 0, 20);
            StartGameText.TextAlignment = GenText.TextAlign.Center;
            StartGameText.HasShadow = true;
            StartGameText.ShadowColor = Color.DarkRed;
            StartGameText.Deceleration.Y = 700;
            Add(StartGameText);

            //Camera.Flash(1.2f, 3f, Color.White);
        }

        public override void Update()
        {
            base.Update();

            //Camera.Shake();

            StartGameText.Rotation = GenU.SineWave(0, 4, 30);
            //startGame.X = GenU.SineWave(Camera.CameraView.Width / 2, 4, Camera.CameraView.Width / 2 - 200);
            //startGame.Y = GenU.SineWave(Camera.CameraView.Height / 2 - 20, 2, Camera.CameraView.Height / 2 - 100);

            StartGameText.X += GenG.GamePads[PlayerIndex.One].ThumbStickLeftX;

            if (GenG.Mouse.Wheel != 0)
                StartGameText.Acceleration.Y += GenG.Mouse.Wheel * 20;
            else
                StartGameText.Acceleration.Y = 0;

            StartGameText.Scale.X = GenU.SineWave(2, 8, 0.2f);
            StartGameText.Scale.Y = StartGameText.Scale.X;

            if (GenG.Keyboards[PlayerIndex.One].IsPressed(Keys.Space) || GenG.GamePads[PlayerIndex.One].IsPressed(Buttons.Start) || GenG.Mouse.JustPressed(GenMouse.Buttons.LeftButton))
                Camera.Fade(1f, Color.Black, StartGame);
        }

        public void StartGame()
        {
            GenG.SwitchState(new PlayState(), new LoadingState());
        }
    }
}