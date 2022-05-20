using System;
using System.Collections.Generic;
using VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation;

namespace VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation
{
    public class Output
    {
        public List<LossDistributionPoint[]> LossDistributions { get; set; }
        public Classification Classification { get; set; }
        public List<LossDistributionPoint[]> FrameLossDistributions { get; set; }
        public Classification FrameClassification { get; set; }
        public List<LossDistributionPoint[]> GlassLossDistributions { get; set; }
        public Classification GlassClassification { get; set; }
        public List<byte[]> HelicopterWaves { get; set; }
        public List<byte[]> TrafficWaves { get; set; }
        public List<byte[]> FireTruckSirensWaves { get; set; }
        public List<byte[]> Airport { get; set; }
        public string SessionId { get; set; }

        public Output()
        {
            LossDistributions = new List<LossDistributionPoint[]>();
            FrameLossDistributions = new List<LossDistributionPoint[]>();
            LossDistributions = new List<LossDistributionPoint[]>();
            Classification = new Classification();
            FrameClassification = new Classification();
            GlassClassification = new Classification();
            HelicopterWaves = new List<byte[]>();
            TrafficWaves = new List<byte[]>();
            FireTruckSirensWaves = new List<byte[]>();
            Airport = new List<byte[]>();
            SessionId = String.Empty;
        }
    }
}