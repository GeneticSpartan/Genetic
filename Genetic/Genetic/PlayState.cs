using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Genetic.Gui;
using Genetic.Input;
using Genetic.Particles;
using Genetic.Path;
using Genetic.Physics;
using Genetic.Sound;

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

        //public GenTimer Timer;

        public GenVerlet Chain;
        public GenVerlet Cloth;

        public GenPath Path;

        public GenEmitter Emitter;

        public GenProgressBar ProgressBar;

        public override void Create()
        {
            base.Create();

            GenG.ShowFps = true;

            // Set the world bounds before creating any object groups.
            // This allows the quadtrees in each group to be sized correctly.
            GenG.WorldBounds = new Rectangle(-GenG.TitleSafeArea.Left, -GenG.TitleSafeArea.Top, GenG.Game.Width, GenG.Game.Height * 4);

            /*Map = new GenTilemap();

            Map.LoadTile("1", new GenTile()).MakeTexture(Color.LightSkyBlue, 16, 16);
            Map.LoadTile("2", new GenTile()).MakeTexture(Color.IndianRed, 16, 16);

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
                , 16, 16, null);
            Add(Map);*/

            Cave = new GenCave();
            Cave.MakeCave(160, 360);
            Add(Cave);

            GenG.BackgroundColor = Color.CornflowerBlue;

            //camera2 = GenG.AddCamera(new GenCamera(0, GenG.Game.Height / 2, GenG.Game.Width, GenG.Game.Height / 2, 2f));
            //camera2.BgColor = new Color(50, 50, 70);

            GenG.Camera.BgColor = new Color(40, 50, 70);

            Path = new GenPath();
            Path.AddNode(new GenPathNode(100, 100));
            Path.AddNode(new GenPathNode(200, 100));
            Path.AddNode(new GenPathNode(200, 200));
            Path.AddNode(new GenPathNode(100, 150));
            Add(Path);

            Chain = new GenVerlet();
            Chain.MakeGrid(300, 200, 5, 15, 1);
            Chain.DrawLines = false;
            Chain.SetMass(1f);
            Chain.SetGravity(0f, 700f);
            Chain.Iterations = 10;
            
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
            ((GenObject)Chain.Members[0]).SetPath(Path, 100, GenPath.Type.Random, GenMove.Axis.Both, GenPath.Movement.Instant);
            Add(Chain);
            
            Boxes = new GenGroup(true);
            Add(Boxes);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Box = new GenSprite(i * 32 + 150, j * 16, GenG.Content.Load<Texture2D>("warthog"), 12, 13);
                    Box.AddAnimation("run", 16, 16, new int[] { 1 }, 0, false);
                    Box.Play("run");
                    Box.DrawOffset.X = -4;
                    Box.DrawOffset.Y = -3;
                    //warthog.scrollFactor = 2f;
                    Box.Mass = 0.5f;
                    Box.Deceleration.X = 400f;
                    //Box.Deceleration.Y = 400f;
                    //warthog.MakeTexture(GenU.randomColor() * 0.5f, 8 + j * 2, 8 + j * 2);
                    //warthog.acceleration.X = -1000f;
                    Box.Acceleration.Y = 700f;
                    //Box.MaxVelocity.X = 200f;
                    Box.MaxVelocity.Y = 400f;
                    Box.Color = GenU.RandomColor(100, 255);
                    Boxes.Add(Box);
                }
            }

            Emitter = new GenEmitter(100, 100);
            Emitter.Width = 16;
            Emitter.Height = 16;
            Emitter.MakeParticles(GenG.Pixel, 1, 1, 400);
            Emitter.EmitQuantity = 5;
            Emitter.EmitFrequency = .05f;
            Emitter.InheritVelocity = true;
            //Emitter.SetXSpeed(-300, 300);
            //Emitter.SetYSpeed(-300, 300);
            Emitter.SetRotationSpeed(-360, 360);
            Emitter.SetLifetime(2.9f);
            Emitter.SetColor(Color.Cyan, Color.MediumVioletRed);
            Emitter.SetAlpha(2f, 0f);
            //Emitter.SetScale(1f, 0f);
            //Emitter.SetGravity(0, 700);
            Emitter.SetDeceleration(100, 100);
            Add(Emitter);

            Player = new GenSprite(100, 0, GenG.Content.Load<Texture2D>("player"), 16, 16);
            Player.AddAnimation("idle", 16, 18, new int[] { 0 }, 6, false, 1);
            Player.AddAnimation("run", 16, 18, new int[] { 1, 0, 2, 0 }, 6, true, 1);
            Player.AddAnimation("jump", 16, 18, new int[] { 1 }, 6, false, 1);
            Player.AddAnimation("fall", 16, 18, new int[] { 3 }, 6, false, 1);
            // Adjust the origin to keep the player's feet from visually penetrating a wall when rotated.
            Player.CenterOrigin(false);
            Player.SetOrigin(Player.Origin.X, Player.Origin.Y + 8);
            Player.DrawOffset.Y -= 2;
            Player.RotationSpeed = 45;
            Player.DrawRotated = false;
            Add(Player);

            Emitter.Parent = Player;
            Emitter.Start(false);

            Warthog3 = new GenSprite(200, 300, GenG.Content.Load<Texture2D>("warthog"), 78, 49);
            Warthog3.Deceleration.X = 400;
            Warthog3.Deceleration.Y = 400;
            Warthog3.MaxVelocity.X = 250;
            Warthog3.MaxVelocity.Y = 400;
            Warthog3.Mass = 2f;
            //Warthog3.IsPlatform = true;
            Warthog3.RotationSpeed = -90;
            Warthog3.SetParent(Player, GenObject.ParentType.Origin);
            Warthog3.ParentOffset = new Vector2(50, 50);
            Add(Warthog3);

            PlayerControl = new GenControl(Player, GenControl.ControlType.Platformer, GenControl.Movement.Accelerates, GenControl.Stopping.Decelerates);
            PlayerControl.SetMovementSpeed(700, 0, 150, 400, 1000, 0);
            PlayerControl.Gravity.Y = 700;
            PlayerControl.JumpSpeed = 300;
            PlayerControl.IdleAnimation = "idle";
            PlayerControl.MoveAnimation = "run";
            PlayerControl.JumpAnimation = "jump";
            PlayerControl.FallAnimation = "fall";
            PlayerControl.UseSpeedAnimation = true;
            PlayerControl.MinAnimationFps = 1f;
            PlayerControl.JumpCount = 2;
            //PlayerControl.JumpInheritVelocity = true;
            PlayerControl.ButtonsSpecial = GenGamePad.ButtonsSpecial.ThumbStickLeft;
            //PlayerControl.LandCallback = PlayerLand;
            PlayerControl.JumpCallback = PlayerJump;
            Add(PlayerControl);

            Warthog4 = new GenSprite(300, 350, GenG.Content.Load<Texture2D>("warthog"), 78, 49);
            //Warthog4.Immovable = true;
            //Warthog4.IsPlatform = true;
            //Warthog4.Mass = 10f;
            Warthog4.Color = Color.Red;
            Warthog4.SetParent(Warthog3, GenObject.ParentType.Origin);
            Warthog4.ParentOffset.X = 75;
            Warthog4.RotationSpeed = 180;
            Add(Warthog4);

            Warthog5 = new GenSprite(500, 100, GenG.Content.Load<Texture2D>("warthog"), 78, 49);
            //Warthog5.Acceleration.Y = 700;
            Warthog5.Parent = Warthog4;
            Warthog5.ParentOffset.X = 150;
            Add(Warthog5);

            //Chain.MakeLink(Warthog3, (GenObject)Chain.Members[14]);
            //Chain.SetRestingDistance(10f);

            Beep = new GenSound("beep");
            //Beep.Play();
            //Beep.SetFollow(Player);
            //Beep.Volume = 1f;
            Add(Beep);

            Text = new GenText("Hello, World!\n------------", 200, 200, 100, 12);
            Text.FontSize = 6;
            Text.TextAlignment = GenText.TextAlign.Center;
            Text.HasShadow = true;
            Text.ShadowColor = Color.Black;
            //Text.Velocity.X = 100;
            //Text.Velocity.Y = 50;
            Text.ScrollFactor = 0f;
            Add(Text);

            ProgressBar = new GenProgressBar(100, 100);
            ProgressBar.LoadTexture(GenG.Pixel);
            ProgressBar.SetSourceRect(0, 0, 100, 10);
            ProgressBar.MinColor = Color.Red;
            ProgressBar.MaxColor = Color.CornflowerBlue;
            //ProgressBar.MinCallback = Shake;
            ProgressBar.Rotation = -90;
            ProgressBar.SetParent(Player, GenObject.ParentType.Position);
            ProgressBar.ParentOffset.X = 30;
            Add(ProgressBar);

            Cloth = new GenVerlet();
            Cloth.MakeGrid(100, 200, 8, 10, 11);
            Cloth.SetGravity(0f, 700f);
            //Cloth.SetDeceleration(100f, 0f);
            Cloth.LineColor = Color.Gray;
            Cloth.Iterations = 2;

            /*for (int i = 0; i < 10; i++)
            {
                ((GenObject)Cloth.Members[i]).Immovable = true;
                ((GenObject)Cloth.Members[i]).Acceleration.Y = 0f;
            }*/

            ((GenObject)Cloth.Members[0]).Immovable = true;
            ((GenObject)Cloth.Members[0]).Acceleration.Y = 0f;
            ((GenObject)Cloth.Members[5]).Immovable = true;
            ((GenObject)Cloth.Members[5]).Acceleration.Y = 0f;
            ((GenObject)Cloth.Members[10]).Immovable = true;
            ((GenObject)Cloth.Members[10]).Acceleration.Y = 0f;

            Add(Cloth);

            GenG.TimeScale = 1f;

            GenG.Camera.Flash(1f, 1f, Color.Black);
            GenG.Camera.CameraFollowType = GenCamera.FollowType.LockOn;
            //GenG.Camera.FollowStrength = 0.05f;
            GenG.Camera.MaxZoom = 10f;
            GenG.Camera.AddTarget(Player);
            //GenG.Camera.AddTarget(Warthog3);
            //GenG.Camera.SetCameraView(0, 0, GenG.Game.Width, GenG.Game.Height / 2);

            //camera2.Flash(1f, 1f, Color.Black);
            //camera2.FollowStrength = 0.05f;
            //camera2.AddTarget(Player);
            //camera2.AddTarget(Warthog3);

            Player.Flicker(40f, 1f, Color.Red, true);

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
            //GenG.Collide(Player, Text);
            GenG.Collide(Cave, Player);
            GenG.Collide(Cave, Boxes);
            //GenG.Collide(Warthog4, Warthog5);
            //GenG.Collide(Cave, Chain);
            //GenG.Collide(Cave, Warthog3);
            //GenG.Collide(Player, Warthog3);
            //GenG.Collide(Player, Warthog4);
            //GenG.Collide(Cave, Emitter);
            GenG.Collide(Player, Boxes);
            GenG.Collide(Boxes, Boxes);

            Text.Y = GenU.SineWave(200, 2, 10);
            Text.Rotation = GenU.SineWave(0, 3, 10);

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

            //GenMove.AccelerateToPoint(Boxes, Player.CenterPosition, 200);
            GenMove.AccelerateToPoint(Emitter, Player.CenterPosition, 500, 100);
            GenMove.AccelerateToPoint(Cloth, Player.CenterPosition, Player.Velocity.Length() * 3, 70);

            Warthog4.Velocity.X = GenU.SineWave(0, 2, 200);
            //Warthog4.Color = new Color(1, GenU.SineWave(0.5f, 10, 0.5f), GenU.CosineWave(0.5f, 10, 0.5f));

            //Warthog3.Rotation = GenMove.VectortoAngle(Warthog3.CenterPosition, Player.CenterPosition);
            //GenMove.AccelerateAtAngle(Warthog3, Warthog3.Rotation, 500);

            if (Warthog5.Parent != null)
                Warthog5.Rotation = Warthog5.Parent.Rotation;

            //Chain.LineColor = GenU.RandomColor();

            //Player.Rotation++;
            if (Player.Velocity.Y < 0)
            {
                Player.Scale.X = MathHelper.Clamp(Math.Abs((Player.Velocity.Y) / Player.MaxVelocity.Y) + 1, 1f, 1.2f);
                Player.Scale.Y = 1 - (Player.Scale.X - 1);
            }
            else if (Player.Velocity.Y > 0)
            {
                Player.Scale.X = MathHelper.Clamp(1 - Math.Abs((Player.Velocity.Y) / Player.MaxVelocity.Y), 0.8f, 1f);
                Player.Scale.Y = 1 + (1 - Player.Scale.X);
            }
            else
            {
                Player.Scale.X = GenU.CosineWave(1, 8, 0.1f);
                Player.Scale.Y = GenU.SineWave(1, 8, 0.1f);
            }

            ProgressBar.Scale.Y = GenU.SineWave(1, 8, 0.2f);

            ProgressBar.Value = GenU.SineWave(50, 2, 51);

            //Cloth.LineColor = new Color(0.5f, (float)MathHelper.Lerp(0, 1, GenU.SineWave(0.5f, 10f, 0.5f)), 0.5f);

            //Player.Alpha = GenU.SineWave(0.5f, 2f, 0.5f);

            /*if (Player.IsTouching(GenObject.Direction.Down))
                Player.Rotation = 0;
            else if (Player.IsTouching(GenObject.Direction.Right))
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

            ((GenSprite)e.ObjectB).Flicker(40f, 2f, Color.OrangeRed, true);
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
            ((GenSprite)e.ObjectB).Color = GenU.RandomColor();
        }

        public void HitCave(GenCollideEvent e)
        {
            ((GenSprite)e.ObjectB).Color = Color.Red;
            //e.Object2.Exists = false;
        }

        public void HitNode()
        {
            ((GenObject)Chain.Members[0]).SetPath(null, 0);
            ((GenObject)Chain.Members[0]).StopMoving();
            GenG.Camera.Flash(0.5f, 1f, Color.Red);
            GenG.Camera.Shake();
        }

        public void PlayerLand()
        {
            GenG.GamePads[PlayerIndex.One].Vibrate(1f, 0.25f, 0.5f, true);
            GenG.Camera.Shake(5f, 0.5f, true, true, null, GenCamera.ShakeDirection.Vertical);
            GenG.Camera.Flash(0.15f, 0.2f, Color.Red);
        }

        public void PlayerJump()
        {
            if (PlayerControl.JumpCounter > 1)
            {
                GenG.Camera.Shake(2f, 0.2f);
                Emitter.InheritVelocity = false;
                Emitter.EmitParticles(100);
                Emitter.InheritVelocity = true;
            }

            Beep.Play();
        }
    }
}