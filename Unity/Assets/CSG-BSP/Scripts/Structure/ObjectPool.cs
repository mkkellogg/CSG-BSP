using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSG
{
    public interface Indexer
    {
        int GetIndex();
        void SetIndex(int index);
    }

    public class ObjectPool<T> where T : Indexer, new()
    {

        private T[] objects;
        private int claimCount;
        private int linearIncrease;

        public ObjectPool(int initialReserveCount, int linearIncrease)
        {
            this.Allocate(initialReserveCount);
            this.claimCount = 0;
            this.linearIncrease = linearIncrease;
        }

        public T ClaimObject()
        {
            if (this.claimCount >= this.objects.Length) {
                this.Allocate(this.objects.Length + this.linearIncrease);
            }
            int claimIndex = this.claimCount;
            T claimObject = this.objects[claimIndex];
            claimObject.SetIndex(claimIndex);
            this.claimCount++;
            return claimObject;
        }

        public void ReturnObject(T obj)
        {
            int returnedIndex = obj.GetIndex();
            T returnedObject = this.objects[returnedIndex];

            int mostRecentIndex = this.claimCount - 1;
            T mostRecentObject = this.objects[mostRecentIndex];

            this.objects[mostRecentIndex] = returnedObject;
            returnedObject.SetIndex(mostRecentIndex);

            this.objects[returnedIndex] = mostRecentObject;
            mostRecentObject.SetIndex(returnedIndex);

            this.claimCount--;
        }

        private void Allocate(int count)
        {
            /*  if (this.objects != null && count < this.objects.Length) return;

              int oldLength = this.objects != null ? this.objects.Length : 0;
              T[] tempObjects = new T[count];

              for (int i = 0; i < oldLength; i++) { 
                  tempObjects[i] = this.objects[i];
              }

              this.objects = tempObjects;

              for (int i = oldLength; i < this.objects.Length; i++) {
                  this.objects[i] = new T();
              }
              */

            int aCount = 1000;
            this.objects = new T[aCount];
            for (int i =0; i < aCount; i++)
            {
                this.objects[i] = new T();
            }
        }

    }
}
