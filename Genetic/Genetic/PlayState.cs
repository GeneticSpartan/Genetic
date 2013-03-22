using System;
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
        public GenTilemap Map;

        public GenGroup Boxes;

        public GenSprite Warthog;
        public GenSprite Player;
        public GenSprite Warthog3;
        public GenSprite Warthog4;
        public GenSprite Warthog5;

        public GenControl PlayerControl;

        //public GenCamera camera2;

        public GenSound Beep;

        public GenText Text;

        public override void Create()
        {
            base.Create();

            Map = new GenTilemap();

            Map.LoadTile("1", new GenTile()).MakeTexture(Color.LightSkyBlue, 8, 8);
            Map.LoadTile("2", new GenTile()).MakeTexture(Color.IndianRed, 8, 8);

            Map.LoadMap(
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,1\n" +
                "1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,1\n" +
                "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1"
                , 8, 8, GenTilemap.ImageAuto);
            Add(Map);
            
            GenG.BackgroundColor = Color.CornflowerBlue;

            //camera2 = GenG.AddCamera(new GenCamera(GenG.Game.Width / 2, 0, GenG.Game.Width / 2, GenG.Game.Height, 1));
            //camera2.BgColor = Color.DarkGray;

            GenG.Camera.BgColor = Color.SlateBlue;
            
            Boxes = new GenGroup();
            Add(Boxes);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Warthog = new GenSprite(i * 32 + 150, j * 16, "warthog", 16, 16);
                    Warthog.AddAnimation("run", 16, 16, new int[] { 1 }, 0);
                    Warthog.Play("run");
                    //warthog.scrollFactor = 2f;
                    Warthog.Mass = 1f;
                    Warthog.Deceleration.X = 400;
                    //warthog.MakeTexture(GenU.randomColor() * 0.5f, 8 + j * 2, 8 + j * 2);
                    //warthog.acceleration.X = -1000;
                    Warthog.Acceleration.Y = 700;
                    Warthog.MaxVelocity.Y = 400;
                    Warthog.Color = GenU.randomColor(100, 255);
                    Boxes.Add(Warthog);
                }
            }

            Player = new GenSprite(100, 100, "player", 16, 16);
            Player.AddAnimation("run", 16, 18, new int[] { 0, 1, 0, 2 }, 6, 1);
            Player.Play("run");
            Player.Mass = 1f;
            // Adjust the origin to keep the player's feet from visually penetrating a wall when rotated.
            Player.Origin.Y += 2;
            Player.DrawOffset.Y -= 2;
            Add(Player);

            Warthog3 = new GenSprite(500, 300, "warthog", 78, 49);
            Warthog3.Deceleration.X = 400;
            Warthog3.Deceleration.Y = 400;
            Warthog3.Mass = 2f;
            Add(Warthog3);

            Warthog4 = new GenSprite(0, 100, "warthog", 78, 49);
            Warthog4.Velocity.X = 500;
            Warthog4.Mass = 0.5f;
            Add(Warthog4);

            Warthog5 = new GenSprite(500, 100, "warthog", 78, 49);
            Warthog5.Mass = 1f;
            Add(Warthog5);

            PlayerControl = new GenControl(Player, GenControl.Movement.Accelerates, GenControl.Stopping.Deccelerates);
            PlayerControl.SetMovementSpeed(800, 0, 250, 400, 700, 0);
            PlayerControl.Gravity.Y = 700;
            PlayerControl.JumpSpeed = 400;
            Add(PlayerControl);

            Beep = new GenSound("beep", 1, true);
            //beep.Play();
            Beep.SetFollow(Player);
            Beep.Volume = 0.1f;
            Add(Beep);

            Text = new GenText("Hello, World!", 100, 150, 100, 12);
            Text.FontSize = 12;
            Text.TextAlignment = GenText.TextAlign.RIGHT;
            Text.HasShadow = true;
            Text.ShadowColor = Color.Black;
            Text.Velocity.X = 100;
            Text.Velocity.Y = 50;
            Add(Text);

            GenG.TimeScale = 1f;

            GenG.Camera.CameraFollowType = GenCamera.FollowType.LockOn;
            GenG.Camera.FollowStrength = 0.05f;
            GenG.Camera.AddTarget(Player);
            //GenG.camera.AddTarget(warthog3);

            //camera2.FollowStrength = 0.05f;
            //camera2.AddTarget(warthog2);

            //GenG.worldBounds = new Rectangle(0, 0, GenG.Game.Width, GenG.Game.Height);

            //camera2.Flash(1, 2, Color.Black, FadeOut);

            Player.Flicker(40f, 1f, Color.Black * 0.5f, true);
        }

        public override void Update()
        {
            base.Update();

            //foreach (GenObject duck in warthogs.members)
            //    duck.acceleration.X = warthog2.X - duck.X;

            GenG.Collide(Player, Warthog3);
            GenG.Collide(Player, Text);
            GenG.Collide(Map, Player);
            GenG.Collide(Map, Boxes);
            GenG.Collide(Player, Boxes);
            //GenG.Collide(warthog3, warthogs);
            GenG.Collide(Boxes, Boxes);
            GenG.Collide(Warthog4, Warthog5);

            //warthog2.rotationSpeed = warthog2.velocity.X;

            //text.FontSize += 0.1f;

            if (GenG.Keyboards.JustPressed(Keys.Tab) || GenG.GamePads.JustPressed(Buttons.X, 1))
                GenG.IsDebug = !GenG.IsDebug;

            if (GenG.Keyboards.JustPressed(Keys.R) || GenG.GamePads.JustPressed(Buttons.Y, 1))
                GenG.ResetState();

            if (GenG.Keyboards.IsPressed(Keys.Z) || GenG.GamePads.IsPressed(Buttons.LeftTrigger))
                GenG.TimeScale = 0.2f;
            else if (GenG.Keyboards.IsPressed(Keys.X) || GenG.GamePads.IsPressed(Buttons.RightTrigger))
                GenG.TimeScale = 2f;
            else
                GenG.TimeScale = 1f;

            if (Player.IsTouching(GenObject.Direction.Down))
                Player.Rotation = 0;
            else if (Player.IsTouching(GenObject.Direction.Right))
                Player.Rotation = 270;
            else if (Player.IsTouching(GenObject.Direction.Left))
                Player.Rotation = 90;
            else if (Player.IsTouching(GenObject.Direction.Up))
                Player.Rotation = 180;
            else if (!Player.IsTouching(GenObject.Direction.Any))
                Player.RotationSpeed = Player.Velocity.X * 4;

            /*
            if (GenG.Keyboards.JustPressed(Keys.Space) || GenG.GamePads.JustPressed(Buttons.A, 1))
            {
                GenG.camera.AddTarget(warthog2);
                //GenG.camera.AddTarget(warthog3);
                //GenG.camera.AddTarget(text);
                //GenG.ResetState();
            }
            if (GenG.Keyboards.JustReleased(Keys.Space) || GenG.GamePads.JustReleased(Buttons.A, 1))
            {
                GenG.camera.RemoveTarget(warthog2);
                //GenG.camera.RemoveTarget(warthog3);
                //GenG.camera.RemoveTarget(text);
                //camera2.SetFollow(warthog2);
                //GenG.ResetState();
            }*/

            /*if (warthog2.X < 0)
            {
                warthog2.X = 0;
                warthog2.velocity.X = 0;
            }
            if (warthog2.X + warthog2.Width > camera2.Viewport.Width)
            {
                warthog2.X = camera2.Viewport.Width - warthog2.Width;
                warthog2.velocity.X = 0;
            }
            if (warthog2.Y < 0)
            {
                warthog2.Y = 0;
                warthog2.velocity.Y = 0;
            }
            if (warthog2.Y + warthog2.Height > camera2.Viewport.Height)
            {
                warthog2.Y = camera2.Viewport.Height - warthog2.Height;
                warthog2.velocity.Y = 0;
            }*/

            //for (int i = 0; i < objects.Count; i++)
            //{
                //(objects[i] as GenSprite).color = Color.CornflowerBlue;
            //}

            //objects.Clear();

            //GenG.quadtree.Retrieve(objects, warthog2.PositionRect);

            //for (int i = 0; i < objects.Count; i++)
            //{
                //(objects[i] as GenSprite).color = Color.Red;
                //GenG.Collide(warthog2, objects[i]);
            //}

            //GenG.camera.Flash(0.5f, 2, Color.Red);
            //GenG.camera.Shake(10, 2, true);

            //camera2.Shake(5, 1, true);
            //camera2.Rotation -= 0.1f;
        }

        public void FadeOut()
        {
            //camera2.Fade(2, Color.Black, EndGame);
        }

        public void EndGame()
        {
            GenG.ResetState();
            //GenG.Game.Exit();
        }
    }
}