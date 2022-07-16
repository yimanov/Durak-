using UnityEngine;

namespace Durak
{
    public class TableBufferData
    { 
        public Transform BufferRoot;
    }

    public class TableBufferController
    {
        private Transform _bufferRoot;
      
        public void Initialize(TableBufferData data)
        {
            _bufferRoot = data.BufferRoot;
        }

        /// <summary>
        /// Send object to buffer container.
        /// </summary>
        /// <param name="obj"></param>
        public void ToBuffer(Transform obj)
        {
            obj.SetParent(_bufferRoot);
        }

    }
}
