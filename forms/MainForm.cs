using System;
using System.Windows.Forms;
using Deadline_oct_15.models;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;

namespace Deadline_oct_15
{
    public partial class MainForm : Form
    {
        private Dictionary<string, Bitmap> _plots = new Dictionary<string, Bitmap>();
        private Dictionary<string, List<Item>> _infos = new Dictionary<string, List<Item>>();

        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach (Button button in this.splitContainer1.Panel2.Controls.OfType<Button>())
            {
                if (button.Tag != null)
                    PrepareData(button.Tag.ToString());
            }
            CreatePointLabel(50);
        }
        private void OnClick(object sender, EventArgs e)
        {
            PlotGraph((sender as Button).Tag.ToString());
        }

        private void PrepareData(string currency)
        {
            ItemsController itemsController = new ItemsController();
            itemsController.LoadData(currency);
            List<Item> points = itemsController.GetPoints(Canvas.Size);

            var image = new Bitmap(Canvas.Width, Canvas.Height);
            Graphics graphics = Graphics.FromImage(image);

            graphics.Clear(Color.GhostWhite);
            DrawGrid(graphics, (int)itemsController.StepX, (int)itemsController.StepY);
            graphics.DrawLines(new Pen(Color.Black, 3f), points.Select(x => x.Point).ToArray());
            DrawStringValues(graphics, points);

            _plots.Add(currency, image);
            _infos.Add(currency, points);
        }

        private void DrawStringValues(Graphics graphics, List<Item> points)
        {
            FontFamily fontFamily = new FontFamily("Lucida Console");
            Font font = new Font(fontFamily, 10, FontStyle.Regular, GraphicsUnit.Point);
            StringFormat stringFormat = new StringFormat
            {
                FormatFlags = StringFormatFlags.DirectionVertical
            };
            for (int i = 0; i < points.Count; i++)
            {
                graphics.DrawString(points[i].Value.ToString(), font, new SolidBrush(Color.FromArgb(66, 60, 54)),
                              points[i].Point.X - 8, points[i].Point.Y + 20, stringFormat);
            }
            graphics.DrawString(points[0].Moment.ToString("yyyy-MM-dd"), font, new SolidBrush(Color.Black),
              20, Canvas.Height - 15);
            graphics.DrawString(points[points.Count - 1].Moment.ToString("yyyy-MM-dd"), font, new SolidBrush(Color.Black),
              Canvas.Width - 110, Canvas.Height - 15);
        }

        private void PlotGraph(string currency)
        {
            Canvas.Image = _plots[currency];
            ResetLabelPoints();
            SetLabelPoints(_infos[currency]);
        }

        private void DrawGrid(Graphics g, int stepX, int stepY)
        {
            int x = -stepX;
            int y = -stepY;
            if (stepX > 0)
                while (x < Canvas.Width)
                    g.DrawLine(new Pen(Color.LightGray, 1f), new Point(x += stepX, Canvas.Height), new Point(x, 0));
            if (stepY > 0)
                while (y < Canvas.Height)
                    g.DrawLine(new Pen(Color.LightGray, 1f), new Point(Canvas.Width, y += stepY), new Point(0, y));
        }

        private void PointLabel_MouseEnter(object sender, EventArgs e)
        {
            PointLabel pL = (PointLabel)sender;
            pL.Text = pL.info;
            pL.Size = new Size(100, 50);
        }

        private void PointLabel_MouseLeave(object sender, EventArgs e)
        {
            PointLabel pL = (PointLabel)sender;
            pL.Text = "";
            pL.Size = new Size(10, 10);
        }

        private List<PointLabel> _pointLabelPool = new List<PointLabel>();
        private void CreatePointLabel(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                PointLabel pL = new PointLabel() { info = "", Location = new Point(0, 0), Size = new Size(10, 10) };
                pL.MouseEnter += PointLabel_MouseEnter;
                pL.MouseLeave += PointLabel_MouseLeave;
                pL.ForeColor = Color.Red;
                pL.BackColor = Color.Transparent;
                Canvas.Invoke(new Action(() => Canvas.Controls.Add(pL)));
                _pointLabelPool.Add(pL);
            }
        }
        private void SetLabelPoints(List<Item> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                _pointLabelPool[i].Location = new Point((int)items[i].Point.X - 5, (int)items[i].Point.Y - 5);
                _pointLabelPool[i].info = items[i].Value.ToString() + "\n" + items[i].Moment.ToString();
            }
        }
        private void ResetLabelPoints()
        {
            foreach(PointLabel pL in _pointLabelPool)
            {
                pL.Location = new Point(0, 0);
                pL.info = "";
            }
        }
    }
}
