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
        public GenTilemap map;

        public GenGroup warthogs;

        public GenSprite warthog;
        public GenSprite warthog2;
        public GenSprite warthog3;

        public GenControl playerControl;

        public GenCamera camera2;

        public GenSound beep;

        public GenText text;

        List<GenBasic> objects = new List<GenBasic>();

        public override void Create()
        {
            base.Create();
            
            map = new GenTilemap();

            map.LoadTile("1", new GenTile()).MakeTexture(Color.LightSkyBlue, 100, 100);
            map.LoadTile("2", new GenTile()).MakeTexture(Color.IndianRed, 100, 100);

            map.LoadMap("2,2,2,2,2\n1,0,1,0,0\n1,0,0,0,1\n1,0,1,0,1\n1,1,1,1,1\n1,1,1,1,1\n1,0,1,0,1\n1,0,0,0,1\n1,0,1,0,1\n1,1,1,1,1\n1,1,1,1,1\n1,0,1,0,1\n1,0,0,0,1\n1,0,1,0,1\n1,1,1,1,1\n1,1,1,1,1\n1,0,1,0,1\n1,0,0,0,1\n1,0,1,0,1\n1,1,1,1,1", 100, 100);
            Add(map);
            
            GenG.bgColor = Color.CornflowerBlue;

            //camera2 = GenG.AddCamera(new GenCamera(GenG.Game.Width / 2, 0, GenG.Game.Width / 2, GenG.Game.Height, 1));
            //camera2.Rotation = 180;
            //camera2.BgColor = Color.DarkGray;

            GenG.camera.BgColor = Color.SlateBlue;

            warthogs = new GenGroup();
            //Add(warthogs);

            for (int i = 0; i < 80; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    warthog = new GenSprite(i * 12, j * 70, "player", 16, 18);
                    //warthog.AddAnimation("run", 16, 18, new int[] { 0, 1, 0, 2 }, 6, 1);
                    //warthog.Play("run");
                    //warthog.scrollFactor = 0.1f;
                    warthogs.Add(warthog);
                }
            }

            GenG.quadTree.Insert(warthogs);

            warthog2 = new GenSprite(100, 100, "player", 16, 18);
            warthog2.AddAnimation("run", 16, 18, new int[] { 0, 1, 0, 2 }, 6, 1);
            warthog2.Play("run");
            Add(warthog2);

            warthog3 = new GenSprite(500, 300, "warthog", 78, 49);
            //warthog3.velocity.X = 20;
            Add(warthog3);

            playerControl = new GenControl(warthog2, GenControl.Movement.Accelerates, GenControl.Stopping.Deccelerates);
            playerControl.SetMovementSpeed(700, 0, 400, 400, 400, 400);
            playerControl.gravity.Y = 400;
            Add(playerControl);

            beep = new GenSound("beep", 1, true);
            //beep.Play();
            beep.SetFollow(warthog2);
            beep.Volume = 0.1f;
            Add(beep);

            text = new GenText("Hello, World!", 200, 100, 100, 12);
            text.FontSize = 12;
            text.textAlign = GenText.TextAlign.RIGHT;
            text.hasShadow = true;
            text.shadowColor = Color.Black;
            text.velocity.Y = 10;
            //text.scrollFactor = 2f;
            //text.rotationSpeed = 45;
            Add(text);

            GenG.timeScale = 1f;

            GenG.camera.followStyle = GenCamera.FollowStyle.LockOn;
            GenG.camera.FollowStrength = 0.05f;
            GenG.camera.AddTarget(warthog2);
            GenG.camera.AddTarget(warthog3);

            //camera2.FollowStrength = 0.05f;
            //camera2.AddTarget(warthog2);

            //GenG.worldBounds = new Rectangle(0, 0, GenG.Game.Width, GenG.Game.Height);

            //camera2.Flash(1, 2, Color.Black, FadeOut);
        }

        public override void Update()
        {
            base.Update();

            GenG.Collide(warthog2, warthog3);
            GenG.Collide(warthog2, text);
            map.Collide(warthog2);

            //text.FontSize += 0.1f;

            if (GenG.GamePads.JustPressed(Buttons.X, 1))
                GenG.isDebug = !GenG.isDebug;

            if (GenG.GamePads.JustPressed(Buttons.Y, 1))
                GenG.ResetState();

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

            for (int i = 0; i < objects.Count; i++)
            {
                (objects[i] as GenSprite).color = Color.White;
            }

            objects.Clear();

            GenG.quadTree.Retrieve(objects, warthog2.PositionRect);

            for (int i = 0; i < objects.Count; i++)
            {
                (objects[i] as GenSprite).color = Color.Red;
            }

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