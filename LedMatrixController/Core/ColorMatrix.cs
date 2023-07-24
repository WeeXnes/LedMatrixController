using System.Collections.Generic;
using Newtonsoft.Json;

namespace LedMatrixController.Core
{
    public class ColorMatrix
    {
        public MColor[] Colors { get; set; }
        public ColorMatrix()
        {
            this.Colors = new MColor[256];
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}