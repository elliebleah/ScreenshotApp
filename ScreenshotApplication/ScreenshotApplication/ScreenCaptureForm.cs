using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
    public class ScreenCaptureForm : Form
    {
        private bool isDrawing;
        private Point startPoint;
        private Rectangle rect;
        private Button captureButton;
        private Button exitButton;
        private Timer hoverTimer;
        private ImageDisplayForm displayForm;

        public ScreenCaptureForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Screen Capture Tool";
            this.Size = new Size(400, 300);

            captureButton = new Button
            {
                Text = "Start Capture",
                Size = new Size(200, 100)
            };
            captureButton.Click += new EventHandler(this.CaptureButton_Click);

            this.Controls.Add(captureButton);

            exitButton = new Button
            {
                Text = "Cancel",
                Size = new Size(300, 50)
            };
            exitButton.Click += new EventHandler(this.ExitButton_Click);

            // Center exitButton at the top of the screen
            var screenBounds = Screen.PrimaryScreen.Bounds;
            exitButton.Location = new Point(
                (screenBounds.Width - exitButton.Width) / 2,
                10);
            exitButton.Visible = false;

            this.Controls.Add(exitButton);

            hoverTimer = new Timer
            {
                Interval = 100
            };
            hoverTimer.Tick += new EventHandler(this.HoverTimer_Tick);

            CenterCaptureButton();

            // Initialize the image display form
            displayForm = new ImageDisplayForm();
            displayForm.Show();
        }

        private void CenterCaptureButton()
        {
            captureButton.Location = new Point(
                (this.ClientSize.Width - captureButton.Width) / 2,
                (this.ClientSize.Height - captureButton.Height) / 2);
        }

        private void CaptureButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.BackColor = Color.White;
            this.Opacity = 0.5;
            this.Cursor = Cursors.Cross;
            this.DoubleBuffered = true;

            exitButton.Visible = true;  // Show the exit button on start capture
            exitButton.BringToFront();  // Ensure the exit button is on top

            this.MouseDown += new MouseEventHandler(this.OnMouseDown);
            this.MouseMove += new MouseEventHandler(this.OnMouseMove);
            this.MouseUp += new MouseEventHandler(this.OnMouseUp);

            hoverTimer.Start();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            hoverTimer.Stop();
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.TopMost = false;
            this.BackColor = SystemColors.Control;
            this.Opacity = 1.0;
            this.Cursor = Cursors.Default;

            this.MouseDown -= new MouseEventHandler(this.OnMouseDown);
            this.MouseMove -= new MouseEventHandler(this.OnMouseMove);
            this.MouseUp -= new MouseEventHandler(this.OnMouseUp);
            exitButton.Visible = false;
        }

        private void HoverTimer_Tick(object sender, EventArgs e)
        {
            if (Cursor.Position.Y <= 50)
            {
                exitButton.Visible = true;
            }
            else
            {
                exitButton.Visible = false;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            startPoint = e.Location;
            rect = new Rectangle(e.X, e.Y, 0, 0);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                rect = new Rectangle(
                    Math.Min(startPoint.X, e.X),
                    Math.Min(startPoint.Y, e.Y),
                    Math.Abs(startPoint.X - e.X),
                    Math.Abs(startPoint.Y - e.Y));
                this.Invalidate();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
            CaptureScreen();
        }

        private void CaptureScreen()
        {
            // Ensure the rectangle has a valid size
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                MessageBox.Show("Invalid capture area. Please select a valid region.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Hide(); // Hide the form before capturing the screen

            // Give some time to ensure the form is hidden
            Application.DoEvents();
            System.Threading.Thread.Sleep(100);

            try
            {
                using (Bitmap bmp = new Bitmap(rect.Width, rect.Height))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        var screenLocation = this.PointToScreen(rect.Location);
                        g.CopyFromScreen(screenLocation, Point.Empty, rect.Size);
                    }

                    if (displayForm == null || displayForm.IsDisposed)
                    {
                        displayForm = new ImageDisplayForm();
                        displayForm.Show();
                    }

                    // Add the captured image to the image display form
                    displayForm.AddImage(new Bitmap(bmp)); // Ensure the image is not disposed
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing screen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Show(); // Show the form after capturing the screen

                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.TopMost = false;
                this.BackColor = SystemColors.Control;
                this.Opacity = 1.0;
                this.Cursor = Cursors.Default;

                this.MouseDown -= new MouseEventHandler(this.OnMouseDown);
                this.MouseMove -= new MouseEventHandler(this.OnMouseMove);
                this.MouseUp -= new MouseEventHandler(this.OnMouseUp);

                exitButton.Visible = false;
                hoverTimer.Stop();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (isDrawing)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (captureButton != null)
            {
                CenterCaptureButton();
            }
        }
    }
}
