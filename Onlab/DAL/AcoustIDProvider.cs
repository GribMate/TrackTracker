using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AcoustID;
using AcoustID.Web;

using Onlab.DAL.FingerprinterUtility;



namespace Onlab.DAL
{
    /*
    Class: AcoustIDProvider
    Description:
        Implements IFingerprintProvider, currently using AcoustID fingerprinting service, NAudio and ChromaPrint libraries.
    */
    public class AcoustIDProvider //TODO: add through interface, add proper async-await functions
    {
        public delegate void FingerPrintCallback(string path, string fingerprint, int duration, NAudioDecoder decoder);

        public void GetFingerprint(string filePath, FingerPrintCallback callback)
        {
            if (filePath == null) throw new ArgumentNullException();
            if (filePath.Length < 8) throw new ArgumentException(); //"C:\x.mp3" is 8 chars long
            if (!File.Exists(filePath)) throw new FileNotFoundException();

            NAudioDecoder decoder = new NAudioDecoder(filePath);

            /*
             * Various audio data for example. Might be implemented later onto GUI.
             * 
            int bits = decoder.Format.BitDepth;
            int channels = decoder.Format.Channels;
            int sampleRate = decoder.Format.SampleRate;

            string audioData = String.Format("{0}Hz, {1}bit{2}, {3}",
                decoder.Format.SampleRate, bits, bits != 16 ? " (not supported)" : "",
                channels == 2 ? "stereo" : (channels == 1 ? "mono" : "multi-channel"));
            */

            Task.Factory.StartNew(() =>
            {
                ChromaContext context = new ChromaContext();
                context.Start(decoder.SampleRate, decoder.Channels);
                decoder.Decode(context.Consumer, 120);
                context.Finish();

                callback(filePath, context.GetFingerprint(), decoder.Duration, decoder); //returning results
            });
        }

        public async Task<string> GetIDByFingerprint(string fingerprint, int duration)
        {
            Configuration.ClientKey = "JRfomg6Xqm";

            LookupService service = new LookupService();
            LookupResponse response = await service.GetAsync(fingerprint, duration, new string[] { "recordings", "compress" });

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("Error", "Error in AcoustID WebAPI.");
                en.ShowDialog();
            }
            else if (response.Results.Count == 0)
            {
                Dialogs.ExceptionNotification en = new Dialogs.ExceptionNotification("Empty", "No results for given fingerprint.");
                en.ShowDialog();
            }

            string resultID = "";
            double maxScore = 0;
            foreach (var result in response.Results)
            {
                if (result.Score > maxScore)
                {
                    maxScore = result.Score;
                    resultID = result.Id;
                }
            }
            return resultID;
        }
    }
}
