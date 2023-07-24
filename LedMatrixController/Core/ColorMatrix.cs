using System.Collections.Generic;
using Newtonsoft.Json;

namespace RgbMatrix
{
    public class ColorMatrix
    {
        public MatrixColor[] Colors { get; set; }
        public ColorMatrix()
        {
            this.Colors = new MatrixColor[256];
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}