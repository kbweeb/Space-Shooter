using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceShooter
{
    public partial class Form1 : Form
    {
        // Variables for stars and background movement
        PictureBox[] stars;
        int backgroundSpeed;
        Random rnd;

        // Player variables
        PictureBox Player;
        int playerSpeed = 5; // Speed for player movement

        // Shooting variables
        Timer shootTimer;
        int bulletSpeed = 10;
        PictureBox[] bullets;
        int bulletIndex = 0;
        int maxBullets = 5; // Max number of bullets that can exist at the same time

        // Enemy variables
        PictureBox[] enemies;
        int enemySpeed = 2;

        // Game state
        int score = 0;
        Label scoreLabel;

        // Constructor
        public Form1()
        {
            InitializeComponent();
        }

        // Form Load Event
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize background stars
            backgroundSpeed = 4;
            stars = new PictureBox[10];
            rnd = new Random();

            // Create stars
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new PictureBox();
                stars[i].BorderStyle = BorderStyle.None;
                stars[i].Location = new Point(rnd.Next(20, 580), rnd.Next(-10, 400));
                if (i % 2 == 1)
                {
                    stars[i].Size = new Size(2, 2);
                    stars[i].BackColor = Color.Wheat;
                }
                else
                {
                    stars[i].Size = new Size(3, 3);
                    stars[i].BackColor = Color.DarkGray;
                }
                this.Controls.Add(stars[i]);
            }

            // Initialize Player
            Player = new PictureBox();
            Player.Size = new Size(50, 50);  // Set player size
            Player.BackColor = Color.Blue;  // Set player color
            Player.Location = new Point(this.ClientSize.Width / 2 - Player.Width / 2, this.ClientSize.Height - 60);  // Start at the bottom center
            this.Controls.Add(Player);

            // Set up background movement timer
            Timer moveBgTimer = new Timer();
            moveBgTimer.Interval = 30;
            moveBgTimer.Tick += MoveBgTimer_Tick;
            moveBgTimer.Start();

            // Set up player movement timers
            Timer leftMoveTimer = new Timer();
            leftMoveTimer.Interval = 30;
            leftMoveTimer.Tick += LeftMoveTimer_Tick;
            leftMoveTimer.Start();

            Timer rightMoveTimer = new Timer();
            rightMoveTimer.Interval = 30;
            rightMoveTimer.Tick += RightMoveTimer_Tick;
            rightMoveTimer.Start();

            Timer upMoveTimer = new Timer();
            upMoveTimer.Interval = 30;
            upMoveTimer.Tick += UpMoveTimer_Tick;
            upMoveTimer.Start();

            Timer downMoveTimer = new Timer();
            downMoveTimer.Interval = 30;
            downMoveTimer.Tick += DownMoveTimer_Tick;
            downMoveTimer.Start();

            // Set up shooting mechanism
            shootTimer = new Timer();
            shootTimer.Interval = 10; // Bullet movement interval
            shootTimer.Tick += ShootTimer_Tick;
            shootTimer.Start();

            bullets = new PictureBox[maxBullets];

            // Initialize Score Label
            scoreLabel = new Label();
            scoreLabel.Text = "Score: 0";
            scoreLabel.ForeColor = Color.White;
            scoreLabel.Font = new Font("Arial", 16);
            scoreLabel.Location = new Point(10, 10);
            this.Controls.Add(scoreLabel);

            // Set up enemies
            enemies = new PictureBox[5];  // Example number of enemies
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new PictureBox();
                enemies[i].Size = new Size(50, 50);
                enemies[i].BackColor = Color.Red;
                enemies[i].Location = new Point(rnd.Next(20, 580), rnd.Next(-400, -100));
                this.Controls.Add(enemies[i]);
            }
        }

        // Background movement timer event
        private void MoveBgTimer_Tick(object sender, EventArgs e)
        {
            // Move stars downwards
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].Top += backgroundSpeed;
                if (stars[i].Top >= this.ClientSize.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }

            // Move second set of stars faster
            for (int i = stars.Length / 2; i < stars.Length; i++)
            {
                stars[i].Top += backgroundSpeed - 2;
                if (stars[i].Top >= this.ClientSize.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }

            // Move enemies
            foreach (var enemy in enemies)
            {
                enemy.Top += enemySpeed;
                if (enemy.Top > this.ClientSize.Height)
                {
                    enemy.Top = -enemy.Height;
                    enemy.Left = rnd.Next(20, 580);
                }
            }
        }

        // Left movement logic
        private void LeftMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Left > 10)
            {
                Player.Left -= playerSpeed;
            }
        }

        // Right movement logic
        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Right < this.ClientSize.Width - 10)
            {
                Player.Left += playerSpeed;
            }
        }

        // Down movement logic
        private void DownMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Bottom < this.ClientSize.Height - 10)
            {
                Player.Top += playerSpeed;
            }
        }

        // Up movement logic
        private void UpMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 10)
            {
                Player.Top -= playerSpeed;
            }
        }

        // Shoot logic
        private void ShootTimer_Tick(object sender, EventArgs e)
        {
            // Move bullets upwards
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i] != null)
                {
                    bullets[i].Top -= bulletSpeed;

                    // Check for collision with enemies
                    foreach (var enemy in enemies)
                    {
                        if (bullets[i].Bounds.IntersectsWith(enemy.Bounds))
                        {
                            // Destroy the enemy and bullet on collision
                            enemy.Top = -enemy.Height;  // Reset enemy position
                            enemy.Left = rnd.Next(20, 580);
                            bullets[i].Top = -bullets[i].Height;  // Reset bullet position

                            // Increase score
                            score += 10;
                            scoreLabel.Text = "Score: " + score;
                        }
                    }

                    // If bullet goes off screen, reset it
                    if (bullets[i].Top < 0)
                    {
                        bullets[i] = null;
                    }
                }
            }
        }

        // Fire bullet
        private void FireBullet()
        {
            // Find an empty spot in the bullets array and create a new bullet
            for (int i = 0; i < maxBullets; i++)
            {
                if (bullets[i] == null)
                {
                    bullets[i] = new PictureBox();
                    bullets[i].Size = new Size(5, 20);
                    bullets[i].BackColor = Color.White;
                    bullets[i].Location = new Point(Player.Left + Player.Width / 2 - 2, Player.Top - 20);
                    this.Controls.Add(bullets[i]);
                    break;
                }
            }
        }

        // Key press event to fire bullets
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space)
            {
                FireBullet();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
