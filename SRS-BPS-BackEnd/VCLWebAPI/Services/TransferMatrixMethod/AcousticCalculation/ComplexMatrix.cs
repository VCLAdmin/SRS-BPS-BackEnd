using MathNet.Numerics.LinearAlgebra;
using System;
using System.Numerics;
using VCLWebAPI.Models.TransferMatrixMethod.AcousticCalculation;

namespace VCLWebAPI.Services.TransferMatrixMethod.AcousticCalculation
{
    public class ComplexMatrix
    {
        /// <summary>
        /// Create a 2-D matrix with all zeros.
        /// </summary>
        /// <returns>The d.</returns>
        /// <param name="X">First dimension.</param>
        /// <param name="Y">Second dimension.</param>
        public static Complex[][] Create2D(int X, int Y)
        {
            Complex[][] result = new Complex[X][];
            for (int i = 0; i < X; ++i)
                result[i] = new Complex[Y];
            return result;
        }

        /// <summary>
        /// Create a 2-D dentity matrix.
        /// </summary>
        /// <returns>The dentity matrix.</returns>
        /// <param name="X">The dimension.</param>
        public static Complex[][] Create2DIdentity(int X)
        {
            var result = Create2D(X, X);
            for (int i = 0; i < X; i++)
                result[i][i] = new Complex(1.0, 0.0);
            return result;
        }

