using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

public class NativeAudioPlayer : IDisposable
{
    [DllImport("winmm.dll")]
    private static extern int waveOutOpen(out IntPtr phwo, uint uDeviceID, ref WaveFormat pwfx, IntPtr dwCallback, IntPtr dwInstance, uint fdwOpen);

    [DllImport("winmm.dll")]
    private static extern int waveOutPrepareHeader(IntPtr hwo, ref WaveHeader pwh, uint cbwh);

    [DllImport("winmm.dll")]
    private static extern int waveOutWrite(IntPtr hwo, ref WaveHeader pwh, uint cbwh);

    [DllImport("winmm.dll")]
    private static extern int waveOutUnprepareHeader(IntPtr hwo, ref WaveHeader pwh, uint cbwh);

    [DllImport("winmm.dll")]
    private static extern int waveOutClose(IntPtr hwo);

    [DllImport("winmm.dll")]
    private static extern int waveOutReset(IntPtr hwo);

    [StructLayout(LayoutKind.Sequential)]
    private struct WaveFormat
    {
        public ushort wFormatTag;
        public ushort nChannels;
        public uint nSamplesPerSec;
        public uint nAvgBytesPerSec;
        public ushort nBlockAlign;
        public ushort wBitsPerSample;
        public ushort cbSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WaveHeader
    {
        public IntPtr lpData;
        public uint dwBufferLength;
        public uint dwBytesRecorded;
        public IntPtr dwUser;
        public uint dwFlags;
        public uint dwLoops;
        public IntPtr lpNext;
        public IntPtr reserved;
    }

    private const uint WAVE_MAPPER = 0xFFFFFFFF;
    private const uint CALLBACK_EVENT = 0x00050000;

    private readonly string _filePath;
    private IntPtr _unmanagedAudioBuffer = IntPtr.Zero;
    private uint _bufferLength = 0;
    private WaveFormat _format;
    
    private Thread _workerThread;
    private AutoResetEvent _playSignal = new AutoResetEvent(false);
    private AutoResetEvent _hardwareSignal = new AutoResetEvent(false);
    private bool _running = true;
    private TaskCompletionSource<bool> _tcs;

    public bool IsDone { get; private set; } = true;

    public NativeAudioPlayer(string filePath) {
        _filePath = Path.GetFullPath(filePath);
    }

    public void Load() {
        if(_unmanagedAudioBuffer != IntPtr.Zero) return;

        if(!File.Exists(_filePath)){
            throw new FileNotFoundException($"Audio file not found: {_filePath}");
        }

        byte[] fileBytes = File.ReadAllBytes(_filePath);

        using (var ms = new MemoryStream(fileBytes))
        using (var br = new BinaryReader(ms)) {
            br.ReadChars(4);
            br.ReadUInt32();
            br.ReadChars(4);

            while (ms.Position < ms.Length) {
                string chunkId = new string(br.ReadChars(4));
                uint chunkSize = br.ReadUInt32();

                if (chunkId == "fmt ") {
                    _format = new WaveFormat
                    {
                        wFormatTag = br.ReadUInt16(),
                        nChannels = br.ReadUInt16(),
                        nSamplesPerSec = br.ReadUInt32(),
                        nAvgBytesPerSec = br.ReadUInt32(),
                        nBlockAlign = br.ReadUInt16(),
                        wBitsPerSample = br.ReadUInt16(),
                        cbSize = 0
                    };
                    if (chunkSize > 16) ms.Position += (chunkSize - 16);
                }
                else if (chunkId == "data") {
                    _bufferLength = chunkSize;
                    _unmanagedAudioBuffer = Marshal.AllocHGlobal((int)chunkSize);
                    byte[] pcmData = br.ReadBytes((int)chunkSize);
                    Marshal.Copy(pcmData, 0, _unmanagedAudioBuffer, pcmData.Length);
                    break;
                }
                else {
                    ms.Position += chunkSize;
                }
            }
        }

        _workerThread = new Thread(AudioLoop) {
            IsBackground = true,
            Name = "AudioLoop_" + Path.GetFileName(_filePath),
            Priority = ThreadPriority.AboveNormal
        };
        _workerThread.Start();
    }

    public async void Play(){
         await PlayAsync();
    }

    public Task PlayAsync() {
        if (_unmanagedAudioBuffer == IntPtr.Zero) {
            Load();
        }

        IsDone = false;
        _tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _playSignal.Set(); 

        return _tcs.Task;
    }

    private void AudioLoop() {
        while (_running) {
            _playSignal.WaitOne(); 
            if (!_running) break;

            IntPtr hWaveOut = IntPtr.Zero;
            IntPtr hEvent = _hardwareSignal.SafeWaitHandle.DangerousGetHandle();

            if (waveOutOpen(out hWaveOut, WAVE_MAPPER, ref _format, hEvent, IntPtr.Zero, CALLBACK_EVENT) == 0) {
                WaveHeader header = new WaveHeader
                {
                    lpData = _unmanagedAudioBuffer,
                    dwBufferLength = _bufferLength,
                    dwFlags = 0,
                    dwLoops = 0
                };

                waveOutPrepareHeader(hWaveOut, ref header, (uint)Marshal.SizeOf(header));
                
                _hardwareSignal.Reset();
                waveOutWrite(hWaveOut, ref header, (uint)Marshal.SizeOf(header));

                while (_running) {
                    _hardwareSignal.WaitOne(50);
                    if ((header.dwFlags & 0x00000001) != 0 || !_running) {
                        break;
                    }
                }

                waveOutReset(hWaveOut);
                waveOutUnprepareHeader(hWaveOut, ref header, (uint)Marshal.SizeOf(header));
                waveOutClose(hWaveOut);
            }

            IsDone = true;
            _tcs?.TrySetResult(true);
        }
    }

    public void Dispose() {
        _running = false;
        _playSignal.Set();
        _hardwareSignal.Set();
        
        if (_unmanagedAudioBuffer != IntPtr.Zero) {
            Marshal.FreeHGlobal(_unmanagedAudioBuffer);
            _unmanagedAudioBuffer = IntPtr.Zero;
        }
        
        _playSignal.Dispose();
        _hardwareSignal.Dispose();
    }
}
