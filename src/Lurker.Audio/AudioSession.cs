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
        set
        {
            _control.SimpleAudioVolume.Volume = ValidateVolume(value) / 100f;
        }
    }

    public float GetMasterPeak()
        => _control.AudioMeterInformation.MasterPeakValue;

    private static int ValidateVolume(int volume)
    {
        if (volume < 0)
        {
            return 0;
        }

        if (volume > 100)
        {
            return 100;
        }

        return volume;
    }
}
