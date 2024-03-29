//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BatchClient
{
    class Program
    {
        // <batchdefinition>
        // Replace with your subscription key
        private const string SubscriptionKey = "17f536f0b51847849f7a0ca5ca940bb2";
  
        // Update with your service region
        private const string HostName = "northeurope.cris.ai";
        private const int Port = 443;

        // recordings and locale
        private const string Locale = "en-GB";
        private const string RecordingsBlobUri = "https://paoltblob.blob.core.windows.net/s2t/consilium6.wma?sp=r&st=2019-04-05T13:56:15Z&se=2019-04-05T21:56:15Z&spr=https&sv=2018-03-28&sig=X%2Buxilck9ulFW6n%2Fxw7zilkTzqGWaiSA4IBP4UVjQXQ%3D&sr=b";
        //private const string RecordingsBlobUri = "https://paoltblob.blob.core.windows.net/s2t/whatstheweatherlike.wav?sp=r&st=2019-03-18T09:17:40Z&se=2019-03-18T17:17:40Z&spr=https&sv=2018-03-28&sig=9saOYYnf9MxMhJhdl4N67HfcqjbD6l1w%2Fdob4X31Sco%3D&sr=b";

        // For usage of baseline models, no acoustic and language model needs to be specified.
        private static Guid[] modelList = new Guid[0];

        // For use of specific acoustic and language models:
        // - comment the previous line
        // - uncomment the next lines to create an array containing the guids of your required model(s)
        // private static Guid AdaptedAcousticId = new Guid("<id of the custom acoustic model>");
        // private static Guid AdaptedLanguageId = new Guid("<id of the custom language model>");
        // private static Guid[] modelList = new[] { AdaptedAcousticId, AdaptedLanguageId };

        //private static Guid AdaptedLanguageId = new Guid("cb82ab19-d3f0-4454-a6ad-353df0c0f002");
        //private static Guid[] modelList = new[] { AdaptedLanguageId };

        //name and description
        private const string Name = "myTranscription";
        private const string Description = "myTranscription";
        // </batchdefinition>

        static void Main(string[] args)
            {
                TranscribeAsync().Wait();
            }

        static async Task TranscribeAsync()
        {
            Console.WriteLine("Starting transcriptions client...");

            // create the client object and authenticate
            var client = BatchClient.CreateApiV2Client(SubscriptionKey, HostName, Port);

            // get all transcriptions for the subscription
            var transcriptions = await client.GetTranscriptionsAsync().ConfigureAwait(false);

            Console.WriteLine("Deleting all existing completed transcriptions.");
            // delete all pre-existing completed transcriptions. If transcriptions are still running or not started, they will not be deleted
            foreach (var item in transcriptions)
            {
                // delete a transcription
                await client.DeleteTranscriptionAsync(item.Id).ConfigureAwait(false);
            }

            Console.WriteLine("Creating transcriptions.");
            var transcriptionLocation = await client.PostTranscriptionAsync(Name, Description, Locale, new Uri(RecordingsBlobUri), modelList).ConfigureAwait(false);

            // get the transcription Id from the location URI
            var createdTranscriptions = new List<Guid>();
            createdTranscriptions.Add(new Guid(transcriptionLocation.ToString().Split('/').LastOrDefault()));

            Console.WriteLine("Checking status.");

            // check for the status of our transcriptions every 30 sec. (can also be 1, 2, 5 min depending on usage)
            int completed = 0, running = 0, notStarted = 0;
            while (completed < 1)
            {
                // <batchstatus>
                // get all transcriptions for the user
                transcriptions = await client.GetTranscriptionsAsync().ConfigureAwait(false);

                completed = 0; running = 0; notStarted = 0;
                // for each transcription in the list we check the status
                foreach (var transcription in transcriptions)
                {
                    switch (transcription.Status)
                    {
                        case "Failed":
                            {
                                Console.WriteLine("Transcription failed with status message: " + transcription.StatusMessage);
                                completed++;
                                break;
                            }
                        case "Succeeded":
                            // we check to see if it was one of the transcriptions we created from this client.
                            if (!createdTranscriptions.Contains(transcription.Id))
                            {
                                // not created form here, continue
                                continue;
                            }
                            completed++;

                            // if the transcription was successfull, check the results
                            if (transcription.Status == "Succeeded")
                            {
                                var resultsUri = transcription.ResultsUrls["channel_0"];

                                WebClient webClient = new WebClient();

                                // Path = C:\Users\sa\AppData\Local\Temp\2
                                var filename = Path.GetTempFileName();
                                webClient.DownloadFile(resultsUri, filename);

                                var results = File.ReadAllText(filename);
                                Console.WriteLine("Transcription succeeded. Results: ");
                                Console.WriteLine(results);
                            }
                            else
                            {
                                Console.WriteLine("Transcription failed with status message: " + transcription.StatusMessage);
                            }
                            break;

                        case "Running":
                            running++;
                            break;

                        case "NotStarted":
                            notStarted++;
                            break;
                    }
                }
                // </batchstatus>

                Console.WriteLine(string.Format("Transcriptions status: {0} completed, {1} running, {2} not started yet", completed, running, notStarted));
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
