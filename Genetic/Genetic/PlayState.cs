using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Genetic.Gui;
using Genetic.Particles;
using Genetic.Path;
using Genetic.Physics;

namespace Genetic
{
    /// <summary>
    /// <
    /// </summary>
    public class PlayState : GenState
    {
        //public GenTilemap Map;
        public GenCave Cave;

        public GenGroup Boxes;

        public GenSprite Box;
        public GenSprite Player;
        public GenSprite Warthog3;
        public GenSprite Warthog4;
        public GenSprite Warthog5;

        public GenControl PlayerControl;

        //public GenCamera camera2;

        public GenSound Beep;
        public GenText Text;
        //public GenSound Music;

        //public GenTimer Timer;

        public GenVerlet Chain;

        public GenPath Path;

        public GenEmitter Emitter;

        public GenProgressBar ProgressBar;

        public override void Create()
        {
            base.Create();

            /*Map = new GenTilemap();

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
                "1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1"
                , 8, 8, GenTilemap.ImageAuto);
            Add(Map);*/

            Cave = new GenCave();
            Cave.MakeCave(160, 320);
            Add(Cave);

            GenG.BackgroundColor = Color.CornflowerBlue;

            //camera2 = GenG.AddCamera(new GenCamera(0, GenG.Game.Height / 2, GenG.Game.Width, GenG.Game.Height / 2, 2f));
            //camera2.BgColor = new Color(50, 50, 70);

            GenG.Camera.BgColor = new Color(70, 50, 50);

            Path = new GenPath();
            Path.AddNode(new GenPathNode(100, 100, 0));
            Path.AddNode(new GenPathNode(200, 100));
            Path.AddNode(new GenPathNode(200, 200));
            Path.AddNode(new GenPathNode(100, 150));
            Add(Path);

            Chain = new GenVerlet();
            Chain.MakeGrid(300, 200, 5, 15, 1);
            Chain.DrawLines = false;
            Chain.SetMass(0.2f);
            Chain.SetGravity(0f, 700f);
            
            for (int i = 0; i < Chain.Members.Count; i++)
            {
                ((GenObject)Chain.Members[i]).Width = ((GenObject)Chain.Members[i]).Height = Chain.Members.Count - i;
                ((GenSprite)Chain.Members[i]).MakeTexture(Color.White * 0.3f, (int)((GenObject)Chain.Members[i]).Width, (int)((GenObject)Chain.Members[i]).Height);
            }

            for (int i = 0; i < Chain.Links.Count; i++)
            {
                Chain.Links[i].OffsetA.X = Chain.Links[i].OffsetA.Y = Chain.Links[i].PointA.Width / 2;
                Chain.Links[i].OffsetB.X = Chain.Links[i].OffsetB.Y = Chain.Links[i].PointB.Width / 2;
            }

            ((GenObject)Chain.Members[0]).Immovable = true;
            ((GenObject)Chain.Members[0]).Acceleration.Y = 0f;
            ((GenObject)Chain.Members[0]).SetPath(Path, 100, GenPath.Type.Yoyo, GenMove.Axis.Both, GenPath.Movement.Instant);
            Add(Chain);
            
            Boxes = new GenGroup();
            Add(Boxes);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Box = new GenSprite(i * 32 + 150, j * 16, "warthog", 12, 13);
                    Box.AddAnimation("run", 16, 16, new int[] { 1 }, 0, false);
                    Box.Play("run");
                    Box.DrawOffset.X = -4;
                    Box.DrawOffset.Y = -3;
                    //warthog.scrollFactor = 2f;
                    Box.Mass = 0.2f;
                    //Box.Deceleration.X = 400;
                    //Box.Deceleration.Y = 400;
                    //warthog.MakeTexture(GenU.randomColor() * 0.5f, 8 + j * 2, 8 + j * 2);
                    //warthog.acceleration.X = -1000;
                    //Box.Acceleration.Y = 700;
                    Box.MaxVelocity.X = 200;
                    Box.MaxVelocity.Y = 200;
                    Box.Color = GenU.RandomColor(100, 255);
                    Boxes.Add(Box);
                }
            }

            Warthog3 = new GenSprite(500, 300, "warthog", 78, 49);
            Warthog3.Deceleration.X = 400;
            Warthog3.Deceleration.Y = 400;
            Warthog3.MaxVelocity.X = 250;
            Warthog3.MaxVelocity.Y = 400;
            Warthog3.Mass = 2f;
            Warthog3.IsPlatform = true;
            Add(Warthog3);

