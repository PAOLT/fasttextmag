## Video Classification with Fast Text trained over MS Academic Graph 

We have some videos from scientific conferences, and we are willing to associate a topic tag to portions of videos. Microsoft Academic Graph stores many papers with abstract and topics (called Field of Study). We want to train a MC-ML classification model over those abstract-FoS pairs in order to label videos.

The project has three phases (on-going):

1) automatic generation of the video textural transcript

2) classification of the transcript

3) optimization tasks

### Transcription of the Video

The transcription is generated via Azure Cognitive Services (Custom Speech Services - https://azure.microsoft.com/en-us/services/cognitive-services/custom-speech-service/).

The following tasks has been developed:

- video has been transformed to an audio file in the right format via FFMPEG (https://ffmpeg.org/). The following command has been issued to FFMPEG: ffmpeg -i madelin.mp4 -vn -ar 16000 -b:a 16k -c:a pcm_s16le madelin.wma. See supported formats here: https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/batch-transcription#the-batch-transcription-api. Find Input and Output files in the resources directory. 

- A Language Model has been generated by manually reviewign the automatic transcription of a few videos from the same speaker talking about similar topics. In addition, a few phrases containing special words (i.e. Parma, a town in Italy) have been manually added to the LM. Find Language Models and transcripts in the resources folder. See how to customize the service with a LM here https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/how-to-customize-language-model

- The Speech batch service has been programmatically invoked via REST to generate the transcript. See the how-to here https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/batch-transcription. Find the C# solution in the folder named CRIS batch. A CLI to interact with the service has also been developed (https://github.com/msimecek/Azure-Speech-CLI#batch-transcriptions) 

- The transcript can be also generated easily with Azure Video Indexer (https://www.videoindexer.ai). Though it let less control over customization, the results were comparable. 

### Classification

** Some links about Fast Text **

- Fast Text tutorial on text classification: https://fasttext.cc/docs/en/supervised-tutorial.html

- Analytics Vidhya tutorial on Fast Text: https://www.analyticsvidhya.com/blog/2017/07/word-representations-text-classification-using-fasttext-nlp-facebook/

- Background on neural text classification:

  - https://medium.com/jatana/report-on-text-classification-using-cnn-rnn-han-f0e887214d5f

  - http://www.davidsbatista.net/blog/2018/03/31/SentenceClassificationConvNets/

  - http://www.wildml.com/2015/12/implementing-a-cnn-for-text-classification-in-tensorflow/

  - http://www.joshuakim.io/understanding-how-convolutional-neural-network-cnn-perform-text-classification-with-word-embeddings/

### Optimization

Several sub-projects are active:

- improve video transcriptions via active learning.

- improve transcript punctuation via deep learning 





https://www.microsoft.com/en-us/research/project/microsoft-academic-graph/
