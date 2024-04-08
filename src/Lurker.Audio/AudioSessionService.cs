using NAudio.CoreAudioApi.Interfaces;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;

namespace Lurker.Audio;

public class AudioSessionService
{
    public List<AudioSession> GetSessions()
    {
        var deviceEnumerator = new MMDeviceEnumerator();
        var device = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        var sessions = device.AudioSessionManager.Sessions;

        var audioSessions = new List<AudioSession>();
        for (int i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            if (session.IsSystemSoundsSession || session.State == AudioSessionState.AudioSessionStateInactive)
            {
                continue;
            }

            var processSession = Process.GetProcessById((int)session.GetProcessID);

            audioSessions.Add(new AudioSession(session)
            { 
                Process = processSession,
                Name = string.IsNullOrEmpty(session.DisplayName) ? session.DisplayName : processSession.MainModule.FileVersionInfo.ProductName,
                Icon = Icon.ExtractAssociatedIcon(processSession.MainModule.FileName).ToBitmap()
            });
        }

        return audioSessions;
    }
}
