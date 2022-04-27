using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using SharedCode;
using AdvancedInspector;

namespace SpaceInsurgency
{
	[AdvancedInspector]
	public class AudioManager : SharedCode.Behaviours.InstanceTracked<AudioManager>
	{
		
		class ActiveAudioSourceInfo
		{
			public AudioSource audioSource;
			public float creationTime;
		}

		[Inspect]
		public AudioMixerGroup mixerDestruction;
		[Inspect]
		public AudioMixerGroup mixerWeapons;
		[Inspect]
		public AudioMixerGroup mixerFTL;

		AutoObjectPool<AudioSource, object> audioSourcePool = null;
		List<ActiveAudioSourceInfo> activeAudioSources = null;

		protected override void OnEnable()
		{
			base.OnEnable();

			SetTimer(1f, true, OnTimeTickOneSecond);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			ClearTimer(OnTimeTickOneSecond);
		}

		protected override void Start()
		{
			base.Start();

			audioSourcePool = new AutoObjectPool<AudioSource, object>(GenerateAudioSource, CleanupAudioSource, 100);
			audioSourcePool.PreloadObjects(null);

			activeAudioSources = new List<ActiveAudioSourceInfo>();
		}

		AudioSource GenerateAudioSource(object a1)
		{
			GameObject obj = new GameObject("Pooled Audio Source");
			obj.SetActive(false);
			AudioSource audioSource = obj.AddComponent<AudioSource>();
			obj.transform.parent = transform;
			CleanupAudioSource(audioSource);
			return audioSource;
		}

		void CleanupAudioSource(AudioSource obj)
		{
			SetDefaultsAudioSource(obj);
			obj.gameObject.SetActive(false);
		}

		void SetDefaultsAudioSource(AudioSource obj)
		{
			obj.clip = null;
			obj.outputAudioMixerGroup = null;
			obj.spatialBlend = 1f;
			obj.loop = false;
			obj.playOnAwake = false;
		}

		public void PlayClipAtPosition(List<AudioClip> clips, AudioMixerGroup mixerGroup, Vector3 position, float volumeScale = 1f)
		{
			if (null == clips)
				return;
			if (0 == clips.Count)
				return;
			
			PlayClipAtPosition(clips[UnityEngine.Random.Range(0, clips.Count)], mixerGroup, position, volumeScale);
		}

		public void PlayClipAtPosition(AudioClip clip, AudioMixerGroup mixerGroup, Vector3 position, float volumeScale = 1f)
		{
			if (null == clip)
				return;
			
			//Debug.Log("PlayClipAtPosition");
			ActiveAudioSourceInfo info = new ActiveAudioSourceInfo();
			info.audioSource = audioSourcePool.Get(null);
			info.audioSource.outputAudioMixerGroup = mixerGroup;
			info.audioSource.transform.position = position;
			info.audioSource.volume = volumeScale;
			info.audioSource.clip = clip;
			info.audioSource.gameObject.SetActive(true);
			info.audioSource.Play();
			info.creationTime = Time.time;
			activeAudioSources.Add(info);
		}


		void OnTimeTickOneSecond()
		{
			int i = activeAudioSources.Count;
			while (--i >= 0)
			{
				ActiveAudioSourceInfo info = activeAudioSources[i];
				if (Time.time > info.creationTime + info.audioSource.clip.length)
				{
					audioSourcePool.Abandon(info.audioSource);
					info.audioSource = null;
					activeAudioSources.Remove(info);
				}
			}
		}
	}
}
