using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
    public class ImageDisplayForm : Form
    {
        private FlowLayoutPanel flowLayoutPanel;
        private List<Bitmap> images;
        public ImageDisplayForm()
        {
            images = new List<Bitmap>();
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Captured Images";
            this.Size = new Size(800, 600);
            flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.Dock = DockStyle.Fill;
            flowLayoutPanel.AutoScroll = true;
            flowLayoutPanel.WrapContents = true;
            flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
            this.Controls.Add(flowLayoutPanel);
        }

        public void AddImage(Bitmap capturedImage)
        {
            images.Add(capturedImage);

            PictureBox pictureBox = new PictureBox
            {
                Image = new Bitmap(capturedImage), // Clone the image to ensure it's not disposed
                Size = new Size(200, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(10),
            };

            pictureBox.ContextMenuStrip = CreateContextMenu(capturedImage, pictureBox);

            flowLayoutPanel.Controls.Add(pictureBox);
        }
        public void RemoveImage(Bitmap image, PictureBox pictureBox)
        {
            images.Remove(image);
            flowLayoutPanel.Controls.Remove(pictureBox);
            pictureBox.Dispose(); // Dispose the PictureBox to release resources
        }




        private ContextMenuStrip CreateContextMenu(Bitmap image, PictureBox pictureBox)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem copyItem = new ToolStripMenuItem("Copy to Clipboard");
            ToolStripMenuItem saveItem = new ToolStripMenuItem("Save Image");
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete Image");
            ToolStripMenuItem viewItem = new ToolStripMenuItem("View Image in Full");

            copyItem.Click += (sender, e) => CopyToClipboard(image);
            saveItem.Click += (sender, e) => SaveImage(image);
            deleteItem.Click += (sender, e) => RemoveImage(image, pictureBox);
            viewItem.Click += (sender, e) => ViewItem(image);

            contextMenu.Items.Add(copyItem);
            contextMenu.Items.Add(saveItem);
            contextMenu.Items.Add(deleteItem);
            contextMenu.Items.Add(viewItem);
            return contextMenu;
        }

        private void CopyToClipboard(Bitmap image)
        {
            Clipboard.SetImage(new Bitmap(image));
        }

        private void ViewItem(Bitmap image)
        {
            ImageFullViewForm viewForm = new ImageFullViewForm(image);
            viewForm.Show();
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
