using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Deadline_oct_15.models
{
    class ItemsController
    {
        private List<Item> _items = new List<Item>();
        internal float StepX { get; set; }
        internal float StepY { get; set; }
        internal void LoadData(string currency)
        {
            if (_items.Count != 0)
            {
                _items.Clear();
            }
            var doc = new XmlDocument();
            doc.Load($"https://www.moex.com/export/derivatives/currency-rate.aspx?language=en&currency={currency}/RUB&moment_start={DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd")}&moment_end={DateTime.Now.ToString("yyyy-MM-dd")}");
            XmlNodeList nodes = doc.SelectNodes("/*/*/*");

            foreach (XmlNode v in nodes)
            {
                _items.Add(new Item()
                {
                    Moment = DateTime.Parse(v.Attributes[0].Value),
                    Value = float.Parse(v.Attributes[1].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture)
                });
            }
        }
        internal List<Item> GetPoints(Size size)
        {
            float step = size.Width / (_items.Count - 1);
            float i = size.Width;
            float min = _items.Min(e => { return e.Value; });
            float max = _items.Max(e => { return e.Value; });
            StepX = step;
            StepY = max - min;
            int h = size.Height;
            if (StepY > 0)
                while (10 > StepY)
                {
                    StepY *= 10;
                }
            StepY = size.Height / StepY;
            _items.ForEach(x => { x.Point = new PointF(i -= step, (size.Height - 100) * (1 - (x.Value - min)/(max - min))); });
            _items.Reverse();
            return _items;
        }
    }
}
