using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VCLWebAPI.Mappers.System;
using VCLWebAPI.Models.Edmx;
using VCLWebAPI.Models.Systems;
using Newtonsoft.Json;

namespace VCLWebAPI.Services
{
    public class GlassService : IDisposable
    {
        private VCLDesignDBEntities _db;

        public GlassService()
        {
            _db = new VCLDesignDBEntities();
        }

        public string GetGlassInfo(string applicationName)
        {
            string response = "";
            if (String.Equals(applicationName, "BPS", StringComparison.OrdinalIgnoreCase))
            {
                List<GlassBPS> glassList = _db.GlassBPS.ToList();
                if (glassList == null)
                {
                    throw new InvalidDataException();
                }
                List<GlassBPSApiModel> articles = glassList.Select(x => new GlassBPSApiModel(x)).ToList();
                response = JsonConvert.SerializeObject(articles);
            }
            else if (String.Equals(applicationName, "SRS", StringComparison.OrdinalIgnoreCase))
            {
                List<GlassSRS> glassList = _db.GlassSRS.ToList();
                if (glassList == null)
                {
                    throw new InvalidDataException();
                }
                List<GlassSRSApiModel> articles = glassList.Select(x => new GlassSRSApiModel(x)).ToList();
                response = JsonConvert.SerializeObject(articles);
            }
            return response;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ArticleService() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}