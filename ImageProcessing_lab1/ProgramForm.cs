using ImageProcessing_lab1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessing_lab1
{
    public enum Mode
    {
        Summ,
        Mult,
        Avg,
        Min,
        Max,
        Cirle,
        Square,
        Rectangle
    }

    public partial class ProgramForm : Form
    {
        public Mode Mode { get { return (Mode)comboBoxModes.SelectedIndex; } set { comboBoxModes.SelectedIndex = (int)value; } }
        public Bitmap FirstImage { get { return (Bitmap)pictureBox1.Image; } set { pictureBox1.Image = value; } }
        public Bitmap SecondImage { get { return (Bitmap)pictureBox2.Image; } set { pictureBox2.Image = value; } }
        public Bitmap ResultImage { get { return (Bitmap)pictureBoxResult.Image; } set { pictureBoxResult.Image = value; } }
        public bool[] SelectedChannels { get { return new bool[] { checkBoxRed.Checked,checkBoxGreen.Checked,checkBoxBlue.Checked}; } }

        public ProgramForm()
        {
            InitializeComponent();
            var itemsNames = new string[]
            {
                "Попиксельная сумма",
                "Произведение",
                "Среднее-арифметическое",
                "Минимум",
                "Максимум",
                "Маска(Круг)",
                "Маска(Квадрат)",
                "Маска(Прямоугольник)",
            };
            comboBoxModes.Items.AddRange(itemsNames);
            Mode = Mode.Summ;
        }

        private void FirstPictureClicked(object sender, EventArgs e)
        {
            var image = SelectImage();
            FirstImage = image;
            Process();
        }

        private void SecondPictureClicked(object sender, EventArgs e)
        {
            var image = SelectImage();
            SecondImage = image;
            Process();
        }

        private void ChannelChanged(object sender, EventArgs e) => Process();

        private void ModeChanged(object sender, EventArgs e) => Process();

        private void ThirdPictureClicked(object sender, EventArgs e)
        {
            SaveImage();
        }

        private void Process()
        {
            if (FirstImage == null) return;
            if(SecondImage == null && !ImageProcessing.isMaskMode(Mode)) 
            {
                ResultImage = FirstImage;
                ResizeResultImage(FirstImage);
                return;
            }
            var result = ImageProcessing.Process(FirstImage, SecondImage, SelectedChannels, Mode);
            ResultImage = result;
            ResizeResultImage(result);
        }

        private void ResizeResultImage(Bitmap image) => pictureBoxResult.Size = new Size(pictureBoxResult.Width, (int)((float)image.Height / (float)image.Width * pictureBoxResult.Width));

        private Bitmap SelectImage() 
        {
            Bitmap image = null;
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (image != null)
                {
                    pictureBox1.Image = null;
                    image.Dispose();
                }

                image = new Bitmap(openFileDialog.FileName);
            }
            return image;
        }

        private void SaveImage()
        {
            using SaveFileDialog saveFileFialog = new SaveFileDialog();
            saveFileFialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileFialog.Filter = "Картинки (png, jpg, bmp, gif) |*.png;*.jpg;*.bmp;*.gif|All files (*.*)|*.*";
            saveFileFialog.RestoreDirectory = true;

            if (saveFileFialog.ShowDialog() == DialogResult.OK)
            {
                if (ResultImage != null)
                {
                    ResultImage.Save(saveFileFialog.FileName);
                }
            }
        }
 
    }
}
