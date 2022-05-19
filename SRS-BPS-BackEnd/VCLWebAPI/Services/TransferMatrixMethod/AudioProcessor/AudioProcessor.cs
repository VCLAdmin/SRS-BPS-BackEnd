using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO;
using VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation;
using VCLWebAPI.Models.TransferMatrixMethod.AudioProcessor;
using VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation;

namespace VCLWebAPI.Services.TransferMatrixMethod.AudioProcessor
{
    public static class AudioProcessor
    {
        private const float BANDWIDTH = 4.32f;
        private const float LOWPASSFILTERBANDWIDTH = 0.7f;
        private const float HIGHPASSFILTERBANDWIDTH = 0.7f;

        public readonly static double[] EXTRA_FREQUENCIES =
        {
            15.6, 19.7, 24.8, 31.3, 39.4, 6300.0, 8000.0, 10000.0, 12700.0, 16000.0, 20100.0
        };

        public static void Process(AudioFileReader audio, string songName, int index, LossDistributionPoint[] distribution, string sessionId)
        {
            var bands = GetEqualizerBands(distribution);
            var lowPassBand = GetLowPassBand();
            var highPassBand = GetHighPassBand();
            ApplyEqualizer(audio, songName, index, bands, lowPassBand, highPassBand, sessionId);
        }

        private static EqualizerBand[] GetEqualizerBands(LossDistributionPoint[] distribution)
        {
            var bands = new EqualizerBand[distribution.Length];
            for (int i = 0; i < distribution.Length; i++)
            {
                var freq = DavyModelSolver.GetFrequency(i);
                bands[i] = new EqualizerBand
                {
                    Bandwidth = BANDWIDTH,
                    Frequency = (float)distribution[i].Frequency,
                    Gain = -(float)distribution[i].STL * 0.7f //TODO decrese STL before process, compensate for add-up decibels in over-lapping region.
                };
            }
            return bands;
        }

        static public EqualizerBand GetLowPassBand()
        {
            return new EqualizerBand
            {
                Frequency = 6300.0f,
                Bandwidth = LOWPASSFILTERBANDWIDTH
            };
        }

        static public EqualizerBand GetHighPassBand()
        {
            return new EqualizerBand
            {
                Frequency = 39.4f,
                Bandwidth = HIGHPASSFILTERBANDWIDTH
            };
        }

        private static void ApplyEqualizer(AudioFileReader audio, string songName, int index, EqualizerBand[] bands,
                                     EqualizerBand lowPassBand, EqualizerBand highPassBand, string sessionId)
        {
            Equalizer equalizedAudio = new Equalizer(audio, bands, lowPassBand, highPassBand);
            SampleToWaveProvider16 processedWave = new SampleToWaveProvider16(equalizedAudio);

            string path = Path.GetTempPath();
            string filename = songName + "_" + index + ".mp3";
            string tempfile = Path.Combine(path, sessionId, filename);

            Directory.CreateDirectory(Path.Combine(path, sessionId));
            MediaFoundationEncoder.EncodeToMp3(processedWave, tempfile, 64000);

            // Reset Audio position - the next wav file will not read bytes otherwise
            audio.Position = 0;
        }
    }
}