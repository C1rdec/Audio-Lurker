using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

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
                Icon = Icon.ExtractAssociatedIcon(GetProcessFilename(processSession)).ToBitmap()
            });
        }

        return audioSessions;
    }

    private string GetSessionName(AudioSessionControl session, Process process)
    {
        if (string.IsNullOrEmpty(session.DisplayName))
        {
            try
            {
                if (string.IsNullOrEmpty(process.MainModule.FileVersionInfo.ProductName))
                {
                    return Path.GetFileNameWithoutExtension(process.MainModule.FileName);
                }
            }
            catch
            {
                return process.ProcessName;
            }

            return process.MainModule.FileVersionInfo.ProductName;
        }

        return session.DisplayName;
    }


    [Flags]
    private enum ProcessAccessFlags : uint
    {
        QueryLimitedInformation = 0x00001000
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool QueryFullProcessImageName(
          [In] IntPtr hProcess,
          [In] int dwFlags,
          [Out] StringBuilder lpExeName,
          ref int lpdwSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(
     ProcessAccessFlags processAccess,
     bool bInheritHandle,
     int processId);

    string GetProcessFilename(Process p)
    {
        int capacity = 2000;
        StringBuilder builder = new StringBuilder(capacity);
        IntPtr ptr = OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, p.Id);
        if (!QueryFullProcessImageName(ptr, 0, builder, ref capacity))
        {
            return String.Empty;
        }

        return builder.ToString();
    }
}
