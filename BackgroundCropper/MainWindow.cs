using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace BackgroundCropper
{
    /// <summary>
    /// windows screen
    /// </summary>
    public partial class MainWindow : Form
    {
        public class NoRoundingRectangle
        {
            public NoRoundingRectangle(double x, double y, double width, double height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public Rectangle RoundedRectangle()
            {
                return new Rectangle(RoundedX, RoundedY, RoundedWidth, RoundedHeight);
            }

            public double X { get; set; }
            public int RoundedX { get { return (int)Math.Round(X); }}

            public double Y { get; set; }
            public int RoundedY { get { return (int)Math.Round(Y); } }

            public double Width { get; set; }
            public int RoundedWidth { get { return (int)Math.Round(Width); } }

            public double Height { get; set; }
            public int RoundedHeight { get { return (int)Math.Round(Height); } }
        }

        private bool m_MainWindowResizeInProgress;
        private readonly Brush m_CropRectangleBrush;
        private double m_ImageScaleFactor = 1.0;
        private double m_AspectRatio = 1.0;
        private Image m_OriginalImage = null;
        private readonly NoRoundingRectangle m_ResizedImageLocation = new NoRoundingRectangle(0, 0, 0, 0);
        private readonly NoRoundingRectangle m_ActualCropRectangleLocation = new NoRoundingRectangle(0, 0, 0, 0);
        private readonly NoRoundingRectangle m_MoveResizeRectangleLocation = new NoRoundingRectangle(0, 0, 0, 0);
        private const int m_ResizeBorderWidth = 10;
        private const int m_MinimumMoveAreaWidth = 400;
        private Size m_OriginalImageSize = new Size(0, 0);
        private NoRoundingRectangle m_CropRectangle = new NoRoundingRectangle(0, 0, 0, 0);
        private bool m_AllowHeightTopResize;
        private bool m_AllowHeightBottomResize;
        private bool m_AllowWidthLeftResize;
        private bool m_AllowWidthRightResize;
        private bool m_AllowMove;
        private bool m_ResizeOutOfBounds;

        private bool m_MouseDown = false;
        private Point m_MouseLocation = new Point(0, 0);

        public MainWindow()
        {
            m_CropRectangleBrush = new SolidBrush(Color.FromArgb(128,Color.Yellow));

            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            //set the selected aspect ratio to the current screen aspect ratio
            int gcd = GetGreatestCommonDivisor(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var aspectRatio = string.Format("{0}:{1}", Screen.PrimaryScreen.Bounds.Width / gcd, Screen.PrimaryScreen.Bounds.Height / gcd);
            m_AspectRatio = (double)Screen.PrimaryScreen.Bounds.Width / (double)Screen.PrimaryScreen.Bounds.Height;

            if (!m_SelectedAspectRatio.Items.Contains(aspectRatio))
            {
                //if not in list yet, add it
                m_SelectedAspectRatio.Items.Add(aspectRatio);
            }

            //select the aspect ration of this monitor
            m_SelectedAspectRatio.SelectedItem = aspectRatio;
        }

        static int GetGreatestCommonDivisor(int a, int b)
        {
            return b == 0 ? a : GetGreatestCommonDivisor(b, a % b);
        }

        private void ImageToCrop_Paint(object sender, PaintEventArgs e)
        {
            //draw the selection rectangle for the cropping
            //http://forums.devx.com/showthread.php?t=160827

            if (m_SourceFileImageBox.Image != null)
            {
                //paint the crop rectangle on the picture
                var resizedCropRect = new Rectangle((int)((m_CropRectangle.X * m_ImageScaleFactor) + m_ResizedImageLocation.X),
                                                      (int)((m_CropRectangle.Y * m_ImageScaleFactor) + m_ResizedImageLocation.Y),
                                                      (int)(m_CropRectangle.Width * m_ImageScaleFactor),
                                                      (int)(m_CropRectangle.Height * m_ImageScaleFactor));
                e.Graphics.FillRectangle(m_CropRectangleBrush, resizedCropRect);
            }
        }

        private NoRoundingRectangle CalcLargestPossibleCropRect(Image image)
        {
            //first check if we take the max width, if the height still fits in the image
            double rectWidth = image.Width;
            double rectHeight = (rectWidth/m_AspectRatio);

            if (rectHeight <= image.Height)
            {
                //in the middle of the height
                return new NoRoundingRectangle(0, (image.Height - rectHeight) / 2.0, rectWidth, rectHeight);
            }

            //with max width does not work, so use the max height
            rectHeight = image.Height;
            rectWidth = (rectHeight*m_AspectRatio);

            //in the middle of the width
            return new NoRoundingRectangle((image.Width - rectWidth) / 2.0, 0, rectWidth, rectHeight);
        }

        private void CropToSelectionButton_Click(object sender, EventArgs e)
        {
            CropAndSaveImage(m_OutputFolderTextBox.Text, false, true);
        }

        private string CropAndSaveImage(string savePath, bool saveAsBmp, bool showMessage)
        {
            //http://stackoverflow.com/questions/734930/how-to-crop-an-image-using-c
            //http://www.dreamincode.net/code/snippet1987.htm
            //http://www.switchonthecode.com/tutorials/csharp-tutorial-image-editing-saving-cropping-and-resizing
            if (!string.IsNullOrEmpty(m_SourceImageFileTextBox.Text))
            {
                Bitmap target = new Bitmap(m_CropRectangle.RoundedWidth, m_CropRectangle.RoundedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(m_OriginalImage, new Rectangle(0, 0, target.Width, target.Height), m_CropRectangle.RoundedRectangle(), GraphicsUnit.Pixel);
                }

                //and store the cropped image
                var originalImageFilename = m_SourceImageFileTextBox.Text;
                var croppedImagePath = savePath;
                var fileExtension = saveAsBmp ? ".bmp" : Path.GetExtension(originalImageFilename);

                var newCroppedImageFilename = Path.GetFileNameWithoutExtension(originalImageFilename) + " - cropped" + fileExtension;
                var newOutputFilename = Path.Combine(croppedImagePath, newCroppedImageFilename);

                //make sure we do not override other files
                var counter = 0;
                while (File.Exists(newOutputFilename))
                {
                    counter++;
                    newCroppedImageFilename = Path.GetFileNameWithoutExtension(originalImageFilename) + " - cropped (" + counter + ")" + fileExtension;
                    newOutputFilename = Path.Combine(croppedImagePath, newCroppedImageFilename);
                }

                if (saveAsBmp)
                {
                    target.Save(newOutputFilename, ImageFormat.Bmp);
                }
                else
                {
                    target.Save(newOutputFilename, m_OriginalImage.RawFormat);
                }

                if (showMessage)
                {
                    MessageBox.Show(this, "Cropped image saved as \"" + newOutputFilename + "\"", @"Cropped image saved",
                                    MessageBoxButtons.OK, MessageBoxIcon.None);
                }

                return newOutputFilename;
            }

            return null;
        }

        private void SelectSourceImageButton_Click(object sender, EventArgs e)
        {
            var result = m_OpenSourceImageDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                InitImage(m_OpenSourceImageDialog.FileName);
            }
        }

        private void InitImage(string imageFilename)
        {
            m_SourceImageFileTextBox.Text = imageFilename;

            //set the output folder the same as the folder of the input image (if not set yet)
            if (string.IsNullOrEmpty(m_OutputFolderTextBox.Text))
            {
                m_OutputFolderTextBox.Text = Path.GetDirectoryName(imageFilename);
            }

            //reset the crop rectangle
            ResetCropRectangle();

            //scale the image so it fits in the imagebox (and store the scale factor and original image size)
            m_OriginalImage = Image.FromFile(imageFilename);
            m_OriginalImageSize = new Size(m_OriginalImage.Width, m_OriginalImage.Height);
            m_CropRectangle = CalcLargestPossibleCropRect(m_OriginalImage);
            var scaledImage = ScaleImageForPictureBox(m_SourceFileImageBox, m_OriginalImage);

            UpdateActualCropRectangleLocation();

            //and show this scaled image
            m_SourceFileImageBox.Image = scaledImage;
        }

        private void UpdateActualCropRectangleLocation()
        {
            //store the actual location on the screen of the crop rect
            m_ActualCropRectangleLocation.X = (m_CropRectangle.X * m_ImageScaleFactor) + m_ResizedImageLocation.X;
            m_ActualCropRectangleLocation.Y = (m_CropRectangle.Y * m_ImageScaleFactor) + m_ResizedImageLocation.Y;
            m_ActualCropRectangleLocation.Width = (m_CropRectangle.Width * m_ImageScaleFactor);
            m_ActualCropRectangleLocation.Height = (m_CropRectangle.Height * m_ImageScaleFactor);

            m_MoveResizeRectangleLocation.X = m_ActualCropRectangleLocation.X + m_ResizeBorderWidth;
            m_MoveResizeRectangleLocation.Y = m_ActualCropRectangleLocation.Y + m_ResizeBorderWidth;
            /*m_MoveResizeRectangleLocation.Width = m_ActualCropRectangleLocation.Width - (m_ResizeBorderWidth * 2.0);
            m_MoveResizeRectangleLocation.Height = m_ActualCropRectangleLocation.Height - (m_ResizeBorderWidth * 2.0);*/
            m_MoveResizeRectangleLocation.Width = m_ActualCropRectangleLocation.Width - (m_ResizeBorderWidth);
            m_MoveResizeRectangleLocation.Height = m_ActualCropRectangleLocation.Height - (m_ResizeBorderWidth);
        }

        #region PictureBox image scaling
        //Resize the image
        private Image ScaleImageForPictureBox(PictureBox pb, Image originalImage)
        {
            //calculate the size of the image
            Size imgSize = GetScaledImageDimensions(originalImage.Width, originalImage.Height, pb.Width, pb.Height);

            //calc the offset to put this image in the center of the picturebox
            if (imgSize.Width < pb.Width)
            {
                m_ResizedImageLocation.X = (pb.Width - imgSize.Width) / 2.0;
                m_ResizedImageLocation.Y = 0;
            }
            if (imgSize.Height < pb.Height)
            {
                m_ResizedImageLocation.X = 0;
                m_ResizedImageLocation.Y = (pb.Height - imgSize.Height) / 2.0;
            }

            m_ResizedImageLocation.Width = imgSize.Width;
            m_ResizedImageLocation.Height = imgSize.Height;

            //create a new Bitmap with the proper dimensions
            Bitmap scaledImg = new Bitmap(originalImage, imgSize.Width, imgSize.Height);

            //create a new Graphics object from the image
            Graphics gfx = Graphics.FromImage(scaledImg);

            //clean up the image (take care of any image loss from resizing)
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //return the scaled image
            return scaledImg;
        }

        //Generate new image dimensions
        public Size GetScaledImageDimensions(int currW, int currH, int destW, int destH)
        {
            //string for holding layout
            string layout;

            //determine if it's Portrait or Landscape
            if (currH > currW) layout = "portrait";
            else layout = "landscape";

            switch (layout.ToLower())
            {
                case "portrait":
                    //calculate multiplier on heights
                    if (destH > destW)
                    {
                        m_ImageScaleFactor = (double)destW / (double)currW;
                    }

                    else
                    {
                        m_ImageScaleFactor = (double)destH / (double)currH;
                    }
                    break;
                case "landscape":
                    //calculate multiplier on widths
                    if (destH > destW)
                    {
                        m_ImageScaleFactor = (double)destW / (double)currW;
                    }

                    else
                    {
                        m_ImageScaleFactor = (double)destH / (double)currH;
                    }
                    break;
            }

            //return the new image dimensions
            return new Size((int)(currW * m_ImageScaleFactor), (int)(currH * m_ImageScaleFactor));
        }
        #endregion

        #region crop rectangle moving
        private void SourceFileImageBox_MouseDown(object sender, MouseEventArgs e)
        {
            m_MouseLocation = e.Location;
            m_MouseDown = true;
            m_ResizeOutOfBounds = false;
        }

        private void SourceFileImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_MouseDown && m_AllowMove)
            {
                CalculateCropMove(e);
            }
            else if (m_MouseDown && 
                (m_AllowHeightTopResize || m_AllowHeightBottomResize || m_AllowWidthLeftResize || m_AllowWidthRightResize))
            {
                CalculateCropResize(e);
            }

            //store the new mouse location
            m_MouseLocation = e.Location;

            if (!m_MouseDown)
            {
                //do not change mode when mouse button is pressed!
                SetCorrectIconAndMode(e);
            }
        }

        private void CalculateCropMove(MouseEventArgs e)
        {
            //calc the movement
            int moveX = m_MouseLocation.X - e.Location.X;
            int moveY = m_MouseLocation.Y - e.Location.Y;

            //adapt the crop rect location
            m_CropRectangle.X -= (moveX / m_ImageScaleFactor);
            m_CropRectangle.Y -= (moveY / m_ImageScaleFactor);

            //make sure it does not go out of bounds
            if (m_CropRectangle.X < 0)
            {
                m_CropRectangle.X = 0;
            }
            if (m_CropRectangle.Y < 0)
            {
                m_CropRectangle.Y = 0;
            }
            if (m_CropRectangle.X + m_CropRectangle.Width > m_OriginalImageSize.Width)
            {
                m_CropRectangle.X = m_OriginalImageSize.Width - m_CropRectangle.Width;
            }
            if (m_CropRectangle.Y + m_CropRectangle.Height > m_OriginalImageSize.Height)
            {
                m_CropRectangle.Y = m_OriginalImageSize.Height - m_CropRectangle.Height;
            }

            //update the actual crop rect
            UpdateActualCropRectangleLocation();

            //redraw
            m_SourceFileImageBox.Invalidate();
        }

        private void CalculateCropResize(MouseEventArgs e)
        {
            if (m_AllowWidthLeftResize)
            {
                CalculateLeftCropResize(e);
            }

            if (m_AllowHeightTopResize)
            {
                CalculateTopCropResize(e);
            }

            if (m_AllowWidthRightResize)
            {
                CalculateRightCropResize(e);
            }

            if (m_AllowHeightBottomResize)
            {
                CalculateBottomCropResize(e);
            }

            //redraw
            m_SourceFileImageBox.Invalidate();
        }

        private void CalculateLeftCropResize(MouseEventArgs e)
        {
            //adapt what we actually see on the screen to the new mouse location to avoid accumulation of rounding errors, then calc the actual crop rect from that again
            var diffX = m_ActualCropRectangleLocation.X - e.Location.X;
            m_ActualCropRectangleLocation.X = e.Location.X;     //change location to new one
            m_ActualCropRectangleLocation.Width += diffX;       //change width to new one
            m_ActualCropRectangleLocation.Height = (m_ActualCropRectangleLocation.Width / m_AspectRatio);

            var cropNewX = ((m_ActualCropRectangleLocation.X - m_ResizedImageLocation.X) / m_ImageScaleFactor);
            var cropDiffX = m_CropRectangle.X - cropNewX;
            var cropNewWidth = m_CropRectangle.Width + cropDiffX;
            var cropNewHeight = cropNewWidth / m_AspectRatio;

            if (cropNewX < 0)
            {
                if (!m_ResizeOutOfBounds)
                {
                    m_ResizeOutOfBounds = true;
                    //to much to the left
                    cropDiffX = m_CropRectangle.X;
                    m_CropRectangle.X = 0;
                    m_CropRectangle.Width += cropDiffX;
                    m_CropRectangle.Height = (m_CropRectangle.Width / m_AspectRatio);

                    //recalc all other rects from the crop rect
                    UpdateActualCropRectangleLocation();
                }
            }
            else if (cropNewWidth < (m_ResizeBorderWidth * 2) + m_MinimumMoveAreaWidth)
            {
                if (!m_ResizeOutOfBounds)
                {
                    m_ResizeOutOfBounds = true;
                    var currentRightSideOfRect = m_CropRectangle.X + m_CropRectangle.Width;

                    //rect gets too small
                    m_CropRectangle.Width = (m_ResizeBorderWidth * 2) + m_MinimumMoveAreaWidth;
                    m_CropRectangle.Height = (m_CropRectangle.Width / m_AspectRatio);
                    m_CropRectangle.X = currentRightSideOfRect - m_CropRectangle.Width;

                    //recalc all other rects from the crop rect
                    UpdateActualCropRectangleLocation();
                }
            }
            else if (m_CropRectangle.Y + cropNewHeight > m_OriginalImageSize.Height)
            {
                if (!m_ResizeOutOfBounds)
                {
                    m_ResizeOutOfBounds = true;
                    var currentRightSideOfRect = m_CropRectangle.X + m_CropRectangle.Width;

                    //gets too high, will go out of bounds at bottom of image
                    m_CropRectangle.Height = m_OriginalImageSize.Height - m_CropRectangle.Y;
                    m_CropRectangle.Width = m_CropRectangle.Height * m_AspectRatio;
                    m_CropRectangle.X = currentRightSideOfRect - m_CropRectangle.Width;

                    //recalc all other rects from the crop rect
                    UpdateActualCropRectangleLocation();
                }
            }
            else
            {
                m_ResizeOutOfBounds = false;

                //not out of bounds, just resize...
                m_CropRectangle.X = cropNewX;
                m_CropRectangle.Width = cropNewWidth;
                m_CropRectangle.Height = (m_CropRectangle.Width / m_AspectRatio);

                m_MoveResizeRectangleLocation.X = m_ActualCropRectangleLocation.X + m_ResizeBorderWidth;
                m_MoveResizeRectangleLocation.Y = m_ActualCropRectangleLocation.Y + m_ResizeBorderWidth;
                m_MoveResizeRectangleLocation.Width = m_ActualCropRectangleLocation.Width - (m_ResizeBorderWidth * 2.0);
                m_MoveResizeRectangleLocation.Height = m_ActualCropRectangleLocation.Height - (m_ResizeBorderWidth * 2.0);
            }
        }

        private void CalculateTopCropResize(MouseEventArgs e)
        {
            //adapt what we actually see on the screen to the new mouse location to avoid accumulation of rounding errors, then calc the actual crop rect from that again
            var diffY = m_ActualCropRectangleLocation.Y - e.Location.Y;
            m_ActualCropRectangleLocation.Y = e.Location.Y;     //change location to new one
            m_ActualCropRectangleLocation.Height += diffY;       //change height to new one
            m_ActualCropRectangleLocation.Width = (m_ActualCropRectangleLocation.Height * m_AspectRatio);

            var cropNewY = ((m_ActualCropRectangleLocation.Y - m_ResizedImageLocation.Y) / m_ImageScaleFactor);
            var cropDiffY = m_CropRectangle.Y - cropNewY;
            var cropNewHeight = m_CropRectangle.Height + cropDiffY;
            var cropNewWidth = cropNewHeight * m_AspectRatio;

            if (cropNewY < 0)
            {
                if (!m_ResizeOutOfBounds)
                {
                    m_ResizeOutOfBounds = true;
                    //to much to the top
                    cropDiffY = m_CropRectangle.Y;
                    m_CropRectangle.Y = 0;
                    m_CropRectangle.Height += cropDiffY;
                    m_CropRectangle.Width = (m_CropRectangle.Height * m_AspectRatio);

                    //recalc all other rects from the crop rect
                    UpdateActualCropRectangleLocation();
                }
            }
            else if (cropNewWidth < (m_ResizeBorderWidth * 2) + m_MinimumMoveAreaWidth)
            {
                if (!m_ResizeOutOfBounds)
                {
                    m_ResizeOutOfBounds = true;
                    var currentBottomSideOfRect = m_CropRectangle.Y + m_CropRectangle.Height;

                    //rect gets too small
                    m_CropRectangle.Width = (m_ResizeBorderWidth * 2) + m_MinimumMoveAreaWidth;
                    m_CropRectangle.Height = (m_CropRectangle.Width / m_AspectRatio);
                    m_CropRectangle.Y = currentBottomSideOfRect - m_CropRectangle.Height;

                    //recalc all other rects from the crop rect
                    UpdateActualCropRectangleLocation();
                }
            }
            else if (m_CropRectangle.X + cropNewWidth > m_OriginalImageSize.Width)
            {
                if (!m_ResizeOutOfBounds)
                {
                    m_ResizeOutOfBounds = true;
                    var currentBottomSideOfRect = m_CropRectangle.Y + m_CropRectangle.Height;

                    //gets too wide, will go out of bounds at right of image
                    m_CropRectangle.Width = m_OriginalImageSize.Width - m_CropRectangle.X;
                    m_CropRectangle.Height = m_CropRectangle.Width / m_AspectRatio;
                    m_CropRectangle.Y = currentBottomSideOfRect - m_CropRectangle.Height;

                    //recalc all other rects from the crop rect
                    UpdateActualCropRectangleLocation();
                }
            }
            else
            {
                m_ResizeOutOfBounds = false;

                //not out of bounds, just resize...
                m_CropRectangle.Y = cropNewY;
                m_CropRectangle.Width = cropNewWidth;
                m_CropRectangle.Height = cropNewHeight;

                m_MoveResizeRectangleLocation.X = m_ActualCropRectangleLocation.X + m_ResizeBorderWidth;
                m_MoveResizeRectangleLocation.Y = m_ActualCropRectangleLocation.Y + m_ResizeBorderWidth;
                m_MoveResizeRectangleLocation.Width = m_ActualCropRectangleLocation.Width - (m_ResizeBorderWidth * 2.0);
                m_MoveResizeRectangleLocation.Height = m_ActualCropRectangleLocation.Height - (m_ResizeBorderWidth * 2.0);
            }
        }

        private void CalculateRightCropResize(MouseEventArgs e)
        {
            /*
            //adapt what we actually see on the screen to the new mouse location to avoid accumulation of rounding errors, then calc the actual crop rect from that again
            var diffWidth = m_ActualCropRectangleLocation.X + m_ActualCropRectangleLocation.Width - e.Location.X;
            m_ActualCropRectangleLocation.Width -= diffWidth;       //change width to new one
            m_ActualCropRectangleLocation.Height = (m_ActualCropRectangleLocation.Width / m_AspectRatio);

            var cropNewWidth = m_CropRectangle.Width - (diffWidth / m_ImageScaleFactor);
            var cropNewHeight = cropNewWidth / m_AspectRatio;

            if (cropNewX < 0)
            {
                if (!m_ResizeOutOfBounds)
                {
                    m_ResizeOutOfBounds = true;
                    //to much to the left
                    cropDiffX = m_CropRectangle.X;
                    m_CropRectangle.X = 0;
                    m_CropRectangle.Width += cropDiffX;
                    m_CropRectangle.Height = (m_CropRectangle.Width / m_AspectRatio);

                    //recalc all other rects from the crop rect
                    UpdateActualCropRectangleLocation();
                }
            }
            else if (cropNewWidth < (m_ResizeBorderWidth * 2) + m_MinimumMoveAreaWidth)
            {
                if (!m_ResizeOutOfBounds)
                {
                    m_ResizeOutOfBounds = true;
                    var currentRightSideOfRect = m_CropRectangle.X + m_CropRectangle.Width;

                    //rect gets too small
                    m_CropRectangle.Width = (m_ResizeBorderWidth * 2) + m_MinimumMoveAreaWidth;
                    m_CropRectangle.Height = (m_CropRectangle.Width / m_AspectRatio);
                    m_CropRectangle.X = currentRightSideOfRect - m_CropRectangle.Width;

                    //recalc all other rects from the crop rect
                    UpdateActualCropRectangleLocation();
                }
            }
            else if (m_CropRectangle.Y + cropNewHeight > m_OriginalImageSize.Height)
            {
                if (!m_ResizeOutOfBounds)
                {
                    m_ResizeOutOfBounds = true;
                    var currentRightSideOfRect = m_CropRectangle.X + m_CropRectangle.Width;

                    //gets too high, will go out of bounds at bottom of image
                    m_CropRectangle.Height = m_OriginalImageSize.Height - m_CropRectangle.Y;
                    m_CropRectangle.Width = m_CropRectangle.Height * m_AspectRatio;
                    m_CropRectangle.X = currentRightSideOfRect - m_CropRectangle.Width;

                    //recalc all other rects from the crop rect
                    UpdateActualCropRectangleLocation();
                }
            }
            else
            {
                m_ResizeOutOfBounds = false;

                //not out of bounds, just resize...
                m_CropRectangle.Width = cropNewWidth;
                m_CropRectangle.Height = (m_CropRectangle.Width / m_AspectRatio);

                m_MoveResizeRectangleLocation.X = m_ActualCropRectangleLocation.X + m_ResizeBorderWidth;
                m_MoveResizeRectangleLocation.Y = m_ActualCropRectangleLocation.Y + m_ResizeBorderWidth;
                m_MoveResizeRectangleLocation.Width = m_ActualCropRectangleLocation.Width - (m_ResizeBorderWidth * 2.0);
                m_MoveResizeRectangleLocation.Height = m_ActualCropRectangleLocation.Height - (m_ResizeBorderWidth * 2.0);
            }*/
        }

        private void CalculateBottomCropResize(MouseEventArgs e)
        {
            ;
        }

        private void SetCorrectIconAndMode(MouseEventArgs e)
        {
            //set correct mouse icon
            if (e.Location.X > m_MoveResizeRectangleLocation.X && e.Location.X < m_MoveResizeRectangleLocation.X + m_MoveResizeRectangleLocation.Width &&
                e.Location.Y > m_MoveResizeRectangleLocation.Y && e.Location.Y < m_MoveResizeRectangleLocation.Y + m_MoveResizeRectangleLocation.Height)
            {
                //show move cursor
                m_SourceFileImageBox.Cursor = Cursors.SizeAll;
                m_AllowMove = true;
                m_AllowHeightTopResize = false;
                m_AllowHeightBottomResize = false;
                m_AllowWidthLeftResize = false;
                m_AllowWidthRightResize = false;
            }
            else if (e.Location.X > m_ActualCropRectangleLocation.X && e.Location.X < m_ActualCropRectangleLocation.X + m_ActualCropRectangleLocation.Width &&
                e.Location.Y > m_ActualCropRectangleLocation.Y && e.Location.Y < m_ActualCropRectangleLocation.Y + m_ActualCropRectangleLocation.Height)
            {
                //find out which resize cursor
                if (e.Location.X <= m_MoveResizeRectangleLocation.X || e.Location.X >= m_MoveResizeRectangleLocation.X + m_MoveResizeRectangleLocation.Width)
                {
                    //show resize cursor
                    m_SourceFileImageBox.Cursor = Cursors.SizeWE;
                    m_AllowHeightTopResize = false;
                    m_AllowHeightBottomResize = false;
                    m_AllowWidthLeftResize = e.Location.X <= m_MoveResizeRectangleLocation.X;
                    m_AllowWidthRightResize = e.Location.X >= m_MoveResizeRectangleLocation.X + m_MoveResizeRectangleLocation.Width;
                }
                else
                {
                    //show resize cursor
                    m_SourceFileImageBox.Cursor = Cursors.SizeNS;
                    m_AllowHeightTopResize = e.Location.Y <= m_MoveResizeRectangleLocation.Y;
                    m_AllowHeightBottomResize = e.Location.Y >= m_MoveResizeRectangleLocation.Y + m_MoveResizeRectangleLocation.Height;
                    m_AllowWidthLeftResize = false;
                    m_AllowWidthRightResize = false;
                }

                m_AllowMove = false;
            }
            else
            {
                //hide move cursor
                m_SourceFileImageBox.Cursor = Cursors.Default;
                m_AllowMove = false;
                m_AllowHeightTopResize = false;
                m_AllowHeightBottomResize = false;
                m_AllowWidthLeftResize = false;
                m_AllowWidthRightResize = false;
            }
        }

        private void SourceFileImageBox_MouseUp(object sender, MouseEventArgs e)
        {
            m_MouseDown = false;
        }
        #endregion

        private void ResetCropRectangle()
        {
            m_CropRectangle.X = 0;
            m_CropRectangle.Y = 0;
            m_CropRectangle.Width = 0;
            m_CropRectangle.Height = 0;

            m_ActualCropRectangleLocation.X = 0;
            m_ActualCropRectangleLocation.Y = 0;
            m_ActualCropRectangleLocation.Width = 0;
            m_ActualCropRectangleLocation.Height = 0;
        }

        private void SelectedAspectRatio_SelectedIndexChanged(object sender, EventArgs e)
        {
            //redraw the crop rect for the new aspect ratio
            if (m_OriginalImage != null)
            {
                //calc new aspect ratio
                m_AspectRatio = CalcAspectRatioFromDropDown();
                m_CropRectangle = CalcLargestPossibleCropRect(m_OriginalImage);
                m_SourceFileImageBox.Invalidate();
            }
        }

        private double CalcAspectRatioFromDropDown()
        {
            var selectedAspectRatio = m_SelectedAspectRatio.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedAspectRatio))
            {
                var splittedAspectRatio = selectedAspectRatio.Split(':');
                var aspectRatioWidth = int.Parse(splittedAspectRatio[0]);
                var aspectRatioHeight = int.Parse(splittedAspectRatio[1]);
                return (double) aspectRatioWidth/(double) aspectRatioHeight;
            }

            return 1.0;
        }

        private void SelectOutputFolderButton_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                m_OutputFolderTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void SetCropAsBackground_Click(object sender, EventArgs e)
        {
            var tempFolder = Path.GetTempPath();
            //save the file as bmp, windows xp only sets wallpapers that are bmp!
            var savedCroppedImageFilename = CropAndSaveImage(tempFolder, true, false);

            if (!string.IsNullOrEmpty(savedCroppedImageFilename))
            {
                SetWallpaper(savedCroppedImageFilename);
            }
        }

        #region Set Wallpaper
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);

        private const uint SPI_SETDESKWALLPAPER = 20;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDWININICHANGE = 0x02;

        private static void SetWallpaper(String path)
        {
            //set wallpaper style to stretch
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (key != null)
            {
                key.SetValue(@"WallpaperStyle", "2");
                key.SetValue(@"TileWallpaper", "0");
                key.Close();
            }

            //and set the path to the correct file
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        #endregion

        private void MainWindow_ResizeBegin(object sender, EventArgs e)
        {
            m_MainWindowResizeInProgress = true;
        }

        private void MainWindow_ResizeEnd(object sender, EventArgs e)
        {
            m_MainWindowResizeInProgress = false;

            if (!string.IsNullOrEmpty(m_SourceImageFileTextBox.Text))
            {
                //recalc and redraw all after window resize
                InitImage(m_SourceImageFileTextBox.Text);
            }
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            if (!m_MainWindowResizeInProgress)
            {
                if (!string.IsNullOrEmpty(m_SourceImageFileTextBox.Text))
                {
                    //recalc and redraw all after window resize
                    InitImage(m_SourceImageFileTextBox.Text);
                }
            }
        }
    }
}