            Emitter = new GenEmitter(100, 100);
            Emitter.Width = 16;
            Emitter.Height = 16;
            Emitter.MakeParticles(GenU.MakeTexture(Color.White, 2, 2), 2, 2, 400);
            Emitter.EmitQuantity = 400;
            Emitter.EmitFrequency = 3f;
            Emitter.InheritVelocity = true;
            Emitter.SetXSpeed(-300, 300);
            Emitter.SetYSpeed(-300, 300);
            Emitter.SetRotationSpeed(-360, 360);
            Emitter.SetLifetime(2.9f);
            Emitter.SetColor(Color.Cyan, Color.MediumVioletRed);
            Emitter.SetAlpha(2f, 0f);
            Emitter.SetScale(1f, 0f);
            //Emitter.SetGravity(0, 700);
            Add(Emitter);

            Player = new GenSprite(100, 0, "player", 16, 16);
            Player.AddAnimation("idle", 16, 18, new int[] { 0 }, 6, false, 1);
            Player.AddAnimation("run", 16, 18, new int[] { 1, 0, 2, 0 }, 6, true, 1);
            Player.AddAnimation("jump", 16, 18, new int[] { 1 }, 6, false, 1);
            Player.AddAnimation("fall", 16, 18, new int[] { 3 }, 6, false, 1);
            Player.Mass = 1f;
            // Adjust the origin to keep the player's feet from visually penetrating a wall when rotated.
            Player.CenterOrigin(false);
            Player.Origin.Y += 10;
            Player.DrawOffset.Y -= 2;
            Add(Player);

            Emitter.Parent = Player;
            Emitter.Start(false);

            PlayerControl = new GenControl(Player, GenControl.Movement.Accelerates, GenControl.Stopping.Deccelerates);
            PlayerControl.SetMovementSpeed(800, 0, 250, 400, 700, 0);
            PlayerControl.Gravity.Y = 700;
            PlayerControl.JumpSpeed = 400;
            PlayerControl.IdleAnimation = "idle";
            PlayerControl.MoveAnimation = "run";
            PlayerControl.JumpAnimation = "jump";
            PlayerControl.FallAnimation = "fall";
            Add(PlayerControl);

            Warthog4 = new GenSprite(0, 100, "warthog", 78, 49);
            Warthog4.Velocity.X = 500;
            Warthog4.Mass = 0.5f;
            Add(Warthog4);

            Warthog5 = new GenSprite(500, 100, "warthog", 78, 49);
            Warthog5.Acceleration.Y = 700;
            Add(Warthog5);

            //Chain.MakeLink(Warthog3, (GenObject)Chain.Members[14]);
            //Chain.SetRestingDistance(10f);

            Beep = new GenSound("beep", 1, true);
            //Beep.Play();
            Beep.SetFollow(Player);
            Beep.Volume = 1f;
            Add(Beep);

            Text = new GenText("Hello, World!", 100, 150, 100, 12);
            Text.FontSize = 12;
            Text.TextAlignment = GenText.TextAlign.RIGHT;
            Text.HasShadow = true;
            Text.ShadowColor = Color.Black;
            Text.Velocity.X = 100;
            Text.Velocity.Y = 50;
            Add(Text);

            ProgressBar = new GenProgressBar(100, 100);
            ProgressBar.LoadTexture(GenG.Pixel);
            ProgressBar.SetSourceRect(0, 0, 100, 10);
            ProgressBar.MinColor = Color.Red;
            ProgressBar.MaxColor = Color.CornflowerBlue;
            //ProgressBar.MinCallback = Shake;
            ProgressBar.Rotation = -90;
            ProgressBar.Parent = Player;
            Add(ProgressBar);

            //Music = new GenSound("music", 1, true);
            //Music.Play();

            GenG.TimeScale = 1f;

            GenG.Camera.Flash(1f, 1f, Color.Black);
            GenG.Camera.CameraFollowType = GenCamera.FollowType.LockOn;
            //GenG.Camera.FollowStrength = 0.05f;
            GenG.Camera.MaxZoom = 10f;
            GenG.Camera.AddTarget(Player);
            //GenG.Camera.AddTarget(Warthog3);
            //GenG.Camera.SetCameraView(0, 0, GenG.Game.Width, GenG.Game.Height / 2);

            GenG.WorldBounds = new Rectangle(-GenG.TitleSafeArea.Left, -GenG.TitleSafeArea.Top, GenG.Game.Width * 4, GenG.Game.Height * 4);
            GenG.Quadtree = new GenQuadtree(GenG.WorldBounds.X, GenG.WorldBounds.Y, GenG.WorldBounds.Width, GenG.WorldBounds.Height);

            //camera2.Flash(1f, 1f, Color.Black);
            //camera2.FollowStrength = 0.05f;
            //camera2.AddTarget(Player);
            //camera2.AddTarget(Warthog3);

            Player.Flicker(40f, 1f, Color.Black * 0.5f, true);

            //Timer = new GenTimer(1f, KillBox);
            //Add(Timer);
            //Timer.Start();

            //Warthog5.Parent = Player;

            //Boxes.Kill();

