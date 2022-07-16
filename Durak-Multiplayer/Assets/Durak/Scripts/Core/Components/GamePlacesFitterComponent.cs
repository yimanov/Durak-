using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Durak
{
    public class GamePlacesFitterComponent : PreInitedMonoBehaviour
    {
#pragma warning disable 649
        [SerializeField]
        private List<PlaceComponent> _places;
        [SerializeField]
        private CanvasScaler _canvasScaler;
        [SerializeField]
        private GamePlacesComponent _gamePlacesComponent;
        [SerializeField]
        private HorizontalLayoutGroup _placesGroup;

        [SerializeField]
        private float _sizeOfCardElement;
#pragma warning restore 649

        private DataHolderConroller _dataHolderController;

        protected override void PreInitialize()
        {
            _dataHolderController = LazySingleton<DataHolderConroller>.Instance;

            _gamePlacesComponent.RebuildCompletedEvent += Rebuild;
            _gamePlacesComponent.InitCompletedEvent += Fit;
        }

        /// <summary>
        /// Fit place by screen resolution.
        /// </summary>
        public void Fit()
        {
            var activePlaces = _places.Count(x => x.IsActive);
            var placeSize = (_canvasScaler.referenceResolution.x - (_placesGroup.spacing * (activePlaces - 1))) / activePlaces;

            for (int i = 0; i < _places.Count; i++)
            {
                var place = _places[i];
                place.Group.enabled = true;
                place.LayoutElmt.enabled = true;
                place.LayoutElmt.minWidth = placeSize;
            }
        }


        /// <summary>
        /// Rebuild spacing between cards.
        /// </summary>
        public void Rebuild()
        {
            var activePlaces = _places.Count(x => x.IsActive);
            var placeSize = (_canvasScaler.referenceResolution.x - (_placesGroup.spacing * (activePlaces - 1))) / activePlaces;

            for (int i = 0; i < _dataHolderController.Players.Count - 1; i++)
            {
                var place = _places[i];

                var placePlayer = _dataHolderController.Players[place.PlayerId];
                var cardCount = place.Transform.childCount;
                var containerSizeWithoutSpacing = _sizeOfCardElement * cardCount;

                place.Group.spacing = containerSizeWithoutSpacing > placeSize
                    ? -(containerSizeWithoutSpacing - placeSize) / (cardCount - 1)
                    : containerSizeWithoutSpacing - placeSize;
            }
        }

        protected override void Dispose()
        {
            if (_gamePlacesComponent != null)
            {
                _gamePlacesComponent.RebuildCompletedEvent -= Rebuild;
                _gamePlacesComponent.InitCompletedEvent -= Fit;
            }
        }
    }
}