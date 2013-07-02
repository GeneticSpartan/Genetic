using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using Genetic.Gui;
using Genetic.Input;
using Genetic.Particles;
using Genetic.Path;
using Genetic.Physics;
using Genetic.Sound;

namespace Genetic
{
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
        //public GenText Text;

        //public GenTimer Timer;

        public GenVerlet Chain;
        //public GenVerlet Cloth;

        public GenPath Path;

        public GenEmitter Emitter;

        public GenProgressBar ProgressBar;
        Stopwatch watch = new Stopwatch();

        public CollideEvent HitCaveCache;

        public override void Create()
        {
            base.Create();

            GenG.ShowDebugInfo = true;

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

            BgColor = Color.CornflowerBlue;

            //camera2 = AddCamera(new GenCamera(0, GenG.Game.Height / 2, GenG.Game.Width, GenG.Game.Height / 2, 2f));
            //camera2.BgColor = new Color(50, 50, 70);

            Camera.BgColor = new Color(40, 50, 70);
            //Camera.SetCameraView(0f, 0f, GenG.Game.Width, (int)(GenG.Game.Height * 0.5f));

            /*Camera.BlendState.ColorSourceBlend = Blend.One;
            Camera.BlendState.ColorDestinationBlend = Blend.InverseSourceColor;
            Camera.BlendState.ColorBlendFunction = BlendFunction.Subtract;
            Camera.BlendState.AlphaSourceBlend = Blend.One;
            Camera.BlendState.AlphaDestinationBlend = Blend.InverseSourceColor;
            Camera.BlendState.AlphaBlendFunction = BlendFunction.Subtract;*/

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
            ((GenObject)Chain.Members[0]).SetPath(Path, 100, GenPath.Type.Yoyo, GenMove.Axis.Both, GenPath.Movement.Instant);
            Add(Chain);
            
            Boxes = new GenGroup();
            Add(Boxes);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Box = new GenSprite(i * 32 + 150, j * 16, GenG.Content.Load<Texture2D>("warthog"), 12, 13);
                    Box.CenterOrigin(false);
                    Box.AddAnimation("run", 16, 16, new int[] { 1 }, 0, false);
                    Box.Play("run");
                    Box.DrawOffset.X = -4;
                    Box.DrawOffset.Y = -3;
                    Box.Mass = 0.5f;
                    Box.Deceleration.X = 400f;
                    Box.Acceleration.Y = 400f;
                    Box.MaxVelocity.Y = 400f;
                    Box.Color = GenU.RandomColor(100, 255);
                    Boxes.Add(Box);
                }
            }

            Emitter = new GenEmitter(100, 100);
            Emitter.Width = 16;
            Emitter.Height = 16;
            Emitter.MakeParticles(GenG.Pixel, 4, 4, 400);
            Emitter.EmitQuantity = 5;
            Emitter.EmitFrequency = .05f;
            Emitter.InheritVelocity = true;
            Emitter.SetXSpeed(-300, 300);
            Emitter.SetYSpeed(-50, 50);
            Emitter.SetRotationSpeed(-360, 360);
            Emitter.SetLifetime(2.9f);
            Emitter.Colors.Add(Color.Cyan);
            Emitter.Colors.Add(Color.MediumVioletRed);
            Emitter.Colors.Add(Color.Orange);
            Emitter.SetAlpha(2f, 0f);
            Emitter.SetScale(2f, 1f);
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
            Emitter.ParentOffset.X = -Player.Bounds.HalfWidth;
            Emitter.ParentOffset.Y = -Player.Bounds.Height;
            Emitter.Start(false);

            Warthog3 = new GenSprite(200, 300, GenG.Content.Load<Texture2D>("warthog"), 78, 49);
            Warthog3.Deceleration.X = 400;
            Warthog3.Deceleration.Y = 400;
            Warthog3.MaxVelocity.X = 250;
            Warthog3.MaxVelocity.Y = 400;
            Warthog3.Mass = 2f;
            //Warthog3.IsPlatform = true;
            Warthog3.RotationSpeed = -90;
            //Warthog3.SetParent(Player, GenObject.ParentType.Origin);
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

            Warthog5 = new GenSprite(2000, 300, GenG.Content.Load<Texture2D>("warthog"), 78, 49);
            //Warthog5.Acceleration.Y = 700;
            Warthog5.Parent = Warthog4;
            Warthog5.ParentOffset.X = 150;
            Warthog5.Velocity.X = -1000;
            Add(Warthog5);

            //Chain.MakeLink(Warthog3, (GenObject)Chain.Members[14]);
            //Chain.SetRestingDistance(10f);

            Beep = new GenSound(GenG.LoadContent<SoundEffect>("beep"), 1f, true);
            Beep.Follow = Warthog5;
            Beep.Volume = 0.1f;
            Add(Beep);

            /*Text = new GenText("Hello, World!\n------------", 200, 200, 100, 12);
            Text.Scale.X = 0.5f;
            Text.Scale.Y = 0.5f;
            Text.TextAlignment = GenText.TextAlign.Center;
            Text.HasShadow = true;
            Text.ShadowColor = Color.Black;
            //Text.Velocity.X = 100;
            //Text.Velocity.Y = 50;
            Text.ScrollFactor.X = 0f;
            Add(Text);*/

            ProgressBar = new GenProgressBar(100, 100);
            ProgressBar.LoadTexture(GenG.Pixel);
            ProgressBar.SetSourceRect(0, 0, 100, 10);
            ProgressBar.Colors.Add(Color.Red);
            ProgressBar.Colors.Add(Color.Orange);
            ProgressBar.Colors.Add(Color.Green);
            ProgressBar.Colors.Add(Color.CornflowerBlue);
            ProgressBar.Colors.Add(Color.White);
            //ProgressBar.BlendColors = true;
            //ProgressBar.MinCallback = Shake;
            ProgressBar.Rotation = -90;
            ProgressBar.SetParent(Player, GenObject.ParentType.Position);
            ProgressBar.ParentOffset.X = 30;
            Add(ProgressBar);

            /*Cloth = new GenVerlet();
            Cloth.MakeGrid(100, 200, 8, 10, 11);
            Cloth.SetGravity(0f, 700f);
            //Cloth.SetDeceleration(100f, 0f);
            Cloth.LineColor = Color.Gray;
            Cloth.Iterations = 2;*/

            /*for (int i = 0; i < 10; i++)
            {
                ((GenObject)Cloth.Members[i]).Immovable = true;
                ((GenObject)Cloth.Members[i]).Acceleration.Y = 0f;
            }*/

            /*((GenObject)Cloth.Members[0]).Immovable = true;
            ((GenObject)Cloth.Members[0]).Acceleration.Y = 0f;
            ((GenObject)Cloth.Members[5]).Immovable = true;
            ((GenObject)Cloth.Members[5]).Acceleration.Y = 0f;
            ((GenObject)Cloth.Members[10]).Immovable = true;
            ((GenObject)Cloth.Members[10]).Acceleration.Y = 0f;

            Add(Cloth);*/

            GenG.TimeScale = 1f;

            Camera.CameraFollowType = GenCamera.FollowType.LockOn;
            //Camera.FollowStrength = 0.05f;
            //Camera.MaxZoom = 10f;
            Camera.AddTarget(Player);
            //Camera.AddTarget(Warthog3);
            //Camera.SetCameraView(0, 0, GenG.Game.Width, GenG.Game.Height / 2);
            //Camera.Rotation = 180f;

            /*camera2.CameraFollowType = GenCamera.FollowType.LockOn;
            camera2.FollowStrength = 0.05f;
            camera2.AddTarget(Warthog5);*/

            Player.Flicker(40f, 1f, Color.Red, true);
            Player.FadeOut(0.5f);

            //Timer = new GenTimer(1f, KillBox);
            //Add(Timer);
            //Timer.Start();

            //Warthog5.Parent = Player;

            //Boxes.Kill();

            //Warthog3.SetPath(Path);
            //Warthog3.PathSpeed = 100;
        }

        /// <summary>
        /// Called once when the state starts running for the first time.
        /// </summary>
        public override void Start()
        {
            base.Start();

            GenG.Flash(1f, 1f, Color.Black);

            Beep.Play();
        }

        public override void Update()
        {
            base.Update();
            
            // Do collision checking first.
            //GenG.Collide(Player, Text);

            // Cache the HitCave delegate to avoid creating garbage for each collision check.
            if (HitCaveCache == null)
            {
                HitCaveCache = HitCave;
            }

            GenG.Collide(Cave, Player, HitCaveCache);
            GenG.Collide(Cave, Boxes);
            
            //GenG.Collide(Chain, Player);
            //GenG.Collide(Warthog4, Warthog5);
            //GenG.Collide(Cave, Chain);
            //GenG.Collide(Cave, Warthog3);
            //GenG.Collide(Player, Warthog3);
            //GenG.Collide(Player, Warthog4);
            GenG.Collide(Cave, Emitter);
            //GenG.Collide(Emitter, Emitter);
            GenG.Collide(Player, Boxes);
            GenG.Collide(Boxes, Boxes);
            //GenG.Collide(Boxes, Emitter);

            //Text.Y = GenU.SineWave(200, 2, 10);
            //Text.Rotation = GenU.SineWave(0, 3, 10);

            if (GenG.GamePads[(int)PlayerIndex.One].JustPressed(Buttons.X))
                GenG.IsDebug = !GenG.IsDebug;
#if WINDOWS
            if (GenG.Keyboards[(int)PlayerIndex.One].JustPressed(Keys.R))
                GenG.ResetState(new LoadingState());

            if (GenG.Keyboards[(int)PlayerIndex.One].IsPressed(Keys.Z))
                GenG.TimeScale = 0.2f;
            else if (GenG.Keyboards[(int)PlayerIndex.One].IsPressed(Keys.X))
                GenG.TimeScale = 2f;
            else
                GenG.TimeScale = 1f;

            if (GenG.Keyboards[(int)PlayerIndex.One].JustPressed(Keys.A))
                GenG.Paused = !GenG.Paused;

            if (GenG.Keyboards[(int)PlayerIndex.One].JustPressed(Keys.Escape))
                GenG.Game.Exit();
#endif
            if (GenG.GamePads[(int)PlayerIndex.One].JustPressed(Buttons.Y))
                GenG.ResetState(new LoadingState());

            /*if (GenG.GamePads[(int)PlayerIndex.One].IsPressed(Buttons.LeftTrigger))
                GenG.TimeScale = 0.2f;
            else if (GenG.GamePads[(int)PlayerIndex.One].IsPressed(Buttons.RightTrigger))
                GenG.TimeScale = 2f;
            else
                GenG.TimeScale = 1f;*/

            if (GenG.GamePads[(int)PlayerIndex.One].JustPressed(Buttons.LeftTrigger))
                Camera.Flash(0.5f, 0.1f, Color.CornflowerBlue);

            if (GenG.GamePads[(int)PlayerIndex.One].JustPressed(Buttons.Start))
                GenG.Paused = !GenG.Paused;

            if (GenG.GamePads[(int)PlayerIndex.One].JustPressed(Buttons.Back))
                GenG.Game.Exit();

            //((GenObject)Chain.Members[0]).X = Player.X;
            //((GenObject)Chain.Members[0]).Y = Player.Y;

            if (!GenG.Paused)
            {
                //GenMove.AccelerateToPoint(Boxes, Player.CenterPosition, 200);
                GenMove.AccelerateToPoint(Emitter, Player.CenterPosition, 500, 100);
                //GenMove.AccelerateToPoint(Cloth, Player.CenterPosition, Player.Velocity.Length() * 3, 70);
            }

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

            //if (GenG.Keyboards[(int)PlayerIndex.One].JustPressed(Keys.Space))
            //    Cave.AddTile((int)(Player.OriginPosition.X / Cave.TileWidth), (int)(Player.OriginPosition.Y / Cave.TileHeight), false);

            if (Warthog5.X < -2000)
            {
                Warthog5.X = 5000;
            }
        }

        public void FadeOut(GenCollideEvent e)
        {
            Camera.Fade(2, Color.Black, EndGame);
            //camera2.Fade(2, Color.White);

            (e.ObjectB as GenSprite).Flicker(40f, 2f, Color.OrangeRed, true);
        }

        public void EndGame()
        {
            GenG.ResetState(new LoadingState());
        }

        public void HitBox(GenCollideEvent e)
        {
            //e.Object1.Velocity.Y = -200;
            (e.ObjectB as GenSprite).Color = GenU.RandomColor();
        }

        public void HitCave(GenCollideEvent e)
        {
            (e.ObjectB as GenSprite).Color = Color.Red;
        }

        public void HitNode()
        {
            (Chain.Members[0] as GenObject).SetPath(null, 0);
            (Chain.Members[0] as GenObject).StopMoving();
            Camera.Flash(0.5f, 1f, Color.Red);
            Camera.Shake();
        }

        public void PlayerLand()
        {
            GenG.GamePads[(int)PlayerIndex.One].Vibrate(1f, 0.25f, 0.5f, true);
            Camera.Shake(5f, 0.5f, true, true, null, GenCamera.ShakeDirection.Vertical);
            Camera.Flash(0.15f, 0.2f, Color.Red);
        }

        public void PlayerJump()
        {
            if (PlayerControl.JumpCounter > 1)
            {
                Camera.Shake(2f, 0.2f);
                Emitter.InheritVelocity = false;
                Emitter.EmitParticles(100);
                Emitter.InheritVelocity = true;
            }

            //Beep.Play();
        }
    }
}