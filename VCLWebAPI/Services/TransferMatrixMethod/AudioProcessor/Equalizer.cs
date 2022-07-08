using NAudio.Dsp;
using NAudio.Wave;
using VCLWebAPI.Models.TransferMatrixMethod.AudioProcessor;

namespace VCLWebAPI.Services.TransferMatrixMethod.AudioProcessor
{
    internal class Equalizer : ISampleProvider
    {
        private readonly ISampleProvider sourceProvider;
        private readonly EqualizerBand[] bands;
        private readonly EqualizerBand lowPassBand;
        private readonly EqualizerBand highPassBand;
        private readonly BiQuadFilter[,] filters;
        private readonly int channels;

        public Equalizer(ISampleProvider sourceProvider, EqualizerBand[] bands,
                         EqualizerBand lowPassBand, EqualizerBand highPassBand)
        {
            this.sourceProvider = sourceProvider;
            this.bands = bands;
            this.lowPassBand = lowPassBand;
            this.highPassBand = highPassBand;
            this.channels = sourceProvider.WaveFormat.Channels;
            this.filters = new BiQuadFilter[channels, bands.Length + 2];
            CreateFilters();
        }

        private void CreateFilters()
        {
            // add peaking EQ
            for (int bandIndex = 0; bandIndex < bands.Length; bandIndex++)
            {
                var band = bands[bandIndex];
                for (int n = 0; n < channels; n++)
                {
                    filters[n, bandIndex] = BiQuadFilter.PeakingEQ(sourceProvider.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                }
            }
            // add low pass
            for (int n = 0; n < channels; n++)
            {
                filters[n, bands.Length] = BiQuadFilter.LowPassFilter(sourceProvider.WaveFormat.SampleRate, lowPassBand.Frequency, lowPassBand.Bandwidth);
            }
            // add high pass
            for (int n = 0; n < channels; n++)
            {
                filters[n, bands.Length + 1] = BiQuadFilter.HighPassFilter(sourceProvider.WaveFormat.SampleRate, highPassBand.Frequency, highPassBand.Bandwidth);
            }
        }

        public WaveFormat WaveFormat => sourceProvider.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = sourceProvider.Read(buffer, offset, count);

            for (int n = 0; n < samplesRead; n++)
            {
                int ch = n % channels;

                for (int band = 0; band < bands.Length; band++)
                {
                    buffer[offset + n] = filters[ch, band].Transform(buffer[offset + n]);
                }
                buffer[offset + n] = filters[ch, bands.Length].Transform(buffer[offset + n]);
                buffer[offset + n] = filters[ch, bands.Length + 1].Transform(buffer[offset + n]);
            }
            return samplesRead;
        }
    }
}