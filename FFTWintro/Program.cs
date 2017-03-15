using System;
using System.Runtime.InteropServices;
using FFTWSharp;

namespace FFTWintro
{
    internal class Program
    {
        private static void Main()
        {
            // Test 1: Real input
            // Define an array of double-precision numbers
            double[] x = { 1.2, 3.4, 5.6, 7.8 };
            // Compute the FFT
            var dft = Fft(x, true);     // true = real input
            // Format and display the results of the FFT
            Console.WriteLine("Test 1: Real input");
            Console.WriteLine();
            Console.WriteLine("FFT =");
            DisplayComplex(dft);
            Console.WriteLine();
            // Compute the IFFT
            var idft = Ifft(dft);
            // Format and display the results of the IFFT
            Console.WriteLine("IFFT =");
            DisplayReal(idft);
            Console.WriteLine();
            // Test 2: Complex input
            x = new double[] { 1, -2, 3, 4, 5, -6, 7, 8 };  // that is, 1 - 2i, . . . , 7 + 8i
            dft = Fft(x, false);        // false = complex input
            Console.WriteLine("Test 2: Complex input");
            Console.WriteLine();
            Console.WriteLine("FFT =");
            DisplayComplex(dft);
            Console.WriteLine();
            idft = Ifft(dft);
            // Format and display the results of the IFFT
            Console.WriteLine("IFFT =");
            DisplayComplex(idft);
            // Prevent the console window from closing immediately
            Console.ReadKey();
        }

        /// <summary>
        /// Computes the fast Fourier transform of a 1-D array of real or complex numbers.
        /// </summary>
        /// <param name="data">Input data.</param>
        /// <param name="real">Real or complex input flag.</param>
        /// <returns>Returns the FFT.</returns>
        private static double[] Fft(double[] data, bool real)
        {
            // If the input is real, make it complex
            if (real)
                data = ToComplex(data);
            // Get the length of the array
            int n = data.Length;
            /* Allocate an unmanaged memory block for the input and output data.
             * (The input and output are of the same length in this case, so we can use just one memory block.) */
            IntPtr ptr = fftw.malloc(n * sizeof(double));
            // Pass the managed input data to the unmanaged memory block
            Marshal.Copy(data, 0, ptr, n);
            // Plan the FFT and execute it (n/2 because complex numbers are stored as pairs of doubles)
            IntPtr plan = fftw.dft_1d(n / 2, ptr, ptr, fftw_direction.Forward, fftw_flags.Estimate);
            fftw.execute(plan);
            // Create an array to store the output values
            var fft = new double[n];
            // Pass the unmanaged output data to the managed array
            Marshal.Copy(ptr, fft, 0, n);
            // Do some cleaning
            fftw.destroy_plan(plan);
            fftw.free(ptr);
            fftw.cleanup();
            // Return the FFT output
            return fft;
        }

        /// <summary>
        /// Computes the inverse fast Fourier transform of a 1-D array of complex numbers.
        /// </summary>
        /// <param name="data">Input data.</param>
        /// <returns>Returns the normalized IFFT.</returns>
        private static double[] Ifft(double[] data)
        {
            // Get the length of the array
            int n = data.Length;
            /* Allocate an unmanaged memory block for the input and output data.
             * (The input and output are of the same length in this case, so we can use just one memory block.) */
            IntPtr ptr = fftw.malloc(n * sizeof(double));
            // Pass the managed input data to the unmanaged memory block
            Marshal.Copy(data, 0, ptr, n);
            // Plan the IFFT and execute it (n/2 because complex numbers are stored as pairs of doubles)
            IntPtr plan = fftw.dft_1d(n / 2, ptr, ptr, fftw_direction.Backward, fftw_flags.Estimate);
            fftw.execute(plan);
            // Create an array to store the output values
            var ifft = new double[n];
            // Pass the unmanaged output data to the managed array
            Marshal.Copy(ptr, ifft, 0, n);
            // Do some cleaning
            fftw.destroy_plan(plan);
            fftw.free(ptr);
            fftw.cleanup();
            // Scale the output values
            for (int i = 0, nh = n / 2; i < n; i++)
                ifft[i] /= nh;
            // Return the IFFT output
            return ifft;
        }

        /// <summary>
        /// Interlaces an array with zeros to match the FFTW convention of representing complex numbers.
        /// </summary>
        /// <param name="real">An array of real numbers.</param>
        /// <returns>Returns an array of complex numbers.</returns>
        private static double[] ToComplex(double[] real)
        {
            int n = real.Length;
            var comp = new double[n * 2];
            for (int i = 0; i < n; i++)
                comp[2 * i] = real[i];
            return comp;
        }

        /// <summary>
        /// Displays complex numbers in the form a +/- bi.
        /// </summary>
        /// <param name="x">An array of complex numbers.</param>
        private static void DisplayComplex(double[] x)
        {
            if (x.Length % 2 != 0)
                throw new Exception("The number of elements must be even.");
            for (int i = 0, n = x.Length; i < n; i += 2)
                if (x[i + 1] < 0)
                    Console.WriteLine("{0} - {1}i", x[i], Math.Abs(x[i + 1]));
                else
                    Console.WriteLine("{0} + {1}i", x[i], x[i + 1]);
        }

        /// <summary>
        /// Displays the real parts of complex numbers.
        /// </summary>
        /// <param name="x">An array of complex numbers.</param>
        private static void DisplayReal(double[] x)
        {
            if (x.Length % 2 != 0)
                throw new Exception("The number of elements must be even.");
            for (int i = 0, n = x.Length; i < n; i += 2)
                Console.WriteLine(x[i]);
        }
    }
}