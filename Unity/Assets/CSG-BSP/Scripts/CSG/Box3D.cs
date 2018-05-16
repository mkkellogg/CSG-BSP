using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG {

    public class Box3D {

        public Vector3D min;
        public Vector3D max;

        public Box3D() {
            this.min = new Vector3D();
            this.max = new Vector3D();
        }

        public Box3D(Vector3D min, Vector3D max) {
            this.min = new Vector3D(min);
            this.max = new Vector3D(max);
        }

        public Box3D(Box3D src) {
            this.min = new Vector3D(src.min);
            this.max = new Vector3D(src.max);
        }

        
        public void Reset() {
            this.min.Set(0, 0, 0);
            this.max.Set(0, 0, 0);
        }

        public bool Intersects(Box3D box) {
            if (this.max.X < box.min.X) return false;
            if (this.min.X > box.max.X) return false;
            if (this.max.Y < box.min.Y) return false;
            if (this.min.Y > box.max.Y) return false;
            if (this.max.Z < box.min.Z) return false;
            if (this.min.Z > box.max.Z) return false;

            return true;
        }

        public void ExpandToContain(Vector3D position) {
            if (position.X < this.min.X) this.min.X = position.X;
            if (position.X > this.max.X) this.max.X = position.X;

            if (position.Y < this.min.Y) this.min.Y = position.Y;
            if (position.Y > this.max.Y) this.max.Y = position.Y;

            if (position.Z < this.min.Z) this.min.Z = position.Z;
            if (position.Z > this.max.Z) this.max.Z = position.Z;
        }
    }

}