        public static Complex[][] Create2DSymmetric(int X)
        {
            var result = ComplexMatrix.Create2D(X, X);
            Random rnd = new Random();
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    if (i != j)
                    {
                        result[i][j] = result[j][i] = new Complex(rnd.NextDouble(), rnd.NextDouble());
                    }
                    else result[i][j] = new Complex(rnd.NextDouble(), rnd.NextDouble());
                }
            }
            return result;
        }

        public static Complex[][][] Create3D(int X, int Y, int Z)
        {
            Complex[][][] result = new Complex[X][][];
            for (int i = 0; i < X; ++i)
                result[i] = Create2D(Y, Z);
            return result;
        }

        public static Complex[][][][] Create4D(int X1, int X2, int X3, int X4)
        {
            Complex[][][][] result = new Complex[X1][][][];
            for (int i = 0; i < X1; ++i)
                result[i] = Create3D(X2, X3, X4);
            return result;
        }

        /// <summary>
        /// Copy the specified matrix.
        /// </summary>
        /// <returns>The copy.</returns>
        /// <param name="matrix">Matrix.</param>
        public static Complex[][] Copy(Complex[][] matrix)
        {
            int n = matrix.Length;
            Complex[][] result = Create2D(n, n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    result[i][j] = matrix[i][j];
            return result;
        }

        public static Complex[][] Flatten(Complex[][][][] matrix)
        {
            int Y1 = matrix.Length;
            int Y2 = matrix[0].Length;
            int Y3 = matrix[0][0].Length;
            int Y4 = matrix[0][0][0].Length;
            Complex[][] result = Create2D(Y1 * Y2, Y3 * Y4);
            for (int y1 = 0; y1 < Y1; y1++)
            {
                for (int y2 = 0; y2 < Y2; y2++)
                {
                    for (int y3 = 0; y3 < Y3; y3++)
                    {
                        for (int y4 = 0; y4 < Y3; y4++)
                        {
                            result[y1 * Y2 + y2][y3 * Y4 + y3] += matrix[y1][y2][y3][y4];
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Matrix as string.
        /// </summary>
        /// <returns>The as string.</returns>
        /// <param name="matrix">Matrix.</param>
        public static string MatrixAsString(Complex[][] matrix)
        {
            string s = "";
            for (int i = 0; i < matrix.Length; ++i)
            {
                for (int j = 0; j < matrix[i].Length; ++j)
                {
                    s += matrix[i][j].ToString("E4").PadLeft(8) + " ";
                }
                s += Environment.NewLine;
            }
            return s;
        }

        // TODO
        public static string MatrixAsString(Complex[,] matrix)
        {
            int aRows = matrix.GetLength(0);
            int aCols = matrix.GetLength(1);

            string s = "";
            for (int i = 0; i < aRows; ++i)
            {
                for (int j = 0; j < aCols; ++j)
                {
                    s += matrix[i, j].ToString("F3").PadLeft(8) + " ";
                }
                s += Environment.NewLine;
            }
            return s;
        }

        /// <summary>
        /// 2D matrix multiplucation.
        /// </summary>
        /// <returns>The product.</returns>
        /// <param name="matrixA">Matrix a.</param>
        /// <param name="matrixB">Matrix b.</param>
        public static Complex[][] MatrixProduct(Complex[][] matrixA,
                                                Complex[][] matrixB)
        {
            int aRows = matrixA.Length;
            int aCols = matrixA[0].Length;
            int bRows = matrixB.Length;
            int bCols = matrixB[0].Length;
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices");

            Complex[][] result = Create2D(aRows, bCols);
            for (int i = 0; i < aRows; ++i) // each row of A
                for (int j = 0; j < bCols; ++j) // each col of B
                    for (int k = 0; k < aCols; ++k) // could use k < bRows
                        result[i][j] += matrixA[i][k] * matrixB[k][j];

            return result;
        }

        /// <summary>
        /// 2D matrix multiplucation.
        /// </summary>
        /// <returns>The product.</returns>
        /// <param name="matrixA">Matrix a.</param>
        /// <param name="matrixB">Matrix b.</param>
        public static Complex[,] MatrixProduct(Complex[,] matrixA,
                                                Complex[,] matrixB)
        {
            int aRows = matrixA.GetLength(0);
            int aCols = matrixA.GetLength(1);
            int bRows = matrixB.GetLength(0);
            int bCols = matrixB.GetLength(1);
            if (aCols != bRows)
                throw new Exception("Non-conformable matrices");

            Complex[,] result = new Complex[aRows, bCols];
            for (int i = 0; i < aRows; ++i) // each row of A
                for (int j = 0; j < bCols; ++j) // each col of B
                    for (int k = 0; k < aCols; ++k) // could use k < bRows
                        result[i, j] += matrixA[i, k] * matrixB[k, j];

            return result;
        }

        /// <summary>
        /// Remove the col'th column
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="SR"></param>
        /// <param name="ER"></param>
        /// <param name="SC"></param>
        /// <param name="EC"></param>
        /// <returns></returns>
        public static Complex[,] DeductMatrix(Complex[,] matrix, int col)
        {
            int rows = matrix.GetLength(0), cols = matrix.GetLength(1);
            Complex[,] result = new Complex[rows, cols - 1];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    result[i, j > col ? j - 1 : j] = matrix[i, j];
                }
            }
            return result;
        }

        public static Complex Determinant(Complex[,] matrix)
        {
            //int rows = matrix.GetLength(0), cols = matrix.GetLength(1);
            var m = CreateMatrix.DenseOfArray<Complex>(matrix);
            return m.Determinant();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="matrixA"></param>
        /// <param name="matrixB"></param>
        /// <returns></returns>
        /*
        public static Complex[,] CombineMatrix(Complex[,] matrixA,
                                                Complex[,] matrixB)
        {
            return result;
        }
        */

        /// <summary>
        /// The product of 2 matrices.
        /// Notice that it is not following the standard definition of matrix multiplication.
        /// </summary>
        /// <returns>The product.</returns>
        /// <param name="matrixA">X1 x X2.</param>
        /// <param name="matrixB">Y1 x Y2 x Y3 x Y4.</param>
        private static Complex[][] MatrixProduct(Complex[][] matrixA,
                                                Complex[][][][] matrixB)
        {
            int X1 = matrixA.Length;
            int X2 = matrixA[0].Length;
            int Y1 = matrixB.Length;
            int Y2 = matrixB[0].Length;
            if (X1 != Y1 || X2 != Y2)
                throw new Exception("Non-conformable matrices");

            int Y3 = matrixB[0][0].Length;
            int Y4 = matrixB[0][0][0].Length;
            Complex[][] result = Create2D(Y3, Y4);
            for (int y1 = 0; y1 < Y1; y1++)
            {
                for (int y2 = 0; y2 < Y2; y2++)
                {
                    for (int y3 = 0; y3 < Y3; y3++)
                    {
                        for (int y4 = 0; y4 < Y4; y4++)
                        {
                            result[y3][y4] += matrixA[y1][y2] * matrixB[y1][y2][y3][y4];
                        }
                    }
                }
            }
            return result;
        }

        // Compute the 0-indexed (row, col) from packed index
        private static Tuple<int, int> ConvertIndex(int index)
        {
            int row = index, col = 1, cnt = 1;
            while (row > cnt)
            {
                row -= cnt;
                cnt++;
                col++;
            }
            return new Tuple<int, int>(row - 1, col - 1);
        }

        private static T GetElement<T>(T[][] A, int index)
        {
            var indexTuple = ConvertIndex(index);
            return A[indexTuple.Item1][indexTuple.Item2];
        }

        private static void SetElement<T>(T[][] A, int index, T toValue)
        {
            var indexTuple = ConvertIndex(index);
            A[indexTuple.Item1][indexTuple.Item2] = toValue;
            A[indexTuple.Item2][indexTuple.Item1] = toValue;
        }

        // Get default accessor for 1-D array
        private static MatrixAccessor<T> GetAccessor<T>(T[] X)
        {
            return GetAccessor(X, 1);
        }

        // Get accessor for 1-D array with offset
        private static MatrixAccessor<T> GetAccessor<T>(T[] X, int offset)
        {
            return new MatrixAccessor<T>
            {
                Get = (int index) =>
                {
                    return X[index + offset - 2];
                },
                Set = (int index, T toValue) =>
                {
                    X[index + offset - 2] = toValue;
                }
            };
        }

        // Get default accessor for 2-D matrix
        private static MatrixAccessor<T> GetAccessor<T>(T[][] A)
        {
            return GetAccessor(A, 1);
        }

        // Get accessor for 2-D matrix with offset
        private static MatrixAccessor<T> GetAccessor<T>(T[][] A, int offset)
        {
            return new MatrixAccessor<T>
            {
                Get = (int index) =>
                {
                    return GetElement(A, index + offset - 1);
                },
                Set = (int index, T toValue) =>
                {
                    SetElement(A, index + offset - 1, toValue);
                }
            };
        }

        // Get accessor from accessor with offset, applicable to 1D and 2D
        private static MatrixAccessor<T> GetAccessor<T>(MatrixAccessor<T> A, int offset)
        {
            return new MatrixAccessor<T>
            {
                Get = (int index) =>
                {
                    return A.Get(index + offset - 1);
                },
                Set = (int index, T toValue) =>
                {
                    A.Set(index + offset - 1, toValue);
                }
            };
        }

        private static double Cabs1(Complex zdum)
        {
            return Math.Abs(zdum.Real) + Math.Abs(zdum.Imaginary);
        }

        private static int SubroutineIZAMAX(int n, MatrixAccessor<Complex> X, int incx)
        {
            int maxIndex = 1;
            for (int ix = incx + 1, icnt = 0; icnt < n - 1; ix += incx, icnt++)
            {
                if (Cabs1(X.Get(ix)) > Cabs1(X.Get(maxIndex))) maxIndex = ix;
            }
            return maxIndex;
        }

        private static void SubroutineZCOPY(int n, MatrixAccessor<Complex> X, int incx, MatrixAccessor<Complex> Y, int incy)
        {
            for (int ix = 1, iy = 1, icnt = 0; icnt < n;
                 ix += incx, iy += incy, icnt++)
            {
                Y.Set(iy, X.Get(ix));
            }
        }

        // Returns the complex dot product of 2 vectors
        private static Complex SubroutineZDOTU(int n, MatrixAccessor<Complex> X, int incx, MatrixAccessor<Complex> Y, int incy)
        {
            Complex dotProduct = new Complex(0.0, 0.0);
            for (int ix = 1, iy = 1, icnt = 0; icnt < n;
                     ix += incx, iy += incy, icnt++)
            {
                dotProduct += X.Get(ix) * Y.Get(iy);
            }
            return dotProduct;
        }

        private static void SubroutineZSWAP(int n, MatrixAccessor<Complex> X, int incx, MatrixAccessor<Complex> Y, int incy)
        {
            for (int ix = 1, iy = 1, icnt = 0; icnt < n; ix += incx, iy += incy, icnt++)
            {
                Complex temp = X.Get(ix);
                X.Set(ix, Y.Get(iy));
                Y.Set(iy, temp);
            }
        }

        // Scales a vector by a constant
        private static void SubroutineZSCAL(int n, Complex alpha, MatrixAccessor<Complex> X, int incx)
        {
            for (int ix = 1, icnt = 0; icnt < n; ix += incx, icnt++)
            {
                X.Set(ix, X.Get(ix) * alpha);
            }
        }

        private static void SubroutineZSPR(int n, Complex alpha, MatrixAccessor<Complex> X, int incx, MatrixAccessor<Complex> A)
        {
            // Parameters
            Complex ZERO = new Complex(0.0, 0.0), temp = new Complex();
            int i = 1, ix = 1, j = 1, jx = 1, k = 1, kk = 1, kx = 1;

            if (n == 0 || alpha == ZERO)
            {
                return;
            }

            // Set the start point in X if the increment is not unity.
            if (incx >= 0)
            {
                kx = 1 - (n - 1) * incx;
            }
            else
            {
                kx = 1;
            }

            // Start the operations
            kk = 1;
            if (incx == 1)
            {
                for (j = 1; j <= n; j++)
                {
                    if (X.Get(j) != ZERO)
                    {
                        temp = alpha * X.Get(j);
                        k = kk;
                        for (i = 1; i <= j - 1; i++)
                        {
                            A.Set(k, A.Get(k) + X.Get(i) * temp);
                            k++;
                        }
                        A.Set(kk + j - 1, A.Get(kk + j - 1) + X.Get(j) * temp);
                    }
                    else
                    {
                        A.Set(kk + j - 1, A.Get(kk + j - 1));
                    }
                    kk += j;
                }
            }
            else
            {
                jx = kx;
                for (j = 1; j <= n; j++)
                {
                    if (X.Get(jx) != ZERO)
                    {
                        temp = alpha * X.Get(jx);
                        ix = kx;
                        for (k = kk; k <= kk + j - 2; k++)
                        {
                            A.Set(k, A.Get(k) + X.Get(ix) * temp);
                            ix = ix + incx;
                        }
                        A.Set(kk + j - 1, A.Get(kk + j - 1) + X.Get(jx) * temp);
                    }
                    else
                    {
                        A.Set(kk + j - 1, A.Get(kk + j - 1));
                    }
                    jx += incx;
                    kk += j;
                }
            }
        }

        private static void SubroutineZSPTRF(int n, MatrixAccessor<Complex> A, MatrixAccessor<int> ipiv)
        {
            // Parameters
            const double ZERO = 0.0, ONE = 1.0, EIGHT = 8.0, SEVTEN = 17.0;
            Complex CONE = new Complex(1.0, 0.0);
            int i = 1, imax = 1, j = 1, jmax = 1, k = 1, kc = 1, kk = 1,
                knc = 1, kp = 1, kpc = 1, kstep = 1, kx = 1;
            double absakk, alpha, colmax, rowmax;
            Complex d11, d12, d22, r1, t, wk, wkm1;

            alpha = (ONE + Math.Sqrt(SEVTEN)) / EIGHT;

            // Factorize A as U*D*U**T using the upper triangle of A
            // K is the main loop index, decreasing from N to 1 in steps of 1 or 2
            k = n;
            kc = (n - 1) * n / 2 + 1;
        L_10:
            knc = kc;
            if (k < 1)
            {
                goto L_110; // exist
            }
            kstep = 1;

            // Determine rows and columns to be interchanged and whether
            // 1-by-1 or 2-by-2 pivot block will be used
            absakk = Cabs1(A.Get(kc + k - 1));

            // imax is the row-index of the largest off-diagonal element in
            // column K, and colmax is its absolute value
            if (k > 1)
            {
                imax = SubroutineIZAMAX(k - 1, GetAccessor<Complex>(A, kc), 1);
                colmax = Cabs1(A.Get(kc + imax - 1));
            }
            else
            {
                colmax = ZERO;
            }

            if (Math.Max(absakk, colmax).Equals(ZERO))
            {
                kp = k;
            }
            else
            {
                if (absakk >= alpha * colmax)
                {
                    // No interchange, use 1-by-1 pivot block
                    kp = k;
                }
                else
                {
                    rowmax = ZERO;
                    jmax = imax;
                    kx = imax * (imax + 1) / 2 + imax;
                    for (j = imax + 1; j <= k; j++)
                    {
                        if (Cabs1(A.Get(kx)) > rowmax)
                        {
                            rowmax = Cabs1(A.Get(kx));
                            jmax = j;
                        }
                        kx = kx + j;
                    }
                    kpc = (imax - 1) * imax / 2 + 1;
                    if (imax > 1)
                    {
                        jmax = SubroutineIZAMAX(imax - 1, GetAccessor<Complex>(A, kpc), 1);
                        rowmax = Math.Max(rowmax, Cabs1(A.Get(kpc + jmax - 1)));
                    }

                    if (absakk >= alpha * colmax * (colmax / rowmax))
                    {
                        // no interchange, use 1-by-1 pivot block
                        kp = k;
                    }
                    else if (Cabs1(A.Get(kpc + imax - 1)) >= alpha * rowmax)
                    {
                        // interchange rows and columns k and imax, use 1-by-1 pivot block
                        kp = imax;
                    }
                    else
                    {
                        // interchange rows and columns k - 1 and imax, use 2-by-2 pivot block
                        kp = imax;
                        kstep = 2;
                    }
                }
                kk = k - kstep + 1;
                if (kstep == 2)
                {
                    knc = knc - k + 1;
                }
                if (kp != kk)
                {
                    // Interchange rows and columns kk and kp in the leading submatrix A(1:k, 1:k)
                    SubroutineZSWAP(kp - 1, GetAccessor<Complex>(A, knc), 1, GetAccessor<Complex>(A, kpc), 1);
                    kx = kpc + kp - 1;
                    for (j = kp + 1; j <= kk - 1; j++)
                    {
                        kx = kx + j - 1;
                        SubroutineZSWAP(1, GetAccessor<Complex>(A, knc + j - 1), 1, GetAccessor<Complex>(A, kx), 1);
                    }

                    SubroutineZSWAP(1, GetAccessor<Complex>(A, knc + kk - 1), 1, GetAccessor<Complex>(A, kpc + kp - 1), 1);
                    if (kstep == 2)
                    {
                        SubroutineZSWAP(1, GetAccessor<Complex>(A, kc + k - 2), 1, GetAccessor<Complex>(A, kc + kp - 1), 1);
                    }
                }

                // Update the leading submatrix
                if (kstep == 1)
                {
                    r1 = CONE / A.Get(kc + k - 1);
                    SubroutineZSPR(k - 1, -r1, GetAccessor<Complex>(A, kc), 1, A);

                    // Store U(k) in column k
                    SubroutineZSCAL(k - 1, r1, GetAccessor<Complex>(A, kc), 1);
                }
                else
                {
                    // 2-by-2 TBD
                    if (k > 2)
                    {
                        d12 = A.Get(k - 1 + (k - 1) * k / 2);
                        d22 = A.Get(k - 1 + (k - 2) * (k - 1) / 2) / d12;
                        d11 = A.Get(k + (k - 1) * k / 2) / d12;
                        t = CONE / (d11 * d22 - CONE);
                        d12 = t / d12;

                        for (j = k - 2; j >= 1; j--)
                        {
                            wkm1 = d12 * (d11 * A.Get(j + (k - 2) * (k - 1) / 2) - A.Get(j + (k - 1) * k / 2));
                            wk = d12 * (d22 * A.Get(j + (k - 1) * k / 2) - A.Get(j + (k - 2) * (k - 1) / 2));
                            for (i = j; i >= 1; i--)
                            {
                                A.Set(i + (j - 1) * j / 2, A.Get(i + (j - 1) * j / 2)
                                      - A.Get(i + (k - 1) * k / 2) * wk
                                      - A.Get(i + (k - 2) * (k - 1) / 2) * wkm1);
                            }
                            A.Set(j + (k - 1) * k / 2, wk);
                            A.Set(j + (k - 2) * (k - 1) / 2, wkm1);
                        }
                    }
                }
            }

            // Store details of the interchanges in ipiv
            if (kstep == 1)
            {
                ipiv.Set(k, kp);
            }
            else
            {
                ipiv.Set(k, -kp);
                ipiv.Set(k - 1, -kp);
            }

            // Decrease k and return to the start of the main loop
            k = k - kstep;
            kc = knc - k;
            goto L_10;

        L_110:
            return;
        }

        private static void SubroutineZSPTRI(int n, MatrixAccessor<Complex> A, MatrixAccessor<int> ipiv, MatrixAccessor<Complex> work)
        {
            // Parameters
            int info = 1;
            Complex ONE = new Complex(1.0, 0.0), ZERO = new Complex(0.0, 0.0);
            int j = 1, k = 1, kc = 1, kcnext = 1, kp = 1, kpc = 1, kstep = 1, kx = 1;
            Complex ak, akkp1, akp1, d, t, temp;

            if (n == 0)
            {
                return;
            }

            // Check that the diagonal matrix D is nonsingular.
            kp = n * (n + 1) / 2;
            for (info = n; info >= 1; info--)
            {
                if (ipiv.Get(info) > 0 && A.Get(kp) == 0)
                {
                    throw new Exception("Diagonal D is singular.");
                }
                kp = kp - info;
            }
            info = 0;

            // Compute inv(A) from the factorization A = U*D*U**T
            // k is the main loop index, increasing from 1 to N in steps of 1 or
            // 2, depending on the size of the diagonal blocks.
            k = 1;
            kc = 1;
        L_30:
            // if k > n, exit from loop
            if (k > n) goto L_50;

            kcnext = kc + k;
            if (ipiv.Get(k) > 0)
            {
                A.Set(kc + k - 1, ONE / A.Get(kc + k - 1));

                // Compute column k of the inverse
                if (k > 1)
                {
                    SubroutineZCOPY(k - 1, GetAccessor<Complex>(A, kc), 1, work, 1);
                    SubroutineZSPMV(k - 1, -ONE, A, work, 1, ZERO, GetAccessor<Complex>(A, kc), 1);
                    A.Set(kc + k - 1, A.Get(kc + k - 1) - SubroutineZDOTU(k - 1, work, 1, GetAccessor<Complex>(A, kc), 1));
                }
                kstep = 1;
            }
            else
            {
                // 2-by-2 diagonal block
                // Invert the diagonal block
                t = A.Get(kcnext + k - 1);
                ak = A.Get(kc + k - 1) / t;
                akp1 = A.Get(kcnext + k) / t;
                akkp1 = A.Get(kcnext + k - 1) / t;
                d = t * (ak * akp1 - ONE);
                A.Set(kc + k - 1, akp1 / d);
                A.Set(kcnext + k, ak / d);
                A.Set(kcnext + k - 1, -akkp1 / d);

                // Compute columns k and k + 1 of the inverse
                if (k > 1)
                {
                    SubroutineZCOPY(k - 1, GetAccessor<Complex>(A, kc), 1, work, 1);
                    SubroutineZSPMV(k - 1, -ONE, A, work, 1, ZERO, GetAccessor<Complex>(A, kc), 1);
                    A.Set(kc + k - 1, A.Get(kc + k - 1) - SubroutineZDOTU(k - 1, work, 1, GetAccessor<Complex>(A, kc), 1));
                    A.Set(kcnext + k - 1,
                          A.Get(kcnext + k - 1)
                          - SubroutineZDOTU(k - 1, GetAccessor<Complex>(A, kc), 1, GetAccessor<Complex>(A, kcnext), 1));
                    SubroutineZCOPY(k - 1, GetAccessor<Complex>(A, kcnext), 1, work, 1);
                    SubroutineZSPMV(k - 1, -ONE, A, work, 1, ZERO, GetAccessor<Complex>(A, kcnext), 1);
                    A.Set(kcnext + k,
                          A.Get(kcnext + k) - SubroutineZDOTU(k - 1, work, 1, GetAccessor<Complex>(A, kcnext), 1));
                }
                kstep = 2;
                kcnext += k + 1;
            }

            kp = Math.Abs(ipiv.Get(k));
            if (kp != k)
            {
                // Interchange rows and columns k and kp in the leading
                // submatrix A(1:k+1, 1:k+1)
                kpc = (kp - 1) * kp / 2 + 1;
                SubroutineZSWAP(kp - 1, GetAccessor<Complex>(A, kc), 1, GetAccessor<Complex>(A, kpc), 1);
                kx = kpc + kp - 1;
                for (j = kp + 1; j <= k - 1; j++)
                {
                    kx += j - 1;
                    temp = A.Get(kc + j - 1);
                    A.Set(kc + j - 1, A.Get(kx));
                    A.Set(kx, temp);
                }
                temp = A.Get(kc + k - 1);
                A.Set(kc + k - 1, A.Get(kpc + kp - 1));
                A.Set(kpc + kp - 1, temp);
                if (kstep == 2)
                {
                    temp = A.Get(kc + k + k - 1);
                    A.Set(kc + k + k - 1, A.Get(kc + k + kp - 1));
                    A.Set(kc + k + kp - 1, temp);
                }
            }

            k += kstep;
            kc = kcnext;
            goto L_30;

        L_50:
            return;
        }

        private static void SubroutineZSPMV(int n, Complex alpha, MatrixAccessor<Complex> A,
                                            MatrixAccessor<Complex> X, int incx, Complex beta,
                                            MatrixAccessor<Complex> Y, int incy)
        {
            Complex ONE = new Complex(1.0, 0.0), ZERO = new Complex(0.0, 0.0);
            int i = 1, ix = 1, iy = 1, j = 1, jx = 1, jy = 1, k = 1, kk = 1, kx = 1, ky = 1;
            Complex temp1, temp2;

            // TBD sanity check

            // Quick return if possible
            if (n == 0 || alpha == ZERO || beta == ONE)
            {
                return;
            }

            // Set up the start points in X and Y
            if (incx > 0)
            {
                kx = 1;
            }
            else
            {
                kx = 1 - (n - 1) * incx;
            }

            if (incy > 0)
            {
                ky = 1;
            }
            else
            {
                ky = 1 - (n - 1) * incy;
            }

            // Start the operations
            if (beta != ONE)
            {
                if (incy == 1)
                {
                    if (beta == ZERO)
                    {
                        for (i = 1; i <= n; i++)
                        {
                            Y.Set(i, ZERO);
                        }
                    }
                    else
                    {
                        for (i = 1; i <= n; i++)
                        {
                            Y.Set(i, beta * Y.Get(i));
                        }
                    }
                }
                else
                {
                    iy = ky;
                    if (beta == ZERO)
                    {
                        for (i = 1; i <= n; i++)
                        {
                            Y.Set(iy, ZERO);
                            iy += incy;
                        }
                    }
                    else
                    {
                        for (i = 1; i <= n; i++)
                        {
                            Y.Set(iy, beta * Y.Get(iy));
                            iy += incy;
                        }
                    }
                }
            }

            if (alpha == ZERO) return;

            kk = 1;
            // From y when AP contains the upper triangle
            if (incx == 1 && incy == 1)
            {
                for (j = 1; j <= n; j++)
                {
                    temp1 = alpha * X.Get(j);
                    temp2 = ZERO;
                    k = kk;
                    for (i = 1; i <= j - 1; i++)
                    {
                        Y.Set(i, Y.Get(i) + temp1 * A.Get(k));
                        temp2 += A.Get(k) * X.Get(i);
                        k++;
                    }
                    Y.Set(j, Y.Get(j) + temp1 * A.Get(kk + j - 1) + alpha * temp2);
                    kk += j;
                }
            }
            else
            {
                jx = kx;
                jy = ky;
                for (j = 1; j <= n; j++)
                {
                    temp1 = alpha * X.Get(jx);
                    temp2 = ZERO;
                    ix = kx;
                    iy = ky;
                    for (k = kk; k <= kk + j - 2; k++)
                    {
                        Y.Set(iy, Y.Get(iy) + temp1 * A.Get(k));
                        temp2 += A.Get(k) * X.Get(ix);
                        ix += incx;
                        iy += incy;
                    }
                    Y.Set(jy, Y.Get(jy) + temp1 * A.Get(kk + j - 1) + alpha * temp2);
                    jx += incx;
                    jy += incy;
                    kk += j;
                }
            }
        }

        public static void GetFactorization(Complex[][] A, out int[] ipiv)
        {
            int n = A.Length;
            ipiv = new int[n];
            SubroutineZSPTRF(n, GetAccessor<Complex>(A), GetAccessor<int>(ipiv));
        }

        public static void GetInverse(Complex[][] A, out Complex[] work)
        {
            int n = A.Length;
            work = new Complex[n];
            int[] ipiv;
            GetFactorization(A, out ipiv);
            SubroutineZSPTRI(n, GetAccessor<Complex>(A), GetAccessor<int>(ipiv), GetAccessor<Complex>(work));
        }
    }
}