using System.Diagnostics;
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

    public Process Process { get; init; }

    public Bitmap Icon { get; init; }

    public string Name { get; init; }

    public int Volume 
    { 
        get => (int)(_control.SimpleAudioVolume.Volume * 100);
        set => _control.SimpleAudioVolume.Volume = value / 100f;
    }

    public float GetMasterPeak()
        => _control.AudioMeterInformation.MasterPeakValue;
}
