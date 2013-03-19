using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic
{
    public class MenuState : GenState
    {
        public float elapsed = 0.0f;
        public GenText startGame;

        public override void Create()
        {
            base.Create();

            GenG.Camera.BgColor = Color.IndianRed;

            startGame = new GenText("Press Start", GenG.Camera.CameraView.Width / 2, GenG.Camera.CameraView.Height / 2, 0, 20);
            startGame.TextAlignment = GenText.TextAlign.CENTER;
            startGame.HasShadow = true;
            startGame.ShadowColor = Color.DarkRed;
            Add(startGame);

            GenG.Camera.Flash(1.2f, 3f, Color.White);
        }

        public override void Update()
        {
            base.Update();

            GenG.Camera.Shake();

            elapsed += GenG.PhysicsTimeStep;

            startGame.Rotation = (float)Math.Sin(4 * elapsed) * 30;
            startGame.Y = GenG.Camera.CameraView.Height / 2 - 20 + (float)Math.Sin(2 * elapsed) * 20;
            startGame.FontSize = 24 + (float)Math.Sin(8 * elapsed) * 2;

            if (GenG.Keyboards.IsPressed(Keys.Space) || GenG.GamePads.IsPressed(Buttons.Start, 1))
                GenG.Camera.Fade(1f, Color.Black, StartGame);
        }

        public void StartGame()
        {
            GenG.SwitchState(new PlayState());
        }
    }
}