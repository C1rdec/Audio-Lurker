using System.Drawing;
using NAudio.CoreAudioApi;

namespace Lurker.Audio;

public class AudioSession
{
    private AudioSessionControl _control;

    public AudioSession(AudioSessionControl control)
    {
        _control = control;
    }

    public Bitmap Icon { get; init; }

    public string Name { get; init; }

    public float Volume { get; init; }

    public void SetVolume(float volume)
        => _control.SimpleAudioVolume.Volume = volume;

    public float GetMasterPeakVolume()
        => _control.AudioMeterInformation.MasterPeakValue;
}
