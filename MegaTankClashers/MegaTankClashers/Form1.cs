﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MegaTankClashers
{
    public partial class Form1 : Form
    {
        //global variables
        enum Direction { Up, Down, Left, Right, None, Shoot };
        Direction PlayerOneDirection = Direction.None, PlayerTwoDirection = Direction.None;
        Direction PlayerOneLastAction = Direction.None, PlayerTwoLastAction = Direction.None;
        Direction BulletOneDirection = Direction.None, BulletTwoDirection = Direction.None;
        bool PlayerOneBulletOut = false, PlayerTwoBulletOut = false;
        int ShootCoolDown = 25, PlayerOneShootCoolDown = 0, PlayerTwoShootCoolDown = 0;
        PictureBox PlayerOneBullet, PlayerTwoBullet;
        public Form1()
        {
            InitializeComponent();
        }
        private bool CheckCollision(PictureBox picEntity, PictureBox Wall, Direction PlayerDirection, int PixelMovement = 10)
        {
            if (PlayerDirection == Direction.Right)
                return Wall.Bounds.Contains(picEntity.Bounds.X + picEntity.Width + PixelMovement, picEntity.Bounds.Y) || Wall.Bounds.Contains(picEntity.Bounds.X + picEntity.Width + PixelMovement, picEntity.Bounds.Y + picEntity.Height) || Wall.Bounds.Contains(picEntity.Bounds.X + picEntity.Width + PixelMovement, picEntity.Bounds.Y + picEntity.Height / 2);
            if (PlayerDirection == Direction.Left)
                return Wall.Bounds.Contains(picEntity.Bounds.X - PixelMovement, picEntity.Bounds.Y + picEntity.Height) || Wall.Bounds.Contains(picEntity.Bounds.X - PixelMovement, picEntity.Bounds.Y) || Wall.Bounds.Contains(picEntity.Bounds.X - PixelMovement, picEntity.Bounds.Y + picEntity.Height / 2);
            if (PlayerDirection == Direction.Up)
                return Wall.Bounds.Contains(picEntity.Bounds.X, picEntity.Bounds.Y - PixelMovement) || Wall.Bounds.Contains(picEntity.Bounds.X + (picEntity.Width / 2), picEntity.Bounds.Y - PixelMovement) || Wall.Bounds.Contains(picEntity.Bounds.X + picEntity.Width, picEntity.Bounds.Y - PixelMovement);
            if (PlayerDirection == Direction.Down)
                return Wall.Bounds.Contains(picEntity.Bounds.X, picEntity.Bounds.Y + picEntity.Height + PixelMovement) || Wall.Bounds.Contains(picEntity.Bounds.X + picEntity.Width, picEntity.Bounds.Y + picEntity.Height + PixelMovement) || Wall.Bounds.Contains(picEntity.Bounds.X + picEntity.Width / 2, picEntity.Bounds.Y + picEntity.Height + PixelMovement);
            return false;
        }

        private PictureBox CreateBullet(int x, int y, int PlayerNumber)
        {
            PictureBox Bullet = new PictureBox();
            Bullet.SizeMode = PictureBoxSizeMode.StretchImage;
            Bullet.Size = new Size(26, 27);
            Bullet.Image = Resource1.Bullet;
            Bullet.Tag = "Bullet" + PlayerNumber;
            Bullet.Location = new Point(x, y);
            Bullet.Visible = true;
            return Bullet;
        }

        private void AddBullet(PictureBox Bullet)
        {
            this.Controls.Add(Bullet);
        }
        private void RemoveBullet(PictureBox Bullet)
        {
            this.Controls.Remove(Bullet);
        }
        private int DistanceBetween(PictureBox Player, PictureBox Wall)
        {
            Point PlayerCenter = new Point(Player.Left + Player.Width / 2, Player.Top + Player.Height / 2);
            Point WallCenter = new Point(Wall.Left + Wall.Width / 2, Wall.Top + Wall.Height / 2);
            return Math.Max(Math.Abs(PlayerCenter.X - WallCenter.X) - (Player.Width + Wall.Width) / 2, Math.Abs(PlayerCenter.Y - WallCenter.Y) - (Player.Height + Wall.Height) / 2);
        }

        private Direction OppositeDirection(Direction NormalDirection)
        {
            switch (NormalDirection)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
            }
            return Direction.Up;
        }
        private void MovePlayer(int PlayerNumber, int Distance = 10)
        {
            if (PlayerNumber == 1)
            {
                switch (PlayerOneDirection)
                {
                    case Direction.Up:
                        picPlayerOne.Top -= Distance;
                        picPlayerOne.Image = Resource1.RedTankUp;
                        break;
                    case Direction.Left:
                        picPlayerOne.Left -= Distance;
                        picPlayerOne.Image = Resource1.RedTankLeft;
                        break;
                    case Direction.Right:
                        picPlayerOne.Left += Distance;
                        picPlayerOne.Image = Resource1.RedTankRight;
                        break;
                    case Direction.Down:
                        picPlayerOne.Top += Distance;
                        picPlayerOne.Image = Resource1.RedTankDown;
                        break;
                    case Direction.Shoot:
                        //Refactor Tip: Shoot() function should suffice.
                        if (!PlayerOneBulletOut)
                        {
                            switch (PlayerOneLastAction)
                            {
                                case Direction.Up:
                                    PlayerOneBullet = CreateBullet(picPlayerOne.Bounds.X + (picPlayerOne.Width / 2), picPlayerOne.Bounds.Y - 40, 1);
                                    BulletOneDirection = Direction.Up;
                                    break;
                                case Direction.Down:
                                    PlayerOneBullet = CreateBullet(picPlayerOne.Bounds.X + picPlayerOne.Width / 2, picPlayerOne.Bounds.Y + picPlayerOne.Height + 40, 1);
                                    BulletOneDirection = Direction.Down;
                                    break;
                                case Direction.Left:
                                    PlayerOneBullet = CreateBullet(picPlayerOne.Bounds.X - 40, picPlayerOne.Bounds.Y - 20, 1);
                                    BulletOneDirection = Direction.Left;
                                    break;
                                case Direction.Right:
                                    PlayerOneBullet = CreateBullet(picPlayerOne.Bounds.X + picPlayerOne.Width + 40, picPlayerOne.Bounds.Y, 1);
                                    BulletOneDirection = Direction.Right;
                                    break;
                            }
                            AddBullet(PlayerOneBullet);
                            PlayerOneBulletOut = true;
                            PlayerOneShootCoolDown = ShootCoolDown;
                        }
                        break;
                }
            }
            else
            {
                switch (PlayerTwoDirection)
                {
                    case Direction.Up:
                        picPlayerTwo.Top -= Distance;
                        picPlayerTwo.Image = Resource1.BlueTankUp;
                        break;
                    case Direction.Left:
                        picPlayerTwo.Left -= Distance;
                        picPlayerTwo.Image = Resource1.BlueTankLeft;
                        break;
                    case Direction.Right:
                        picPlayerTwo.Left += Distance;
                        picPlayerTwo.Image = Resource1.BlueTankRight;
                        break;
                    case Direction.Down:
                        picPlayerTwo.Top += Distance;
                        picPlayerTwo.Image = Resource1.BlueTankDown;
                        break;
                    case Direction.Shoot:
                        if (!PlayerTwoBulletOut)
                        {
                            switch (PlayerTwoLastAction)
                            {
                                case Direction.Up:
                                    PlayerTwoBullet = CreateBullet(picPlayerTwo.Bounds.X + (picPlayerTwo.Width / 2), picPlayerTwo.Bounds.Y - 40, 1);
                                    BulletTwoDirection = Direction.Up;
                                    break;
                                case Direction.Down:
                                    PlayerTwoBullet = CreateBullet(picPlayerTwo.Bounds.X + picPlayerTwo.Width / 2, picPlayerTwo.Bounds.Y + picPlayerTwo.Height + 40, 1);
                                    BulletTwoDirection = Direction.Down;
                                    break;
                                case Direction.Left:
                                    PlayerTwoBullet = CreateBullet(picPlayerTwo.Bounds.X - 40, picPlayerTwo.Bounds.Y - 20, 1);
                                    BulletTwoDirection = Direction.Left;
                                    break;
                                case Direction.Right:
                                    PlayerTwoBullet = CreateBullet(picPlayerTwo.Bounds.X + picPlayerTwo.Width + 40, picPlayerTwo.Bounds.Y, 1);
                                    BulletTwoDirection = Direction.Right;
                                    break;
                            }
                            AddBullet(PlayerTwoBullet);
                            PlayerTwoBulletOut = true;
                            PlayerTwoShootCoolDown = ShootCoolDown;
                        }
                        break;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //player one
                case (Keys.W):
                    PlayerOneDirection = Direction.Up;
                    PlayerOneLastAction = Direction.Up;
                    break;
                case (Keys.A):
                    PlayerOneDirection = Direction.Left;
                    PlayerOneLastAction = Direction.Left;
                    break;
                case (Keys.D):
                    PlayerOneDirection = Direction.Right;
                    PlayerOneLastAction = Direction.Right;
                    break;
                case (Keys.S):
                    PlayerOneDirection = Direction.Down;
                    PlayerOneLastAction = Direction.Down;
                    break;
                case (Keys.P):
                    PlayerOneDirection = Direction.Shoot;
                    break;
                case (Keys.Up):
                    PlayerTwoDirection = Direction.Up;
                    PlayerTwoLastAction = Direction.Up;
                    break;
                case (Keys.Left):
                    PlayerTwoDirection = Direction.Left;
                    PlayerTwoLastAction = Direction.Left;
                    break;
                case (Keys.Right):
                    PlayerTwoDirection = Direction.Right;
                    PlayerTwoLastAction = Direction.Right;
                    break;
                case (Keys.Down):
                    PlayerTwoDirection = Direction.Down;
                    PlayerTwoLastAction = Direction.Down;
                    break;
                case (Keys.NumPad0):
                    PlayerTwoDirection = Direction.Shoot;
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tmrGame.Enabled = true;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //player one
                case (Keys.W):
                    PlayerOneDirection = Direction.None;
                    break;
                case (Keys.A):
                    PlayerOneDirection = Direction.None;
                    break;
                case (Keys.D):
                    PlayerOneDirection = Direction.None;
                    break;
                case (Keys.S):
                    PlayerOneDirection = Direction.None;
                    break;
                case (Keys.P):
                    //shooting
                    break;
                case (Keys.Up):
                    PlayerTwoDirection = Direction.None;
                    break;
                case (Keys.Left):
                    PlayerTwoDirection = Direction.None;
                    break;
                case (Keys.Right):
                    PlayerTwoDirection = Direction.None;
                    break;
                case (Keys.Down):
                    PlayerTwoDirection = Direction.None;
                    break;
                case (Keys.NumPad0):
                    break;
            }
        }

       
        //check and handle collisions with walls
        private void CheckWallPlayer(int PlayerNumber, Control X)
        {
            PictureBox Wall = (PictureBox)X;
            int DistanceToWall;

            if (PlayerNumber == 1)
            {
                if (CheckCollision(picPlayerOne, Wall, PlayerOneDirection))
                {
                    DistanceToWall = DistanceBetween(picPlayerOne, Wall);
                    MovePlayer(1, DistanceToWall);
                }
            }
            else
            {
                if (CheckCollision(picPlayerTwo, Wall, PlayerTwoDirection))
                {
                    DistanceToWall = DistanceBetween(picPlayerTwo, Wall);
                    MovePlayer(2, DistanceToWall);
                }
            }
        }

        //movebullet
        private void MoveBullet(PictureBox Bullet, Direction BulletDirection, int Distance = 3)
        {
            switch (BulletDirection)
            {
                case Direction.Up:
                    Bullet.Top -= Distance;
                    break;
                case Direction.Down:
                    Bullet.Top += Distance;
                    break;
                case Direction.Left:
                    Bullet.Left -= Distance;
                    break;
                case Direction.Right:
                    Bullet.Left += Distance;
                    break;
            }
        }

        //check collision with bullet
        private void CheckBulletPlayer(int PlayerNumber, Control X)
        {
            PictureBox Bullet = (PictureBox)X;
            //check Player1 bullet
            if (Bullet.Tag.ToString().Contains("1"))
            {
                //check if it hits player 1
                if (CheckCollision(picPlayerOne, Bullet, OppositeDirection(BulletOneDirection), 3))
                {
                    int DistanceTo = DistanceBetween(picPlayerOne, Bullet);
                    MoveBullet(Bullet, BulletOneDirection, DistanceTo);
                    RemoveBullet(Bullet);
                    this.Controls.Remove(picPlayerOne);
                    GameOver(2);
                }
                //check if it hits player 2
                if (CheckCollision(picPlayerTwo, Bullet, OppositeDirection(BulletOneDirection), 3))
                {
                    int DistanceTo = DistanceBetween(picPlayerTwo, Bullet);
                    MoveBullet(Bullet, BulletTwoDirection, DistanceTo);
                    RemoveBullet(Bullet);
                    this.Controls.Remove(picPlayerTwo);
                    GameOver(1);
                }
            }
            //check Player 2 bullet
            else
            {
                //check if it hits player 1
                if (CheckCollision(picPlayerOne, Bullet, OppositeDirection(BulletTwoDirection), 3))
                {
                    int DistanceTo = DistanceBetween(picPlayerOne, Bullet);
                    MoveBullet(Bullet, BulletTwoDirection, DistanceTo);
                    RemoveBullet(Bullet);
                    this.Controls.Remove(picPlayerOne);
                    GameOver(2);
                }
                //check if it hits player 2
                if (CheckCollision(picPlayerTwo, Bullet, OppositeDirection(BulletTwoDirection), 3))
                {
                    int DistanceTo = DistanceBetween(picPlayerTwo, Bullet);
                    MoveBullet(Bullet, BulletTwoDirection, DistanceTo);
                    RemoveBullet(Bullet);
                    this.Controls.Remove(picPlayerTwo);
                    GameOver(1);
                }
            }
        }

        private void CheckBulletWall(int BulletNumber, Control X)
        {
            PictureBox Wall = (PictureBox)X;
            int DistanceToWall;

            if (BulletNumber == 1)
            {
                if (CheckCollision(PlayerOneBullet, Wall, BulletOneDirection))
                {
                    DistanceToWall = DistanceBetween(PlayerOneBullet, Wall);
                    MoveBullet(PlayerOneBullet,BulletOneDirection,DistanceToWall);
                    this.Controls.Remove(PlayerOneBullet);
                }
                else
                {
                    MoveBullet(PlayerOneBullet, BulletOneDirection);
                }
            }
            else
            {
                if (CheckCollision(PlayerTwoBullet, Wall, BulletTwoDirection))
                {
                    DistanceToWall = DistanceBetween(PlayerTwoBullet, Wall);
                    MoveBullet(PlayerTwoBullet, BulletTwoDirection, DistanceToWall);
                    this.Controls.Remove(PlayerTwoBullet);
                }
                else
                {
                    MoveBullet(PlayerTwoBullet, BulletTwoDirection);
                }
            }
        }

        private void CoolBullets()
        {
            if (PlayerOneShootCoolDown > 0)
                PlayerOneShootCoolDown--;
            else
            {
                PlayerOneBulletOut = false;
                RemoveBullet(PlayerOneBullet);
            }
            if (PlayerTwoShootCoolDown > 0)
                PlayerTwoShootCoolDown--;
            else
            {
                PlayerTwoBulletOut = false;
                RemoveBullet(PlayerTwoBullet);
            }
        }

        private void TmrGame_Tick(object sender, EventArgs e)
        {
            //check everything on the form
            foreach (Control X in this.Controls)
            {
                if (X.Tag.ToString().Equals("Wall"))
                {
                    //handle collisions with walls
                    CheckWallPlayer(1, X);
                    CheckWallPlayer(2, X);
                }

                //check bullet collisions
                else if (X.Tag.ToString().Contains("Bullet"))
                {
                    //check bullet vs. player collisions
                    CheckBulletPlayer(1, X);
                    CheckBulletPlayer(2, X);

                    //check bullet vs. wall collisions
                    foreach (Control Y in this.Controls)
                    {
                        if (Y.Tag.ToString().Equals("Wall"))
                        {
                            CheckBulletWall(1, Y);
                            CheckBulletWall(2, Y);
                        }
                    }
                }
            }
            //adjust bullet cooldowns
            CoolBullets();

            //move players
            MovePlayer(1);
            MovePlayer(2);
        }
        private void GameOver(int WinnerPlayer)
        {
            tmrGame.Enabled = false;
            if (WinnerPlayer == 2)
            {
                this.Controls.Remove(picPlayerOne);
            }
            else
                this.Controls.Remove(picPlayerTwo);
            lblWin.Visible = true;
            lblWin.Text = "Player" + WinnerPlayer + " is the Winner!";
        }
    }
}