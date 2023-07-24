using System.CodeDom;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RgbMatrix
{
    public class MatrixColor
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public MatrixColor(int r = 0, int g = 0, int b = 0)
        {
            this.Red = r;
            this.Green = g;
            this.Blue = b;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}