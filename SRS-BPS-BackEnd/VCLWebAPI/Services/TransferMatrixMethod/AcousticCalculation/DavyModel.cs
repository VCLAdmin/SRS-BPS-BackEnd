using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Hosting;
using VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation;

namespace VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation
{
    public class DavyModel
    {
        //private const string ORIGINAL_AUDIO_PATH_Helicopiter = "C:\\Users\\Grace Huang\\Desktop\\Acoustic Project\\Noise Sample\\Helicopter.wav"; // TODO
        //private const string ORIGINAL_AUDIO_PATH_Traffic = "C:\\Users\\Grace Huang\\Desktop\\Acoustic Project\\Noise Sample\\Traffic.wav"; // TODO
        //private const string ORIGINAL_AUDIO_PATH_FireSirens = "C:\\Users\\Grace Huang\\Desktop\\Acoustic Project\\Noise Sample\\FireTrucksSirens.wav"; // TODO
        //private const string ORIGINAL_AUDIO_PATH_Airport = "C:\\Users\\Grace Huang\\Desktop\\Acoustic Project\\Noise Sample\\Airport.wav"; // TODO
        //private static string ORIGINAL_AUDIO_PATH = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Audio/acoustics/waterfall.wav");

        /* PRODUCTION AUDIO LOCATIONS */
        private static string ORIGINAL_AUDIO_PATH_Helicopiter = HostingEnvironment.MapPath("~/Content/Audio/acoustics/Helicopter.mp3");
        private static readonly string ORIGINAL_AUDIO_PATH_Traffic = HostingEnvironment.MapPath("~/Content/Audio/acoustics/Traffic.mp3");
        private static readonly string ORIGINAL_AUDIO_PATH_FireSirens = HostingEnvironment.MapPath("~/Content/Audio/acoustics/FireTrucksSirens.mp3");
        private static readonly string ORIGINAL_AUDIO_PATH_Airport = HostingEnvironment.MapPath("~/Content/Audio/acoustics/Airport.mp3");

        public static void Run2(string sessionId = "test-directory")
        {
            AudioFileReader audioH = new AudioFileReader(ORIGINAL_AUDIO_PATH_Helicopiter);
            AudioFileReader audioT = new AudioFileReader(ORIGINAL_AUDIO_PATH_Traffic);
            AudioFileReader audioF = new AudioFileReader(ORIGINAL_AUDIO_PATH_FireSirens);
            AudioFileReader audioA = new AudioFileReader(ORIGINAL_AUDIO_PATH_Airport);

            double[] facadeTypeAfilter = { 6.4, 22.9, 28, 31.2, 26.9, 34.7, 29.9, 34.7, 36.6, 37.5, 36.8, 38.7, 40.7, 41.7, 42.2, 43.4, 43.2, 44.2, 47.4, 50.5, 51.1 };
            double[] facadeTypeBfilter = { 7.1, 21.8, 23.8, 25.8, 24.8, 30.4, 29.3, 34.1, 35.8, 36.9, 36.5, 38.6, 40.7, 41.5, 42.2, 43.3, 43.2, 44.2, 47.2, 50.4, 51 };
            var TypeAdata = PredefinedFilter.ComputeLossDistributionPoint(facadeTypeAfilter);
            var TypeBdata = PredefinedFilter.ComputeLossDistributionPoint(facadeTypeBfilter);
            AudioProcessor.AudioProcessor.Process(audioT, "Traffic", 1, TypeAdata, sessionId);
            AudioProcessor.AudioProcessor.Process(audioT, "Traffic", 2, TypeBdata, sessionId);

            AudioProcessor.AudioProcessor.Process(audioF, "Firetruck", 1, TypeAdata, sessionId);
            AudioProcessor.AudioProcessor.Process(audioF, "Firetruck", 2, TypeBdata, sessionId);
        }

