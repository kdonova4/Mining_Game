using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dono.MiningGame.Game
{


    [Serializable]
    public struct CrosshairData
    {
        [Tooltip("The image that will be used for this tool's crosshair")]
        public Sprite CrosshairSprite;
        [Tooltip("The size of the crosshair image")]
        public int CrosshairSize;

        [Tooltip("The color of the crosshair image")]
        public Color CrosshairColor;


    }
    [RequireComponent(typeof(AudioSource))]
    public class ToolController : MonoBehaviour
    {

        [Tooltip("Time until the tool overheats")]
        public int OverheatTime = 8;

        [Tooltip("Time until the gun cools down and is ready to fire")]
        public int CooldownTime = 4;

        [Tooltip("Beam Range")]
        public float BeamRange = 1f;

        [Tooltip("Delay between Mine Shots")]
        public float DelayBetweenMineShots = 1.0f;

        [Tooltip("The root object for the tool")]
        public GameObject ToolRoot;

        [Tooltip("Tip of Mining Tool")]
        public Transform TipOfTool;

        [Tooltip("The force that will push back the tool while shooting")]
        public float RecoilForce = 1.0f;

        [Tooltip("Overheat Sound")]
        public AudioClip OverheatSound;

        [Tooltip("Cool down sound")]
        public AudioClip CooldownSound;

        [Tooltip("Shoot sound")]
        public AudioClip ShootSound;

        LineRenderer m_LineRenderer;
        AudioSource m_OneShotAudioSource;
        AudioSource m_ContinuousAudioSource;

        void Awake()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            DebugUtility.HandleErrorIfNullGetComponent<LineRenderer, ToolController>(m_LineRenderer, this, gameObject);

            m_OneShotAudioSource = GetComponent<AudioSource>();
            DebugUtility.HandleErrorIfNullGetComponent<AudioSource, ToolController>(m_OneShotAudioSource, this, gameObject);

            m_ContinuousAudioSource = GetComponent<AudioSource>();
            DebugUtility.HandleErrorIfNullGetComponent<AudioSource, ToolController>(m_ContinuousAudioSource, this, gameObject);
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

