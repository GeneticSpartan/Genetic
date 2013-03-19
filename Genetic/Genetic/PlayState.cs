﻿using System;
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

        public GenGroup boxes;

        public GenSprite warthog;
        public GenSprite player;
        public GenSprite warthog3;
        public GenSprite warthog4;
        public GenSprite warthog5;

        public GenControl playerControl;

        //public GenCamera camera2;

        public GenSound beep;

        public GenText text;

        List<GenBasic> objects = new List<GenBasic>();

        public override void Create()
        {
            base.Create();

            map = new GenTilemap();

            map.LoadTile("1", new GenTile()).MakeTexture(Color.LightSkyBlue, 99, 99);
            map.LoadTile("2", new GenTile()).MakeTexture(Color.IndianRed, 99, 99);

            map.LoadMap("2,2,2,2,2,0,0,0,0,0,1\n1,0,0,0,0,0,0,0,0,0,1\n1,0,0,0,0,0,0,0,0,0,1\n1,0,0,0,0,0,0,0,0,0,1\n1,0,0,0,0,0,0,0,0,0,1\n1,1,1,1,1,1,1,1,1,1,1", 100, 100);
            Add(map);
            
            GenG.bgColor = Color.CornflowerBlue;

            //camera2 = GenG.AddCamera(new GenCamera(GenG.Game.Width / 2, 0, GenG.Game.Width / 2, GenG.Game.Height, 1));
            //camera2.BgColor = Color.DarkGray;

            GenG.camera.BgColor = Color.SlateBlue;

            boxes = new GenGroup();
            Add(boxes);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    warthog = new GenSprite(i * 25 + 150, j * 18 + 100, "warthog", 16, 16);
                    warthog.AddAnimation("run", 16, 16, new int[] { 1 }, 0);
                    warthog.Play("run");
                    //warthog.scrollFactor = 2f;
                    warthog.mass = 0.5f;
                    warthog.deceleration.X = 400;
                    //warthog.MakeTexture(GenU.randomColor() * 0.5f, 8 + j * 2, 8 + j * 2);
                    //warthog.acceleration.X = -1000;
                    warthog.acceleration.Y = 700;
                    warthog.maxVelocity.Y = 400;
                    warthog.color = Color.Red;
                    boxes.Add(warthog);
                }
            }

            player = new GenSprite(100, 100, "player", 16, 18);
            player.AddAnimation("run", 16, 18, new int[] { 0, 1, 0, 2 }, 6, 1);
            player.Play("run");
            player.mass = 1f;
            Add(player);

            warthog3 = new GenSprite(500, 300, "warthog", 78, 49);
            warthog3.deceleration.X = 400;
            warthog3.deceleration.Y = 400;
            warthog3.mass = 2f;
            Add(warthog3);

            warthog4 = new GenSprite(0, 100, "warthog", 78, 49);
            warthog4.velocity.X = 500;
            warthog4.mass = 0.5f;
            Add(warthog4);

            warthog5 = new GenSprite(500, 100, "warthog", 78, 49);
            warthog5.mass = 1f;
            Add(warthog5);

            playerControl = new GenControl(player, GenControl.Movement.Accelerates, GenControl.Stopping.Deccelerates);
            playerControl.SetMovementSpeed(700, 0, 400, 400, 400, 0);
            playerControl.gravity.Y = 700;
            playerControl.jumpSpeed = 400;
            Add(playerControl);

            beep = new GenSound("beep", 1, true);
            //beep.Play();
            beep.SetFollow(player);
            beep.Volume = 0.1f;
            Add(beep);

            text = new GenText("Hello, World!", 100, 150, 100, 12);
            text.FontSize = 12;
            text.textAlign = GenText.TextAlign.RIGHT;
            text.hasShadow = true;
            text.shadowColor = Color.Black;
            text.velocity.X = 100;
            text.velocity.Y = 50;
            Add(text);

            GenG.timeScale = 1f;

            GenG.camera.followStyle = GenCamera.FollowStyle.LockOn;
            GenG.camera.FollowStrength = 0.05f;
            GenG.camera.AddTarget(player);
            //GenG.camera.AddTarget(warthog3);

            //camera2.FollowStrength = 0.05f;
            //camera2.AddTarget(warthog2);

            //GenG.worldBounds = new Rectangle(0, 0, GenG.Game.Width, GenG.Game.Height);

            //camera2.Flash(1, 2, Color.Black, FadeOut);
        }

        public override void Update()
        {
            base.Update();

            //foreach (GenObject duck in warthogs.members)
            //    duck.acceleration.X = warthog2.X - duck.X;

            GenG.Collide(player, warthog3);
            GenG.Collide(player, text);
            GenG.Collide(map, player);
            GenG.Collide(map, boxes);
            GenG.Collide(player, boxes);
            //GenG.Collide(warthog3, warthogs);
            GenG.Collide(boxes, boxes);
            GenG.Collide(warthog4, warthog5);

            //warthog2.rotationSpeed = warthog2.velocity.X;

            //text.FontSize += 0.1f;

            if (GenG.Keyboards.JustPressed(Keys.Tab) || GenG.GamePads.JustPressed(Buttons.X, 1))
                GenG.isDebug = !GenG.isDebug;

            if (GenG.Keyboards.JustPressed(Keys.R) || GenG.GamePads.JustPressed(Buttons.Y, 1))
                GenG.ResetState();

            if (GenG.Keyboards.IsPressed(Keys.Z) || GenG.GamePads.IsPressed(Buttons.RightTrigger))
                GenG.timeScale = 0.2f;
            else
                GenG.timeScale = 1f;

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