        public static Output Run(Input input, string sessionId = "test-directory")
        {
            // Sanity check
            Stopwatch a = new Stopwatch();
            a.Start();
            if (input.Components.Count <= 0)
            {
                throw new Exception("Empty components.");
            }
            if (input.Components.Count > 2)
            {
                throw new Exception("Maximum split pane count is TWO.");
            }
            // Prepocess all plate list to merge laminated layer
            AssignPlateInterlayerProperties(input);
            CalculateLaminatedPlateProperties(input);
            // Final output: Composite
            var output = new Output();

            var distribution = DavyModelSolver.ComputeAverageSTLDistribution(input.Components,
                                                                             input.Frame);
            var glassdist = DavyModelSolver.ComputeSTLDistributionOnePlateWithAverage(input.Components[0]);
            var framedist = input.Frame.GetDistribution();
            int count = input.Components.Count;
            //Final output: Composite
            output.Classification = DavyModelSolver.ComputeClassification(distribution);

            output.LossDistributions = new List<LossDistributionPoint[]>();

            int index = 0;

            // For each open percentage, we need to compute the STL distribution
            foreach (double openPercentage in input.OpenPercentages)
            {
                var openDistribution = DavyModelSolver.ComputeOpenSTLDistribution(input, glassdist, framedist, openPercentage);
                output.LossDistributions.Add(openDistribution);
                index++;
            }
            Debug.WriteLine("The composite classification is" + output.Classification);
            // Final output: Glass
            output.GlassLossDistributions = new List<LossDistributionPoint[]>();
            output.GlassLossDistributions.Add(glassdist);
            output.GlassClassification = DavyModelSolver.ComputeClassification(glassdist);
            Debug.WriteLine("The glass classification is" + output.GlassClassification);
            // Final output: Frame
            output.FrameLossDistributions = new List<LossDistributionPoint[]>();
            output.FrameLossDistributions.Add(framedist);
            output.FrameClassification = DavyModelSolver.ComputeClassification(framedist);
            Debug.WriteLine("The frame classification is" + output.FrameClassification);
            Debug.WriteLine(output);
            a.Stop();
            Debug.WriteLine(a.ElapsedMilliseconds + " ms has elapsed");
            return output;
        }

        // Process Audio Tracks
        public static void RunAudio(List<LossDistributionPoint[]> LossDistributions, String sessionId)
        {
            string ORIGINAL_AUDIO_PATH_Helicopiter = HostingEnvironment.MapPath("~/Content/Audio/acoustics/Helicopter.mp3");
            string ORIGINAL_AUDIO_PATH_Traffic = HostingEnvironment.MapPath("~/Content/Audio/acoustics/Traffic.mp3");
            string ORIGINAL_AUDIO_PATH_FireSirens = HostingEnvironment.MapPath("~/Content/Audio/acoustics/FireTrucksSirens.mp3");
            string ORIGINAL_AUDIO_PATH_Airport = HostingEnvironment.MapPath("~/Content/Audio/acoustics/Airport.mp3");

            AudioFileReader audioT = new AudioFileReader(ORIGINAL_AUDIO_PATH_Traffic);
            AudioFileReader audioF = new AudioFileReader(ORIGINAL_AUDIO_PATH_FireSirens);
            AudioFileReader audioA = new AudioFileReader(ORIGINAL_AUDIO_PATH_Airport);
            int index = 0;

            foreach (LossDistributionPoint[] openDistribution in LossDistributions)
            {
                AudioProcessor.AudioProcessor.Process(audioT, "TrafficWaves", index, openDistribution, sessionId);
                AudioProcessor.AudioProcessor.Process(audioF, "FireTruckSirensWaves", index, openDistribution, sessionId);
                AudioProcessor.AudioProcessor.Process(audioA, "Airport", index, openDistribution, sessionId);
                index++;
            }
        }

