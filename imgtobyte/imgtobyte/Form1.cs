using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace imgtobyte
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static void Bytebeat(int[] byten, int bytenl, int samplr, double dur, bool play = true)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                writer.Write("RIFF".ToCharArray());  // chunk id
                writer.Write((UInt32)0);             // chunk size
                writer.Write("WAVE".ToCharArray());  // format

                writer.Write("fmt ".ToCharArray());  // chunk id
                writer.Write((UInt32)16);            // chunk size
                writer.Write((UInt16)1);             // audio format

                var channels = 1;
                var sample_rate = samplr;
                var bits_per_sample = 8;

                writer.Write((UInt16)channels);
                writer.Write((UInt32)sample_rate);
                writer.Write((UInt32)(sample_rate * channels * bits_per_sample / 8)); // byte rate
                writer.Write((UInt16)(channels * bits_per_sample / 8));               // block align
                writer.Write((UInt16)bits_per_sample);

                writer.Write("data".ToCharArray());

                var seconds = dur;

                var data = new byte[(int)(sample_rate * seconds)];

                for (var t = 0; t < data.Length; t++)
                    data[t] = (byte)(
                        byten[t%bytenl]
                        );

                writer.Write((UInt32)(data.Length * channels * bits_per_sample / 8));

                foreach (var elt in data) writer.Write(elt);

                writer.Seek(4, SeekOrigin.Begin);                     // seek to header chunk size field
                writer.Write((UInt32)(writer.BaseStream.Length - 8)); // chunk size

                stream.Seek(0, SeekOrigin.Begin);
                if (play = true)
                {
                    new SoundPlayer(stream).Play();
                }
                else { new SoundPlayer(stream).Stop(); }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            // Load the image
            Bitmap image = new Bitmap(pictureBox1.Image);

            // Create an int array to store the pixel values
            int[] pixels = new int[image.Width * image.Height];

            // Loop through each pixel and store its value in the array
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    int index = y * image.Width + x;
                    pixels[index] = color.ToArgb();
                }
            }
            Bytebeat(pixels, pixels.Length, (int)numericUpDown1.Value,(double)numericUpDown2.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Load the image
            Bitmap image = new Bitmap(pictureBox1.Image);

            // Create an int array to store the pixel values
            int[] pixels = new int[image.Width * image.Height];

            // Loop through each pixel and store its value in the array
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    int index = y * image.Width + x;
                    pixels[index] = color.ToArgb();
                }
            }

            Random random = new Random();
            int width = pictureBox2.Width;
            int height = pictureBox2.Height;
            int[] pixelArray = new int[width * height];
            for (int t = 0; t < pixelArray.Length; t++)
            {

                pixelArray[t] = (int)(
                    pixels[t % pixels.Length]);
                    
            }

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            BitmapData bmpData = bmp.LockBits(
                                new Rectangle(0, 0, bmp.Width, bmp.Height),
                                ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(pixelArray, 0, bmpData.Scan0, pixelArray.Length);

            bmp.UnlockBits(bmpData);
            pictureBox2.Image = bmp;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int[] empty = { 0 };
            Bytebeat(empty, 0, 0, 0, false);
        }
    }
}
