using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MediaData", menuName = "ScriptableObjects/MediaData", order = 1)]
public class MediaDataScriptableObject : ScriptableObject
{
    public string imagesResourcesPath = "Images";
    public string audioClipsResourcesPath = "AudioClips";

    [Header("Audio Data")]
    public AudioData audioData;
}

[Serializable]
public class AudioData
{
    public AudioClip backgroundAudioClip;
    public float audioFadeDuration = 1f;
}
