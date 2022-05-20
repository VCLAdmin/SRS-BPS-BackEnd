using BpsUnifiedModelLib;
using NPOI.XSSF.UserModel;
using SRS_Solver.Services;
using System;
using System.Collections.Generic;
using System.IO;
using static SRS_Solver.Services.BOM;

namespace SRS_Solver
{
    public class SRSAnalysis
    {
        /// <summary>
        /// Generate Proposal using JSON file stored in Resources (For testing purposes only)
        /// </summary>
        /// <returns></returns>
        public MemoryStream GenerateProposal_FromJsonFile( string ProposalOrBOM)
        {
            try
            {
                string inputData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\DefaultModels\ASE\2D1i_DefaultModel.json");
                string _resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\");
                string template = _resourceFolderPath + @"srs-templates\SRS_Proposal_AWS75SI+.PDF";
                var model = Proposal.LoadNewModelFromFile(inputData);

                string outputFile = "";
                if (ProposalOrBOM == "BOM")
                {
                    outputFile = GetBOMPath("05-Excel", DateTime.Now.Ticks.ToString()); //saves to 05-Excel folder on local
                }
                else
                {
                    outputFile = GetPdfPath("04-Output", DateTime.Now.Ticks.ToString()); //saves to 04-Output folder on local
                }

                return BuildProposalBOM(template, model, outputFile, ProposalOrBOM);
                

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public MemoryStream GenerateProposal_FromJsonString(string jsonString, string templateName = "SRS_Proposal_AWS75SI+.PDF")
        {
            try
            {
                string _resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\");
                string template = _resourceFolderPath + @"srs-templates\" + templateName;
                // Load the Unified Model  
                var model = Proposal.LoadModelFromJsonString(jsonString);
                string outputFile = GetPdfPath("04-Output", DateTime.Now.Ticks.ToString());
                return BuildProposal(template, model, outputFile);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Generate Proposal on Server
        /// </summary>
        /// <param name="problemGuid"></param>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public MemoryStream GenerateProposal(string problemGuid, string jsonString, string ProposalOrBOM) 
        {
            try
            {
                //string problemGuid = unifiedProblem.ProblemGuid.ToString();
                //string jsonString = unifiedProblem.UnifiedModel;
                string _resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\");
                string template = _resourceFolderPath + @"srs-templates\SRS_Proposal_AWS75SI+.PDF";
                // Load the Unified Model  
                var model = Proposal.LoadModelFromJsonString(jsonString);
                string outputFile = "";
                if (ProposalOrBOM == "BOM")
                {
                     outputFile = GetBOMPath(problemGuid, model.ProblemSetting.ConfigurationName); //should save Excel to same folder as PDF
                }
                else
                {
                     outputFile = GetPdfPath(problemGuid, model.ProblemSetting.ConfigurationName);
                }
                
                
                
                return BuildProposalBOM(template, model, outputFile, ProposalOrBOM);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        /// <summary>
        /// Build the proposal
        /// </summary>
        /// <param name="template">Default is AWS, will switch to ADS based on Door System count</param>
        /// <param name="model">JSON File</param>
        /// <param name="outputFile">PDF output</param>
        /// <returns></returns>
        private static MemoryStream BuildProposal(string template, BpsUnifiedModel model, string outputFile)
        {

            // Create Trace Data
            var td = Proposal.CreateTraceData(model);

            // Get Control Points
            var points = Proposal.ControlPoints(model, td.Project.Scale);


            // Create PDF file

            //Check for Door Systems to decide whether to switch to ADS template for PDF
            if (model.ModelInput.Geometry.DoorSystems != null)
            {
                string _resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\");
                template = _resourceFolderPath + @"srs-templates\SRS_Proposal_ADS75SI+.PDF";                
            }
            else if (model.ModelInput.Geometry.SlidingDoorSystems != null)
            {
                string _resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\");
                template = _resourceFolderPath + @"srs-templates\SRS_Proposal_ASE.PDF";
                
                foreach(var v in td.Vents)
                {
                    var pointA = v.CornerPoints[0];
                    var pointB = v.CornerPoints[2];

                    points.Add(pointA);
                    points.Add(pointB);
                }
                
               
            }
            
            var pdf = PDF.Create(template, outputFile);

            // Write Specifications
            PDF.Specifications(td, pdf, points);

            // Add Elevation 
            PDF.DrawElevation(points, td, pdf);

            // Close PDF
            PDF.Close(pdf, outputFile);
            return Proposal.GetReport(outputFile);
        }

        /// <summary>
        /// Generate Proposal or BOM based on what user has selected
        /// </summary>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="outputFile"></param>
        /// <param name="ProposalOrBOM"></param>
        /// <returns></returns>
        private static MemoryStream BuildProposalBOM(string template, BpsUnifiedModel model, string outputFile, string ProposalOrBOM)
        {
            BOM.BOMExcelWorkbook.BOMWorkbook = new XSSFWorkbook();
            BOM.BOMExcelWorkbook.we = BOM.BOMExcelWorkbook.BOMWorkbook.CreateSheet("Extrusions");

            // Create Trace Data
            TraceData td = new TraceData();
            List<TraceData.Point> points = new List<TraceData.Point>();
            template = CreateTraceData(template, model, out td, out points);

            if (ProposalOrBOM == "BOM")
            {
                //BOM
                BOM.WriteBOMWorkbook(td);

                BOM.SaveBOMWorkbook(outputFile);

                var BOMOutput = BOM.GetReport(outputFile);

                return BOMOutput;
            }
            else
            {
                var pdf = PDF.Create(template, outputFile);

                // Write Specifications
                PDF.Specifications(td, pdf, points);

                // Add Elevation 
                PDF.DrawElevation(points, td, pdf);

                // Close PDF
                PDF.Close(pdf, outputFile);

                var ProposalOutput = Proposal.GetReport(outputFile);

                return ProposalOutput;
            }


        }

        private static string CreateTraceData(string template, BpsUnifiedModel model, out TraceData td, out List<TraceData.Point> points)
        {
            td = Proposal.CreateTraceData(model);

            // Get Control Points
            points = Proposal.ControlPoints(model, td.Project.Scale);


            // Create PDF file

            //Check for Door Systems to decide whether to switch to ADS template for PDF
            if (model.ModelInput.Geometry.DoorSystems != null)
            {
                string _resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\");
                template = _resourceFolderPath + @"srs-templates\SRS_Proposal_ADS75SI+.PDF";
            }
            else if (model.ModelInput.Geometry.SlidingDoorSystems != null)
            {
                string _resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\");
                template = _resourceFolderPath + @"srs-templates\SRS_Proposal_ASE.PDF";

                foreach (var v in td.Vents)
                {
                    var pointA = v.CornerPoints[0];
                    var pointB = v.CornerPoints[2];

                    points.Add(pointA);
                    points.Add(pointB);
                }


            }

            return template;
        }

        // Returns the path to the saved PDF.
        public string GetPdfPath(string problemGuid, string problemName)
        {
            var error = string.Empty;
            string pdfFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                $"Content\\srs-proposal\\{problemGuid}\\");
            Directory.CreateDirectory(pdfFolderPath);
            string pdfFileName = problemName + "_Proposal.pdf";
            string pdfFilePath = pdfFolderPath + pdfFileName;
            return pdfFilePath;
        }
        public string GetBOMPath(string problemGuid, string problemName)
        {
            var error = string.Empty;
            string BOMFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                $"Content\\srs-proposal\\{problemGuid}\\");
            Directory.CreateDirectory(BOMFolderPath);
            string BOMFileName = problemName + "_BOM.xlsx";
            string BOMFilePath = BOMFolderPath + BOMFileName;
            return BOMFilePath;
        }
    }
}
