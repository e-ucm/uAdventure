// Type: UnityEngine.Vector2
// Assembly: UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Program Files (x86)\Unity\Editor\Data\Managed\UnityEngine.dll
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
    public class RectD
    {
        public Vector2d Min { get; private set; }
        public Vector2d Size { get; private set; }

        public Vector2d Center
        {
            get
            {
                return new Vector2d(Min.x + Size.x / 2, Min.y + Size.y / 2);
            }
            set
            {
                Min = new Vector2d(value.x - Size.x / 2, value.x - Size.y / 2);
            }
        }

        public double Height
        {
            get { return Size.y; }
        }

        public double Width
        {
            get { return Size.x; }
        }

        public RectD(Vector2d min, Vector2d size)
        {
            Min = min;
            Size = size;
        }

        public bool Contains(Vector2d point)
        {
            bool flag = Width < 0.0 && point.x <= Min.x && point.x > (Min.x + Size.x) || Width >= 0.0 && point.x >= Min.x && point.x < (Min.x + Size.x);
            return flag && (Height < 0.0 && point.y <= Min.y && point.y > (Min.y + Size.y) || Height >= 0.0 && point.y >= Min.y && point.y < (Min.y + Size.y));
        }

        public Rect ToRect()
        {
            return new Rect((float)Min.x, (float)Min.y, (float)(Width), (float)(Height));
        }


        public bool Intersects(RectD other)
        {
            var a0 = Min;
            var a1 = Min + Size;
            var b0 = other.Min;
            var b1 = other.Min + other.Size;

            return a0.x < b1.x && a1.x > b0.x && a0.y < b1.y && a1.y > b0.y;
        }

        public override string ToString()
        {
            return Min.x + " " + Min.y + " " + Size.x + " " + Size.y;
        }

        public static RectD Parse(string input)
        {
            if (String.IsNullOrEmpty(input)) throw new ArgumentException(input);

            var numbers = input.Split(' ').Select(t => double.Parse(t)).ToArray();
            return new RectD(new Vector2d(numbers[0], numbers[1]), new Vector2d(numbers[2], numbers[3]));
        }

        public static bool TryParse(string input, out RectD result)
        {
            if (String.IsNullOrEmpty(input) || string.IsNullOrEmpty(input.Trim()) || input.Length < 7)
            {
                result = new RectD(Vector2d.zero, Vector2d.zero);
                return false;
            }

            var numbers = input.Split(' ').Select(t => t.Trim()).ToArray();
            if (numbers.Length < 4)
            {
                result = new RectD(Vector2d.zero, Vector2d.zero);
                return false;
            }

            double x, y, w, h;

            if (!double.TryParse(numbers[0], out x) || !double.TryParse(numbers[1], out y) ||
                !double.TryParse(numbers[2], out w) || !double.TryParse(numbers[3], out h))
            {
                result = new RectD(Vector2d.zero, Vector2d.zero);
                return false;
            }

            result = new RectD(new Vector2d(x, y), new Vector2d(w, h));
            return true;
        }

        public static bool TryParse(string input, out RectD result, IFormatProvider format)
        {
            if (String.IsNullOrEmpty(input) || string.IsNullOrEmpty(input.Trim()) || input.Length < 7)
            {
                result = new RectD(Vector2d.zero, Vector2d.zero);
                return false;
            }

            var numbers = input.Split(' ').Select(t => t.Trim()).ToArray();
            if (numbers.Length < 4)
            {
                result = new RectD(Vector2d.zero, Vector2d.zero);
                return false;
            }

            double x, y, w, h;

            if (!double.TryParse(numbers[0], NumberStyles.Any, format, out x) || !double.TryParse(numbers[1], NumberStyles.Any, format, out y) ||
                !double.TryParse(numbers[2], NumberStyles.Any, format, out w) || !double.TryParse(numbers[3], NumberStyles.Any, format, out h))
            {
                result = new RectD(Vector2d.zero, Vector2d.zero);
                return false;
            }

            result = new RectD(new Vector2d(x, y), new Vector2d(w, h));
            return true;
        }
    }
}
