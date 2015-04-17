using System;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;

namespace Shoy.Laboratory
{
    /// <summary>
    /// 不能用~~
    /// </summary>
    public class ReaderHelper
    {
        /// <summary>
        /// 语音识别引擎
        /// </summary>
        private readonly SpeechRecognitionEngine _speechRecognition;

        /// <summary>
        /// 语音合成器
        /// </summary>
        private readonly SpeechSynthesizer _speech;

        public ReaderHelper()
        {
            foreach (RecognizerInfo info in SpeechRecognitionEngine.InstalledRecognizers())
            {
                Console.WriteLine(info.Description);
            }
            _speech = new SpeechSynthesizer();
            var gb = new GrammarBuilder(new Choices("杨本国"));
            _speechRecognition.LoadGrammar(new Grammar(gb));
            //有匹配的输入
            _speechRecognition.SpeechRecognized += _speechRecognition_SpeechRecognized;
            _speechRecognition.SpeechRecognitionRejected += _speechRecognition_SpeechRecognitionRejected;
        }

        public void ReadWord(string wavePath)
        {
            _speechRecognition.SetInputToWaveFile(wavePath);
        }

        private void _speechRecognition_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            _speechRecognition.RecognizeAsyncStop();
            Thread.Sleep(30);
            _speech.Speak("请再说一遍");
            _speechRecognition.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// 语音识别后的处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _speechRecognition_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // 关闭识别，防止speech说出来的话被误识别
            _speechRecognition.RecognizeAsyncStop();
            Console.WriteLine(e.Result.Text);
            Thread.Sleep(30);
            if (e.Result.Confidence > 0.95)
            {
                _speech.Speak(e.Result.Text);
            }
            _speechRecognition.RecognizeAsync(RecognizeMode.Multiple);
        }
    }
}
