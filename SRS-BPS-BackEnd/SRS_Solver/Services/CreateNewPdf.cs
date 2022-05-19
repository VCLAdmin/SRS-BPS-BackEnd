namespace SRS_Solver.Services
{
    internal class CreateNewPdf
    {
        private string source;
        private string outputFile;

        public CreateNewPdf(string source, string outputFile)
        {
            this.source = source;
            this.outputFile = outputFile;
        }
    }
}