            //Warthog3.SetPath(Path);
            //Warthog3.PathSpeed = 100;
        }

        public override void Update()
        {
            base.Update();

            // Do collision checking first.
            GenG.Collide(Player, Text);
            GenG.Collide(Cave, Player, HitCave);
            GenG.Collide(Cave, Boxes);
            GenG.Collide(Player, Boxes, HitBox);
            //GenG.Collide(Boxes, Boxes);
            //GenG.Collide(Warthog4, Warthog5);
            //GenG.Collide(Cave, Chain);
            GenG.Collide(Cave, Warthog3);
            GenG.Collide(Player, Warthog3);
            GenG.Collide(Cave, Emitter);

            if (GenG.GamePads[PlayerIndex.One].JustPressed(Buttons.X))
                GenG.IsDebug = !GenG.IsDebug;

            if (GenG.Keyboards[PlayerIndex.One].JustPressed(Keys.R) || GenG.GamePads[PlayerIndex.One].JustPressed(Buttons.Y))
                GenG.ResetState();

            if (GenG.Keyboards[PlayerIndex.One].IsPressed(Keys.Z) || GenG.GamePads[PlayerIndex.One].IsPressed(Buttons.LeftTrigger))
                GenG.TimeScale = 0.2f;
            else if (GenG.Keyboards[PlayerIndex.One].IsPressed(Keys.X) || GenG.GamePads[PlayerIndex.One].IsPressed(Buttons.RightTrigger))
                GenG.TimeScale = 2f;
            else
                GenG.TimeScale = 1f;

            if (GenG.GamePads[PlayerIndex.One].JustPressed(Buttons.LeftTrigger))
                GenG.Camera.Flash(0.5f, 0.1f, Color.CornflowerBlue);

            if (GenG.Keyboards[PlayerIndex.One].JustPressed(Keys.A) || GenG.GamePads[PlayerIndex.One].JustPressed(Buttons.Start))
                GenG.Paused = !GenG.Paused;

            if (GenG.Keyboards[PlayerIndex.One].JustPressed(Keys.Escape) || GenG.GamePads[PlayerIndex.One].JustPressed(Buttons.Back))
                GenG.Game.Exit();

            //((GenObject)Chain.Members[0]).X = Player.X;
            //((GenObject)Chain.Members[0]).Y = Player.Y;

            //GenMove.AccelerateToPoint(Chain.Members[0], Player.Position, 200);
            GenMove.AccelerateToPoint(Emitter, Player.CenterPosition, 500, 100);

            Warthog3.Rotation = GenMove.VectortoAngle(Warthog3.CenterPosition, Player.CenterPosition);
            GenMove.AccelerateToAngle(Warthog3, Warthog3.Rotation, 500);

            //Chain.LineColor = GenU.RandomColor();

            //Player.Rotation++;
            Player.Scale.X = GenU.CosineWave(1, 8, 0.1f);
            Player.Scale.Y = GenU.SineWave(1, 8, 0.1f);

            ProgressBar.Scale.Y = GenU.SineWave(1, 8, 0.2f);

            ProgressBar.Value = GenU.SineWave(50, 2, 51);

            if (Player.IsTouching(GenObject.Direction.Down))
                Player.Rotation = 90;
            /*ielse if (Player.IsTouching(GenObject.Direction.Right))
                Player.Rotation = 270;
            else if (Player.IsTouching(GenObject.Direction.Left))
                Player.Rotation = 90;
            else if (Player.IsTouching(GenObject.Direction.Up))
                Player.Rotation = 180;
            else if (!Player.IsTouching(GenObject.Direction.Any))
                Player.RotationSpeed = Player.Velocity.X * 4;*/
        }

        public void FadeOut(GenCollideEvent e)
        {
            GenG.Camera.Fade(2, Color.Black, EndGame);
            //camera2.Fade(2, Color.White);

            ((GenSprite)e.Object2).Flicker(40f, 2f, Color.OrangeRed, true);
        }

        public void EndGame()
        {
            GenG.ResetState();
            //GenG.Game.Exit();
        }

        /*public void KillBox()
        {
            GenBasic box = Boxes.GetRandom();

            if (box != null)
            {
                box.Exists = !box.Exists;
                //GenG.Camera.Shake(5f, 0.5f, true);
                //GenG.Camera.Flash(0.2f, 0.5f, Color.Magenta);
            }

            Timer.Start();
        }*/

        /*public void Shake()
        {
            GenG.Camera.Shake(5, 1, true);
        }*/

        public void HitBox(GenCollideEvent e)
        {
            //e.Object1.Velocity.Y = -200;
            ((GenSprite)e.Object2).Color = GenU.RandomColor();
        }

        public void HitCave(GenCollideEvent e)
        {
            ((GenSprite)e.Object2).Color = Color.Red;
            //e.Object2.Exists = false;
        }

        public void HitNode()
        {
            ((GenObject)Chain.Members[0]).SetPath(null, 0);
            ((GenObject)Chain.Members[0]).StopMoving();
            GenG.Camera.Flash(0.5f, 1f, Color.Red);
            GenG.Camera.Shake();
        }
    }
}