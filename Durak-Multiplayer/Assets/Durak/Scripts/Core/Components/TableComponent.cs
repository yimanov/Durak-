using UnityEngine;

namespace Durak
{
    public class TableComponent : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private Transform _attackingCardRoot;
        [SerializeField]
        private Transform _defendingCardRoot;
        [SerializeField]
        private Transform _bufferCardRoot;
#pragma warning restore 649
        private TableController _tableController;
        private TableBufferController _tableBufferController;

        public void Initialize()
        {
            _tableBufferController = LazySingleton<TableBufferController>.Instance;
            _tableBufferController.Initialize(new TableBufferData() { BufferRoot = _bufferCardRoot });
            _tableController = LazySingleton<TableController>.Instance;
            _tableController.Initialize(new TableData() { AttackingRoot = _attackingCardRoot, DefendingRoot = _defendingCardRoot });
        }

        /// <summary>
        /// Clear childs in table roots.
        /// </summary>
        public void Clear()
        {
            foreach (Transform child in _attackingCardRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in _defendingCardRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in _bufferCardRoot)
            {
                Destroy(child.gameObject);
            }
        }
    }
}