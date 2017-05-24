﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace MonoGameWorld.Audio
{
    public class AudioManager
    {
        public AudioEngine AudioEngine { get; private set; }
        public WaveBank WaveBank { get; set; }
        public SoundBank SoundBank { get; set; }
        public AudioListener Listener { get; set; }
        public Dictionary<String, CueData> CueDictionary { get; set; }

        public AudioManager(String projectPath, String waveBankPath, String soundBankPath)
        {
            SetAudioEngineByPath(projectPath);
            SetWaveBankByPath(waveBankPath);
            SetSoundBankByPath(soundBankPath);

            Listener = new AudioListener();
            CueDictionary = new Dictionary<string, CueData>();
        }

        public void SetAudioEngineByPath(String projectPath)
        {
            AudioEngine = new AudioEngine(projectPath);
        }

        public void SetWaveBankByPath(String waveBankPath)
        {
            WaveBank = new WaveBank(AudioEngine, waveBankPath);
        }

        public void SetSoundBankByPath(String soundBankPath)
        {
            SoundBank = new SoundBank(AudioEngine, soundBankPath);
        }

        public bool AddCue(String cueName, String soundName, bool is3DEmitter)
        {
            if (!CueDictionary.ContainsKey(cueName))
            {
                CueData cueData = new CueData(is3DEmitter)
                {
                    Cue = SoundBank.GetCue(soundName)
                };
                cueData.Update(Listener);

                CueDictionary.Add(cueName, cueData);
                return true;
            }

            return false;
        }
        
        public void Update()
        {
            foreach (CueData cue in CueDictionary.Values)
            {
                cue.Update(Listener);
            }

            AudioEngine.Update();
        }
    }
}
