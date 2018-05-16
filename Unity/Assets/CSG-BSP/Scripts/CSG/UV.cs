using UnityEngine;
using System.Collections;

namespace CSG {
    public class UV {
        public float U;
        public float V;

        public UV() {
            this.Set(0, 0);
        }

        public UV(float u, float v) {
            this.Set(u, v);
        }

        public UV(UV uv) {
            this.Set(uv.U, uv.V);
        }

        public void Set(float u, float v) {
            U = u;
            V = v;
        }

        public UV Lerped(UV dest, float t, UV outUV) {
            outUV.U = (dest.U - U) * t + U;
            outUV.V = (dest.V - V) * t + V;
            return outUV;
        }
    }
}