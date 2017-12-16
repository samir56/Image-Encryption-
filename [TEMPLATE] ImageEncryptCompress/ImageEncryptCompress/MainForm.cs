using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix,encryptedimg , decryptedmg;
        

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
           double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value ;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }
        public string toBinaryString(string s)
        {
            string res = "";
            for (int i = 0; i < s.Length; i++)
            {
                res += bytetobinary(s[i]);
            }
            return res;
        }
        public RGBPixel[,] encryptOrdecrypt(RGBPixel[,] ImageMatrix)
        {
            int width = ImageOperations.GetWidth(ImageMatrix);
            string seed;
            bool useAlphaPassword = false;
            if (checkBox1.Checked == true)
            {
                useAlphaPassword = true;
            }
            if (useAlphaPassword)
                seed = toBinaryString(textBox2.Text);
            else seed = textBox2.Text;
            int tapIndex = int.Parse(textBox1.Text);
            int height = ImageOperations.GetHeight(ImageMatrix);
            RGBPixel[,] EncryptedImageMatrix = new RGBPixel[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    RGBPixel pixel = ImageMatrix[i, j];
                    for (int k = 0; k < 8; k++)
                        seed = step(seed, tapIndex);
                    int index=seed.Length-8;
                    string e="";
                    for(int empededZ=0;empededZ<(8-seed.Length);empededZ++)e+="0";
                    string xoringseed = seed.Length >= 8 ? seed.Substring(index, 8) : e + seed;
                    string newRed = xor(xoringseed, pixel.red);
                    for (int k = 0; k < 8; k++)
                        seed = step(seed, tapIndex);
                     index = seed.Length - 8;
                     e = "";
                    for (int empededZ = 0; empededZ < (8 - seed.Length); empededZ++) e += "0";
                     xoringseed = seed.Length >= 8 ? seed.Substring(index, 8) : e + seed;
                     string newGreen = xor(xoringseed, pixel.green);
                    for (int k = 0; k < 8; k++)
                        seed = step(seed, tapIndex);
                    index = seed.Length - 8;
                    e = "";
                    for (int empededZ = 0; empededZ < (8 - seed.Length); empededZ++) e += "0";
                    xoringseed = seed.Length >= 8 ? seed.Substring(index, 8) : e + seed;
                    string newBlue = xor(xoringseed, pixel.blue);
                    RGBPixel NewPixel = new RGBPixel();
                    NewPixel.red = binarytobyte(newRed);
                    NewPixel.green = binarytobyte(newGreen);
                    NewPixel.blue = binarytobyte(newBlue);
                    EncryptedImageMatrix[i, j] = NewPixel;

              }
            }
            return EncryptedImageMatrix;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
             if (comboBox1.SelectedItem == "Encrypted")
             {
                 encryptedimg = encryptOrdecrypt(ImageMatrix);
                 ImageOperations.DisplayImage(encryptedimg, pictureBox2);
        
             }
             else if ( comboBox1.SelectedItem == "Decrypted")
             {

                 decryptedmg = encryptOrdecrypt(ImageMatrix);
                 ImageOperations.DisplayImage(decryptedmg, pictureBox2);
                      

             }


        }
        public string xor(string s1, string s2)
        {
            string res = "";
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] == s2[i])
                    res += "0";
                else
                    res += "1";
            }
            return res;
        }
        public byte binarytobyte(string binaryValue)
        {
            double res = 0;
           
            for (int i = binaryValue.Length-1; i >=0 ; i--)
            {
                if (binaryValue[i]=='1')
                res+=Math.Pow(2,7-i);
            }
            return byte.Parse(res.ToString());
        }
        public string bytetobinary(int value)
        {
            string s = "";
            while (value > 0)
            {
                if ((value / 2)*2 != value)
                {
                    s += "1";
                }
                else
                { 
                    s+="0";
                }
                    value /= 2;

            }
            for (int i = s.Length; i < 8; i++)
            {
                s += "0";
            }
            string res = "";
            for (int i = s.Length - 1; i >= 0; i--)
            {
                res += s[i].ToString();
            }
          
            return res;
        }
        public string xor(string s1, byte value)
        {
            string res = "";
            string s2 = bytetobinary(value);
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] == s2[i])
                    res += "0";
                else
                    res += "1";
            }
            return res;
        }
        public string simulateAstep(string s, string xoringResult)
        {
            string res = "";
            for (int i = 1; i < s.Length; i++)
            {
                res += s[i];
            }
            res += xoringResult;
            return res;
        }
        public string ShiftLeft(string s, int n)
        {
            string res = "";
            for (int i = n; i < s.Length; i++)
            {
                res += s[i];
            }
            for (int i = 0; i < n; i++)
            {
                res += 0;
            }
            return res;
        }
        public string ShiftRight(string s, int n)
        {
            string res = "";
            for (int i = 0; i <n; i++)
            {
                res += 0;
            }
            for (int i = 0; i < s.Length-n; i++)
            {
                res += s[i];
            }
            return res;
        }

        public string step(string seed,int tapeIndex)
        {
            
            string lastBit = seed[0].ToString();
            string tapBit = seed[(seed.Length-1) - tapeIndex].ToString();
            string lastBitXorTapBit=xor(lastBit,tapBit);
            string resultSeed = simulateAstep(seed,  lastBitXorTapBit);
            return resultSeed;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*----------------------------------CONSTRUCTION Begin------------------------------------------------------*/

            if (comboBox2.SelectedItem == "Compress")
            {
                int[] RedFreq = new int[256];
                int[] GreenFreq = new int[256];
                int[] BlueFreq = new int[256];
                int height = ImageOperations.GetHeight(encryptedimg);
                int width = ImageOperations.GetWidth(encryptedimg);
                RGBPixel[,] CompressedImg = new RGBPixel[height, width];

                for (int intializer = 0; intializer < 256; intializer++)
                {
                   RedFreq[intializer] = 0;
                   GreenFreq[intializer] = 0;
                   BlueFreq[intializer] = 0;
                }
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        RGBPixel pixel = encryptedimg[i, j];
                        RedFreq[pixel.red] += 1;
                        GreenFreq[pixel.green] += 1;
                        BlueFreq[pixel.blue] += 1;

                    }
                }
                IDictionary<byte, int> HuffmanRed = new Dictionary<byte, int>();
                IDictionary<byte, int> HuffmanGreen = new Dictionary<byte, int>();
                IDictionary<byte, int> HuffmanBlue = new Dictionary<byte, int>();
                byte key = 0;
                for (int looper = 0; looper <= 255; looper++)
                {
                    HuffmanRed.Add(key, RedFreq[key]);
                    HuffmanGreen.Add(key, GreenFreq[key]);
                    HuffmanBlue.Add(key, BlueFreq[key]);
                    key++;
                }
                ImageEncryptCompress.HuffmanTree RedTree = new ImageEncryptCompress.HuffmanTree(HuffmanRed);
                ImageEncryptCompress.HuffmanTree GreenTree = new ImageEncryptCompress.HuffmanTree(HuffmanGreen);
                ImageEncryptCompress.HuffmanTree BlueTree = new ImageEncryptCompress.HuffmanTree(HuffmanBlue);

                /*----------CONSTRUCTION END------------------------------------------------------*/

                IDictionary<byte, string> RedEncodings = RedTree.CreateEncodings();
                IDictionary<byte, string> GreenEncodings = GreenTree.CreateEncodings();
                IDictionary<byte, string> BlueEncodings = BlueTree.CreateEncodings();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        RGBPixel pixel = encryptedimg[i, j];
                        string redValue = RedEncodings[pixel.red];
                        string greenValue = GreenEncodings[pixel.green];
                        string blueValue = BlueEncodings[pixel.blue];
                        RGBPixel newPixel =new RGBPixel();
                        newPixel.red = Convert.ToByte(redValue, 2);
                        newPixel.green = Convert.ToByte(greenValue,2);
                        newPixel.blue = Convert.ToByte(blueValue,2);
                        CompressedImg[i, j] = newPixel;

                    }
                }
            //   ImageOperations.DisplayImage(CompressedImg, pictureBox2);



            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
       
       
    }
}