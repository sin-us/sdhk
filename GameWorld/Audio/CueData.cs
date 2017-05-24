using System;
using Microsoft.Xna.Framework.Audio;

namespace MonoGameWorld.Audio
{
    public class CueData
    {
        public Cue Cue { get; set; }
        public bool Is3DEmitter { get; set; }
        public AudioEmitter Emitter { get; set; }

        public CueData(SoundBank soundBank, String soundName, AudioListener listener, bool is3DEmitter)
        {
            Cue = soundBank.GetCue(soundName);

            Is3DEmitter = is3DEmitter;
            if (is3DEmitter)
            {
                Emitter = new AudioEmitter();
                Cue.Apply3D(listener, Emitter);
            }
        }

        public void Update(AudioListener listener)
        {
            if (Is3DEmitter)
            {
                Cue.Apply3D(listener, Emitter);
            }
        }
    }
}
