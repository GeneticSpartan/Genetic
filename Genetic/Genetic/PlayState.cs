using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Genetic
{
    /// <summary>
    /// <
    /// </summary>
    public class PlayState : GenState
    {
        public GenGroup warthogs;

        public GenSprite warthog;
        public GenSprite warthog2;
        public GenSprite warthog3;

        public GenCamera camera2;

        public GenSound beep;

        public GenText text;

        List<GenBasic> objects = new List<GenBasic>();

        public override void Create()
        {
            base.Create();
            
            GenG.bgColor = Color.CornflowerBlue;

            camera2 = GenG.AddCamera(new GenCamera(GenG.Game.Width / 2, 0, GenG.Game.Width / 2, GenG.Game.Height, 1));
            //camera2.Rotation = 180;
            camera2.BgColor = Color.DarkGray;

            //GenG.camera.BgColor = Color.Red;

            warthogs = new GenGroup();
            Add(warthogs);

            for (int i = 0; i < 80; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    warthog = new GenSprite(i * 12, j * 70, "player", 16, 18);
                    warthog.AddAnimation("run", 16, 18, new int[] { 0, 1, 0, 2 }, 6, 1);
                    warthog.Play("run");
                    warthog.scrollFactor = 0.5f;
                    warthogs.Add(warthog);
                }
            }

            GenG.quadTree.Insert(warthogs);

            warthog2 = new GenSprite(100, 100, "player", 16, 18);
            warthog2.AddAnimation("run", 16, 18, new int[] { 0, 1, 0, 2 }, 6, 1);
            warthog2.Play("run");
            Add(warthog2);

            warthog3 = new GenSprite(0, 50, "warthog", 78, 49);
            Add(warthog3);

            beep = new GenSound("beep", 1, true);
            beep.SetFollow(warthog2);
            beep.Volume = 0.1f;
            Add(beep);

            text = new GenText("Hello, World!", 200, 100, 10, 10);
            text.FontSize = 12;
            text.textAlign = GenText.TextAlign.CENTER;
            text.hasShadow = true;
            text.shadowColor = Color.DarkOrange;
            text.scrollFactor = 1.5f;
            Add(text);

            GenG.timeScale = 1f;
        }

        public override void Update()
        {
            base.Update();

            if (GenG.Keyboards.JustPressed(Keys.Space) || GenG.GamePads.JustPressed(Buttons.Start))
                GenG.ResetState();

            if (GenG.Keyboards.IsPressed(Keys.Left) || GenG.GamePads.IsPressed(Buttons.LeftThumbstickLeft))
            {
                warthog2.velocity.X = -200;
                warthog2.Facing = Facing.LEFT;
            }
            else if (GenG.Keyboards.IsPressed(Keys.Right) || GenG.GamePads.IsPressed(Buttons.LeftThumbstickRight))
            {
                warthog2.velocity.X = 200;
                warthog2.Facing = Facing.RIGHT;
            }
            else
                warthog2.velocity.X = 0;

            if (GenG.Keyboards.IsPressed(Keys.Up) || GenG.GamePads.IsPressed(Buttons.LeftThumbstickUp))
            {
                warthog2.velocity.Y = -200;
                warthog2.Facing = Facing.UP;
            }
            else if (GenG.Keyboards.IsPressed(Keys.Down) || GenG.GamePads.IsPressed(Buttons.LeftThumbstickDown))
            {
                warthog2.velocity.Y = 200;
                warthog2.Facing = Facing.DOWN;
            }
            else
                warthog2.velocity.Y = 0;

            for (int i = 0; i < objects.Count; i++)
            {
                (objects[i] as GenSprite).color = Color.White;
            }

            objects.Clear();

            GenG.quadTree.Retrieve(objects, warthog2.PositionRect);

            for (int i = 0; i < objects.Count; i++)
            {
                (objects[i] as GenSprite).color = Color.CornflowerBlue;
            }

            GenG.camera.ScrollX = warthog2.X - 80;
            GenG.camera.ScrollY = warthog2.Y - 90;

            camera2.Flash(0.5f, 1, Color.Red);
            camera2.Shake(5, 1, true);
        }

        public void EndGame()
        {
            GenG.Game.Exit();
        }
    }
}