using NAudio.CoreAudioApi.Interfaces;
using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

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
                Id = session.GetSessionIdentifier,
                Process = processSession,
                Name = GetSessionName(session, processSession),
                Icon = Icon.ExtractAssociatedIcon(processSession.MainModule.FileName).ToBitmap()
            });
        }

        return audioSessions;
    }

    private string GetSessionName(AudioSessionControl session, Process process)
    {
        if (string.IsNullOrEmpty(session.DisplayName))
        {
            if (string.IsNullOrEmpty(process.MainModule.FileVersionInfo.ProductName))
            {
                return Path.GetFileNameWithoutExtension(process.MainModule.FileName);
            }

            return process.MainModule.FileVersionInfo.ProductName;
        }

        return session.DisplayName;
    }
}
