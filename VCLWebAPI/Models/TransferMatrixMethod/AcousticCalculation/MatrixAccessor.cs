using System;

namespace VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation
{
    internal class MatrixAccessor<T>
    {
        public Action<int, T> Set { get; set; }
        public Func<int, T> Get { get; set; }
    }
}