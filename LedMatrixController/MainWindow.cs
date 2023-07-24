using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LedMatrixController.Core;

namespace LedMatrixController
{
    public partial class MainWindow : Form
    {
        public static Color currentColor = Color.Black;
        public static int PanelGap = 18;
        public static Size panelSize = new Size(17, 17);
        public static Point panelStart = new Point(370, 277);
        public MainWindow()
        {
            InitializeComponent();
            SetDarkMode();
            CreatePanels();
            SetColorFix();
        }

        private void SetColorFix()
        {
            currentColor = selectColorDialog.Color; 
        }
        private void SetDarkMode()
        {
            Color darkModeColor = Color.FromArgb(22, 26, 33);
            Color darkModeColor2 = Color.FromArgb(31, 38, 48);
            this.BackColor = darkModeColor;
            foreach (var but in GetButtons())
            {
               but.ForeColor = Color.White; 
               but.BackColor = darkModeColor2;
               but.FlatStyle = FlatStyle.Flat;
               but.FlatAppearance.BorderSize = 0;
            }


        }
        private void CreatePanels()
        {
            int indexcounter = 1;
            for (int i = 0; i <= 15; i++)
            {
                if (i % 2 != 0)
                {
                    for (int j = 0; j <= 15; j++)
                    {
                        Panel newPanel = new Panel();
                        newPanel.Size = panelSize;
                        Point newPos = panelStart;
                        newPos.Y = newPos.Y - i * PanelGap;
                        newPos.X = newPos.X - j * PanelGap;
                        newPanel.Location = newPos;
                        newPanel.BackColor = currentColor;
                        newPanel.TabIndex = indexcounter;
                        AddPanel(newPanel);
                        indexcounter++;
                    }
                }
                else
                {
                    for (int j = 0; j <= 15; j++)
                    {
                        Panel newPanel = new Panel();
                        newPanel.Size = panelSize;
                        Point newPos = panelStart;
                        newPos.Y = newPos.Y - i * PanelGap;
                        newPos.X = (newPos.X - (0-j) * PanelGap)-15*PanelGap;
                        newPanel.Location = newPos;
                        newPanel.BackColor = currentColor;
                        newPanel.TabIndex = indexcounter;
                        AddPanel(newPanel);
                        indexcounter++;
                    }
                }
            }
        }
        void AddPanel(Panel _panel)
        {
            _panel.Click += PanelOnClick;
            this.Controls.Add(_panel);
        }
        private void PanelOnClick(object sender, EventArgs e)
        {
            Panel selectedPanel = (Panel)sender;
            selectedPanel.BackColor = currentColor;
        }
        public IEnumerable<Panel> GetPanels()
        {
            return this.Controls.OfType<Panel>().OrderBy(panel => panel.TabIndex);
        }
        public IEnumerable<Button> GetButtons()
        {
            return this.Controls.OfType<Button>().OrderBy(but => but.TabIndex);
        }

        private void sendDatatoESP(string JsonData)
        {
            string jsonData = JsonData;

            // Replace "http://your_esp8266_ip_or_hostname/endpoint" with the actual address of your ESP8266
            string esp8266Endpoint = "http://192.168.178.122/endpoint";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(esp8266Endpoint, content);

                    if (response.Result.StatusCode == HttpStatusCode.OK)
                    {
                        //MessageBox.Show("JSON data sent successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to send JSON data. Status code: " + response.Result.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void LoadTexture(string path)
        {
            List<Color> colors = new List<Color>();
            Bitmap img = new Bitmap(path);
            for (int j = 15; j >= 0; j--)
            {
                if (j % 2 != 0)
                {
                    for (int i = 0; i <= 15; i++)
                    {
                        Color pixel = img.GetPixel(i, j);
                        colors.Add(pixel);
                    }
                }
                else
                {
                    
                    for (int i = 15; i >= 0; i--)
                    {
                        Color pixel = img.GetPixel(i, j);
                        colors.Add(pixel);
                    }
                }
            }
            
            
            for (int i = 0; i < colors.Count; i++)
            {
                Panel panel = GetPanels().ElementAt(i);
                panel.BackColor = colors[i];
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            selectColorDialog.ShowDialog();  
            currentColor = selectColorDialog.Color; 
        }
        private void button2_Click(object sender, EventArgs e)
        {
            foreach(var pnl in GetPanels())
            {
                pnl.BackColor = currentColor;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "png files (*.png)|*.png";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadTexture(openFileDialog.FileName);
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            ColorMatrix matrix = new ColorMatrix();
            var panels = GetPanels();
            for (int i = 0; i < panels.Count(); i++)
            {
                Color c = panels.ElementAt(i).BackColor;
                MColor color = new MColor(c.R, c.G, c.B);
                matrix.Colors[i] = color;
            }

            //MessageBox.Show(matrix.ToJson());
            //Clipboard.SetText(matrix.ToJson());
            sendDatatoESP(matrix.ToJson());
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            Color[] colors = new Color[256];
            var panels = GetPanels();
            for (int i = 0; i < panels.Count(); i++)
            {
                colors[i] = panels.ElementAt(i).BackColor;
            }
            int counter = 0;
            using (Bitmap newImage = new Bitmap(16, 16))
            {
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    for (int j = 15; j >= 0; j--)
                    {
                        if (j % 2 != 0)
                        {
                            for (int i = 0; i <= 15; i++)
                            {
                                g.FillRectangle(new SolidBrush(colors[counter]), i, j, 1, 1);
                                counter++;
                            }
                        }
                        else
                        {
                    
                            for (int i = 15; i >= 0; i--)
                            {
                                g.FillRectangle(new SolidBrush(colors[counter]), i, j, 1, 1);
                                counter++;
                            }
                        }
                    }
                }
                //newImage.Save("monke.png", System.Drawing.Imaging.ImageFormat.Png);
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "png files (*.png)|*.png";
                    saveFileDialog.RestoreDirectory = true;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        newImage.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
            
        }
    }
}