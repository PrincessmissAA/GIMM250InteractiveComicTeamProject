using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]

public class PlayVideo : MonoBehaviour
{
    public VideoClip videoClip;

    private VideoPlayer videoPlayer;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();

        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = GetComponent<Renderer>();
        videoPlayer.targetMaterialProperty = "_MainTex";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
            PlayPause();
    }

    private void PlayPause()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
        else
            videoPlayer.Play();
    }
}