        public static void AssignPlateInterlayerProperties(Input input)
        {
            foreach (var component in input.Components)
            {
                var rooms = component.Rooms;
                var plates = component.Plates;

                for (int i = 0; i < plates.Count; i++)
                {
                    switch (plates[i].Material)
                    {
                        case Plate.MaterialT.lamiPVB: // todo
                            plates[i].InterRho = 1070;
                            plates[i].InterG = 0.52 * 1e6;
                            plates[i].InterEta = 0.4;
                            break;

                        case Plate.MaterialT.glass: // ??????????????????
                            plates[i].InterRho = 0.0;
                            plates[i].InterG = 0.0;
                            plates[i].InterEta = 0.0;
                            //Debug.WriteLine(Material);
                            break;

                        case Plate.MaterialT.lamiSGP: // todo
                            plates[i].InterRho = 1070;
                            plates[i].InterG = 0.52 * 1e6;
                            plates[i].InterEta = 0.4;
                            break;

                        case Plate.MaterialT.lamiSC:
                            plates[i].InterRho = 1070;
                            plates[i].InterG = 0.52 * 1e6;
                            plates[i].InterEta = 0.4;
                            break;

                        default:
                            throw new Exception("Unsupported material type");
                    }
                }

                for (int i = 0; i < rooms.Count; i++)
                {
                    switch (rooms[i].CavityT)
                    {
                        case Room.CavityType.Air:
                            rooms[i].Rho = 1.217;
                            //T = 1.5;
                            break;

                        case Room.CavityType.Argon: // TODO
                            rooms[i].Rho = 1.217;
                            //T = 1.5;
                            break;

                        default:
                            throw new Exception("Unsupported cavity type");
                    }
                }
            }
        }

        public static void CalculateLaminatedPlateProperties(Input input)
        {
            foreach (var component in input.Components)
            {
                var rooms = component.Rooms;
                var plates = component.Plates;

                var tempRooms = new List<Room>();
                var tempPlates = new List<Plate>();

                int i = 0;
                while (i < plates.Count)
                {
                    if (plates[i].Material == Plate.MaterialT.lamiPVB) // todo
                    {
                        tempPlates.Add(ProcessLaminatedSinglePane(input, plates[i]));
                        tempRooms.Add(rooms[i]);
                        i++;
                    }
                    else
                    {
                        tempPlates.Add(plates[i]);
                        tempRooms.Add(rooms[i]);
                        i++;
                    }
                }

                component.Rooms = tempRooms;
                component.Plates = tempPlates;
            }
        }

        public static double CalculateEffThickness(Plate plate)
        {
            // Laminated Glass Properties (ASTM E1300 -09)
            double ts = plate.H * 0.5 + plate.InterH;
            double ts1 = ts * plate.H * 0.5 / (plate.H);
            double ts2 = ts * plate.H * 0.5 / (plate.H);
            double Is = 0.5 * plate.H * Math.Pow(ts2, 2.0) + 0.5 * plate.H * Math.Pow(ts1, 2.0);

            double tPVB;
            if (plate.Material == Plate.MaterialT.lamiPVB)
            {
                double GammaPVB = 1.0 / (1.0 + 9.6 * (plate.E * Is * plate.InterH / (plate.InterG * Math.Pow(ts, 2.0) * Math.Pow(plate.Lx, 2.0))));
                double term1 = Math.Pow(plate.H * 0.5, 3.0) * 2.0 + 12 * GammaPVB * Is;
                double tPVBd = Math.Pow(term1, 1.0 / 3.0);
                return tPVB = tPVBd + plate.InterH / 3.0;
            }
            else
            {
                throw new Exception("SGP and SC PVB interlayer have not been implemented yet");
            }
        }

        public static double CalculateEffEta(Input input, Plate plate)
        {
            int count = input.Components[0].Plates.Count;
            double etaZero;
            if (count == 1)
            {
                etaZero = plate.Eta + plate.InterH * plate.InterEta / (0.5 * plate.H);
            }
            else
            {
                etaZero = plate.Eta + plate.InterH * plate.InterEta / (plate.H);
            }
            return etaZero;
        }

        public static Plate ProcessLaminatedSinglePane(Input input, Plate plate)
        {
            //Debug.WriteLine("TMMModel 104 / Plate 1 material is: " + plate.Material);
            var plateNew = new Plate()
            {
                //Material = plate.Material,
                H = CalculateEffThickness(plate),
                Lx = plate.Lx,
                Ly = plate.Ly,
                Eta = CalculateEffEta(input, plate),
                E = plate.E,
                InterEta = plate.InterEta,
                InterRho = plate.InterRho,
                InterG = plate.InterG,
                InterH = plate.InterH,
                Material = plate.Material
            };

            return plateNew;
        }
    }
}