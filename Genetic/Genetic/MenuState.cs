using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic
{
    public class MenuState : GenState
    {
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

            //GenG.Camera.Shake();

            startGame.Rotation = GenU.SineWave(0, 4, 30);
            startGame.X = GenU.SineWave(GenG.Camera.CameraView.Width / 2, 4, GenG.Camera.CameraView.Width / 2 - 200);
            startGame.Y = GenU.SineWave(GenG.Camera.CameraView.Height / 2 - 20, 2, GenG.Camera.CameraView.Height / 2 - 100);
            startGame.FontSize = GenU.SineWave(24, 8, 2);

            if (GenG.Keyboards.IsPressed(Keys.Space) || GenG.GamePads.IsPressed(Buttons.Start, 1))
                GenG.Camera.Fade(1f, Color.Black, StartGame);
        }

        public void StartGame()
        {
            GenG.SwitchState(new PlayState());
        }
    }
}