using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Web.Hosting;
using System.Threading;
using BpsUnifiedModelLib;

namespace SBA.Services
{
    public class PdfService

    // 
    // This service creates and saves a pdf file of the input and output data.
    // 
    //     Steps:
    //
    //      1. copy the background form to a temperary location
    //      2. Get user to specify a filename
    //      3. Fill the form entries in the pdf File with the results
    //      4. Display the graphics data.
    //

    {
        // --------- Main entery point ------------------------------------------------
        public PdfService()
        {
            _resourceFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\");
        }

        public bool UpdateUserNote(string pdfFilePath, string UserNotes)
        {
            PdfDocument report = PdfReader.Open(pdfFilePath, PdfDocumentOpenMode.Modify);
            if (report is null) return false;
            string UserNotesFieldName = GetFullFieldName(report, "UserNotes");
            InsertValue(report, UserNotesFieldName, UserNotes);
            // flatten and close the pdf file
            PdfSecuritySettings securitySettings = report.SecuritySettings;
            securitySettings.PermitFormsFill = false;

            // Restrict some rights.
            securitySettings.PermitAccessibilityExtractContent = false;
            securitySettings.PermitAnnotations = false;
            securitySettings.PermitAssembleDocument = false;
            securitySettings.PermitExtractContent = false;
            securitySettings.PermitFormsFill = false;
            securitySettings.PermitFullQualityPrint = true;
            securitySettings.PermitModifyDocument = false;
            securitySettings.PermitPrint = true;

            report.Flatten();
            report.Save(pdfFilePath);
            report.Close();

            return true;
        }

        public bool UpdateConfigurationName(string pdfFilePath, string configurationName)
        {
            PdfDocument report = PdfReader.Open(pdfFilePath, PdfDocumentOpenMode.Modify);
            if (report is null) return false;
            string CoverPageConfigurationFieldName = GetFullFieldName(report, "CoverPageConfiguration");
            string ConfigurationNameFieldName = GetFullFieldName(report, "ConfigurationName");

            InsertValue(report, CoverPageConfigurationFieldName, configurationName);
            InsertValue(report, ConfigurationNameFieldName, configurationName);
            // flatten and close the pdf file
            PdfSecuritySettings securitySettings = report.SecuritySettings;
            securitySettings.PermitFormsFill = false;

            // Restrict some rights.
            securitySettings.PermitAccessibilityExtractContent = false;
            securitySettings.PermitAnnotations = false;
            securitySettings.PermitAssembleDocument = false;
            securitySettings.PermitExtractContent = false;
            securitySettings.PermitFormsFill = false;
            securitySettings.PermitFullQualityPrint = true;
            securitySettings.PermitModifyDocument = false;
            securitySettings.PermitPrint = true;

            report.Flatten();
            report.Save(pdfFilePath);
            report.Close();

            return true;
        }

        public bool UpdateProjectInfo(string pdfFilePath, string ProjectName, string Location)
        {
            PdfDocument report = PdfReader.Open(pdfFilePath, PdfDocumentOpenMode.Modify);
            if (report is null) return false;
            if (!String.IsNullOrEmpty(ProjectName))
            {
                string ProjectNameFieldName = GetFullFieldName(report, "ProjectName");
                string CoverPageProjectNameFieldName = GetFullFieldName(report, "CoverPageProjectName");
                InsertValue(report, ProjectNameFieldName, ProjectName);
                InsertValue(report, CoverPageProjectNameFieldName, ProjectName);
            }
            if (!String.IsNullOrEmpty(Location))
            {
                string LocationFieldName = GetFullFieldName(report, "Location");
                string CoverPageLocationFieldName = GetFullFieldName(report, "CoverPageLocation");
                InsertValue(report, LocationFieldName, Location);
                InsertValue(report, CoverPageLocationFieldName, Location);
            }
            // flatten and close the pdf file
            PdfSecuritySettings securitySettings = report.SecuritySettings;
            securitySettings.PermitFormsFill = false;

            // Restrict some rights.
            securitySettings.PermitAccessibilityExtractContent = false;
            securitySettings.PermitAnnotations = false;
            securitySettings.PermitAssembleDocument = false;
            securitySettings.PermitExtractContent = false;
            securitySettings.PermitFormsFill = false;
            securitySettings.PermitFullQualityPrint = true;
            securitySettings.PermitModifyDocument = false;
            securitySettings.PermitPrint = true;

            report.Flatten();
            report.Save(pdfFilePath);
            report.Close();

            return true;
        }

        private string GetFullFieldName(PdfDocument myTemplate, string shortFieldName)
        {
            string fullFieldName = myTemplate.AcroForm.Fields.Names.FirstOrDefault(x => x.Contains(shortFieldName));
            return fullFieldName;
        }

        public void InsertValue(PdfDocument myTemplate, string fieldName, string fieldValue, bool isRed = false)
        {
            try
            {

                PdfAcroForm form = myTemplate.AcroForm;

                if (form.Elements.ContainsKey("/NeedAppearances"))
                {
                    form.Elements["/NeedAppearances"] = new PdfSharp.Pdf.PdfBoolean(true);
                }
                else
                {
                    form.Elements.Add("/NeedAppearances", new PdfSharp.Pdf.PdfBoolean(true));
                }

                // Get all form fields of the whole document

                PdfAcroField.PdfAcroFieldCollection fields = form.Fields;

                // this sets the value for the field selected
                if (fieldValue == null) fieldValue = "";

                PdfAcroField field = fields[fieldName];
                PdfTextField txtField;
                if ((txtField = field as PdfTextField) != null)
                {
                    txtField.ReadOnly = false;

                    if (fieldValue.Contains(" > 1.0     NG") || isRed)
                    {
                        txtField.Elements.SetString(PdfTextField.Keys.DA, "/UniversForSchueco-630Bold 7 Tf 1 0 0 rg");
                    }
                    txtField.Value = new PdfString(fieldValue);
                    txtField.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        string _resourceFolderPath;
    }
}
