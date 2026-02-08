using System.Diagnostics.Contracts;

namespace Chip_8
{
    public partial class Form1 : Form
    {
        Chip8 chip = new Chip8();
        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.ClientSize = new Size(64 * 15, 32 * 15);

            this.Text = "Chip - 8 Emulator";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                chip.Cycle();
            }
            chip.UpdateTimer();
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Brush pixelBrush = Brushes.Green;


            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (chip.GetDisplay(j, i))
                    {
                        e.Graphics.FillRectangle(pixelBrush, j * scale, i * scale, scale, scale);
                    }
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1:
                    chip.SetKey(0x1, true);
                    break;
                case Keys.D2:
                    chip.SetKey(0x2, true);
                    break;
                case Keys.D3:
                    chip.SetKey(0x3, true);
                    break;
                case Keys.D4:
                    chip.SetKey(0xC, true);
                    break;
                case Keys.Q:
                    chip.SetKey(0x4, true);
                    break;
                case Keys.W:
                    chip.SetKey(0x5, true);
                    break;
                case Keys.E:
                    chip.SetKey(0x6, true);
                    break;
                case Keys.R:
                    chip.SetKey(0xD, true);
                    break;
                case Keys.A:
                    chip.SetKey(0x7, true);
                    break;
                case Keys.S:
                    chip.SetKey(0x8, true);
                    break;
                case Keys.D:
                    chip.SetKey(0x9, true);
                    break;
                case Keys.F:
                    chip.SetKey(0xE, true);
                    break;
                case Keys.Z:
                    chip.SetKey(0xA, true);
                    break;
                case Keys.X:
                    chip.SetKey(0x0, true);
                    break;
                case Keys.C:
                    chip.SetKey(0xB, true);
                    break;
                case Keys.V:
                    chip.SetKey(0xF, true);
                    break;
            }

        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1:
                    chip.SetKey(0x1, false);
                    break;
                case Keys.D2:
                    chip.SetKey(0x2, false);
                    break;
                case Keys.D3:
                    chip.SetKey(0x3, false);
                    break;
                case Keys.D4:
                    chip.SetKey(0xC, false);
                    break;
                case Keys.Q:
                    chip.SetKey(0x4, false);
                    break;
                case Keys.W:
                    chip.SetKey(0x5, false);
                    break;
                case Keys.E:
                    chip.SetKey(0x6, false);
                    break;
                case Keys.R:
                    chip.SetKey(0xD, false);
                    break;
                case Keys.A:
                    chip.SetKey(0x7, false);
                    break;
                case Keys.S:
                    chip.SetKey(0x8, false);
                    break;
                case Keys.D:
                    chip.SetKey(0x9, false);
                    break;
                case Keys.F:
                    chip.SetKey(0xE, false);
                    break;
                case Keys.Z:
                    chip.SetKey(0xA, false);
                    break;
                case Keys.X:
                    chip.SetKey(0x0, false);
                    break;
                case Keys.C:
                    chip.SetKey(0xB, false);
                    break;
                case Keys.V:
                    chip.SetKey(0xF, false);
                    break;
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openROmToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openRomToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "CHIP-8 Rom|*.ch8|All files|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                chip.LoadRom(openFileDialog.FileName);

                timer1.Start();

                this.Text = $"{System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName)}";
            }
        }

        private void fIleToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }
    }
}
