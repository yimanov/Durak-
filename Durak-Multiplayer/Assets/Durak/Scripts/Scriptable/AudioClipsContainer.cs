using System;
using System.Collections.Generic;
using UnityEngine;

namespace Durak
{ 
    [CreateAssetMenu(menuName = "ScriptableContainers/AudioClipsContainer")]
    public class AudioClipsContainer : ScriptableObject
    {
        public List<AudioClipData> Clips = new List<AudioClipData>();
    }

    public enum ClipType
    {
        Null = -1,
        BakgroundMusic,
        Click,
        Take,
        GiveUp,
        Place,
        LoseResult,
        WinResult,
        NewDeal,
        Cancel,
        DiscardPile,
        DrawnResult
    }

    [Serializable]
    public class AudioClipData
    {
        public ClipType Type;
        public AudioClip Clip;
    }
}
