using System;
using System.Collections.Generic;
using UnityEngine;

namespace Durak
{
    public class GamePlacesData
    {
        public int PlayersCount;
        public bool HideCards;
    }

    public enum GamePlaceState
    {
        None = -1,
        Attack = 0,
        Defense
    }

    public class GamePlacesComponent : PreInitedMonoBehaviour
    {
        public event Action RebuildCompletedEvent;
        public event Action InitCompletedEvent;

#pragma warning disable 649
        [SerializeField]
        private List<PlaceComponent> _places;
        [SerializeField]
        private Sprite _attackPlaceStateSprite;
        [SerializeField]
        private Sprite _defensePlaceStateSprite;
#pragma warning restore 649

        private DataHolderConroller _dataHolderController;

        protected override void PreInitialize()
        {
            _dataHolderController = LazySingleton<DataHolderConroller>.Instance;
        }

        public void Initialize(GamePlacesData data)
        {
            if (_places.Count < data.PlayersCount)
            {
                Debug.LogError("Can not create places because of Players Count > Places count.");
                return;
            }

            var playerControllers = new List<PlayerController>();

            for (int i = 0; i < _places.Count; i++)
            {
                if (data.PlayersCount - 1 < i)
                {
                    _places[i].SetActive(false);
                    continue;
                }
                else
                {
                    if (!_places[i].gameObject.activeInHierarchy)
                    {
                        _places[i].SetActive(true);
                    }
                }

                var player = new PlayerController();
                player.Dispose();
                player.Initialize(new PlayerGameData()
                {
                    Id = i,
                    IsAi = i != 0,
                    Place = _places[i].Transform,
                    HideCards = data.HideCards,
                    GamePlacesComponent = this
                });
                _places[i].PlayerId = i;

                playerControllers.Add(player);
            }

            _dataHolderController.SetPlayers(playerControllers);

            InitCompletedEvent?.Invoke();
        }


        /// <summary>
        /// Rebuild all game places immediately
        /// </summary>
        public void Rebuild()
        {
            for (int i = 0; i < _places.Count; i++)
            {
                if (_places[i].IsActive)
                {
                    _places[i].Rebuild();
                }
            }

            RebuildCompletedEvent?.Invoke();
        }

        /// <summary>
        /// Rebuild game place by id.
        /// </summary>
        /// <param name="placeId"></param>
        public void Rebuild(int placeId)
        {
            if (_places[placeId].IsActive)
            {
                _places[placeId].Rebuild();
            }

            RebuildCompletedEvent?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackHand"></param>
        /// <param name="defendHand"></param>
        public void SetPlaceActive(int attackHand, int defendHand)
        {
            for (int i = 0; i < _places.Count; i++)
            {
                var place = _places[i];

                if (place)
                {
                    GamePlaceState state = i == attackHand ? GamePlaceState.Attack : 
                                           i == defendHand ? GamePlaceState.Defense : GamePlaceState.None;

                    place.HighlightPlace(GetPlaceStateSprite(state));
                }
            }
        }

        /// <summary>
        /// Get sprite by GamePlaceState
        /// </summary>
        public Sprite GetPlaceStateSprite(GamePlaceState state)
        {
            switch (state)
            {
                case GamePlaceState.Attack:
                    return _attackPlaceStateSprite;
                case GamePlaceState.Defense:
                    return _defensePlaceStateSprite;
                case GamePlaceState.None:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Clear all places 
        /// </summary>
        public void ClearPlaces()
        {
            for (int i = 0; i < _places.Count; i++)
            {
                _places[i].Clear();
            }
        }
    }
}