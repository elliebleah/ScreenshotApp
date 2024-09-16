using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
    public class ImageFullViewForm : Form
    {
        private PictureBox pictureBox;

        public ImageFullViewForm(Bitmap capturedImage)
        {
            InitializeComponents(capturedImage);
        }

        private void InitializeComponents(Bitmap capturedImage)
        {
            this.Text = "View Image";
            this.StartPosition = FormStartPosition.CenterScreen; // Center the form on the screen
            this.Size = new Size(700, 700);

            pictureBox = new PictureBox
            {
                Image = capturedImage,
                SizeMode = PictureBoxSizeMode.Zoom, // Ensures the image is scaled to fit the PictureBox while maintaining aspect ratio
                Dock = DockStyle.Fill // Fills the PictureBox within the form

            };
            pictureBox.ContextMenuStrip = CreateContextMenu(capturedImage, pictureBox);
            this.Controls.Add(pictureBox);
        }

        private ContextMenuStrip CreateContextMenu(Bitmap image, PictureBox pictureBox)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem copyItem = new ToolStripMenuItem("Copy to Clipboard");
            ToolStripMenuItem saveItem = new ToolStripMenuItem("Save Image");

            copyItem.Click += (sender, e) => CopyToClipboard(image);
            saveItem.Click += (sender, e) => SaveImage(image);

            contextMenu.Items.Add(copyItem);
            contextMenu.Items.Add(saveItem);

            return contextMenu;
        }

        private void CopyToClipboard(Bitmap image)
        {
            Clipboard.SetImage(new Bitmap(image));
        }

        private void SaveImage(Bitmap image)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                saveFileDialog.Title = "Save an Image File";
                saveFileDialog.FileName = "image.png"; // Default file name

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Save the image to the selected path
                    switch (saveFileDialog.FilterIndex)
                    {
                        case 1:
                            image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case 2:
                            image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case 3:
                            image.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                    }
                }
            }
        }
    }
}
