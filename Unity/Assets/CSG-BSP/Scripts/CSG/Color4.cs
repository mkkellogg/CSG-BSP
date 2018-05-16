using UnityEngine;
using System.Collections;

namespace CSG {
    public class Color4 {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color4() {
            this.Set(0, 0, 0, 0);
        }

        public Color4(float r, float g, float b, float a) {
            this.Set(r, g, b, a);
        }

        public Color4(Color4 color) {
            this.Set(color.R, color.G, color.B, color.A);
        }

        public void Set(float r, float g, float b, float a) {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public Color4 Lerped(Color4 dest, float t, Color4 outColor) {
            outColor.R = (dest.R - R) * t + R;
            outColor.G = (dest.G - G) * t + G;
            outColor.B = (dest.B - B) * t + B;
            outColor.A = (dest.A - A) * t + A;
            return outColor;
        }
    }
}