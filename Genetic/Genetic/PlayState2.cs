using Microsoft.Xna.Framework;

namespace Genetic
{
    public class PlayState2 : GenState
    {
        public GenSprite warthog;

        public GenCamera camera2;

        public override void Create()
        {
            base.Create();
            
            GenG.bgColor = Color.LightGray;

            camera2 = GenG.AddCamera(new GenCamera(0, 0, 500, 200, 1));
            camera2.BgColor = Color.Red;

            //GenG.camera.BgColor = Color.Red;

            warthog = new GenSprite(0, 100, "warthog", 78, 49);
            //warthog.acceleration.X = 200;
            warthog.maxVelocity.X = 400;
            warthog.color = Color.Red;
            //warthog.visible = false;
            Add(warthog);

            GenG.camera.Flash(1f, 2f, Color.Black);
            //GenGame.camera.Shake(30, 3, true, ChangeColor, GenCamera.ShakeDirection.Both);
        }

        public override void Update()
        {
            base.Update();
            //camera2.Rotation++;
            //camera2._scroll.Y-= 0.4f;
            camera2.Zoom += 0.01f;

            GenG.camera.Shake(30, 3, true, ChangeColor, GenCamera.ShakeDirection.Both);

            if (!GenG.camera.Shaking)
                warthog.color = Color.White;

            if ((warthog.X + warthog.Width) >= 640)
                warthog.X = 640 - warthog.Width;
        }

        public void ChangeColor()
        {
            warthog.color = Color.White;
            GenG.camera.Fade(2f, null, EndGame);
            camera2.Shake(5f, 1f, true);
            camera2.Flash();
        }

        public void EndGame()
        {
            //GenG.Game.Exit();
            GenG.SwitchState(new PlayState());
        }
    }
}